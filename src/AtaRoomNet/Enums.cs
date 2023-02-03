namespace AtaRoomNet {
    public struct EngineEvtType {
        public const string InSteerUpdate = "steer";
        public const string InPlayerJoin = "player";
        public const string InPlayerLeave = "leave";
        public const string InBoardVessel = "board";

        public const string OutPlayerReject = "reject";
        public const string OutVesselUpdate = "vessel";
        public const string OutCrewUpdate = "crew";
        public const string OutVesselRemove = "rm-vessel";
        public const string OutPlayerJoined = "joined";
        public const string OutCharUpdate = "chars";
        public const string OutWorldUpdate = "world";
    }
}