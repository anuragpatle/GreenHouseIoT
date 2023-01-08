using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace SampleFunctionsApp.TableEntities
{
    public class GreenhouseEntity : TableEntity
    {
        public GreenhouseEntity(){}
        private double co2Level;
        private double temperature;
        private double humidityLevel;
        private double mositureLevel;
        private double soilphValue;
        private string cropName;
        private double lightCount;
        private double lightCapacity;
        private double lightDuration;
        public double Co2Level
        {
            get
            {
                return co2Level;
            }

            set
            {
                co2Level = value;
            }
        }

        public double Temperature
        {
            get
            {
                return temperature;
            }

            set
            {
                temperature = value;
            }
        }
        public double HumidityLevel
        {
            get
            {
                return humidityLevel;
            }

            set
            {
                humidityLevel = value;
            }
        }
        public double MositureLevel
        {
            get
            {
                return mositureLevel;
            }

            set
            {
                mositureLevel = value;
            }
        }
        public double SoilphValue
        {
            get
            {
                return soilphValue;
            }

            set
            {
                soilphValue = value;
            }
        }
        public string CropName
        {
            get
            {
                return cropName;
            }

            set
            {
                cropName = value;
            }
        }

        public double LightCount
        {
            get
            {
                return lightCount;
            }
            set
            {
                lightCount = value;
            }
        }

        public double LightCapacity
        {
            get
            {
                return lightCapacity;
            }
            set
            {
                lightCapacity = value;
            }
        }

        public double LightDuration
        {
            get
            {
                return lightDuration;
            }
            set
            {
                lightDuration = value;
            }
        }
    }
}
