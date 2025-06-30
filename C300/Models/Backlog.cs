using System;
using System.Collections.Generic;

namespace C300.Models
{
    public partial class Backlog
    {
        public int BacklogId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public DateTime Datetime { get; set; }
    }
}
