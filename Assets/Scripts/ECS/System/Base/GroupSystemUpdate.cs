using Unity.Entities;

namespace Game.ECS.System
{
    public abstract partial class InitSystemBase : SystemBase
    {
    }
    
    [UpdateAfter(typeof(InitSystemBase))]
    public abstract partial class UpdateSystem : SystemBase
    {
    }
    
    [UpdateAfter(typeof(UpdateSystem))]
    public abstract partial class PreMovementSystem : SystemBase
    {
    }
    
    [UpdateAfter(typeof(PreMovementSystem))]
    public abstract partial class MovementSystem : SystemBase
    {
    }
    
    [UpdateAfter(typeof(MovementSystem))]
    public abstract partial class PostMovementSystem : SystemBase
    {
    }

    [UpdateAfter(typeof(PostMovementSystem))]
    public abstract partial class PreDestroySystem : SystemBase
    {
    }
    
    [UpdateAfter(typeof(PreDestroySystem))]
    public abstract partial class DestroySystemBase : SystemBase
    {
    }
}