using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerMenuController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] protected Enums.PlayerMenuType _menuType;
        public Enums.PlayerMenuType MenuType
        {
            get
            {
                return _menuType;
            }
        }

        [Header("Components")]
        [SerializeField] protected PlayerMenuView _view;

        [Header("Runtime")]
        [SerializeField] protected bool _isActive = false;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
        }

        public virtual void Show()
        {
            if (_isActive)
            {
                return;
            }

            if (_view != null)
            {
                if (PlayerMenuControllerSingleton.Instance != null)
                {
                    _view.Show();

                    _isActive = true;

                    PlayerMenuControllerSingleton.Instance.PlayerMenuOpened(_menuType);
                }
                else
                {
                    Debug.LogError("PlayerMenuControllerSingleton not initialized!", this);
                }
            }
            else
            {
                Debug.LogError("PlayerMenuView not assigned to PlayerMenuController", this);
            }
        }

        public virtual void Hide()
        {
            if (!_isActive)
            {
                return;
            }

            if (_view != null && PlayerMenuControllerSingleton.Instance != null)
            {
                if (PlayerMenuControllerSingleton.Instance != null)
                {
                    _view.Hide();

                    _isActive = false;

                    PlayerMenuControllerSingleton.Instance.PlayerMenuClosed(_menuType);
                }
                else
                {
                    Debug.LogError("PlayerMenuControllerSingleton not initialized!", this);
                }
            }
            else
            {
                Debug.LogError("PlayerMenuView not assigned to PlayerMenuController", this);
            }
        }

        public void SetViewOrderIndex(int orderIndex)
        {
            if (_view != null)
            {
                _view.SetCanvasOrderIndex(orderIndex);
            }
            else
            {
                Debug.LogError("PlayerMenuView not assigned to PlayerMenuController", this);
            }
        }
    }
}
