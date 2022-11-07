using System.Collections.Generic;
using CraftCar.InitGame.GameResources.Adressables;
using Cysharp.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

public class Test : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    [SerializeField] private WoodenTileBaseFactory testBaseFactory;
    [SerializeField] private GameObject testPrefabGo;
    private BlobAssetStore _blobAssetStore;
    private Entity testEntity;

    public WoodenTileBaseFactory TestBaseFactory => testBaseFactory;

    void Start()
    {
        _blobAssetStore = new BlobAssetStore();
        var test = TestCreatePrefab();
        _blobAssetStore.Dispose();
    }

    //метод 1 создает префаб и потом еще 3 компонентаа
    private async UniTask TestCreatePrefab()
    {
        // var prefab = await testBaseFactory.GetWoodTile();
        // GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        // var testEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
        // _blobAssetStore.Dispose();
        // var mngr = World.DefaultGameObjectInjectionWorld.EntityManager;
        // mngr.Instantiate(testEntity);
        // mngr.Instantiate(testEntity);
        // mngr.Instantiate(testEntity);
    }

    //как я понял должна быть система которая запросит этот гейм обжект и тогда он сконверится
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        testEntity = conversionSystem.GetPrimaryEntity(testPrefabGo);
        var mngr = World.DefaultGameObjectInjectionWorld.EntityManager;
        mngr.Instantiate(testEntity);
        mngr.Instantiate(testEntity);
        mngr.Instantiate(testEntity);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(testPrefabGo);
    }
}
