using System;
using System.Text;
using Godot;
using Newtonsoft.Json;

namespace AtaRoomNet
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class WSResponse : Godot.Reference
    {
        public ResponseType type;

        public int sender;

        public string id;

        public long send;

        public long relay;

        public string name;

        public string payload;

        public override string ToString() {
            var builder = new StringBuilder();
            builder.AppendLine("==============WSResponse============");
            builder.AppendLine("Type:    " + System.Enum.GetName(typeof(ResponseType), type));
            builder.AppendLine($"ID:          {id}");
            builder.AppendLine($"Sender:      {sender}");
            builder.AppendLine($"Name:        {name}");
            builder.AppendLine($"Sent:        {send}");
            builder.AppendLine($"Relayed:     {relay}");
            builder.AppendLine($"Raw Payload: {payload}");
            builder.AppendLine("====================================");

            return builder.ToString();
        }

        public TPayload ParsePayload<TPayload>() where TPayload : class {
            if(string.IsNullOrEmpty(payload)) {
                return null;
            }

            try {
                var bytes = System.Convert.FromBase64String(payload);
                var decoded = System.Text.Encoding.UTF8.GetString(bytes);
                return JsonConvert.DeserializeObject<TPayload>(decoded);
            } catch(Exception e) {
                GD.PrintErr(e);
                return null;
            }
        }
    }
}
