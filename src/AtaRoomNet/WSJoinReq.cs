using System;
using Godot;
using Newtonsoft.Json;

namespace AtaRoomNet {
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class WSJoinRequest {
        public string token;
        public string username;
    }
}