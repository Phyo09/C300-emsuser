using System;
using System.Collections.Generic;

namespace C300.Models
{
    public partial class Light
    {
        public int LightId { get; set; }
        public int LLevel { get; set; }
        public DateTime LDatetime { get; set; }
        public DateTime LrDatetime { get; set; }
    }
}
