using System;

namespace ServerDemo.Forum
{
    public class ForumPost
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Content { get; set; }
        public DateTimeOffset PostedAt { get; set; }
    }
}