using Unity.Entities;

namespace Game.ECS.Components
{
    public struct Timer : IComponentData
    {
        public float TimeLeft;
        public float Timescale;
        public float TotalTime;
        public float Percent => TimeLeft / TotalTime;

        public bool IsPaused;
        public bool IsScalable;
        public bool IsCompleted => TimeLeft <= 0;
    }
}