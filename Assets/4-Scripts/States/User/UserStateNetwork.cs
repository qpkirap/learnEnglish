namespace Game
{
    public class UserStateNetwork
    {
        public string username;
        public string email;

        public UserStateNetwork() {
        }

        public UserStateNetwork(string username, string email) {
            this.username = username;
            this.email = email;
        }
    }
}