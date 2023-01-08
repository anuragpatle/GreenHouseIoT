using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DeviceSimulator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Sample 1: Create device if you didn't have one in Azure IoT Hub, FIRST YOU NEED SPECIFY connectionString first in AzureIoTHub.cs
            //await CreateDeviceIdentity();

            // Sample 2: comment above line and uncomment following line, FIRST YOU NEED SPECIFY connectingString and deviceConnectionString in AzureIoTHub.cs

            await SimulateDeviceToSendD2cAndReceiveD2c();

            // For C2D functionality : Send message to IoT hub
            //await SendCloudToDeviceMessage();

            // To test Decode message use below code to test
            //var msg = "eyJjbzJMZXZlbCI6ICIzODAiLCAidGVtcGVyYXR1cmUiOiAiMDAiLCAiaHVtaWRpdHlMZXZlbCI6ICIwMCIsICJtb3NpdHVyZUxldmVsIjogIjAwIiwgImxpZ2h0Q291bnQiOiAiMSIsICJsaWdodENhcGFjaXR5IjogIjIwIiwgImxpZ2h0RHVyYXRpb24iOiAiOCIsICJmYW5TdGF0dXMiOiAiZmFsc2UiLCAibGlnaHRpbmdTdGF0dXMiOiAiZmFsc2UiLCAid2F0ZXJwdW1wU3RhdHVzIjogImZhbHNlIiwgImRlaHVtaWRpZmllclN0YXR1cyI6ICJmYWxzZSIsICJjcm9wSW5mbyI6IHsiY3JvcFR5cGUiOiAiRnJ1aXRzIiwgImNyb3BOYW1lIjogIkxpdGNoaSIsICJzb2lscGhWYWx1ZSI6ICI0Ljc1In0sICJkZXZpY2VJbmZvIjogeyAiZGV2aWNlSWQiOiAic21hcnQtZGV0ZWN0b3ItMi4wIiwgImRldmljZVZlcnNpb24iOiAiMSIsICJkZXZpY2VUaW1lc3RhbXAiOiAiMjAyMi0xMC0xMVQxMjozMDowMy40NDY3NzE1WiIgfSwgImxvY2F0aW9uSW5mbyI6IHsgImxhdCI6ICIxOC41MTY3MjYiLCAibG9uIjogIjczLjg1NjI1NSIsICJ0aW1lem9uZSI6ICJDb29yZGluYXRlZCBVbml2ZXJzYWwgVGltZSIgfSAgfQ==";
            //var deviceMessageBody = "{'co2Level':660,'temperature':33,'humidityLevel':30.633030303030303,'mositureLevel':5,'lightCount':1,'lightCapacity':20,'lightDuration':8,'fanStatus':false,'lightingStatus':false,'waterpumpStatus':false,'dehumidifierStatus':false,'cropInfo':{ 'cropType':'Vegetables','cropName':'Tomato','soilphValue':8.25},'deviceInfo':{ 'deviceId':'smart-detector-1.0','deviceVersion':1,'deviceTimestamp':'2022-07-11T11:53:19.7075484Z'},'locationInfo':{ 'lat':18.516726,'lon':73.856255,'timezone':'Coordinated Universal Time'} }";

            //await EncodeDecode(msg);
            //await EncodeDecode(deviceMessageBody);
        }

        public static async Task CreateDeviceIdentity()
        {
            string deviceName = "smart-detector-1.0";
            await AzureIoTHub.CreateDeviceIdentityAsync(deviceName);
            Console.WriteLine($"Device with name '{deviceName}' was created/retrieved successfully");
        }

        private static async Task SimulateDeviceToSendD2cAndReceiveD2c()
        {

            var tokenSource = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                tokenSource.Cancel();
                Console.WriteLine("Exiting...");
            };
            Console.WriteLine("Press CTRL+C to exit");

            await Task.WhenAll(
                AzureIoTHub.SendDeviceToCloudMessageAsync(tokenSource.Token),
                AzureIoTHub.ReceiveMessagesFromDeviceAsync(tokenSource.Token));

            tokenSource.Dispose();
        }
        private static async Task SendCloudToDeviceMessage()
        {
            var tokenSource = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                tokenSource.Cancel();
                Console.WriteLine("Exiting...");
            };
            Console.WriteLine("Press CTRL+C to exit");

            await Task.WhenAll(
                //AzureIoTHub.SendCloudToDeviceMessageToADTAsync(tokenSource.Token),
                AzureIoTHub.SendCloudToDeviceMessageAsync(tokenSource.Token));
            //AzureIoTHub.ReceiveFeedbackAsync());

            tokenSource.Dispose();

        }
        private static async Task EncodeDecode(string msg)
        {
            var rawDeviceMessage = msg;
            JObject deviceMessage;
            if (await IsBase64Encoded(rawDeviceMessage))
            {
                var base64Bytes = System.Convert.FromBase64String(rawDeviceMessage);
                var msgDecoded = System.Text.Encoding.UTF8.GetString(base64Bytes);
                deviceMessage = (JObject)JsonConvert.DeserializeObject(msgDecoded);
            }
            else {
                deviceMessage = (JObject)JsonConvert.DeserializeObject(rawDeviceMessage);
            }

            Console.WriteLine("The message =>" + deviceMessage);
        }

        private static async Task<bool> IsBase64Encoded(String str)
        {
            //var axx = str.StartsWith('{');
            try
            {
                if (str.StartsWith('{'))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                // If exception is caught, then it is not a base64 encoded string
                return false;
            }
        }
    }
}
