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
        [SerializeField] private PlayerController _playerController = null;
        public PlayerController PlayerController
        {
            get
            {
                return _playerController;
            }
        }

        #region Setup/Unsetup

        public override void Setup()
        {
            _maxActionbarCapacity = Constants.PLAYER_ACTIONBAR_MAX_CAPACITY;

            base.Setup();
        }

        public override void Unsetup()
        {
            base.Unsetup();
        }

        #endregion

        #region HUD Management

        public void ToggleInteractText(bool toggle, string interactText = "")
        {
            (_view as PlayerMenuHUDView).ToggleInteractText(toggle, interactText);
        }

        public void ToggleCrosshair(bool toggle)
        {
            (_view as PlayerMenuHUDView).ToggleCrosshair(toggle);
        }

        #endregion

        #region Actionbar Management

        public void SetPlayerController(PlayerController playerController)
        {
            _playerController = playerController;
        }

        public void UpdateActionbarSlotController(ItemSlot itemSlot)
        {
            (_view as PlayerMenuHUDView).UpdateActionbarSlotController(itemSlot);
        }

        public void UpdateActionbarSelection(int actionbarSlotIndex, out string actionbarSelectionItemID)
        {
            (_view as PlayerMenuHUDView).UpdateActionbarSelection(actionbarSlotIndex, out actionbarSelectionItemID);
        }

        #endregion
    }
}
