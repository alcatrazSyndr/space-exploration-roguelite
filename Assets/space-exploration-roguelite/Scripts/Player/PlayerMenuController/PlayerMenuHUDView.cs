using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SpaceExplorationRoguelite
{
    public class PlayerMenuHUDView : PlayerMenuView
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI _interactText;
        [SerializeField] private RectTransform _actionbarSlotControllerRoot;
        [SerializeField] private Image _crosshairImage;

        [Header("Prefabs")]
        [SerializeField] private GameObject _actionbarSlotControllerPrefab;

        [Header("Runtime")]
        [SerializeField] private List<ActionbarSlotController> _actionbarSlotControllerList = new List<ActionbarSlotController>();

        #region Setup/Unsetup

        public override void Setup()
        {
            base.Setup();

            var maxActionbarCapacity = (_controller as PlayerMenuHUDController).MaxActionbarCapacity;

            for (int i = 0; i < maxActionbarCapacity; i++)
            {
                var actionbarSlotControllerGO = Instantiate(_actionbarSlotControllerPrefab, _actionbarSlotControllerRoot);
                var actionbarSlotController = actionbarSlotControllerGO.GetComponent<ActionbarSlotController>();
                if (actionbarSlotController != null)
                {
                    actionbarSlotController.Setup();
                    actionbarSlotController.SetActionbarIndexText(i + 1);
                    _actionbarSlotControllerList.Add(actionbarSlotController);
                }
                else
                {
                    Destroy(actionbarSlotControllerGO);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_actionbarSlotControllerRoot);
        }

        public override void Unsetup()
        {
            base.Unsetup();

            for (int i = _actionbarSlotControllerList.Count - 1; i >= 0; i--)
            {
                var actionbarSlotController = _actionbarSlotControllerList[i];
                actionbarSlotController.Unsetup();
                _actionbarSlotControllerList.RemoveAt(i);
                Destroy(actionbarSlotController.gameObject);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_actionbarSlotControllerRoot);
        }

        #endregion

        #region Show/Hide

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        #endregion

        #region HUD Management

        public void ToggleInteractText(bool toggle, string interactText = "")
        {
            _interactText.gameObject.SetActive(toggle);

            _interactText.text = interactText;
        }

        public void ToggleCrosshair(bool toggle)
        {
            _crosshairImage.enabled = toggle;
        }

        #endregion

        #region Actionbar Management

        #endregion
    }
}
