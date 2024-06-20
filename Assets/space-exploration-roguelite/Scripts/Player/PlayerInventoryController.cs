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
                _inventoryItemSlotList.Add(new ItemSlot());
            }

            _actionbarMaxCapacity = Constants.PLAYER_ACTIONBAR_MAX_CAPACITY;
            for (int i = 0; i < _actionbarMaxCapacity; i++)
            {
                _actionbarItemSlotList.Add(new ItemSlot());
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

        public void ForceUpdateInventoryData(List<ItemSlot> itemSlotList)
        {

        }

        public void ForceUpdateActionbarData(List<ItemSlot> itemSlotList)
        {

        }

        #endregion
    }
}
