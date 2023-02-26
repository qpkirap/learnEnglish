namespace Game
{
    public class UserState
    {
        private string email;
        private string pass;
        
        public string Email => email;

        public string Pass => pass;

        public void SetData(string email, string pass)
        {
            this.email = email;
            this.pass = pass;
        }

        public void SaveData()
        {
            ES3.Save(SaveKeys.emailKey, email);
            ES3.Save(SaveKeys.passKey, pass);
        }

        public void LoadSave()
        {
            email = ES3.LoadString(SaveKeys.emailKey, string.Empty);
            pass = ES3.LoadString(SaveKeys.passKey, string.Empty);
        }
    }
}