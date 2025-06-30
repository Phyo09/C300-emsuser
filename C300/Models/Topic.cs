using System;
using System.Collections.Generic;

namespace C300.Models
{
    public partial class Topic
    {
        public int TopicId { get; set; }
        public string TopicName { get; set; }
        public string Description { get; set; }
        public int? ThreadCount { get; set; }
        public DateTime? DateTime { get; set; }
        public int UserId { get; set; }
    }
}
