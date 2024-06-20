using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerViewModelController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform _viewModelRoot;

        [Header("Runtime")]
        [SerializeField] private bool _setup = false;
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private GameObject _currentViewModelGO = null;

        #region Setup/Unsetup

        public void Setup(PlayerController playerController)
        {
            if (_setup)
            {
                return;
            }

            _playerController = playerController;

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

        #region View Model Management

        public void UpdateViewModel(string itemID)
        {
            ResetViewModel();

            if (!string.IsNullOrEmpty(itemID) && ItemDataManagerSingleton.Instance != null)
            {
                var itemDataSO = ItemDataManagerSingleton.Instance.GetItemDataSOWithItemID(itemID);

                if (itemDataSO != null)
                {
                    if (itemDataSO.ItemType == Enums.ItemType.Weapon)
                    {
                        var weaponDataSO = itemDataSO as WeaponDataSO;

                        if (weaponDataSO != null && weaponDataSO.ViewModelPrefab != null)
                        {
                            _currentViewModelGO = Instantiate(weaponDataSO.ViewModelPrefab, _viewModelRoot);
                        }
                    }
                }
            }
        }

        private void ResetViewModel()
        {
            if (_currentViewModelGO != null)
            {
                Destroy(_currentViewModelGO);
                _currentViewModelGO = null;
            }
        }

        #endregion
    }
}
