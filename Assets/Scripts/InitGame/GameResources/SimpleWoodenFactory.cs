using System;
using CraftCar.InitGame.GameResources.Adressables;
using UnityEngine;

namespace CraftCar.InitGame.GameResources
{
    [CreateAssetMenu(menuName = "Car/SimpleWoodenFactory", fileName = "SimpleWoodenFactory")]
    public class SimpleWoodenFactory : WoodenTileBaseFactory
    {
        [SerializeField] private Test _test;
        public override WoodFactoryConfig GetFactoryConfig => _test.config;
    }

    [Serializable]
    public class Test
    {
        public WoodFactoryConfig config;
    }
}