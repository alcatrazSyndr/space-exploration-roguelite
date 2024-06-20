using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerMenuView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected PlayerMenuController _controller;
        [SerializeField] protected Canvas _canvas;

        public virtual void Setup()
        {

        }

        public virtual void PostSetup()
        {

        }

        public virtual void Unsetup()
        {

        }

        public virtual void Show()
        {
            if (_canvas != null)
            {
                _canvas.enabled = true;
                _canvas.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Canvas not assigned to PlayerMenuView", this);
            }
        }

        public virtual void Hide()
        {
            if (_canvas != null)
            {
                _canvas.enabled = false;
                _canvas.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Canvas not assigned to PlayerMenuView", this);
            }
        }

        public virtual void SetCanvasOrderIndex(int orderIndex)
        {
            if (_canvas != null)
            {
                _canvas.sortingOrder = orderIndex;
            }
            else
            {
                Debug.LogError("Canvas not assigned to PlayerMenuView", this);
            }
        }
    }
}
