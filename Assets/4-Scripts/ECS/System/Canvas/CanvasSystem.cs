using Game.ECS_UI.Components;
using Unity.Entities;

namespace Game.ECS.System
{
    public partial class CanvasSystem : UpdateSystem
    {
        private UICanvasController canvas;

        protected override void OnUpdate()
        {
            if(GetCanvas() == null) return;
            
            
        }
        
        private UICanvasController GetCanvas()
        {
            if (canvas != null) return this.canvas;

            Entities.WithAll<UICanvasController>().ForEach((Entity e, in UICanvasController canvasController) =>
            {
                canvas = canvasController;
            }).WithoutBurst().Run();

            return canvas;
        }
    }
}