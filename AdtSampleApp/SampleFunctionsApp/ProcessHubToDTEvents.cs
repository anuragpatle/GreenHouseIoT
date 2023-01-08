using Azure;
using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SampleFunctionsApp.Model;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SampleFunctionsApp
{
    // This class processes telemetry events from IoT Hub, reads temperature of a device
    // and sets the "Temperature" property of the device with the value of the telemetry.
    public class ProcessHubToDTEvents
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static string adtServiceUrl = Environment.GetEnvironmentVariable("ADT_SERVICE_URL");

        [FunctionName("ProcessHubToDTEvents")]
        public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            // After this is deployed, you need to turn the Managed Identity Status to "On",
            // Grab Object Id of the function and assigned "Azure Digital Twins Owner (Preview)" role
            // to this function identity in order for this function to be authorized on ADT APIs.
            log.LogInformation("<----- ProcessHubToDTEvents STARTED -----> ");
            //Authenticate with Digital Twins
            var credentials = new DefaultAzureCredential();
            DigitalTwinsClient client = new DigitalTwinsClient(
                new Uri(adtServiceUrl), credentials, new DigitalTwinsClientOptions
                { Transport = new HttpClientTransport(httpClient) });
            log.LogInformation($"ADT service client connection created.");

            if (eventGridEvent != null && eventGridEvent.Data != null)
            {
                log.LogInformation(eventGridEvent.Data.ToString());

                // Reading deviceId and temperature for IoT Hub JSON
                JObject deviceMessage = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
                string deviceId = (string)deviceMessage["systemProperties"]["iothub-connection-device-id"];
                //var temperature = deviceMessage["body"]["Temperature"];

                log.LogInformation($"Device:{deviceId} ");

                var reslt = deviceMessage["body"].ToString();
                log.LogInformation("reslt : " + reslt);
                // Changes for : Base64 decode
                if (!reslt.StartsWith('{'))
                {
                    var base64Bytes = System.Convert.FromBase64String(reslt);
                    var msgDecoded = System.Text.Encoding.UTF8.GetString(base64Bytes);
                    log.LogInformation("Decoded Received Message : => " + msgDecoded);
                    await SetTwinProperties(deviceId, msgDecoded, client, log);
                }
                else {
                    await SetTwinProperties(deviceId, reslt, client, log);
                }
            }
            log.LogInformation("<----- ProcessHubToDTEvents COMPLETED -----> ");
        }
        private static async Task SetTwinProperties(string deviceId, string deviceMessageBody, DigitalTwinsClient client, ILogger log)
        {
            log.LogInformation(" :===: Task : SetTwinProperties has Started :===: ");
            //log.LogInformation(" Task deviceId : " + deviceId + ", deviceMessageBody " + deviceMessageBody + ", client " + client.ToString() + ", log " + log.ToString());
            var deviceMessage = JsonConvert.DeserializeObject<SmartDetector>(deviceMessageBody);
            log.LogInformation($"Device:{deviceId} has reading as :{deviceMessage}");
            try
            {
                if (deviceMessage != null)
                {
                    //Update device ADT
                    var updateTwinData = new JsonPatchDocument();
                    updateTwinData.AppendAdd("/CO2level", deviceMessage.co2Level);
                    updateTwinData.AppendAdd("/Temperature", deviceMessage.temperature);
                    updateTwinData.AppendAdd("/Humiditylevel", deviceMessage.humidityLevel);
                    updateTwinData.AppendAdd("/Moisturelevel", deviceMessage.mositureLevel);
                    updateTwinData.AppendAdd("/LightCount", deviceMessage.lightCount);
                    updateTwinData.AppendAdd("/LightCapacity", deviceMessage.lightCapacity);
                    updateTwinData.AppendAdd("/LightDuration", deviceMessage.lightDuration);
                    updateTwinData.AppendAdd("/WaterPumpStatus", deviceMessage.waterpumpStatus);
                    updateTwinData.AppendAdd("/DehumidifierStatus", deviceMessage.dehumidifierStatus);
                    updateTwinData.AppendAdd("/LightingStatus", deviceMessage.lightingStatus);
                    updateTwinData.AppendAdd("/FanStatus", deviceMessage.fanStatus);
                    updateTwinData.AppendAdd("/cropinfo/croptype", deviceMessage.cropInfo.cropType); 
                    updateTwinData.AppendAdd("/cropinfo/soilphvalue", deviceMessage.cropInfo.soilphValue);
                    updateTwinData.AppendAdd("/cropinfo/cropname", deviceMessage.cropInfo.cropName);
                    updateTwinData.AppendAdd("/deviceinfo/deviceid", deviceMessage.deviceInfo.deviceId);  
                    updateTwinData.AppendAdd("/deviceinfo/deviceversion", deviceMessage.deviceInfo.deviceVersion);
                    updateTwinData.AppendAdd("/deviceinfo/devicetimestamp", deviceMessage.deviceInfo.deviceTimestamp);
                    updateTwinData.AppendAdd("/locationinfo/lat", deviceMessage.locationInfo.lat);
                    updateTwinData.AppendAdd("/locationinfo/lon", deviceMessage.locationInfo.lon);
                    updateTwinData.AppendAdd("/locationinfo/timezone", deviceMessage.locationInfo.timezone);


                    await client.UpdateDigitalTwinAsync(deviceId, updateTwinData);
                }
                else
                {
                    log.LogError(" :===: Task : SetTwinProperties didn't updated due to missing device body list object  :===: ");
                }
                log.LogInformation(" :===: Task : SetTwinProperties has completed :===: ");
            }
            catch (Exception ex)
            {
                log.LogInformation($"SetTwinProperties has execption as--> :" + ex);
            }
        }
    }
}