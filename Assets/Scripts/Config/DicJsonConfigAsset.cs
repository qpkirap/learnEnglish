using UnityEngine;

namespace CraftCar.Config
{
    [CreateAssetMenu(menuName = "LEARN/Configs/DicJsonConfigAsset", fileName = "DicJsonConfigAsset")]

    public class DicJsonConfigAsset : ScriptableObject
    {
        [SerializeField] private TextAsset engToRuDicData;

        public TextAsset EngToRuDicData => engToRuDicData;
    }
}