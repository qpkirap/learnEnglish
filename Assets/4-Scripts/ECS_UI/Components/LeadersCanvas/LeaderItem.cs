using TMPro;
using UnityEngine;

namespace Game.ECS_UI.Components
{
    public class LeaderItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text currentClickPoint;
        [SerializeField] private TMP_Text nameText;

        public void Inject(string name, string point)
        {
            nameText.text = name;
            currentClickPoint.text = point;
        }
    }
}