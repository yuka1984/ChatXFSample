using System;
using System.Collections.Generic;
using System.Text;

namespace ChatSample
{
    public class ChatMessageModel
    {
        public string UserName { get; set; }
        public string Message { get; set; }
        public long RoomId { get; set; }
        public DateTimeOffset RecieveTime { get; set; }
    }

    public class JoinMessage
    {
        public string MessageType { get; } = "JoinMessage";
        public string UserName { get; set; }
        public long? RoomId { get; set; }
    }
}
