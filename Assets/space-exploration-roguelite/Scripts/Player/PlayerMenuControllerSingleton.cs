using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceExplorationRoguelite
{
    public class PlayerMenuControllerSingleton : MonoBehaviour
    {
        private static PlayerMenuControllerSingleton _instance = null;
        public static PlayerMenuControllerSingleton Instance
        {
            get
            {
                return _instance;
            }
        }

        [Header("Runtime")]
        [SerializeField] private Dictionary<Enums.PlayerMenuType, PlayerMenuController> _playerMenuTypeControllerDict = new Dictionary<Enums.PlayerMenuType, PlayerMenuController>();
        [SerializeField] private List<PlayerMenuController> _currentActivePlayerMenuControllerList = new List<PlayerMenuController>();
        public int CurrentTopPlayerMenuControllerCanvasOrder
        {
            get
            {
                return _currentActivePlayerMenuControllerList.Count;
            }
        }

        [Header("Events")]
        public UnityEvent<PlayerMenuController> OnCurrentTopPlayerMenuChanged = new UnityEvent<PlayerMenuController>();

        #region Setup/Unsetup

        public void Setup()
        {
            _instance = this;

            PopulateMenuTypeControllerDict();
        }

        public void Unsetup()
        {
            foreach (var menuController in _playerMenuTypeControllerDict.Values)
            {
                menuController.Unsetup();
            }

            _instance = null;

            _playerMenuTypeControllerDict.Clear();
            _currentActivePlayerMenuControllerList.Clear();
        }

        #endregion

        #region Menu Manipulation

        public void OpenPlayerMenu(Enums.PlayerMenuType playerMenuType)
        {
            if (_playerMenuTypeControllerDict.ContainsKey(playerMenuType))
            {
                _playerMenuTypeControllerDict[playerMenuType].Show();
            }
        }

        public void ClosePlayerMenu(Enums.PlayerMenuType playerMenuType)
        {
            if (_playerMenuTypeControllerDict.ContainsKey(playerMenuType))
            {
                _playerMenuTypeControllerDict[playerMenuType].Hide();
            }
        }

        private void PopulateMenuTypeControllerDict()
        {
            var playerMenuControllerArray = gameObject.GetComponentsInChildren<PlayerMenuController>(true);

            foreach (var playerMenuController in playerMenuControllerArray)
            {
                if (!_playerMenuTypeControllerDict.ContainsKey(playerMenuController.MenuType))
                {
                    _playerMenuTypeControllerDict.Add(playerMenuController.MenuType, playerMenuController);

                    playerMenuController.Setup();
                    playerMenuController.PostSetup();
                }
                else
                {
                    Debug.LogError("Duplicate MenuType controller found!", playerMenuController);
                }
            }
        }

        public void PlayerMenuOpened(Enums.PlayerMenuType playerMenuType)
        {
            if (_playerMenuTypeControllerDict.ContainsKey(playerMenuType))
            {
                if (!_currentActivePlayerMenuControllerList.Contains(_playerMenuTypeControllerDict[playerMenuType]))
                {
                    _currentActivePlayerMenuControllerList.Add(_playerMenuTypeControllerDict[playerMenuType]);
                }
                else
                {
                    _currentActivePlayerMenuControllerList.Remove(_playerMenuTypeControllerDict[playerMenuType]);
                    _currentActivePlayerMenuControllerList.Add(_playerMenuTypeControllerDict[playerMenuType]);
                }
            }
            else
            {
                PopulateMenuTypeControllerDict();
                if (_playerMenuTypeControllerDict.ContainsKey(playerMenuType))
                {
                    if (!_currentActivePlayerMenuControllerList.Contains(_playerMenuTypeControllerDict[playerMenuType]))
                    {
                        _currentActivePlayerMenuControllerList.Add(_playerMenuTypeControllerDict[playerMenuType]);
                    }
                    else
                    {
                        _currentActivePlayerMenuControllerList.Remove(_playerMenuTypeControllerDict[playerMenuType]);
                        _currentActivePlayerMenuControllerList.Add(_playerMenuTypeControllerDict[playerMenuType]);
                    }
                }
                else
                {
                    Debug.LogError("MenuType controller dict doesn't contain key " + playerMenuType.ToString(), this);
                }
            }

            UpdateMenuControllersOrderIndex();
        }

        public void PlayerMenuClosed(Enums.PlayerMenuType playerMenuType)
        {
            if (_playerMenuTypeControllerDict.ContainsKey(playerMenuType))
            {
                if (_currentActivePlayerMenuControllerList.Contains(_playerMenuTypeControllerDict[playerMenuType]))
                {
                    _currentActivePlayerMenuControllerList.Remove(_playerMenuTypeControllerDict[playerMenuType]);
                }
            }
            else
            {
                PopulateMenuTypeControllerDict();
                if (_playerMenuTypeControllerDict.ContainsKey(playerMenuType))
                {
                    if (_currentActivePlayerMenuControllerList.Contains(_playerMenuTypeControllerDict[playerMenuType]))
                    {
                        _currentActivePlayerMenuControllerList.Remove(_playerMenuTypeControllerDict[playerMenuType]);
                    }
                }
                else
                {
                    Debug.LogError("MenuType controller dict doesn't contain key " + playerMenuType.ToString(), this);
                }
            }

            UpdateMenuControllersOrderIndex();
        }

        public PlayerMenuController CurrentTopMenu()
        {
            if (_currentActivePlayerMenuControllerList.Count <= 0)
            {
                return null;
            }

            return _currentActivePlayerMenuControllerList[_currentActivePlayerMenuControllerList.Count - 1];
        }

        private void UpdateMenuControllersOrderIndex()
        {
            for (int i = 0; i < _currentActivePlayerMenuControllerList.Count; i++)
            {
                _currentActivePlayerMenuControllerList[i].SetViewOrderIndex(i);
            }

            OnCurrentTopPlayerMenuChanged?.Invoke(CurrentTopMenu());
        }

        public PlayerMenuController GetPlayerMenuController(Enums.PlayerMenuType playerMenuType)
        {
            if (_playerMenuTypeControllerDict.ContainsKey(playerMenuType))
            {
                return _playerMenuTypeControllerDict[playerMenuType];
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
