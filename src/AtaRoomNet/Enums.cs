namespace AtaRoomNet {
    public enum RequestType
    {
        GameEvtReq = 0,
        LeaveReq = 1,
        LockReq = 2,
        UnlockReq = 3,
    }
    public enum ResponseType
    {
        GameEvtRes = 0,
        JoinReject = 1,
        YouJoinRes = 2,
        PlayerJoinRes = 3,
        LeaveRes = 4,
        LockRes = 5,
        UnlockRes = 6
    }
}