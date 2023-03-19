using TMPro;
using Unity.Entities;

namespace Game.ECS_UI.Components
{
    [GenerateAuthoringComponent]
    public class LeaderCurrentPointClickController : IComponentData
    {
        public TMP_Text currentClickPoint;
    }
}