using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerInteractionController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private LayerMask _interactableObjectLayerMask;
        [SerializeField] private float _interactableObjectMinDistance = 1f;

        [Header("Components")]
        [SerializeField] private Transform _cameraTransform;

        [Header("Runtime")]
        [SerializeField] private bool _setup = false;
        [SerializeField] private InteractableObjectController _currentInteractableObjectTarget = null;
        [SerializeField] private PlayerController _playerController = null;

        #region Setup/Unsetup/Update

        public void Setup(PlayerController playerController)
        {
            if (_setup)
            {
                return;
            }

            _playerController = playerController;

            _setup = true;

            if (PlayerMenuControllerSingleton.Instance != null)
            {
                PlayerMenuControllerSingleton.Instance.OnCurrentTopPlayerMenuChanged.AddListener(CurrentTopPlayerMenuChanged);
            }
        }

        public void Unsetup()
        {
            if (!_setup)
            {
                return;
            }

            _setup = false;

            if (PlayerMenuControllerSingleton.Instance != null)
            {
                PlayerMenuControllerSingleton.Instance.OnCurrentTopPlayerMenuChanged.RemoveListener(CurrentTopPlayerMenuChanged);
            }
        }

        private void Update()
        {
            if (!_setup)
            {
                return;
            }

            if (_playerController == null)
            {
                return;
            }

            RaycastHit[] hits;

            hits = Physics.RaycastAll(_cameraTransform.position, _cameraTransform.forward.normalized, _interactableObjectMinDistance, _interactableObjectLayerMask);

            if (hits.Length > 0)
            {
                InteractableObjectController firstHit = null;

                var hitList = new List<RaycastHit>(hits);

                hitList.Sort((hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

                for (int i = 0; i < hitList.Count; i++)
                {
                    var hit = hitList[i];

                    var interactableObjectController = hit.collider.GetComponent<InteractableObjectController>();
                    if (interactableObjectController != null && interactableObjectController.Interactable.Value)
                    {
                        firstHit = interactableObjectController;
                        break;
                    }
                }

                if (_currentInteractableObjectTarget != firstHit)
                {
                    _currentInteractableObjectTarget = firstHit;
                    InteractableObjectChanged();
                }
            }
            else if (_currentInteractableObjectTarget != null)
            {
                _currentInteractableObjectTarget = null;
                InteractableObjectChanged();
            }
        }

        #endregion

        #region Interaction

        public void InteractInput()
        {
            if (!_setup)
            {
                return;
            }

            if (_currentInteractableObjectTarget == null)
            {
                return;
            }

            if (_playerController == null)
            {
                return;
            }

            if (_currentInteractableObjectTarget != null && _currentInteractableObjectTarget.Interactable.Value)
            {
                _currentInteractableObjectTarget.Interact();

                if (_currentInteractableObjectTarget.ControllableObject != null)
                {
                    if (_playerController.CurrentControlledObject != null)
                    {
                        return;
                    }

                    _currentInteractableObjectTarget.ControllableObject.ClaimOwnership();
                }
            }
        }

        private void InteractableObjectChanged()
        {
            if (!_setup)
            {
                return;
            }

            if (PlayerMenuControllerSingleton.Instance != null)
            {
                var hudMenu = PlayerMenuControllerSingleton.Instance.GetPlayerMenuController(Enums.PlayerMenuType.HUD);
                var currentTopMenu = PlayerMenuControllerSingleton.Instance.CurrentTopMenu();

                if (currentTopMenu == hudMenu && _currentInteractableObjectTarget != null)
                {
                    (hudMenu as PlayerMenuHUDController).ToggleInteractText(true, _currentInteractableObjectTarget.InteractActionText);
                }
                else
                {
                    (hudMenu as PlayerMenuHUDController).ToggleInteractText(false);
                }
            }
        }

        private void CurrentTopPlayerMenuChanged(PlayerMenuController topMenu)
        {
            if (!_setup)
            {
                return;
            }

            InteractableObjectChanged();
        }

        #endregion
    }
}
