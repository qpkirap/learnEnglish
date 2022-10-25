using System.Collections.Generic;
using InitGame.GameResources.Adressables;
using Unity.Entities;
using UnityEngine;

public class Test : MonoBehaviour, IDeclareReferencedPrefabs
{
    [SerializeField] private WoodenTileBaseFactory testBaseFactory;
    void Start()
    {
        var test = testBaseFactory.GetFactoryConfig;
        var test2 = testBaseFactory.GetWoodTile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        throw new System.NotImplementedException();
    }
}
