using Unity.Collections;

namespace Game
{
    public class UserState
    {
        private string email;
        private string pass;
        
        public string Email => email;

        public string Pass => pass;

        public void SetData(FixedString512Bytes email, FixedString512Bytes pass)
        {
            this.email = email.Value;
            this.pass = pass.Value;
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