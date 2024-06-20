using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerMenuInventoryController : PlayerMenuController
    {
        [Header("Runtime")]
        [SerializeField] private int _maxInventoryCapacity = 0;
        public int MaxInventoryCapacity
        {
            get
            {
                return _maxInventoryCapacity;
            }
        }

        #region Setup/Unsetup

        public override void Setup()
        {
            _maxInventoryCapacity = Constants.PLAYER_INVENTORY_MAX_CAPACITY;

            base.Setup();
        }

        public override void Unsetup()
        {
            base.Unsetup();
        }

        #endregion

        #region Inventory Management

        #endregion
    }
}
