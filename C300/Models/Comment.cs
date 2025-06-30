using System;
using System.Collections.Generic;

namespace C300.Models
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public int ThreadId { get; set; }
        public string Picture { get; set; }
        public int? Anonymous { get; set; }
        public int? Report { get; set; }
        public int? Like { get; set; }

        public virtual Thread Thread { get; set; }
    }
}
