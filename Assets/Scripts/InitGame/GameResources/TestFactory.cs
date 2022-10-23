using System;
using InitGame.GameResources.Adressables;
using UnityEngine;

namespace InitGame.GameResources
{
    [CreateAssetMenu(menuName = "Car/TestFactory", fileName = "TestFactory")]
    public class TestFactory : AdressablesSharedGOFactory
    {
        [SerializeField] private Test _test;
        public override AdressablesSharedGOFactoryConfig GetFactoryConfig => _test.config;
    }

    [Serializable]
    public class Test
    {
        public AdressablesSharedGOFactoryConfig config;
    }
}