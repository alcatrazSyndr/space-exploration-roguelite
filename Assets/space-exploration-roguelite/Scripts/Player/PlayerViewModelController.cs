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
        [SerializeField] private ViewModelController _currentViewModelController = null;

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
                    if (itemDataSO.ViewModelPrefab != null)
                    {
                        _currentViewModelGO = Instantiate(itemDataSO.ViewModelPrefab, _viewModelRoot);
                        if (_currentViewModelGO.TryGetComponent<ViewModelController>(out _currentViewModelController))
                        {
                            _currentViewModelController.Setup(_playerController, itemDataSO);
                        }
                    }
                }
            }
        }

        private void ResetViewModel()
        {
            if (_currentViewModelController != null)
            {
                _currentViewModelController.Unsetup();
            }

            if (_currentViewModelGO != null)
            {
                Destroy(_currentViewModelGO);
                _currentViewModelGO = null;
            }
        }

        public void PrimaryActionInputChanged(bool input)
        {
            if (_currentViewModelController != null)
            {
                _currentViewModelController.PrimaryActionInputChanged(input);
            }
        }

        #endregion
    }
}
