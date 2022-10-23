using Unity.Entities;
using UnityEngine;

namespace InitGame.GameResources.Base
{
    public interface IEntitySharedGameObjectConfig
    {
        ISharedComponentData CreateSharedComponentData(ref Entity entity);
        MonoBehaviour monoBehaviour { get; }
    }
}