using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceSimulator.Model
{
    public class SmartDetector
    {
        public double co2Level { get; set; }
        public double temperature  { get; set; }
        public double humidityLevel { get; set; }
        public double mositureLevel { get; set; }
        public double lightCount { get; set; }
        public double lightCapacity { get; set; }
        public double lightDuration { get; set; }
        public bool fanStatus { get; set; }
        public bool lightingStatus { get; set; }
        public bool waterpumpStatus { get; set; }
        public bool dehumidifierStatus { get; set; }
        public CropInfo cropInfo { get; set; } 
        public DeviceInfo deviceInfo { get; set; }
        public LocationInfo locationInfo { get; set; }
    }

    public class CropInfo
    {
        public string cropType { get; set; }
        public string cropName { get; set; }
        public double soilphValue { get; set; }
    }

    public class DeviceInfo
    {
        public string deviceId { get; set; }
        public double deviceVersion { get; set; }
        public DateTime deviceTimestamp { get; set; }
    }
    public class LocationInfo
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public string timezone { get; set; }
    }
}
