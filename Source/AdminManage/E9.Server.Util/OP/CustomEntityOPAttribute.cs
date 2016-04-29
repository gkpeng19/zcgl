using System;
using System.Collections.Generic;
using System.Text;

namespace NM.Service
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomEntityOPAttribute : Attribute
    {

        public CustomEntityOPAttribute()
        {
        }

        public CustomEntityOPAttribute(string friendName)
        {
            FriendName = friendName;
        }

        public CustomEntityOPAttribute(Type entityType, string friendName)
        {
            EntityType = entityType;
            FriendName = friendName;
        }

        public Type EntityType { get; set; }

        public string FriendName { get; set; }

        public int Index { get; set; }
    }
}
