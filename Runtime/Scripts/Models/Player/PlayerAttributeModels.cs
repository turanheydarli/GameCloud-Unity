using System;

namespace GameCloud.Models
{
    [Serializable]
    public class PlayerAttribute
    {
        public string key;
        public object value;
        public string collection;
        public string updatedAt;
    }

    [Serializable]
    public class AttributeRequest
    {
        public string key;
        public object value;
    }

    [Serializable]
    public class AttributeCollection
    {
        public PlayerAttribute[] attributes;
    }
}