namespace Game.Settings
{
    public class SettingsState
    {
        public bool IsAutoNextCard { get; private set; }
        public bool IsActiveEngSpeech { get; private set; }

        public void SwitchAutoNextCard(bool isAutoNext)
        {
            IsAutoNextCard = isAutoNext;
        }
        
        public void SwitchEngSpeech(bool isActive)
        {
            IsActiveEngSpeech = isActive;
        }
    }
}