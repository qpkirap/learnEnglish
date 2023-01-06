using CraftCar.ECS_UI.Components;
using CraftCar.ECS.Components;
using CraftCar.ECS.Components.SpawnData;
using Unity.Entities;

namespace CraftCar.ECS.System
{
    public partial class UpdateWordCardSystem: SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<CardTag, InstanceTag, DicElementData>().ForEach((Entity entity, in UICardControllerComponent uiCard, in DicElementData word) =>
            {
                uiCard.uiCardInstance.DescText1.text = word.ru.Value;
                uiCard.uiCardInstance.DescText2.text = word.en.Value;
            }).WithoutBurst().Run();
        }
    }
}