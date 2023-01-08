using Azure;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using SampleFunctionsApp.Model;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SampleFunctionsApp.TableEntities;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Data;
using System.Text;
using System.IO;
using SampleFunctionsApp.Helper;
using Microsoft.Azure.WebJobs;
using System.Globalization;

namespace FunctionBuilder
{
    public class Program
    {
        public static string storageconn = "DefaultEndpointsProtocol=https;AccountName=sagreenhouse;AccountKey=7/aG8qXX5USnLw5OP0fpY26Itg9DnwUIMyIIiyQTR3dtNF0nKw2USF63j9K4h6iOktAyfoTRm802BtlnIX9joA==;EndpointSuffix=core.windows.net";
        public static string tableName = "tblgreenhouse";
        public static string accountName = "sagreenhouse";
        public static string accountKey = "7/aG8qXX5USnLw5OP0fpY26Itg9DnwUIMyIIiyQTR3dtNF0nKw2USF63j9K4h6iOktAyfoTRm802BtlnIX9joA==";
        public static string Pkey = "";
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Test the function code !!!");
            string adtInstanceUrl = "https://dt-greenhouse.api.neu.digitaltwins.azure.net";
            var credentials = new DefaultAzureCredential();
            var client = new DigitalTwinsClient(new Uri(adtInstanceUrl), credentials);
            Console.WriteLine($"Service client created – ready to go");
            var deviceId = "smart-detector-1.0";
            //OLD //var deviceMessageBody = "{'co2Level':24,'temperature':24,'humidityLevel':13,'mositureLevel':2352,'fanStatus':false,'lightingStatus':false,'cropInfo': {'cropType': 'Fruits','cropName': 'Litchi','soilphValue': 6},'deviceInfo': {'deviceId': 'smart-detector-1.0','deviceVersion': 1.1,'deviceTimestamp': '2022-02-24T21:55:03.1947168Z'},'locationInfo': {'lat': 18.516726,'lon': 73.856255,'timezone': 'GMT+5:30'}}";
            var deviceMessageBody = "{'co2Level':660,'temperature':33,'humidityLevel':30.633030303030303,'mositureLevel':5,'lightCount':1,'lightCapacity':20,'lightDuration':8,'fanStatus':false,'lightingStatus':false,'waterpumpStatus':false,'dehumidifierStatus':false,'cropInfo':{ 'cropType':'Vegetables','cropName':'Tomato','soilphValue':8.25},'deviceInfo':{ 'deviceId':'smart-detector-1.0','deviceVersion':1,'deviceTimestamp':'2022-07-11T11:53:19.7075484Z'},'locationInfo':{ 'lat':18.516726,'lon':73.856255,'timezone':'Coordinated Universal Time'} }";

            //JObject deviceMessage = (JObject)JsonConvert.DeserializeObject(deviceMessageBody);
            //// Uncomment below to test the ProcessDTRoutedData functionality
            var deviceMessageResponse = JsonConvert.DeserializeObject<SmartDetector>(deviceMessageBody);


            try
            {
                //// TO Test the ProcessHubToDTEvents  uncomment below line and run
                // await ProcessHubToDTEvents(deviceMessageResponse, client, deviceId);
                //// TO Test the SignalRFunctions uncomment below line and run
                //await SingalR_check();

                //// TO Test the Reading table storage
                ///
                //var xy = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                //Console.WriteLine(xy);
                await ReadDataAndUploadToBlob(Pkey);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured " + ex);
                throw;
            }
            Console.ReadLine();
        }

        public static async Task ProcessHubToDTEvents(SmartDetector deviceMessageResponse, DigitalTwinsClient client, string deviceId)
        {
            var updateTwinData = new JsonPatchDocument();
            updateTwinData.AppendAdd("/CO2level", deviceMessageResponse.co2Level);
            updateTwinData.AppendAdd("/Temperature", deviceMessageResponse.temperature);
            updateTwinData.AppendAdd("/Humiditylevel", deviceMessageResponse.humidityLevel);
            updateTwinData.AppendAdd("/Moisturelevel", deviceMessageResponse.mositureLevel);
            updateTwinData.AppendAdd("/LightCount", deviceMessageResponse.lightCount);
            updateTwinData.AppendAdd("/LightCapacity", deviceMessageResponse.lightCapacity);
            updateTwinData.AppendAdd("/LightDuration", deviceMessageResponse.lightDuration);
            updateTwinData.AppendAdd("/WaterPumpStatus", deviceMessageResponse.waterpumpStatus);
            updateTwinData.AppendAdd("/DehumidifierStatus", deviceMessageResponse.dehumidifierStatus);
            updateTwinData.AppendAdd("/LightingStatus", deviceMessageResponse.lightingStatus);
            updateTwinData.AppendAdd("/FanStatus", deviceMessageResponse.fanStatus);
            updateTwinData.AppendAdd("/cropinfo/croptype", deviceMessageResponse.cropInfo.cropType); // need to check if it works
            updateTwinData.AppendAdd("/cropinfo/soilphvalue", deviceMessageResponse.cropInfo.soilphValue);
            updateTwinData.AppendAdd("/cropinfo/cropname", deviceMessageResponse.cropInfo.cropName);
            updateTwinData.AppendAdd("/deviceinfo/deviceid", deviceMessageResponse.deviceInfo.deviceId); // need to check if it works
            updateTwinData.AppendAdd("/deviceinfo/deviceversion", deviceMessageResponse.deviceInfo.deviceVersion);
            updateTwinData.AppendAdd("/deviceinfo/devicetimestamp", deviceMessageResponse.deviceInfo.deviceTimestamp);
            updateTwinData.AppendAdd("/locationinfo/lat", deviceMessageResponse.locationInfo.lat);
            updateTwinData.AppendAdd("/locationinfo/lon", deviceMessageResponse.locationInfo.lon);
            updateTwinData.AppendAdd("/locationinfo/timezone", deviceMessageResponse.locationInfo.timezone);
            await client.UpdateDigitalTwinAsync(deviceId, updateTwinData).ConfigureAwait(false);
        }
        public static async Task SingalR_check()
        {
            var msg = "{'data': {'modelId': 'dtmi:tsystem:DigitalTwins:Smartdetector;1','patch': [{'value': 1,'path': '/LightCount','op': 'add'},{'value': 8,'path': '/LightDuration','op': 'add'},{'value': 20,'path': '/LightCapacity','op': 'add'},{'value': 42,'path': '/CO2level','op': 'add'},{'value': 40,'path': '/Temperature','op': 'add'},{'value': 13,'path': '/Humiditylevel','op': 'add'},{'value': 2352,'path': '/Moisturelevel','op': 'add'},{'value': false,'path': '/WaterPumpStatus','op': 'add'},{'value': false,'path': '/DehumidifierStatus','op': 'add'},{'value': false,'path': '/LightingStatus','op': 'add'},{'value': false,'path': '/FanStatus','op': 'add'},{'value': 'Fruits','path': '/cropinfo/croptype','op': 'add'},{'value': 6,'path': '/cropinfo/soilphvalue','op': 'add'},{'value': 'Litchi','path': '/cropinfo/cropname','op': 'add'},{'value': 'smart-detector-1.0','path': '/deviceinfo/deviceid','op': 'add'},{'value': 1,'path': '/deviceinfo/deviceversion','op': 'add'},{'value': '2022-02-28T14:36:19.6196615Z','path': '/deviceinfo/devicetimestamp','op': 'add'},{'value': 18.516726,'path': '/locationinfo/lat','op': 'add'},{'value': 73.856255,'path': '/locationinfo/lon','op': 'add'},{'value': 'GMT+5:30','path': '/locationinfo/timezone','op': 'add'}]},'contenttype': 'application/json','traceparent': '00-f1fa668384678e40acd149e9ec08b1c7-a975548c2a9d674e-01'}";
            //var msg = "[ { 'value': { 'devicetimestamp': '2021 - 07 - 11T10: 20:11Z' }, 'path': ' / deviceinfo', 'op': 'add' } ]";
            JObject eventGridData = (JObject)JsonConvert.DeserializeObject(msg.ToString());
            var patchArray = (JArray)eventGridData["data"]["patch"];
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
                SmartAlertResponse smartAlertResponse = new SmartAlertResponse();
                //set all double types values
                smartAlertResponse.co2Level = propwithDoublevalue["/CO2level"];
                smartAlertResponse.temperature = propwithDoublevalue["/Temperature"];
                smartAlertResponse.humidityLevel = propwithDoublevalue["/Humiditylevel"];
                smartAlertResponse.mositureLevel = propwithDoublevalue["/Moisturelevel"];
                smartAlertResponse.lightCapacity = propwithDoublevalue["/LightCapacity"];
                smartAlertResponse.lightCount = propwithDoublevalue["/LightCount"];
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
                //smartResponse.deviceInfo.deviceVersion = propwithDoublevalue["/deviceinfo/deviceversion"];
                LocationInfo _locationData = new LocationInfo
                {
                    lat = propwithDoublevalue["/locationinfo/lat"],
                    lon = propwithDoublevalue["/locationinfo/lon"],
                    timezone = propwithStringvalue["/locationinfo/timezone"]
                };
                smartAlertResponse.locationInfo = _locationData;

                //set all boolean types values
                smartAlertResponse.waterpumpStatus = propwithBooleanvalue["/WaterPumpStatus"];
                smartAlertResponse.dehumidifierStatus = propwithBooleanvalue["/DehumidifierStatus"];
                smartAlertResponse.fanStatus = propwithBooleanvalue["/FanStatus"];
                smartAlertResponse.lightingStatus = propwithBooleanvalue["/LightingStatus"];

                PerformRuleBasedAnalytics(smartAlertResponse);
                IngestToTableStorage(smartAlertResponse, DateTime.Now.ToUniversalTime());
                var response = JsonConvert.SerializeObject(smartAlertResponse);
                Console.WriteLine("Final Response == > \n" + response);
                GeneratePredictivePercentage(DateTime.Now.ToUniversalTime(), Pkey);

            }
        }
        public static void PerformRuleBasedAnalytics(SmartAlertResponse smartAlertResponse)
        {
            // Add logic if the soil Ph based on that turn on the waterpump
            // For tomato ideal range is 5.5-7.5      ---?? and if current value falling in that range
            // similary for Lichi
            //similary for other devices - fan to turn on if there is high temparture  also turn on dehumidifiers
            //if the humditiy falls in certain range and climate darkness turn on lights
            smartAlertResponse.hasSchedule = true;
            var currentTime = DateTime.UtcNow;
            //var waterpumStart = currentTime.ToLongTimeString();
            //var waterpumEnd = currentTime.AddMinutes(20).ToLongTimeString();
            ScheduleInfo waterScheduleInfo = new ScheduleInfo
            {
                scheduleId = Guid.NewGuid(),
                scheduleStarttime = "08:20:37 AM",
                scheduleFinishtime = "08:50:47 AM",
                scheduleFor = "Waterpump"
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

            // Add tempearture series data
            // get the device recorded time
            // from that time create 5 time series data with current tempearture at current time and next tempearture
            //GraphSeriesData tempGraphSeriesData = new GraphSeriesData();
            //tempGraphSeriesData.graphfor = "Temperture";
            //tempGraphSeriesData.xAxisData = GenerateGraphSeriesData("Tempearture", smartAlertResponse,"x");
            //tempGraphSeriesData.yAxisData = GenerateGraphSeriesData("Tempearture", smartAlertResponse);
            //smartAlertResponse.tempatureSeriesData = tempGraphSeriesData;
            //GraphSeriesData humidityGraphSeriesData = new GraphSeriesData();
            //humidityGraphSeriesData.graphfor = "Humidity";
            //humidityGraphSeriesData.xAxisData = GenerateGraphSeriesData("Humidity", smartAlertResponse,"x");
            //humidityGraphSeriesData.yAxisData = GenerateGraphSeriesData("Humidity", smartAlertResponse);
            //smartAlertResponse.humiditySeriesData = humidityGraphSeriesData;
            //smartAlertResponse.tempatureSeriesData = GenerateGraphSeriesData("Tempearture",smartAlertResponse);
            //smartAlertResponse.humiditySeriesData = GenerateGraphSeriesData("Humidity", smartAlertResponse);
            smartAlertResponse.xAxisData = GenerateGraphXaxisData(smartAlertResponse);
            smartAlertResponse.tempatureSeriesData = GenerateGraphSeriesData("Tempearture", smartAlertResponse);
            smartAlertResponse.humiditySeriesData = GenerateGraphSeriesData("Humidity", smartAlertResponse);
        }
        public static List<double> GenerateGraphSeriesData(string graphFor, SmartAlertResponse smartAlertResponse, string axisType = null)
        {
            var deviceTime = smartAlertResponse.deviceInfo.deviceTimestamp.TimeOfDay.Hours;
            //Dictionary<double, double> seriesData = new Dictionary<double, double>();
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
            //Dictionary<double, double> seriesData = new Dictionary<double, double>();
            var currentTimeString = DateTime.UtcNow.ToShortTimeString();
            List<double> timenumber = new List<double>();
            //var timevalue =Convert.ToInt32(currentTimeString.Substring(0, 5).TrimEnd());
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
        //public static Dictionary<double, double> GenerateGraphSeriesData1(string graphFor, SmartAlertResponse smartAlertResponse)
        //{
        //    var deviceTime = smartAlertResponse.deviceInfo.deviceTimestamp.TimeOfDay.Hours;
        //    Dictionary<double, double> seriesData = new Dictionary<double, double>();
        //    var rand = new Random();
        //    double maximum, minimum = 0.0D;

        //    if (graphFor.Equals("Tempearture"))
        //    {
        //        //var rangeNumber = rand.Next(17, 38);
        //        maximum = smartAlertResponse.temperature * 2 - 1;
        //        minimum = smartAlertResponse.temperature;
        //    }
        //    else
        //    {
        //        maximum = smartAlertResponse.humidityLevel * 2 - 1;
        //        minimum = smartAlertResponse.humidityLevel;
        //    }

        //    for (int i = 1; i < 6; i++)
        //    {
        //        seriesData.Add(deviceTime + i, rand.NextDouble() * (maximum - minimum) + minimum);
        //    }
        //    return seriesData;
        //}
        public static void IngestToTableStorage(SmartAlertResponse smartAlertResponse, DateTime dt)
        {
            //CloudStorageAccount storageaccount = CloudStorageAccount.Parse(storageconn);
            //CloudTableClient tableClient = storageaccount.CreateCloudTableClient();
            //CloudTable table = tableClient.GetTableReference(tableName);
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
                SoilphValue = smartAlertResponse.cropInfo.soilphValue,
                CropName = smartAlertResponse.cropInfo.cropName,
                LightCapacity = smartAlertResponse.lightCapacity,
                LightCount = smartAlertResponse.lightCount,
                LightDuration = smartAlertResponse.lightDuration
            };
            try
            {
                TableOperation insert = TableOperation.Insert(newEntity);
                table.ExecuteAsync(insert);

                TableOperation entity = TableOperation.Retrieve(formattedPKey, formattedKey);

                var result = table.ExecuteAsync(entity);
                var flag = false;
                if (result.Result != null)
                {
                    flag = true;
                }
                Console.WriteLine("Flag status" + flag);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during insert to storage" + ex);
            }
        }
        public static void GeneratePredictivePercentage(DateTime dtInstance, string Pkey)
        {
            double result = 0.0D;
            // Assumption : ideal temp for effective tomato growth should be 20 to 27
            // Get the total temp for the day from table sotrage
            // count the total temp
            // 0-25	Poor , 25 - 50 Average,5 - 75 Good,75 - 100 Excellent
            // calclaute the % = temp count in the range of ideal '27	35'/total temp recorded for the day till the time
            //
            //return result;
            // Use the <see cref="TableServiceClient"> to query the service. Passing in OData filter strings is optional.
            //string rkey = "2022-03-08_12_46";
            StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);

            CloudTableClient client = account.CreateCloudTableClient();

            CloudTable table = client.GetTableReference(tableName);

            //TableQuery<GreenhouseEntity> query = new TableQuery<GreenhouseEntity>()
            //    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, pkey));
            //foreach (GreenhouseEntity entity in  table.ExecuteAsync(query))
            //{
            //    Console.WriteLine(entity.Temperature + " " + entity.PartitionKey);
            //}
            //// Code to fetch single record
            //TableOperation tableOperation = TableOperation.Retrieve<GreenhouseEntity>(pkey, rkey);
            //TableResult tableResult = table.ExecuteAsync(tableOperation).Result;
            //Console.WriteLine("Reading Temperature from Storage Table : "+((GreenhouseEntity)tableResult.Result).Temperature);

            ////// Code to fetch all records
            TableContinuationToken token = null;
            TableQuery<GreenhouseEntity> query = new TableQuery<GreenhouseEntity>()
    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Pkey));

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
                result = Math.Round(Convert.ToDouble(idealRange) / Convert.ToDouble(totalRecordsInTime) * 100, 2, MidpointRounding.AwayFromZero);
            }
            Console.WriteLine("% Prediction :" + result);
        }

        /// <summary>
        /// Run this function to read the data from table storage and upload to blob location
        /// </summary>
        public static async Task ReadDataAndUploadToBlob(string Pkey)
        {
            // First read data from Table Storage
            // Second create CSV
            // Upload to blob
            StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);
            CloudTableClient client = account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(tableName);
            try
            {
                List<GreenhouseEntity> listGreenhouseEntities = new List<GreenhouseEntity>();
                listGreenhouseEntities = await GetGreenHouseEntityAsync(table);
               await CreateCSVFile(listGreenhouseEntities);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during insert to storage" + ex);
            }
        }

        public static async Task<List<GreenhouseEntity>> GetGreenHouseEntityAsync( CloudTable table)
        {
            List<GreenhouseEntity> activities = new List<GreenhouseEntity>();
            //CloudTable cloudTable = TableConnection("NodeEvents");
            string filter = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.LessThanOrEqual, DateTime.Now.ToUniversalTime());
            TableContinuationToken continuationToken = null;

            do
            {
                var result = await table.ExecuteQuerySegmentedAsync(new TableQuery<GreenhouseEntity>().Where(filter), continuationToken);
                continuationToken = result.ContinuationToken;
                //int index = 0;
                if (result.Results != null)
                {
                    foreach (GreenhouseEntity entity in result.Results)
                    {
                        activities.Add(entity);
                        //index++;
                        //if (index == 500)
                        //    break;
                    }
                }

            } while (continuationToken != null);

            return activities;
        }
        public static async Task CreateCSVFile(List<GreenhouseEntity> listData, ExecutionContext context=null)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Timestamp", typeof(DateTimeOffset));
            dataTable.Columns.Add("Co2Level", typeof(double));
            dataTable.Columns.Add("Temperature", typeof(double));
            dataTable.Columns.Add("HumidityLevel", typeof(double));
            dataTable.Columns.Add("MositureLevel", typeof(double));
            dataTable.Columns.Add("SoilphValue", typeof(double));
            dataTable.Columns.Add("CropName", typeof(string));
            dataTable.Columns.Add("LightCount", typeof(double));
            //string formattedDate = date.ToString("yyyy-MM-dd HH:mm:ss");
            //Console.WriteLine(formattedDate);
            foreach (var item in listData)
            {
                dataTable.Rows.Add(item.Timestamp, item.Co2Level,item.Temperature,item.HumidityLevel,item.MositureLevel,item.SoilphValue,item.CropName,item.LightCount);
            }
            // Create CSV file
            //string localPath = context.FunctionAppDirectory;
            //string fileName = "Greenhous-data_" + DateTime.Now.Month+"_"+ DateTime.Now.Year + ".csv";
           // string localFilePath = Path.Combine(localPath, fileName);
            string fileName = "greenhouse-data" + "_" + DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmssfff") + ".csv";

            await Common.CreateCSVFileConsole(dataTable, fileName);
            // Upload to blob storage
           await Common.SaveFileToBlobConsole( fileName);
        }

        //private static async Task SaveFileToBlob(string filepath, ILogger log, string fileName)
        //{
        //    string blobconnection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        //    string containerName = Environment.GetEnvironmentVariable("ContainerName");
        //    //log.LogInformation(" :===: Task : PerformOperation --> 1.2 SaveFileToBlob has Started :===: ");
        //    //string blobconnection = "DefaultEndpointsProtocol=https;AccountName=storageaccountazarc;AccountKey=wKQKBjJdcwH+9t0/Q5ZKaUlihxyRo7wQf8RAqZ479cu4zxr4k5AF5Gh+sHKiDV1cjRX0y4nmHYKz+AStKWJ2Lg==;EndpointSuffix=core.windows.net";
        //    try
        //    {

        //        // START : Code to upload to blob
        //        BlobServiceClient blobServiceClient = new BlobServiceClient(blobconnection);

        //        bool bContainerExists = false;
        //        Azure.Storage.Blobs.BlobContainerClient containerClient = null;
        //        Azure.Storage.Blobs.Models.BlobContainerItem containerItem = null;

        //        // Create the container and return a container client object
        //        foreach (Azure.Storage.Blobs.Models.BlobContainerItem
        //        blobContainerItem in blobServiceClient.GetBlobContainers())
        //        {
        //            if (blobContainerItem.Name == containerName)
        //            {
        //                bContainerExists = true;
        //                containerItem = blobContainerItem;
        //                break;
        //            }
        //        }

        //        // Create or use existing Azure container as client.
        //        if (!bContainerExists)
        //        {
        //            containerClient = blobServiceClient.CreateBlobContainer(
        //         containerName);
        //        }
        //        else
        //            containerClient = blobServiceClient.GetBlobContainerClient(
        //         containerName);
        //        // Get a reference to a blob
        //        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        //        log.LogInformation(" Uploading to Blob storage as blob:\n\t {0}\n :===: ", blobClient.Uri);

        //        // Upload data from the local file
        //        await blobClient.UploadAsync(filepath, true);
        //        //log.LogInformation(" :===: Task : PerformOperation --> 1.2 SaveFileToBlob has Completed :===: ");
        //    }
        //    catch (Exception ex)
        //    {
        //        //log.LogError(" :===: Task : PerformOperation Failed --> 1.2 SaveFileToBlob Failed with  :===: " + ex);
        //    }

        //}
    }
}
