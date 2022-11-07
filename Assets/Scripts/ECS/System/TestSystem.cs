using Unity.Entities;
using UnityEngine;

namespace CraftCar.ECS.System
{
    //[UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
    public partial class TestSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Debug.Log("Update TestSystem");
            Entities.ForEach((TestComponentData input) =>
            {
                // Get the destination world entity associated with the authoring GameObject
                Debug.Log(input);
              
            }).WithoutBurst().Run();
        }
    }
}