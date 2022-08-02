using System;
using Godot;
using Newtonsoft.Json;

namespace AtaRoomNet
{

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class WSRequest
    {
        public RequestType type;
        public int sender;
        public string id;
        public long send;
        public long relay;
        public string name;
        public string payload;

        public void MarshalPayload<TPayload>(TPayload payloadObj) where TPayload : class {
            if(payloadObj == null) {
                payload = null;
                return;
            }

            try {
                var rawJSON = JsonConvert.SerializeObject(payloadObj);
                var jsonBytes = System.Text.Encoding.UTF8.GetBytes(rawJSON);
                payload = System.Convert.ToBase64String(jsonBytes); 
            } catch(Exception e) {
                GD.PrintErr(e);
            }
        }
    }
}