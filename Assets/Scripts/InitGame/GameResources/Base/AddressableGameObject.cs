using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LearnEnglish.InitGame.GameResources
{
    [CreateAssetMenu(fileName = "AddressableGameObject", menuName = "Game/Configs/AddressableGameObject")]
    public class AddressableGameObject : ScriptableObject
    {
        [SerializeField] private AssetReference goAsset;

        private GameObject go;
        private AsyncOperationHandle handle;
        
        public bool IsRelease { get; private set; }
        public bool IsReady { get; private set; }
        
        public void Release()
        {
            Addressables.Release(handle);
            
            handle = default;
            go = default;
           
            IsRelease = true;
            IsReady = false;
        }

        public async UniTask<GameObject> GetGameObjectPrefabAsync(CancellationToken token)
        {
            if (go != null) return go;

            if (IsReady) return null;
            
            if (goAsset == null || !goAsset.RuntimeKeyIsValid())
            {
                Debug.LogError($"ключ не валиден {goAsset}");
                
                return null;
            }
            
            handle = Addressables.LoadAssetAsync<GameObject>(goAsset);

            var obj = await handle.Task;

            if (token.IsCancellationRequested) return null;

            if (handle.Status == AsyncOperationStatus.Succeeded && obj != null)
            {
                go = obj as GameObject;

                return go;
            }
            else
            {
                Debug.LogError($"Не удалось загрузить adressable key={goAsset}");
            }

            IsReady = true;
            
            return null;
        }
    }
}