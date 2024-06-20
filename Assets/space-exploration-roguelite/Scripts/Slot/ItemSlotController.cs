using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SpaceExplorationRoguelite
{
    public class ItemSlotController : SlotController
    {
        [Header("Components")]
        [SerializeField] private Image _itemImage;
        [SerializeField] private TextMeshProUGUI _itemCountText;

        public virtual void UpdateItemSlot(ItemSlot itemSlot)
        {
            if (string.IsNullOrEmpty(itemSlot.ItemID) || itemSlot.ItemCount <= 0)
            {
                _itemImage.enabled = false;
                _itemCountText.text = string.Empty;

                return;
            }

            _itemCountText.text = itemSlot.ItemCount.ToString();
        }
    }
}
