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
        [SerializeField] protected Image _itemImage;
        [SerializeField] protected TextMeshProUGUI _itemCountText;

        [Header("Runtime")]
        [SerializeField] protected ItemSlot _itemSlot = null;
        public ItemSlot ItemSlot
        {
            get
            {
                return _itemSlot;
            }
        }

        public virtual void UpdateItemSlot(ItemSlot itemSlot)
        {
            _itemSlot = itemSlot;

            if (string.IsNullOrEmpty(itemSlot.ItemID) || itemSlot.ItemCount <= 0 || ItemDataManagerSingleton.Instance == null)
            {
                _itemImage.enabled = false;
                _itemCountText.text = string.Empty;

                return;
            }

            var itemDataSO = ItemDataManagerSingleton.Instance.GetItemDataSOWithItemID(itemSlot.ItemID);

            if (itemDataSO == null)
            {
                _itemImage.enabled = false;
                _itemCountText.text = string.Empty;

                return;
            }

            if (!itemDataSO.Stackable)
            {
                _itemCountText.text = string.Empty;
            }
            else
            {
                _itemCountText.text = itemSlot.ItemCount.ToString();
            }

            _itemImage.sprite = itemDataSO.ItemSprite;
            _itemImage.enabled = itemDataSO.ItemSprite != null;
        }
    }
}
