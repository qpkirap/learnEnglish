using System;
using System.Collections.Generic;

namespace Game
{
    public static class DInjector
    {
        private static Dictionary<string, CompositionRoot> rootByTag = new Dictionary<string, CompositionRoot>();

        private static string lastRootTag;
        private static CompositionRoot lastRoot;

        public static event Action<CompositionRoot> OnRootCreated;

        public static ICompositionRoot GetCompositionRoot(string compositionTag)
        {
            if (string.Equals(lastRootTag, compositionTag, StringComparison.InvariantCulture))
            {
                if (lastRoot != null && !lastRoot.Disposed) return lastRoot;

                lastRootTag = null;
                lastRoot = null;
            }

            lastRootTag = compositionTag;
            return lastRoot = rootByTag[lastRootTag];
        }

        public static bool HasRootWithTag(string compositionTag) => rootByTag.ContainsKey(compositionTag);

        public static CompositionRoot CreateCompositionRoot(string compositionTag)
        {
            if (HasRootWithTag(compositionTag))
            {
                UnityEngine.Debug.LogError($"Create CompositionRoot with tag \"{compositionTag}\" twice.");
                return null;
            }

            var root = new CompositionRoot(compositionTag);
            rootByTag[compositionTag] = root;

            OnRootCreated?.Invoke(root);
            return root;
        }

        public static void DestroyCompositionRoot(CompositionRoot root)
        {
            if (root == null || root.Disposed) return;
            rootByTag.Remove(root.Tag);
            if (lastRoot == root)
            {
                lastRootTag = null;
                lastRoot = null;
            }

            ((System.IDisposable) root).Dispose();
        }

        public static void SetRootDependency(CompositionRoot root, string dependencyTag)
        {
            CompositionRoot dependency;
            rootByTag.TryGetValue(dependencyTag, out dependency);

            root.SetDependency(dependency);
        }
    }
}