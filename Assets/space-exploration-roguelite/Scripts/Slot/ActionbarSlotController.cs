using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceExplorationRoguelite
{
    public class ActionbarSlotController : ItemSlotController
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI _actionbarIndexText;
        [SerializeField] private Image _actionbarSelectionObjectImage;

        [Header("Runtime")]
        [SerializeField] private bool _currentlySelected = false;
        public bool CurrentlySelected
        {
            get
            {
                return _currentlySelected;
            }
        }

        public void SetActionbarIndexText(int actionbarIndex)
        {
            _actionbarIndexText.text = actionbarIndex.ToString();
        }

        public void ToggleActionbarSelection(bool toggle)
        {
            _actionbarSelectionObjectImage.enabled = toggle;

            _currentlySelected = toggle;
        }
    }
}
