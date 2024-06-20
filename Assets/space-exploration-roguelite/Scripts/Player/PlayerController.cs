using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerController : NetworkBehaviour
    {
        [Header("Prefabs - Client")]
        [SerializeField] private GameObject _playerCameraControllerPrefab;
        [SerializeField] private GameObject _playerInputControllerPrefab;
        [SerializeField] private GameObject _playerMenuControllerSingletonPrefab;
        [SerializeField] private GameObject _playerInventoryControllerPrefab;

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
        [SerializeField] private ControllableObjectController _currentControlledObject = null;
        public ControllableObjectController CurrentControlledObject
        {
            get
            {
                return _currentControlledObject;
            }
        }
        [SerializeField] private int _previousCameraPerspectiveTransformIndex = -1;
        [SerializeField] private Transform _currentCameraPerspectiveTransform = null;
        [SerializeField] private PlayerViewModelController _playerViewModelController = null;
        [SerializeField] private PlayerInventoryController _playerInventoryController = null;

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
                SetupCameraControllers();
                SetupInputController();
                SetupInventoryController();

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

                UnsetupCameraControllers();
                UnsetupInputController();
                UnsetupMenuControllerSingleton();
                UnsetupInventoryController();
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

            var hudMenuController = _playerMenuControllerSingleton.GetPlayerMenuController(Enums.PlayerMenuType.HUD);
            if (hudMenuController != null)
            {
                (hudMenuController as PlayerMenuHUDController).SetPlayerController(this);
            }

            var inventoryMenuController = _playerMenuControllerSingleton.GetPlayerMenuController(Enums.PlayerMenuType.Inventory);
            if (inventoryMenuController != null)
            {
                (inventoryMenuController as PlayerMenuInventoryController).SetPlayerController(this);
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
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerCameraController != null && _playerPawnTransform != null)
            {
                _playerCameraController.transform.position = _currentCameraPerspectiveTransform != null ? _currentCameraPerspectiveTransform.position : _playerPawnTransform.position;
                _playerCameraController.transform.rotation = _currentCameraPerspectiveTransform != null ? _currentCameraPerspectiveTransform.rotation : _playerPawnTransform.rotation;
            }
        }

        private void SetupCameraControllers()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerCameraController != null)
            {
                UnsetupCameraControllers();
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
                _playerInteractionController.Setup(this);
            }
            _playerViewModelController = playerCameraControllerGO.GetComponent<PlayerViewModelController>();
            if (_playerViewModelController != null)
            {
                _playerViewModelController.Setup(this);
            }
        }

        private void UnsetupCameraControllers()
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
            if (_playerViewModelController != null)
            {
                _playerViewModelController.Unsetup();
                _playerViewModelController = null;
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
            if (!base.IsOwner)
            {
                return;
            }

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
                _playerInputController.OnCameraPerspectiveInputPerformed.AddListener(CameraPerspectiveInput);
                _playerInputController.OnPlayerInventoryInputPerformed.AddListener(PlayerInventoryInput);
                _playerInputController.OnPlayerActionbarInputPerformed.AddListener(PlayerActionbarInput);
                _playerInputController.OnPrimaryActionInputChanged.AddListener(PrimaryActionInputChanged);
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
                _playerInputController.OnCameraPerspectiveInputPerformed.RemoveListener(CameraPerspectiveInput);
                _playerInputController.OnPlayerInventoryInputPerformed.RemoveListener(PlayerInventoryInput);
                _playerInputController.OnPlayerActionbarInputPerformed.RemoveListener(PlayerActionbarInput);
                _playerInputController.OnPrimaryActionInputChanged.RemoveListener(PrimaryActionInputChanged);

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

            if (_currentControlledObject != null && _currentControlledObject.Owner == base.Owner && _playerMenuControllerSingleton != null)
            {
                var shipHudMenu = _playerMenuControllerSingleton.GetPlayerMenuController(Enums.PlayerMenuType.ShipHUD);
                if (shipHudMenu != null)
                {
                    (shipHudMenu as PlayerMenuShipHUDController).FlightTargetInputChange(inputDelta);

                    _currentControlledObject.RotationInputChanged((shipHudMenu as PlayerMenuShipHUDController).GetCurrentFlightTargetPosition());
                }
            }
            else if (PlayerPawnController.Value != null)
            {
                if (PlayerPawnController.Value.ArtificialGravityController != null && _playerCameraController != null)
                {
                    _playerCameraController.RotateCameraOnLocalXAxis(inputDelta.y);
                }

                PlayerPawnController.Value.RotationInputChange(inputDelta);
            }
        }

        private void LeanInputChanged(float leanValue)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentControlledObject != null && _currentControlledObject.Owner == base.Owner)
            {
                _currentControlledObject.LeanInputChanged(leanValue);
            }
            else if (PlayerPawnController.Value != null)
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

            if (_currentControlledObject != null && _currentControlledObject.Owner == base.Owner)
            {
                _currentControlledObject.MovementInputChanged(input);
            }
            else if (PlayerPawnController.Value != null)
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

            if (_currentControlledObject != null && _currentControlledObject.Owner == base.Owner)
            {
                _currentControlledObject.CrouchInputChanged(crouch);
            }
            else if (PlayerPawnController.Value != null)
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

            if (_currentControlledObject != null && _currentControlledObject.Owner == base.Owner)
            {
                _currentControlledObject.JumpInputChanged(jump);
            }
            else if (PlayerPawnController.Value != null)
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

            if (_playerInteractionController != null && _currentControlledObject == null)
            {
                _playerInteractionController.InteractInput();
            }
            else if (_currentControlledObject != null)
            {
                ReleaseControlledObjectOwnership();
            }
        }

        private void CameraPerspectiveInput()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentControlledObject != null)
            {
                _previousCameraPerspectiveTransformIndex++;

                var canSeeShipHUD = false;
                _currentCameraPerspectiveTransform = _currentControlledObject.ControllableObjectOptionalCameraTransform(_previousCameraPerspectiveTransformIndex, out canSeeShipHUD);

                if (_currentCameraPerspectiveTransform == null)
                {
                    _previousCameraPerspectiveTransformIndex = -1;
                }

                var shipHUDMenu = _playerMenuControllerSingleton.GetPlayerMenuController(Enums.PlayerMenuType.ShipHUD);
                if (shipHUDMenu != null)
                {
                    (shipHUDMenu as PlayerMenuShipHUDController).ToggleShipHUD(canSeeShipHUD, _currentControlledObject.ControllableObjectType);
                }

                if (!canSeeShipHUD && _playerInputController != null)
                {
                    _playerInputController.CameraInputChangeOverride(Vector2.zero);
                }
            }
        }

        private void PlayerInventoryInput()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerMenuControllerSingleton != null)
            {
                var inventoryMenuController = _playerMenuControllerSingleton.GetPlayerMenuController(Enums.PlayerMenuType.Inventory);
                var hudMenuController = _playerMenuControllerSingleton.GetPlayerMenuController(Enums.PlayerMenuType.HUD);

                if (inventoryMenuController != null && _playerInputController != null && hudMenuController != null)
                {
                    if (_currentControlledObject != null)
                    {

                    }
                    else
                    {
                        if (inventoryMenuController.IsActive)
                        {
                            _playerMenuControllerSingleton.ClosePlayerMenu(Enums.PlayerMenuType.Inventory);

                            _playerInputController.ToggleCameraInput(true);

                            (hudMenuController as PlayerMenuHUDController).ToggleCrosshair(true);
                        }
                        else
                        {
                            _playerMenuControllerSingleton.OpenPlayerMenu(Enums.PlayerMenuType.Inventory);

                            _playerInputController.ToggleCameraInput(false);

                            (hudMenuController as PlayerMenuHUDController).ToggleCrosshair(false);
                        }
                    }
                }
            }
        }

        private void PlayerActionbarInput(int selectionInput)
        {
            if (!base.IsOwner)
            {
                return;
            }

            ActionbarSelectionChanged(selectionInput);
        }

        private void PrimaryActionInputChanged(bool input)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentControlledObject == null && _playerViewModelController != null)
            {
                EquippedItemPrimaryActionInputChange(input);
            }
        }

        #endregion

        #region Controlled Object

        public void CurrentControlledObjectChanged(ControllableObjectController controllableObject)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentControlledObject != null && controllableObject == null && PlayerPawnController.Value != null && _currentControlledObject.ControllableObjectExitTransform != null)
            {
                PlayerPawnController.Value.transform.position = _currentControlledObject.ControllableObjectExitTransform.position;
                PlayerPawnController.Value.transform.rotation = _currentControlledObject.ControllableObjectExitTransform.rotation;
            }
            else if (_currentControlledObject == null && controllableObject != null && PlayerPawnController.Value != null && controllableObject.ControllableObjectSeatTransform != null)
            {
                PlayerPawnController.Value.transform.position = controllableObject.ControllableObjectSeatTransform.position;
                PlayerPawnController.Value.transform.rotation = controllableObject.ControllableObjectSeatTransform.rotation;
            }
            PlayerPawnController.Value.CacheCurrentArtificialGravityLocalPosition();
            PlayerPawnController.Value.CacheCurrentArtificialGravityLocalRotation();

            if (_playerCameraController != null)
            {
                _playerCameraController.ResetCameraOnLocalXAxis();
            }

            _currentControlledObject = controllableObject;

            if (_playerMenuControllerSingleton != null && _playerInputController != null)
            {
                _playerInputController.ResetActionbarInput();
                _playerInputController.ToggleCameraInput(true);

                var hudMenuController = _playerMenuControllerSingleton.GetPlayerMenuController(Enums.PlayerMenuType.HUD);
                if (hudMenuController != null)
                {
                    (hudMenuController as PlayerMenuHUDController).ToggleCrosshair(true);
                }    

                if (_currentControlledObject != null)
                {
                    var inventoryMenuController = _playerMenuControllerSingleton.GetPlayerMenuController(Enums.PlayerMenuType.Inventory);
                    if (inventoryMenuController != null && inventoryMenuController.IsActive)
                    {
                        _playerMenuControllerSingleton.ClosePlayerMenu(Enums.PlayerMenuType.Inventory);
                    }

                    _playerMenuControllerSingleton.ClosePlayerMenu(Enums.PlayerMenuType.HUD);
                    _playerMenuControllerSingleton.OpenPlayerMenu(Enums.PlayerMenuType.ShipHUD);

                    var canSeeShipHUD = false;
                    _currentCameraPerspectiveTransform = _currentControlledObject.ControllableObjectOptionalCameraTransform(_previousCameraPerspectiveTransformIndex, out canSeeShipHUD);

                    if (_currentCameraPerspectiveTransform == null)
                    {
                        _previousCameraPerspectiveTransformIndex = -1;
                    }

                    var shipHUDMenu = _playerMenuControllerSingleton.GetPlayerMenuController(Enums.PlayerMenuType.ShipHUD);
                    if (shipHUDMenu != null)
                    {
                        (shipHUDMenu as PlayerMenuShipHUDController).ToggleShipHUD(canSeeShipHUD, _currentControlledObject.ControllableObjectType);
                    }

                    if (!canSeeShipHUD)
                    {
                        _playerInputController.CameraInputChangeOverride(Vector2.zero);
                    }
                }
                else
                {
                    _playerMenuControllerSingleton.ClosePlayerMenu(Enums.PlayerMenuType.ShipHUD);
                    _playerMenuControllerSingleton.OpenPlayerMenu(Enums.PlayerMenuType.HUD);

                    _currentCameraPerspectiveTransform = null;
                }
            }
        }

        private void ReleaseControlledObjectOwnership()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentControlledObject != null)
            {
                _currentControlledObject.ReleaseOwnership();
            }
        }

        #endregion

        #region Inventory Management

        private void SetupInventoryController()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerInventoryController != null)
            {
                UnsetupInventoryController();
            }

            var playerInventoryControllerGO = Instantiate(_playerInventoryControllerPrefab, transform);
            _playerInventoryController = playerInventoryControllerGO.GetComponent<PlayerInventoryController>();
            if (_playerInventoryController != null)
            {
                _playerInventoryController.Setup(this);
            }

            if (GameManagerSingleton.Instance != null)
            {
                GameManagerSingleton.Instance.RequestInitialInventoryDataFromServer(ClientManager.Connection);
            }
        }

        private void UnsetupInventoryController()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerInventoryController != null)
            {
                var playerInventoryControllerGO = _playerInventoryController.gameObject;
                _playerInventoryController.Unsetup();
                Destroy(playerInventoryControllerGO);
                _playerInventoryController = null;
            }
        }

        [TargetRpc]
        public void InventoryItemSlotDataReceiveFromServer(NetworkConnection playerConnection, string itemSlotDataJson)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerInventoryController == null)
            {
                return;
            }

            var itemSlotList = JsonConvert.DeserializeObject<List<ItemSlot>>(itemSlotDataJson);
            if (itemSlotList != null)
            {
                _playerInventoryController.ForceUpdateInventoryDataFromServer(itemSlotList);
            }
        }

        [TargetRpc]
        public void ActionbarItemSlotDataReceiveFromServer(NetworkConnection playerConnection, string itemSlotDataJson)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerInventoryController == null)
            {
                return;
            }

            var itemSlotList = JsonConvert.DeserializeObject<List<ItemSlot>>(itemSlotDataJson);
            if (itemSlotList != null)
            {
                _playerInventoryController.ForceUpdateActionbarDataFromServer(itemSlotList);
            }
        }

        #endregion

        #region Tool Management

        private void ActionbarSelectionChanged(int selectionInput)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_playerMenuControllerSingleton == null)
            {
                return;
            }

            var hudMenuController = _playerMenuControllerSingleton.GetPlayerMenuController(Enums.PlayerMenuType.HUD);
            var actionbarSelectionItemID = string.Empty;

            if (hudMenuController != null)
            {
                (hudMenuController as PlayerMenuHUDController).UpdateActionbarSelection(selectionInput, out actionbarSelectionItemID);
            }

            if (_playerViewModelController != null)
            {
                _playerViewModelController.UpdateViewModel(actionbarSelectionItemID);
            }
        }

        private void EquippedItemPrimaryActionInputChange(bool input)
        {
            if (!base.IsOwner)
            {
                return;
            }

            _playerViewModelController.PrimaryActionInputChanged(input);
        }

        #endregion
    }
}
