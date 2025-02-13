using System;
using System.Collections.Generic;

namespace GameCloud.Models
{
    [Serializable]
    public class PlayerAttribute
    {
        public string username;
        public string collection;
        public string key;
        public string value;
        public string version;
        public Dictionary<string, object> permissionRead;
        public Dictionary<string, object> permissionWrite;
    }

    [Serializable]
    public class AttributeCollection
    {
        public Dictionary<string, PlayerAttribute> collections;
    }

    [Serializable]
    public class AttributeWriteRequest
    {
        public string key;
        public object value;
        public string expectedVersion;
        public int? expiresIn;
        public Dictionary<string, object> permissionRead;
        public Dictionary<string, object> permissionWrite;
    }
}