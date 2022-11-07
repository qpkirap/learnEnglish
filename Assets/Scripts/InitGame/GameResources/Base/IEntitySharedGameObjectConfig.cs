using Unity.Entities;
using UnityEngine;

namespace CraftCar.InitGame.GameResources.Base
{
    public interface IEntitySharedGameObjectConfig
    {
        ISharedComponentData CreateSharedComponentData(ref Entity entity);
        MonoBehaviour monoBehaviour { get; }
    }
}