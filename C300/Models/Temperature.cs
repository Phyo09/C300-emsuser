using System;
using System.Collections.Generic;

namespace C300.Models
{
    public partial class Temperature
    {
        public int TemperatureId { get; set; }
        public int TLevel { get; set; }
        public DateTime TDatetime { get; set; }
        public DateTime TrDatetime { get; set; }
    }
}
