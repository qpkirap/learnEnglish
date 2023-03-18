using Unity.Entities;
using UnityEngine;

namespace Game.ECS_UI.Components
{
    public class UIItemControllerBase : MonoBehaviour
    {
        [SerializeField] protected RectTransform root;
        
        protected Entity entity;
        protected EntityManager manager;

        public float currentWidth;

        public void Inject(Entity entity, EntityManager manager)
        {
            this.entity = entity;
            this.manager = manager;
        }
    }
}