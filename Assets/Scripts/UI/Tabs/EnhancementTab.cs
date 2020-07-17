using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EnhancementTab : Tab
    {
        [SerializeField] private Image _enhancementIcon = default;
        public void SetSprite(Sprite s)
        {
            _enhancementIcon.sprite = s;
        }
    }
}