using Game.Leaders;
using JohanPolosn.UnityInjector;

namespace Game
{
    public class GameState
    {
        private readonly UserState userState = new();
        private readonly LeadersState leadersState = new();

        public UserState UserState => userState;
        public LeadersState LeadersState => leadersState;

        public void LoadSave()
        {
            userState.LoadSave();
            
            GlobalInjector.singleton.AddSingleton(this);
        }

        public void CreateSave()
        {
            userState.SaveData();
        }
    }
    
    public class LazyInject<T> where T : class
    {
        private T value;

        public T Value => value ??= GlobalInjector.singleton?.Get<T>();
    }
}