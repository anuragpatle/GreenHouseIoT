using System;
using System.Collections.Generic;
using System.Text;

namespace SampleFunctionsApp.Model
{
    public class SmartAlertResponse : SmartDetector
    {
        public bool hasSchedule { get; set; }
        public List<ScheduleInfo> scheduleInfo { get; set; }
        public List<string> xAxisData { get; set; }
        public List<double> tempatureSeriesData { get; set; }
        public List<double> humiditySeriesData { get; set; }

        public double predictivePercentage { get; set; }
    }
    public class ScheduleInfo
    {
        public Guid scheduleId { get; set; }
        public string scheduleStarttime { get; set; }
        public string scheduleFinishtime { get; set; }
        public string scheduleFor { get; set; }
    }
}
