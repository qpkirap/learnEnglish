using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CraftCar.InitGame.GameResources.Adressables
{
    [CreateAssetMenu(menuName = "LEARN/Factory/Config/ViewLearnFactoryConfig", fileName = "ViewLearnFactoryConfig")]
    public class ViewLearnFactoryConfig : ScriptableObject
    {
        public AssetReference uiCardPrefab;
    }
}