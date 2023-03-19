using TMPro;
using Unity.Entities;

namespace Game.ECS_UI.Components
{
    [GenerateAuthoringComponent]
    public class LeaderBottomBoardController : IComponentData
    {
        public TMP_Text currentNick;
    }
}