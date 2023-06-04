using Game.ECS_UI.Components;
using Unity.Entities;

namespace Game.ECS.System
{
    public partial class InitCanvasSystem : UpdateSystem
    {
        private bool isInject;
        private LazyInject<UICanvasController> canvas = new();
        private Entity canvasEntity;
        
        protected override void OnUpdate()
        {
            if (!HasSingleton<UICanvasControllerTag>()) return;
            
            if (isInject) return;
            
            if (GetCanvas(out var entity) == null) return;

            var c = GetCanvas(out var e);
            
            c.LeaderBoard.Inject(e);

            isInject = true;
        }
        
        private UICanvasController GetCanvas(out Entity entity)
        {
            var canvas = this.canvas.Value;

            entity = canvas.entity;

            return canvas;
        }
    }
}