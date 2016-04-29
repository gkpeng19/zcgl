using System;
using NM.Util;

namespace NM.Model
{
    public class MessageItem : TJson
    {
        public MessageItem()
        {
            id = 0;
            message_id = 0;
            to_staffer_id = "";
            parameter = "";
        }
        public int id { get; set; }
        public int staffer_id { get; set; }
        public int message_id { get; set; }
        public string message { get; set; }
        public string to_staffer_id { get; set; }
        public string parameter { get; set; }
        public int response_id { get; set; }
        public string addon { get; set; }
    }
}
