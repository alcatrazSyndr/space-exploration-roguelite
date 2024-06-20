using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class ActionbarSlotController : ItemSlotController
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI _actionbarIndexText;

        public void SetActionbarIndexText(int actionbarIndex)
        {
            _actionbarIndexText.text = actionbarIndex.ToString();
        }
    }
}
