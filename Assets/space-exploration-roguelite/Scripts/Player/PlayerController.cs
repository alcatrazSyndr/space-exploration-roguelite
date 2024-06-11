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
        [SerializeField] private GameObject _playerMenuControllerSingletonPrefab;

        [Header("Runtime - Client")]
        [SerializeField] private PlayerCameraController _playerCameraController = null;
        public Transform CameraTransform
        {
            get
            {
                if (_playerCameraController != null)
                {
                    return _playerCameraController.transform;
                }
                else
                {
                    return null;
                }
            }
        }
        [SerializeField] private PlayerInputController _playerInputController = null;
        [SerializeField] private PlayerInteractionController _playerInteractionController = null;
        [SerializeField] private PlayerMenuControllerSingleton _playerMenuControllerSingleton = null;
        [SerializeField] private Transform _playerPawnTransform = null;
        [SerializeField] private float _tickRate = 0f;

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
                    PlayerPawnController.Value.Setup(this);
                    _playerPawnTransform = PlayerPawnController.Value.transform;
                }

                PlayerPawnController.OnChange += OnPlayerPawnControllerChanged;

                SetupMenuControllerSingleton();
                SetupCameraAndInteractionController();
                SetupInputController();

                _playerMenuControllerSingleton.OpenPlayerMenu(Enums.PlayerMenuType.HUD);

                _tickRate = (float)TimeManager.TickDelta;
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
                UnsetupMenuControllerSingleton();
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
                next.Setup(this);
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

        #region Menu Controller Singleton

        private void SetupMenuControllerSingleton()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerMenuControllerSingleton != null)
            {
                UnsetupMenuControllerSingleton();
            }

            var playerMenuControllerSingletonGO = Instantiate(_playerMenuControllerSingletonPrefab);
            _playerMenuControllerSingleton = playerMenuControllerSingletonGO.GetComponent<PlayerMenuControllerSingleton>();
            if (_playerMenuControllerSingleton != null)
            {
                _playerMenuControllerSingleton.Setup();
            }
        }

        private void UnsetupMenuControllerSingleton()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerMenuControllerSingleton != null)
            {
                var playerMenuControllerSingletonGO = _playerMenuControllerSingleton.gameObject;
                _playerMenuControllerSingleton.Unsetup();
                Destroy(playerMenuControllerSingletonGO);
                _playerMenuControllerSingleton = null;
            }
        }

        #endregion

        #region Camera

        private void CameraTransformFollow()
        {
            if (_playerCameraController != null && _playerPawnTransform != null)
            {
                if (PlayerPawnController.Value != null && PlayerPawnController.Value.ArtificialGravityController != null && _playerInputController != null)
                {
                    var projectedForwardDirection = Vector3.ProjectOnPlane(_playerPawnTransform.forward, PlayerPawnController.Value.ArtificialGravityController.transform.up).normalized;

                    var targetRotation = Quaternion.LookRotation(projectedForwardDirection, PlayerPawnController.Value.ArtificialGravityController.transform.up);
                    var currentRotation = _playerPawnTransform.rotation;

                    var angle = Quaternion.Angle(targetRotation, currentRotation);

                    _playerCameraController.transform.position = _playerPawnTransform.position;
                    if (angle > 0f)
                    {
                        _playerCameraController.transform.rotation = _playerPawnTransform.rotation;
                    }
                    else
                    {
                        _playerCameraController.transform.rotation *= Quaternion.Euler(0f, _playerCameraController.RotationRate * _playerInputController.CurrentCameraInput.x, 0f);
                    }
                }
                else
                {
                    _playerCameraController.transform.position = _playerPawnTransform.position;
                    _playerCameraController.transform.rotation = _playerPawnTransform.rotation;
                }
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
                var playerCameraControllerGO = _playerCameraController.gameObject;
                _playerCameraController.Unsetup();
                Destroy(playerCameraControllerGO);
                _playerCameraController = null;
            }
        }

        public void ArtificialGravityControllerChanged()
        {
            if (PlayerPawnController.Value != null && _playerCameraController != null)
            {
                if (PlayerPawnController.Value.ArtificialGravityController != null)
                {
                    _playerCameraController.SetupArtificialGravity();
                }
                else
                {
                    _playerCameraController.UnsetupArtificialGravity();
                }
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

                var playerInputControllerGO = _playerInputController.gameObject;
                _playerInputController.Unsetup();
                Destroy(playerInputControllerGO);
                _playerInputController = null;
            }
        }

        private void CameraInputChanged(Vector2 inputDelta)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (PlayerPawnController.Value != null)
            {
                if (PlayerPawnController.Value.ArtificialGravityController != null && _playerCameraController != null)
                {
                    _playerCameraController.RotateCameraOnLocalXAxis(inputDelta.y);
                }
                else
                {
                    PlayerPawnController.Value.RotationInputChange(inputDelta);
                }
            }
        }

        private void LeanInputChanged(float leanValue)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (PlayerPawnController.Value != null)
            {
                PlayerPawnController.Value.LeanInputChange(leanValue);
            }
        }

        private void MovementInputChanged(Vector2 input)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (PlayerPawnController.Value != null)
            {
                PlayerPawnController.Value.MovementInputChange(input);
            }
        }

        private void CrouchInputChanged(bool crouch)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (PlayerPawnController.Value != null)
            {
                PlayerPawnController.Value.CrouchInputChanged(crouch);
            }
        }

        private void JumpInputChanged(bool jump)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (PlayerPawnController.Value != null)
            {
                PlayerPawnController.Value.JumpInputChanged(jump);
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
