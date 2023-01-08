using Azure.Messaging.EventHubs.Consumer;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DeviceSimulator.Model;
using System.Linq;

namespace DeviceSimulator
{
    public static class AzureIoTHub
    {
        /// <summary>
        /// Please replace with correct connection string value
        /// The connection string could be got from Azure IoT Hub -> Shared access policies -> iothubowner -> Connection String:
        /// </summary>
        private const string iotHubConnectionString = "HostName=ih-greenhouse.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=MC2XufRp+DKtUS9iuQVfESov/I3u+qx0gM6NYWu/C4A=";
        private static ServiceClient serviceClient;
        private static string iotConnectionString = "HostName=ih-greenhouse.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=MC2XufRp+DKtUS9iuQVfESov/I3u+qx0gM6NYWu/C4A=";
        private static string targetDevice = "smart-detector-1.0";
        /// <summary>
        /// Please replace with correct device connection string
        /// The device connect string could be got from Azure IoT Hub -> Devices -> {your device name } -> Connection string
        /// </summary>
        private const string deviceConnectionString = "HostName=ih-greenhouse.azure-devices.net;DeviceId=smart-detector-1.0;SharedAccessKey=0Pbb0a+Of7Ii8ApmsxmVdUTe1FNwOO5pi+eXN7bxKrs=";

        public static async Task<string> CreateDeviceIdentityAsync(string deviceName)
        {
            var registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
            var device = new Device(deviceName);
            try
            {
                device = await registryManager.AddDeviceAsync(device);
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceName);
            }

            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        public static async Task SendDeviceToCloudMessageAsync(CancellationToken cancelToken)
        {
            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString);

            //double avgTemperature = 26.0D;
            var rand = new Random();
            //var rangeNumber = rand.Next(17, 38);
           // var maximum = 36.2D;
            //var minimum = 12.5D;
            while (!cancelToken.IsCancellationRequested)
            {
               // double currentTemperature = avgTemperature + rand.NextDouble() * (maximum - minimum) + minimum;
                //double currentTemperature =  rand.NextDouble() * (maximum - minimum) + minimum;
                //var telemetryDataPoint = new
                //{
                //    Temperature = currentTemperature
                    
                //};
                var smartdeviceResponse=  BuildTheSmartDeviceResponse();

                var messageString = JsonSerializer.Serialize(smartdeviceResponse);
                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(messageString))
                {
                    ContentType = "application/json",
                    ContentEncoding = "utf-8"
                };
                await deviceClient.SendEventAsync(message);
                Console.WriteLine($"{DateTime.Now.ToUniversalTime()} > Sending message: {messageString}");
                
                //Keep this value above 1000 to keep a safe buffer above the ADT service limits
                //See https://aka.ms/adt-limits for more info
                await Task.Delay(30000);
            }
        }

        public static SmartDetector BuildTheSmartDeviceResponse()
        {
            //var rand = new Random();
            //var rangeNumber = rand.Next(17, 38);
            //double co2emissionFactor = 0.85;
            var rand = new Random();
            var tempeartureNumber = rand.Next(16, 40);
            var rand2 = new Random();
            var humidityNumber = rand2.Next(50, 95);
            var rand3 = new Random();
            var tempearturNumber2 = rand3.Next(20, 42);
            var rand4 = new Random();
            var mositureNumber = rand4.Next(1, 11);
            var rand5 = new Random();
            var rangeNumber5 = rand5.Next(1, 3);
            var rand6 = new Random();
            var rangeNumber6 = rand5.NextDouble();

            SmartDetector smartDetector = new SmartDetector();
            //smartDetector.co2Level = rangeNumber;
            //smartDetector.temperature = rangeNumber;
            //smartDetector.mositureLevel = rangeNumber * 100 - 48;
            //smartDetector.humidityLevel = rangeNumber * 19 / 33;
            smartDetector.fanStatus = false;
            smartDetector.lightingStatus = false;
            smartDetector.waterpumpStatus = false;
            smartDetector.dehumidifierStatus = false;
            smartDetector.lightCapacity = 20.0;
            smartDetector.lightDuration = 8;
            smartDetector.lightCount = rangeNumber5;
            // CO2 calculation =Input(KwH/hr) * (emission factor) 0.85
            // 1. no of lights more = more temperature
            // 2.more temperature = Co2 more
            //3.more temperature = less humidity and more co2
            // Co2 between 600 and 1,000 ppm
            if (smartDetector.lightCount == 1)
            {

                smartDetector.co2Level = tempeartureNumber * 1000 / 50;
                smartDetector.temperature = tempeartureNumber;
                smartDetector.mositureLevel = mositureNumber;
            }
            else
            {
                
                smartDetector.co2Level = Math.Round((tempeartureNumber * 1000 / 50) * rangeNumber6,2);
                smartDetector.temperature = tempeartureNumber * 0.25 + tempeartureNumber;
                smartDetector.mositureLevel = mositureNumber * 0.25 + mositureNumber;
            }
            smartDetector.humidityLevel = Math.Round((100 / smartDetector.temperature) * 10 + (smartDetector.temperature / 100),2);
            CropInfo _cropInfo = new CropInfo();
            _cropInfo.soilphValue = tempeartureNumber * 5.5 / 22;
            if (tempeartureNumber >= 27)
            {
                _cropInfo.cropType = "Vegetables";
                _cropInfo.cropName = "Tomato";
            }
            else {
                _cropInfo.cropType = "Fruits";
                _cropInfo.cropName = "Litchi";
            }

            smartDetector.cropInfo = _cropInfo;
            smartDetector.deviceInfo = new DeviceInfo {
                deviceId = "smart-detector-1.0",
                deviceVersion =1,
                deviceTimestamp = DateTime.UtcNow
            };
            TimeZone timeZone = TimeZone.CurrentTimeZone;
            
            smartDetector.locationInfo = new LocationInfo
            {
                lat = 18.516726,
                lon = 73.856255,
                timezone = timeZone.StandardName
            };
            return smartDetector;
        }
        
        public static async Task<string> ReceiveCloudToDeviceMessageAsync()
        {
            var oneSecond = TimeSpan.FromSeconds(1);
            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString);

            while (true)
            {
                var receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null)
                {
                    await Task.Delay(oneSecond);
                    continue;
                }

                var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                await deviceClient.CompleteAsync(receivedMessage);
                return messageData;
            }
        }

        public static async Task ReceiveMessagesFromDeviceAsync(CancellationToken cancelToken)
        {
            try
            {
                string eventHubConnectionString = await IotHubConnection.GetEventHubsConnectionStringAsync(iotHubConnectionString);
                await using var consumerClient = new EventHubConsumerClient(
                    EventHubConsumerClient.DefaultConsumerGroupName,
                    eventHubConnectionString);

                await foreach (PartitionEvent partitionEvent in consumerClient.ReadEventsAsync(cancelToken))
                {
                    if (partitionEvent.Data == null) continue;

                    string data = Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray());
                    Console.WriteLine($"Message received. Partition: {partitionEvent.Partition.PartitionId} Data: '{data}'");
                }
                await Task.Delay(30000);

            }
            catch (TaskCanceledException) { } // do nothing
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading event: {ex}");
            }
        }
        /// <summary>
        ///     This function will send the updated message to ADT 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task SendCloudToDeviceMessageToADTAsync(CancellationToken cancellationToken)
        {
            
        }
        /// <summary>
        /// This function will send the updated message to IoT
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task SendCloudToDeviceMessageAsync(CancellationToken cancellationToken)
        {
            //var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString);
            ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(iotConnectionString);
            var msg = "{'co2Level':256,'temperature':57,'humidityLevel':30.633030303030303,'mositureLevel':5,'lightCount':1,'lightCapacity':20,'lightDuration':8,'fanStatus':false,'lightingStatus':true,'waterpumpStatus':false,'dehumidifierStatus':false,'cropInfo':{ 'cropType':'Vegetables','cropName':'Tomato','soilphValue':8.25},'deviceInfo':{ 'deviceId':'smart-detector-1.0','deviceVersion':1,'deviceTimestamp':'2022-07-11T11:53:19.7075484Z'},'locationInfo':{ 'lat':18.516726,'lon':73.856255,'timezone':'Coordinated Universal Time'} }";
            //var smartdeviceResponse = msg;

            var messageString = JsonSerializer.Serialize(msg);
            var samplemsg = new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes(messageString));
            await serviceClient.SendAsync(targetDevice, samplemsg);
            samplemsg.Ack = Microsoft.Azure.Devices.DeliveryAcknowledgement.Full;

            Console.WriteLine($"{DateTime.Now.ToUniversalTime()} > Sending message: {messageString}");
            await ReceiveFeedbackAsync(serviceClient);
            //Keep this value above 1000 to keep a safe buffer above the ADT service limits
            //See https://aka.ms/adt-limits for more info
            await Task.Delay(30000);
        }
        public  static async Task ReceiveFeedbackAsync(ServiceClient serviceClient)
        {
            var feedbackReceiver = serviceClient.GetFeedbackReceiver();

            Console.WriteLine("\nReceiving c2d feedback from service");
            while (true)
            {
                var feedbackBatch = await feedbackReceiver.ReceiveAsync();
                if (feedbackBatch == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received feedback: {0}",
                  string.Join(", ", feedbackBatch.Records.Select(f => f.StatusCode)));
                Console.ResetColor();

                await feedbackReceiver.CompleteAsync(feedbackBatch);
            }
        }
    }
}
