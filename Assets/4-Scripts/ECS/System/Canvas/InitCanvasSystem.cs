using Game.ECS_UI.Components;
using Unity.Entities;

namespace Game.ECS.System
{
    public partial class InitCanvasSystem : UpdateSystem
    {
        private bool isInject;
        private UICanvasController canvas;
        private Entity canvasEntity;
        
        protected override void OnUpdate()
        {
            if (isInject) return;
            
            if (GetCanvas(out var entity) == null) return;

            var c = GetCanvas(out var e);
            
            c.LeaderBoard.Inject(e);

            isInject = true;
        }
        
        private UICanvasController GetCanvas(out Entity entity)
        {
            entity = canvasEntity;
            
            if (canvas != null)
            {
                return this.canvas;
            }

            Entities.WithAll<UICanvasController>().ForEach((Entity e, in UICanvasController canvasController) =>
            {
                canvas = canvasController;
                canvasEntity = e;

            }).WithoutBurst().Run();

            return canvas;
        }
    }
}