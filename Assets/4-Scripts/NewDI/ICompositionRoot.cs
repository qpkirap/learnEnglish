namespace Game
{
    public interface ICompositionRoot
    {
        string Tag { get; }

        void Add<T>(T obj) where T : class;
        T Get<T>() where T : class;
    }
}