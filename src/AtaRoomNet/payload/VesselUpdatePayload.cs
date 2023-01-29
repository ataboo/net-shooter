
using Newtonsoft.Json;

namespace AtaRoomNet {

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
	public class VesselUpdatePayload {
		public string id;

		public string cls;

		public TransformPayload tran;

		public string owner_id;

		public SteeringPayload steer;
	}
}