using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CraftCar.InitGame.GameResources.Adressables
{
    [CreateAssetMenu(menuName = "Car/WoodFactoryConfig", fileName = "WoodFactoryConfig")]
    public class WoodFactoryConfig : ScriptableObject
    {
        public AssetReference woodPrefab;
    }

    
}