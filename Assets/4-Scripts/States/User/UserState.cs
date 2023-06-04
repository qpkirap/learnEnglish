using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Unity.Collections;

namespace Game
{
    public partial class UserState
    {
        private long pointClick;

        private string firebaseId;

        private string email;
        private string pass;
        private string nick;
        private bool isRegisteredComplete;

        public string FirebaseId => firebaseId;
        
        public long PointClick => pointClick;
        
        public string Email => email;

        public string Pass => pass;
        public string Nick => nick;
        public bool IsRegisteredComplete => isRegisteredComplete;

        public void SetData(string idFirebase, FixedString512Bytes email, FixedString512Bytes pass,  FixedString512Bytes nick)
        {
            this.firebaseId = idFirebase;
            this.email = email.Value;
            this.pass = pass.Value;
            this.nick = nick.Value;
            this.isRegisteredComplete = true;
        }

        public void UpgradePointClick(int step = 1)
        {
            pointClick += step;
            SaveData();
        }

        public void SaveData()
        {
            ES3.Save(SaveKeys.emailKey, email);
            ES3.Save(SaveKeys.passKey, pass);
            ES3.Save(SaveKeys.firebaseIdKey, firebaseId);
            ES3.Save(SaveKeys.pointClickKey, pointClick);
            ES3.Save(SaveKeys.nickKey, nick);
            ES3.Save(SaveKeys.isRegComplete, isRegisteredComplete);
        }

        public void LoadSave()
        {
            email = ES3.LoadString(SaveKeys.emailKey, string.Empty);
            pass = ES3.LoadString(SaveKeys.passKey, string.Empty);
            firebaseId = ES3.LoadString(SaveKeys.firebaseIdKey, String.Empty);
            nick = ES3.LoadString(SaveKeys.nickKey, $"NewUser");
            pointClick = ES3.Load<long>(SaveKeys.pointClickKey, 0);
            isRegisteredComplete = ES3.Load<bool>(SaveKeys.isRegComplete, false);
        }
        
    }
    
    public partial class UserState
    {
        public static UserState FromJson(string json) =>
            JsonConvert.DeserializeObject<UserState>(json, Settings);
        
        public static readonly JsonSerializerSettings Settings = new()
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Formatting = Formatting.Indented,
            Converters =
            {
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            },
        };
        
        public string ToJson() => JsonConvert.SerializeObject(this, Settings);
    }
}