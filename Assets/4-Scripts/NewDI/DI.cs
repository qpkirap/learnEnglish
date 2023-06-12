namespace Game
{
    public static class DI
    {
        public static T Get<T>(string tag = DITag.game) where T : class
        {
            return DInjector.GetCompositionRoot(tag).Get<T>();
        }

        public static void Add<T>(T obj, string tag = DITag.game) where T : class
        {
            DInjector.GetCompositionRoot(tag).Add(obj);
        }
    }
}