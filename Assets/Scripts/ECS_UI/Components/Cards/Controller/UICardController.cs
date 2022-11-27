using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace CraftCar.ECS_UI.Components
{
    public class UICardController : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("First language")] 
        [SerializeField] private Text headText1;
        [SerializeField] private Text descText1; 
        [Header("Two language")] 
        [SerializeField] private Text headText2;
        [SerializeField] private Text descText2;

        [Header("Body")] 
        [SerializeField] private Button nexButton;

        public Text HeadText1 => headText1;

        public Text DescText1 => descText1;

        public Text HeadText2 => headText2;

        public Text DescText2 => descText2;

        public Button NexButton => nexButton;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddSharedComponentData(entity, new UICardControllerComponent()
            {
                uiCardController = this
            });
        }
    }
}