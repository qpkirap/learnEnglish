using Newtonsoft.Json;

namespace Game.Config
{
    [JsonObject(MemberSerialization.Fields)]
    public class DicJsonConfig
    {
        public readonly DicJsonDataArray[] _dataArrays;
    }

    [JsonObject(MemberSerialization.Fields)]
    public class DicJsonDataArray
    {
        public readonly int id;
        public readonly string en;
        public readonly string ru;
        public readonly string tr;
    }
}