using System;
using Cysharp.Threading.Tasks;
using Game.ECS_UI.Components;
using Unity.Entities;
using UnityEngine;

namespace Game.Config
{
    public abstract class PanelRegMonoSharedComponent : Component
    {
        public abstract UniTask Init(Entity entity);
        public abstract UIRegistrationPanel GetInstance(Entity entity, RectTransform parent = null);

        public abstract Type GetSharedType { get; }
    }
}