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

        public string FirebaseId => firebaseId;
        
        public long PointClick => pointClick;
        
        public string Email => email;

        public string Pass => pass;

        public void SetData(string idFirebase, FixedString512Bytes email, FixedString512Bytes pass)
        {
            this.firebaseId = idFirebase;
            this.email = email.Value;
            this.pass = pass.Value;
        }

        public void SaveData()
        {
            ES3.Save(SaveKeys.emailKey, email);
            ES3.Save(SaveKeys.passKey, pass);
            ES3.Save(SaveKeys.firebaseIdKey, firebaseId);
            ES3.Save(SaveKeys.pointClickKey, pointClick);
        }

        public void LoadSave()
        {
            email = ES3.LoadString(SaveKeys.emailKey, string.Empty);
            pass = ES3.LoadString(SaveKeys.passKey, string.Empty);
            firebaseId = ES3.LoadString(SaveKeys.firebaseIdKey, String.Empty);
            pointClick = ES3.Load<long>(SaveKeys.pointClickKey, 0);
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