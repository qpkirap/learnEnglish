using System;
using Cysharp.Threading.Tasks;
using Game.ECS_UI.Components;
using Unity.Entities;
using UnityEngine;

namespace Game.Config
{
    public abstract class CardMonoSharedComponent : Component
    {
        public abstract UniTask Init(Entity entity);
        public abstract UICardController GetInstance(Entity entity, RectTransform parent = null);

        public abstract Type GetSharedType { get; }
    }
}