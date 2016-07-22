using Newtonsoft.Json;
using System;

namespace ASPWiki.Model
{
    public class Session
    {
        [JsonProperty(PropertyName = "id")]
        public string Id;
        [JsonProperty(PropertyName = "token")]
        public string Token;
        [JsonProperty(PropertyName = "expires")]
        public DateTime Expires;
        [JsonProperty(PropertyName = "username")]
        public string Username;

        public override string ToString()
        {
            return "id:" + Id + " token:" + Token + " expires:" + Expires + " user:" + Username;
        }
    }
}
