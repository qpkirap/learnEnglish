using System;
using System.Collections.Generic;

namespace Game
{
    public sealed class CompositionRoot : ICompositionRoot, IDisposable
    {
        private CompositionRoot dependency;
        private List<CompositionRoot> childs;

        private Dictionary<Type, object> objectByType = new Dictionary<Type, object>();

        public CompositionRoot(string tag)
        {
            this.Tag = tag;
        }

        public string Tag { get; private set; }

        public bool Disposed { get; private set; }

        public void Add<T>(T obj) where T : class
        {
            if (obj == null) return;
            Add(typeof(T), obj);
        }

        public void Add(Type t, object obj) => objectByType[t] = obj;

        public T Get<T>() where T : class
        {
            if (Disposed) return null;

            var type = typeof(T);
            object obj;

            if (objectByType.TryGetValue(type, out obj)) return obj as T;
            if (dependency != null) return dependency.Get<T>();
            return null;
        }

        public void SetDependency(CompositionRoot dependencyRoot)
        {
            if (Disposed)
            {
                UnityEngine.Debug.LogWarning($"Can't set dependncy to Disposed CompositionRoot. Tag \"{Tag}\"");
                return;
            }

            if (dependencyRoot == this)
            {
                UnityEngine.Debug.LogWarning($"Can't set as dependency root as self. Tag \"{Tag}\"");
                return;
            }

            if (dependencyRoot == null || dependencyRoot.Disposed)
            {
                UnityEngine.Debug.LogWarning($"Can't set null or Disposed Root");
                return;
            }

            if (HasChild(dependencyRoot))
            {
                UnityEngine.Debug.LogWarning(
                    $"Can't set as dependency own child. Tag \"{Tag}\", DependencyTag \"{dependencyRoot.Tag}\"");
                return;
            }

            this.dependency = dependencyRoot;
            dependencyRoot.AddChild(this);
        }

        void System.IDisposable.Dispose()
        {
            if (Disposed) return;

            objectByType.Clear();
            objectByType = null;

            if (childs != null)
            {
                foreach (var child in childs)
                {
                    if (child == null || child.Disposed) continue;
                    if (child.dependency == this) child.dependency = null;
                }

                childs.Clear();
                childs = null;
            }

            if (dependency != null && !dependency.Disposed && dependency.childs != null)
            {
                dependency.childs.Remove(this);
                dependency = null;
            }

            Disposed = true;
        }

        private void AddChild(CompositionRoot child)
        {
            if (Disposed) return;

            if (childs == null) childs = new List<CompositionRoot>();
            childs.Add(child);
        }

        private bool HasChild(CompositionRoot child)
        {
            if (Disposed || childs == null) return false;
            return childs.Contains(child);
        }
    }
}