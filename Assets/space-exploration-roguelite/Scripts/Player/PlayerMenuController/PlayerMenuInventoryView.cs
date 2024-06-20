using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceExplorationRoguelite
{
    public class PlayerMenuInventoryView : PlayerMenuView
    {
        [Header("Components")]
        [SerializeField] private RectTransform _inventorySlotControllerRoot;

        [Header("Prefabs")]
        [SerializeField] private GameObject _inventorySlotControllerPrefab;

        [Header("Runtime")]
        [SerializeField] private List<InventorySlotController> _inventorySlotControllerList = new List<InventorySlotController>();

        #region Setup/Unsetup

        public override void Setup()
        {
            base.Setup();

            var maxInventoryCapacity = (_controller as PlayerMenuInventoryController).MaxInventoryCapacity;

            for (int i = 0; i < maxInventoryCapacity; i++)
            {
                var inventorySlotControllerGO = Instantiate(_inventorySlotControllerPrefab, _inventorySlotControllerRoot);
                var inventorySlotController = inventorySlotControllerGO.GetComponent<InventorySlotController>();
                if (inventorySlotController != null)
                {
                    inventorySlotController.Setup();
                    _inventorySlotControllerList.Add(inventorySlotController);
                }
                else
                {
                    Destroy(inventorySlotControllerGO);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_inventorySlotControllerRoot);
        }

        public override void Unsetup()
        {
            base.Unsetup();

            for (int i = _inventorySlotControllerList.Count - 1; i >= 0; i--)
            {
                var inventorySlotController = _inventorySlotControllerList[i];
                inventorySlotController.Unsetup();
                _inventorySlotControllerList.RemoveAt(i);
                Destroy(inventorySlotController.gameObject);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_inventorySlotControllerRoot);
        }

        #endregion

        #region Inventory Management

        public void UpdateInventorySlotController(ItemSlot itemSlot)
        {
            var slotIndex = itemSlot.SlotIndex;

            if (slotIndex < 0 || slotIndex >= _inventorySlotControllerList.Count)
            {
                return;
            }

            _inventorySlotControllerList[slotIndex].UpdateItemSlot(itemSlot);
        }

        #endregion
    }
}
