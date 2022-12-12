using Unity.Entities;
using Unity.Scenes;
using NotImplementedException = System.NotImplementedException;

namespace CraftCar.ECS.System.Scene
{
    public class LoadSceneSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, SubScene scene) => {
                if (EntityManager.HasComponent<RequestSceneLoaded>(entity)) {
                    EntityManager.RemoveComponent<RequestSceneLoaded>(entity);
                    scene.gameObject.SetActive(true);
                } else {
                    EntityManager.AddComponent<RequestSceneLoaded>(entity);
                }
            });
        }
    }
}