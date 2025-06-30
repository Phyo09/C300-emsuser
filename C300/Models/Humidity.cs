using System;
using System.Collections.Generic;

namespace C300.Models
{
    public partial class Humidity
    {
        public int HumidityId { get; set; }
        public int HLevel { get; set; }
        public DateTime HDatetime { get; set; }
        public DateTime HrDatetime { get; set; }
    }
}
