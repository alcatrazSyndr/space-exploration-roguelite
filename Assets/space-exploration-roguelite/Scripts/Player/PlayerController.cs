using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerController : NetworkBehaviour
    {
        [Header("Data - Client")]
        [SerializeField] private float _playerCameraPositionSmoothing = 1f;
        [SerializeField] private float _playerCameraRotationSmoothing = 1f;

        [Header("Prefabs - Client")]
        [SerializeField] private GameObject _playerCameraControllerPrefab;
        [SerializeField] private GameObject _playerInputControllerPrefab;

        [Header("Runtime - Client")]
        [SerializeField] private PlayerCameraController _playerCameraController = null;
        [SerializeField] private PlayerInputController _playerInputController = null;
        [SerializeField] private PlayerInteractionController _playerInteractionController = null;
        [SerializeField] private Transform _playerPawnTransform = null;

        [Header("Runtime - Network")]
        public readonly SyncVar<PlayerPawnController> PlayerPawnController = new();

        #region Setup/Unsetup/Update

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (base.IsOwner)
            {
                if (PlayerPawnController.Value != null)
                {
                    PlayerPawnController.Value.Setup();
                    _playerPawnTransform = PlayerPawnController.Value.transform;
                }

                PlayerPawnController.OnChange += OnPlayerPawnControllerChanged;

                SetupCameraAndInteractionController();
                SetupInputController();
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (base.IsOwner)
            {
                if (PlayerPawnController.Value != null)
                {
                    PlayerPawnController.Value.Unsetup();
                    _playerPawnTransform = null;
                }

                PlayerPawnController.OnChange -= OnPlayerPawnControllerChanged;

                UnsetupCameraAndInteractionController();
                UnsetupInputController();
            }
        }

        private void OnPlayerPawnControllerChanged(PlayerPawnController prev, PlayerPawnController next, bool asServer)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (prev != null)
            {
                prev.Unsetup();
                _playerPawnTransform = null;
            }

            if (next != null)
            {
                next.Setup();
                _playerPawnTransform = next.transform;
            }
        }

        private void Update()
        {
            if (!base.IsOwner)
            {
                return;
            }

            CameraTransformFollow();
        }

        #endregion

        #region Camera

        private void CameraTransformFollow()
        {
            if (_playerCameraController != null && _playerPawnTransform != null)
            {
                _playerCameraController.transform.position = Vector3.Lerp(_playerCameraController.transform.position, _playerPawnTransform.position, Time.deltaTime * _playerCameraPositionSmoothing);
                _playerCameraController.transform.rotation = Quaternion.Lerp(_playerCameraController.transform.rotation, _playerPawnTransform.rotation, Time.deltaTime * _playerCameraRotationSmoothing);
            }
        }

        private void SetupCameraAndInteractionController()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerCameraController != null)
            {
                UnsetupCameraAndInteractionController();
            }

            var playerCameraControllerGO = Instantiate(_playerCameraControllerPrefab, transform);
            _playerCameraController = playerCameraControllerGO.GetComponent<PlayerCameraController>();
            if (_playerCameraController != null)
            {
                _playerCameraController.Setup();
            }
            _playerInteractionController = playerCameraControllerGO.GetComponent<PlayerInteractionController>();
            if (_playerInteractionController != null)
            {
                _playerInteractionController.Setup();
            }
        }

        private void UnsetupCameraAndInteractionController()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerInteractionController != null)
            {
                _playerInteractionController.Unsetup();
                _playerInteractionController = null;
            }
            if (_playerCameraController != null)
            {
                _playerCameraController.Unsetup();
                Destroy(_playerCameraController.gameObject);
                _playerCameraController = null;
            }
        }

        #endregion

        #region Input

        private void SetupInputController()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerInputController != null)
            {
                UnsetupInputController();
            }

            var playerInputControllerGO = Instantiate(_playerInputControllerPrefab, transform);
            _playerInputController = playerInputControllerGO.GetComponent<PlayerInputController>();
            if (_playerInputController != null)
            {
                _playerInputController.Setup();

                _playerInputController.OnCameraInputChanged.AddListener(CameraInputChanged);
                _playerInputController.OnMovementInputChanged.AddListener(MovementInputChanged);
                _playerInputController.OnInteractInputPerformed.AddListener(InteractInput);
                _playerInputController.OnLeanInputChanged.AddListener(LeanInputChanged);
                _playerInputController.OnJumpInputChanged.AddListener(JumpInputChanged);
                _playerInputController.OnCrouchInputChanged.AddListener(CrouchInputChanged);
            }
        }

        private void UnsetupInputController()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerInputController != null)
            {
                _playerInputController.OnCameraInputChanged.RemoveListener(CameraInputChanged);
                _playerInputController.OnMovementInputChanged.RemoveListener(MovementInputChanged);
                _playerInputController.OnInteractInputPerformed.RemoveListener(InteractInput);
                _playerInputController.OnLeanInputChanged.RemoveListener(LeanInputChanged);
                _playerInputController.OnJumpInputChanged.RemoveListener(JumpInputChanged);
                _playerInputController.OnCrouchInputChanged.RemoveListener(CrouchInputChanged);

                _playerInputController.Unsetup();
                Destroy(_playerInputController.gameObject);
                _playerInputController = null;
            }
        }

        private void CameraInputChanged(Vector2 inputDelta)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (PlayerPawnController.Value != null && _playerInputController != null && _playerCameraController != null)
            {
                var rotationInputVector = new Vector3(-inputDelta.y, inputDelta.x, _playerInputController.CurrentLeanInput);

                PlayerPawnController.Value.RotationInputChange(rotationInputVector);

                MovementInputChanged(_playerInputController.CurrentMovementInput);
            }
        }

        private void LeanInputChanged(float leanValue)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (PlayerPawnController.Value != null && _playerInputController != null)
            {
                PlayerPawnController.Value.LeanInputChange(leanValue);

                MovementInputChanged(_playerInputController.CurrentMovementInput);
            }
        }

        private void MovementInputChanged(Vector2 input)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (PlayerPawnController.Value != null && _playerCameraController != null && _playerInputController != null)
            {
                var upDownModifier = (_playerInputController.CurrentJumpInput ? 1f : 0f) + (_playerInputController.CurrentCrouchInput ? -1f : 0f);
                var movementInputVector = new Vector3(input.x, upDownModifier, input.y);
                var movementVector = _playerCameraController.CameraRelativeMovementInput(movementInputVector).normalized;

                PlayerPawnController.Value.MovementInputChange(movementVector);
            }
        }

        private void CrouchInputChanged(bool crouch)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerInputController != null)
            {
                MovementInputChanged(_playerInputController.CurrentMovementInput);
            }
        }

        private void JumpInputChanged(bool jump)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerInputController != null)
            {
                MovementInputChanged(_playerInputController.CurrentMovementInput);
            }
        }

        private void InteractInput()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerInteractionController != null)
            {
                _playerInteractionController.InteractInput();
            }
        }

        #endregion
    }
}
