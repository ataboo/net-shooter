using Newtonsoft.Json;

namespace AtaRoomNet {

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]

    public class TransformPayload {
        public PayloadVector pos;

        public PayloadVector vel;

        public double rot;

        public double hdg;
    }
}