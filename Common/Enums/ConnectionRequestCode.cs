namespace Common.Enums
{
    public enum ConnectionRequestCode
    {
        Connect,
        Disconnect = 1005,
        LoginIsAlreadyTaken = 1101,
        Inactivity = 1100,
        ServerNotResponding = 1006
    }
}
