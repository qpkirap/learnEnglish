using Unity.Entities;

namespace Game.ECS.System
{
    public class InitSystemGroup : ComponentSystemGroup
    {
    }
    
    
    [UpdateInGroup(typeof(InitSystemGroup))]
    public abstract partial class InitSystemBase : SystemBase
    {
    }
    
    [UpdateAfter(typeof(InitSystemGroup))]
    public class UpdateSystemSystemGroup : ComponentSystemGroup
    {
    }
    
    [UpdateInGroup(typeof(UpdateSystemSystemGroup))]
    public abstract partial class UpdateSystem : SystemBase
    {
    }
    
    [UpdateAfter(typeof(UpdateSystemSystemGroup))]
    public class PreMovementSystemGroup : ComponentSystemGroup
    {
    }
    
    [UpdateInGroup(typeof(PreMovementSystemGroup))]
    public abstract partial class PreMovementSystem : SystemBase
    {
    }
    
    [UpdateAfter(typeof(PreMovementSystemGroup))]
    public class MovementSystemGroup : ComponentSystemGroup
    {
    }
    
    [UpdateInGroup(typeof(MovementSystemGroup))]
    public abstract partial class MovementSystem : SystemBase
    {
    }
    
    [UpdateAfter(typeof(MovementSystemGroup))]
    public class PostMovementSystemGroup : ComponentSystemGroup
    {
    }
    
    [UpdateInGroup(typeof(PostMovementSystemGroup))]
    public abstract partial class PostMovementSystem : SystemBase
    {
    }

    [UpdateAfter(typeof(PostMovementSystemGroup))]
    public class PreDestroySystemGroup : ComponentSystemGroup
    {
    }
    
    [UpdateInGroup(typeof(PreDestroySystemGroup))]
    public abstract partial class PreDestroySystem : SystemBase
    {
    }
    
    [UpdateAfter(typeof(PreDestroySystemGroup))]
    public class DestroySystemGroup : ComponentSystemGroup
    {
    }
    
    [UpdateInGroup(typeof(DestroySystemGroup))]
    public abstract partial class DestroySystemBase : SystemBase
    {
    }
}