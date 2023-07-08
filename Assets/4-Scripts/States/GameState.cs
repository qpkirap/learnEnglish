using Game.Leaders;
using Game.Settings;
using JohanPolosn.UnityInjector;

namespace Game
{
    public class GameState
    {
        private readonly UserState userState = new();
        private readonly LeadersState leadersState = new();
        private readonly SettingsState settingsState = new();

        public UserState UserState => userState;
        public LeadersState LeadersState => leadersState;
        public SettingsState SettingsState => settingsState;

        public void LoadSave()
        {
            userState.LoadSave();
            
            DI.Add(this);
        }

        public void CreateSave()
        {
            userState.SaveData();
        }
    }
    
    public class LazyInject<T> where T : class
    {
        private T value;

        public T Value => value ??= DI.Get<T>();
    }
}