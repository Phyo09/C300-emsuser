using System;
using System.Collections.Generic;

namespace C300.Models
{
    public partial class Thread
    {
        public Thread()
        {
            Comment = new HashSet<Comment>();
        }

        public int ThreadId { get; set; }
        public string ThreadName { get; set; }
        public string ThreadDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TopicId { get; set; }
        public int UserId { get; set; }
        public int? CommentCount { get; set; }
        public int? Like { get; set; }

        public virtual ICollection<Comment> Comment { get; set; }
    }
}
