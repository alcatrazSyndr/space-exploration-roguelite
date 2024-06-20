using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerMenuHUDController : PlayerMenuController
    {
        [Header("Runtime")]
        [SerializeField] private int _maxActionbarCapacity = 0;
        public int MaxActionbarCapacity
        {
            get
            {
                return _maxActionbarCapacity;
            }
        }

        public override void Setup()
        {
            _maxActionbarCapacity = Constants.PLAYER_ACTIONBAR_MAX_CAPACITY;

            base.Setup();
        }

        public void ToggleInteractText(bool toggle, string interactText = "")
        {
            (_view as PlayerMenuHUDView).ToggleInteractText(toggle, interactText);
        }

        public void ToggleCrosshair(bool toggle)
        {
            (_view as PlayerMenuHUDView).ToggleCrosshair(toggle);
        }
    }
}
