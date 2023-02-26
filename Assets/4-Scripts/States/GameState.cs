using Unity.Entities;

namespace Game
{
    public class GameState : IComponentData
    {
        private readonly UserState userState = new();

        public UserState UserState => userState;

        public void LoadSave()
        {
            userState.LoadSave();
        }

        public void CreateSave()
        {
            userState.SaveData();
        }
    }
}