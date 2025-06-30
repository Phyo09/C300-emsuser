using System;
using System.Collections.Generic;

namespace C300.Models
{
    public partial class Event
    {
        public int EventId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? EndDay { get; set; }
        public string ThemeColor { get; set; }
        public byte? IsFullDay { get; set; }
    }
}
