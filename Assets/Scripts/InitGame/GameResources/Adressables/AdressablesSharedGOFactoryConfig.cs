using System;
using UnityEngine;

namespace InitGame.GameResources.Adressables
{
    [CreateAssetMenu(menuName = "Car/AdressablesSharedGOFactoryConfig", fileName = "AdressablesSharedGOFactoryConfig")]
    public class AdressablesSharedGOFactoryConfig : ScriptableObject
    {
        public TestController TestControllerPrefab;
    }
}