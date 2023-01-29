using System;
using Godot;
using Newtonsoft.Json;

namespace AtaRoomNet
{

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class WSRequest
    {
        public string id;
        public long send;
        public string type;
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