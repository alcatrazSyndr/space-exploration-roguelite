using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerController : NetworkBehaviour
    {
        [Header("Prefabs - Client")]
        [SerializeField] private GameObject _playerCameraControllerPrefab;
        [SerializeField] private GameObject _playerInputControllerPrefab;

        [Header("Runtime - Client")]
        [SerializeField] private PlayerCameraController _playerCameraController = null;
        [SerializeField] private PlayerInputController _playerInputController = null;
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

                SetupCamera();
                SetupInput();
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

                UnsetupCamera();
                UnsetupInput();
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
                _playerCameraController.transform.position = _playerPawnTransform.position;
                _playerCameraController.transform.rotation = _playerPawnTransform.rotation;
            }
        }

        private void SetupCamera()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerCameraController != null)
            {
                UnsetupCamera();
            }

            var playerCameraControllerGO = Instantiate(_playerCameraControllerPrefab, transform);
            _playerCameraController = playerCameraControllerGO.GetComponent<PlayerCameraController>();
            if (_playerCameraController != null)
            {
                _playerCameraController.Setup();
            }
        }

        private void UnsetupCamera()
        {
            if (!base.IsOwner)
            {
                return;
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

        private void SetupInput()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerInputController != null)
            {
                UnsetupInput();
            }

            var playerInputControllerGO = Instantiate(_playerInputControllerPrefab, transform);
            _playerInputController = playerInputControllerGO.GetComponent<PlayerInputController>();
            if (_playerInputController != null)
            {
                _playerInputController.Setup();

                _playerInputController.OnCameraInputChanged.AddListener(CameraInputChanged);
                _playerInputController.OnMovementInputChanged.AddListener(MovementInputChanged);
                _playerInputController.OnInteractInputPerformed.AddListener(InteractInput);
            }
        }

        private void UnsetupInput()
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
        }

        private void MovementInputChanged(Vector2 input)
        {
            if (!base.IsOwner)
            {
                return;
            }
        }

        private void InteractInput()
        {
            if (!base.IsOwner)
            {
                return;
            }
        }

        #endregion
    }
}
