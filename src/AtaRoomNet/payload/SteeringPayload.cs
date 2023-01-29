using Newtonsoft.Json;

namespace AtaRoomNet {

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class SteeringPayload {
        public int throttle;

        public int steering;

        public int config;
    }
}