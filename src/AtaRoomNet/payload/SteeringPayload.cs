using Newtonsoft.Json;

namespace AtaRoomNet {

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class SteeringPayload {
        public float throttle;

        public float steering;

        public int config;
    }
}