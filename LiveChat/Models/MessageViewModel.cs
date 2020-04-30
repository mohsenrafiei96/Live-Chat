using System;

namespace LiveChat.Models
{
    public class MessageViewModel
    {
        public string SenderName { get; set; }
        public string Text{ get; set; }
        public DateTime SendAt { get; set; }
    }
}