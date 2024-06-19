using UnityEngine;
using TMPro;

namespace SpaceExplorationRoguelite
{
    public class PlayerMenuHUDView : PlayerMenuView
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI _interactText;

        public void ToggleInteractText(bool toggle, string interactText = "")
        {
            _interactText.gameObject.SetActive(toggle);

            _interactText.text = interactText;
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
