using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(menuName = "Game/Configs/DicJsonConfigAsset", fileName = "DicJsonConfigAsset")]

    public class DicJsonConfigAsset : ScriptableObject
    {
        [SerializeField] private TextAsset engToRuDicData;

        public TextAsset EngToRuDicData => engToRuDicData;
    }
}