using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.Messaging.EventGrid;
using SampleFunctionsApp.Model;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using SampleFunctionsApp.TableEntities;
using System.Diagnostics;

namespace SignalRFunction
{
    public static class SignalRFunctions
    {
        public static double temperature;
        public static List<double> tempArr = new List<double>();
        public static string tableName = "tblgreenhouse";
        public static string accountName = "sagreenhouse";
        public static string accountKey = "7/aG8qXX5USnLw5OP0fpY26Itg9DnwUIMyIIiyQTR3dtNF0nKw2USF63j9K4h6iOktAyfoTRm802BtlnIX9joA==";
        public static string Pkey = "";

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "dttelemetry")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("broadcast")]
        public static Task SendMessage(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            [SignalR(HubName = "dttelemetry")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            var eventsubject = eventGridEvent.Subject.ToString();
            log.LogInformation("Event Grid Event Subject --> " + eventsubject);

            JObject eventGridData = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());

            log.LogInformation($"Event grid message: {eventGridData}");

            var patch = (JObject)eventGridData["data"]["patch"][0];

            var patchArray = (JArray)eventGridData["data"]["patch"];
            log.LogInformation($"patch Array =>> : " + patchArray);
            var result = GetSmartAlert(patchArray,log);

            log.LogInformation("Smart device response --> :" + result);

            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "newMessage",
                    Arguments = new[] { result }
                });
        }

        public static string GetSmartAlert(JArray patchArray, ILogger log)
        {
            //SmartDetector smartResponse = new SmartDetector();
            SmartAlertResponse smartAlertResponse = new SmartAlertResponse();
            if (patchArray?.Count > 0)
            {
                Dictionary<string, double> propwithDoublevalue = new Dictionary<string, double>();
                Dictionary<string, bool> propwithBooleanvalue = new Dictionary<string, bool>();
                Dictionary<string, string> propwithStringvalue = new Dictionary<string, string>();
                Dictionary<string, DateTime> propwithDateTimevalue = new Dictionary<string, DateTime>();

                foreach (JObject content in patchArray.Children<JObject>())
                {
                    if (content["path"].ToString().Contains("Status"))
                    {
                        propwithBooleanvalue.Add(content["path"].ToString(), content["value"].ToObject<bool>());
                    }
                    else if (content["path"].ToString().Contains("/locationinfo/timezone") || content["path"].ToString().Contains("/deviceinfo/deviceid") || content["path"].ToString().Contains("/cropinfo/cropname") || content["path"].ToString().Contains("/cropinfo/croptype"))
                    {
                        propwithStringvalue.Add(content["path"].ToString(), content["value"].ToObject<string>());
                    }
                    else if (content["path"].ToString().Contains("/deviceinfo/devicetimestamp"))
                    {
                        propwithDateTimevalue.Add(content["path"].ToString(), content["value"].ToObject<DateTime>());
                    }
                    else
                    {
                        propwithDoublevalue.Add(content["path"].ToString(), content["value"].ToObject<double>());
                    }
                }
                // if we get the dictionary with desired data, map it to the model
                if (propwithBooleanvalue?.Count > 0 && propwithStringvalue?.Count > 0 && propwithDateTimevalue?.Count > 0 && propwithDoublevalue?.Count > 0)
                {

                    //set all double types values
                    smartAlertResponse.co2Level = propwithDoublevalue["/CO2level"];
                    smartAlertResponse.temperature = propwithDoublevalue["/Temperature"];
                    smartAlertResponse.humidityLevel = propwithDoublevalue["/Humiditylevel"];
                    smartAlertResponse.mositureLevel = propwithDoublevalue["/Moisturelevel"];
                    smartAlertResponse.lightCount=propwithDoublevalue["/LightCount"];
                    smartAlertResponse.lightCapacity=propwithDoublevalue["/LightCapacity"];
                    smartAlertResponse.lightDuration = propwithDoublevalue["/LightDuration"];
                    CropInfo _cropData = new CropInfo
                    {
                        soilphValue = propwithDoublevalue["/cropinfo/soilphvalue"],
                        cropType = propwithStringvalue["/cropinfo/croptype"],
                        cropName = propwithStringvalue["/cropinfo/cropname"]
                    };
                    smartAlertResponse.cropInfo = _cropData;
                    DeviceInfo _deviceData = new DeviceInfo
                    {
                        deviceVersion = propwithDoublevalue["/deviceinfo/deviceversion"],
                        deviceId = propwithStringvalue["/deviceinfo/deviceid"],
                        deviceTimestamp = propwithDateTimevalue["/deviceinfo/devicetimestamp"]
                    };
                    smartAlertResponse.deviceInfo = _deviceData;
                    LocationInfo _locationData = new LocationInfo
                    {
                        lat = propwithDoublevalue["/locationinfo/lat"],
                        lon = propwithDoublevalue["/locationinfo/lon"],
                        timezone = propwithStringvalue["/locationinfo/timezone"]
                    };
                    smartAlertResponse.locationInfo = _locationData;
                    smartAlertResponse.waterpumpStatus = propwithBooleanvalue["/WaterPumpStatus"];
                    smartAlertResponse.dehumidifierStatus = propwithBooleanvalue["/DehumidifierStatus"];
                    smartAlertResponse.fanStatus = propwithBooleanvalue["/FanStatus"];
                    smartAlertResponse.lightingStatus = propwithBooleanvalue["/LightingStatus"];
                    PerformRuleBasedAnalytics(smartAlertResponse);
                    //var response = JsonConvert.SerializeObject(smartAlertResponse);
                }
            }
            IngestToTableStorage(smartAlertResponse,DateTime.Now,log);
            smartAlertResponse.predictivePercentage = GeneratePredictivePercentage(Pkey, log);

            return JsonConvert.SerializeObject(smartAlertResponse);
        }
        public static void PerformRuleBasedAnalytics(SmartAlertResponse smartAlertResponse)
        {
            // Add logic if the soil Ph based on that turn on the waterpump
            // For tomato ideal range is 5.5-7.5      ---?? and if current value falling in that range 
            // Optimum daytime temperatures should be maintained between 70 degrees(21 Celicus) to 80 degrees F(26 Celicus)
            // humidity in range 50-70%
            // similary for Lichi
            //similary for other devices - fan to turn on if there is high temparture  also turn on dehumidifiers
            //if the humditiy falls  and climate darkness turn on lights 
            if (smartAlertResponse.cropInfo.cropName == "Tomato")
            {
                if (smartAlertResponse.cropInfo.soilphValue >= 5.5 && smartAlertResponse.cropInfo.soilphValue <= 7.5)
                {
                    smartAlertResponse.waterpumpStatus = false;
                }
                else
                {
                    smartAlertResponse.waterpumpStatus = true;
                }

                if (smartAlertResponse.temperature >= 21 && smartAlertResponse.temperature < 27)
                {
                    smartAlertResponse.fanStatus = false;
                }
                else
                {
                    smartAlertResponse.fanStatus = true;
                }

                if (smartAlertResponse.humidityLevel >= 50 && smartAlertResponse.humidityLevel < 70)
                {
                    smartAlertResponse.dehumidifierStatus = false;
                }
                else
                {
                    smartAlertResponse.dehumidifierStatus = true;
                }
            }
            smartAlertResponse.hasSchedule = true;
            ScheduleInfo waterScheduleInfo = new ScheduleInfo
            {
                scheduleId = Guid.NewGuid(),
                scheduleStarttime = "08:20:37 AM",
                scheduleFinishtime = "08:50:47 AM",
                scheduleFor = "Water Irrigation "
            };
            ScheduleInfo lightScheduleInfo = new ScheduleInfo
            {
                scheduleId = Guid.NewGuid(),
                scheduleStarttime = "06:10:47 PM",
                scheduleFinishtime = "11:50:47 PM",
                scheduleFor = "Lighting"
            };
            List<ScheduleInfo> scheduleData = new List<ScheduleInfo>
                    {
                        waterScheduleInfo,
                        lightScheduleInfo
                    };

            smartAlertResponse.scheduleInfo = scheduleData;
            smartAlertResponse.xAxisData = GenerateGraphXaxisData(smartAlertResponse);
            smartAlertResponse.tempatureSeriesData = GenerateGraphSeriesData("Tempearture", smartAlertResponse);
            smartAlertResponse.humiditySeriesData = GenerateGraphSeriesData("Humidity", smartAlertResponse);
        }
        public static List<double> GenerateGraphSeriesData(string graphFor, SmartAlertResponse smartAlertResponse, string axisType = null)
        {
            var deviceTime = smartAlertResponse.deviceInfo.deviceTimestamp.TimeOfDay.Hours;
            List<double> axisData = new List<double>();
            int formattedValue;
            List<double> temp = new List<double>();
            if (graphFor.Equals("Tempearture"))
            {
                formattedValue = Convert.ToInt32(smartAlertResponse.temperature);
            }
            else
            {
                formattedValue = Convert.ToInt32(smartAlertResponse.humidityLevel);
            }
            var rand1 = new Random();

            for (int k = 1; k < 6; k++)
            {
                axisData.Add(formattedValue);
                formattedValue = rand1.Next(formattedValue, formattedValue + 10);
            }
            var orderedData = axisData.OrderByDescending(x => x).ToList();
            return orderedData;
        }
        public static List<string> GenerateGraphXaxisData(SmartAlertResponse smartAlertResponse)
        {
            var currentTime = DateTime.UtcNow.TimeOfDay.Hours;
            var currentTimeString = DateTime.UtcNow.ToShortTimeString();
            List<double> timenumber = new List<double>();
            List<string> resultAxis = new List<string>();
            var k = 5;
            for (int j = 0; j < k; j++)
            {
                timenumber.Add(currentTime - j);
            }
            var orderdList = timenumber.OrderBy(x => x).ToList();
            foreach (var item in orderdList)
            {
                if (item == 0)
                {
                    resultAxis.Add(item + ".00 PM");
                }
                else if (item == 12)
                {
                    resultAxis.Add(item + ".00 PM");
                }
                else if (item > 12)
                {
                    resultAxis.Add(item - 12 + ".00 PM");
                }
                else { resultAxis.Add(item + ".00 AM"); }
            }
            return resultAxis;
        }
        public static void IngestToTableStorage(SmartAlertResponse smartAlertResponse, DateTime dt, ILogger log)
        {
            //CloudStorageAccount storageaccount = CloudStorageAccount.Parse(storageconn);
            //CloudTableClient tableClient = storageaccount.CreateCloudTableClient();
            //CloudTable table = tableClient.GetTableReference(tableName);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            log.LogInformation("Ingestion to Azure Storage Started -->  " + sw.ElapsedMilliseconds);
            StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);

            CloudTableClient client = account.CreateCloudTableClient();

            CloudTable table = client.GetTableReference(tableName);

            //var response = JsonConvert.SerializeObject(smartAlertResponse);
            var formattedKey = dt.Date.ToString("yyyy-MM-dd") + "_" + dt.TimeOfDay.Hours + "_" + dt.TimeOfDay.Minutes;
            var formattedPKey = smartAlertResponse.deviceInfo.deviceId + "-" + dt.Date.ToString("yyyy-MM-dd");
            Pkey = formattedPKey;
            var newEntity = new GreenhouseEntity()
            {
                PartitionKey = formattedPKey,
                RowKey = formattedKey,
                Co2Level = smartAlertResponse.co2Level,
                Temperature = smartAlertResponse.temperature,
                MositureLevel = smartAlertResponse.mositureLevel,
                HumidityLevel = smartAlertResponse.humidityLevel,
                SoilphValue=smartAlertResponse.cropInfo.soilphValue,
                CropName=smartAlertResponse.cropInfo.cropName,
                LightCount=smartAlertResponse.lightCount,
                LightCapacity=smartAlertResponse.lightCapacity,
                LightDuration=smartAlertResponse.lightDuration
            };
            try
            {
                TableOperation insert = TableOperation.Insert(newEntity);
                table.ExecuteAsync(insert);

                TableOperation entity = TableOperation.Retrieve(formattedPKey, formattedKey);

                var result = table.ExecuteAsync(entity);
                //var flag = false;
                if (result.Result != null)
                {
                    sw.Stop();
                }
                
                log.LogInformation("Ingestion to Azure Storage Completed -->  : " + sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                log.LogError("Exception during inserting to Azure storage" + ex);
            }
        }
        public static double GeneratePredictivePercentage(string Pkey,ILogger log)
        {
            log.LogInformation("Yield Production % using Prediction Analysis Started , Parition Key -->  "+Pkey);
            double result =0;
            // Assumption : ideal temp for effective tomato growth should be 20 to 27
            // Get the total temp for the day from table sotrage
            // count the total temp
            // 0-25	Poor , 25 - 50 Average,5 - 75 Good,75 - 100 Excellent
            // calclaute the % = temp count in the range of ideal '27	35'/total temp recorded for the day till the time 
            StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);
            CloudTableClient client = account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(tableName);
             ////// Code to fetch all records 
            TableContinuationToken token = null;
    //        TableQuery<GreenhouseEntity> query = new TableQuery<GreenhouseEntity>()
    //.Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Pkey));

            TableQuery<GreenhouseEntity> combineQuery = new TableQuery<GreenhouseEntity>().Where(
 TableQuery.CombineFilters(
 TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Pkey),
     TableOperators.And,
 TableQuery.GenerateFilterCondition("CropName", QueryComparisons.Equal, "Tomato")));

            var entities = new List<GreenhouseEntity>();
            do
            {
                var queryResult = table.ExecuteQuerySegmentedAsync(combineQuery, token);
                entities.AddRange(queryResult.Result.Results);
                token = queryResult.Result.ContinuationToken;
            } while (token != null);


            var idealRange = entities.Where(x => x.Temperature > 25 && x.Temperature < 32).ToList().Count;
            var totalRecordsInTime = entities.Count;
            if (idealRange > 0 && totalRecordsInTime > 0)
            {
                result = Math.Round(Convert.ToDouble(idealRange) / Convert.ToDouble(totalRecordsInTime) * 100, 2,MidpointRounding.AwayFromZero);
            }
            log.LogInformation("Yield Production % by Prediction Analysis -->  " + result);
            return result;
        }
    }
}