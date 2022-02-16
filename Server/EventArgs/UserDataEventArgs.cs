namespace Server.EventArgs
{
    public class UserDataEventArgs : System.EventArgs
    {
        public int Id { get; set; }

        public UserDataEventArgs(int id)
        {
            Id = id;
        }
    }
}
