using System.Text;

namespace AtaRoomNet {
    public class PlayerPayload {
        public Player[] players;

        public int subject;

        public override string ToString() {
            var builder = new StringBuilder();
            builder.AppendLine($"Subject:   {subject}");
            builder.AppendLine($"Players:");
            foreach(var player in players) {
                builder.AppendLine($"\t{player.id} - {player.name}");
            }

            return builder.ToString();
        }
    }
}