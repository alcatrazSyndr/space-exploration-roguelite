using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerInventoryController : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private bool _setup = false;
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private List<ItemSlot> _inventoryItemSlotList = new List<ItemSlot>();
        [SerializeField] private List<ItemSlot> _actionbarItemSlotList = new List<ItemSlot>();
        [SerializeField] private int _inventoryMaxCapacity = 0;
        [SerializeField] private int _actionbarMaxCapacity = 0;

        #region Setup/Unsetup

        public void Setup(PlayerController playerController)
        {
            if (_setup)
            {
                return;
            }

            _playerController = playerController;

            _inventoryMaxCapacity = Constants.PLAYER_INVENTORY_MAX_CAPACITY;
            for (int i = 0; i < _inventoryMaxCapacity; i++)
            {
                var itemSlot = new ItemSlot();
                itemSlot.SlotIndex = i;

                _inventoryItemSlotList.Add(itemSlot);
            }

            _actionbarMaxCapacity = Constants.PLAYER_ACTIONBAR_MAX_CAPACITY;
            for (int i = 0; i < _actionbarMaxCapacity; i++)
            {
                var itemSlot = new ItemSlot();
                itemSlot.SlotIndex = i;

                _actionbarItemSlotList.Add(itemSlot);
            }

            _setup = true;
        }

        public void Unsetup()
        {
            if (!_setup)
            {
                return;
            }

            _setup = false;
        }

        #endregion

        #region Inventory Management

        public void ForceUpdateInventoryDataFromServer(List<ItemSlot> itemSlotList)
        {
            if (itemSlotList.Count != _inventoryItemSlotList.Count)
            {
                DebugLogManagerSingleton.LogMessage($"PlayerInventoryController:61\nForce Update Inventory Data error! Sent ItemSlot List count({itemSlotList.Count}) is different to cached ItemSlot List count({_inventoryItemSlotList.Count})!", Enums.DebugLogMessageType.Error, false);

                return;
            }

            for (int i = 0; i < itemSlotList.Count; i++)
            {
                var sentItemSlot = itemSlotList[i];
                var cachedItemSlot = _inventoryItemSlotList[i];

                if (!cachedItemSlot.ItemID.Equals(sentItemSlot.ItemID) || cachedItemSlot.ItemCount != sentItemSlot.ItemCount)
                {
                    // DIFFERENCE BETWEEN SERVER AND CLIENT

                    cachedItemSlot.ItemID = sentItemSlot.ItemID;
                    cachedItemSlot.ItemCount = sentItemSlot.ItemCount;

                    ItemSlotChanged(cachedItemSlot);
                }
            }
        }

        public void ForceUpdateActionbarDataFromServer(List<ItemSlot> itemSlotList)
        {
            if (itemSlotList.Count != _actionbarItemSlotList.Count)
            {
                DebugLogManagerSingleton.LogMessage($"PlayerInventoryController:71\nForce Update Actionbar Data error! Sent ItemSlot List count({itemSlotList.Count}) is different to cached ItemSlot List count({_actionbarItemSlotList.Count})!", Enums.DebugLogMessageType.Error, false);

                return;
            }

            for (int i = 0; i < itemSlotList.Count; i++)
            {
                var sentItemSlot = itemSlotList[i];
                var cachedItemSlot = _actionbarItemSlotList[i];

                if (!cachedItemSlot.ItemID.Equals(sentItemSlot.ItemID) || cachedItemSlot.ItemCount != sentItemSlot.ItemCount)
                {
                    // DIFFERENCE BETWEEN SERVER AND CLIENT

                    cachedItemSlot.ItemID = sentItemSlot.ItemID;
                    cachedItemSlot.ItemCount = sentItemSlot.ItemCount;

                    ItemSlotChanged(cachedItemSlot);
                }
            }
        }

        private void ItemSlotChanged(ItemSlot itemSlot)
        {
            if (PlayerMenuControllerSingleton.Instance != null)
            {
                if (_inventoryItemSlotList.Contains(itemSlot))
                {
                    var inventoryMenuController = PlayerMenuControllerSingleton.Instance.GetPlayerMenuController(Enums.PlayerMenuType.Inventory);

                    if (inventoryMenuController != null)
                    {
                        (inventoryMenuController as PlayerMenuInventoryController).UpdateInventorySlotController(itemSlot);
                    }
                }
                else if (_actionbarItemSlotList.Contains(itemSlot))
                {
                    var hudMenuController = PlayerMenuControllerSingleton.Instance.GetPlayerMenuController(Enums.PlayerMenuType.HUD);

                    if (hudMenuController != null)
                    {
                        (hudMenuController as PlayerMenuHUDController).UpdateActionbarSlotController(itemSlot);
                    }
                }
            }
        }

        #endregion
    }
}
