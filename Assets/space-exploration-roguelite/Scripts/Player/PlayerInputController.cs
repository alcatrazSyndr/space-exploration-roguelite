using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace SpaceExplorationRoguelite
{
    public class PlayerInputController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private float _interactInputCooldown = 0.2f;

        [Header("Input Actions")]
        [SerializeField] private InputAction _movementInput;
        [SerializeField] private InputAction _cameraXInput;
        [SerializeField] private InputAction _cameraYInput;
        [SerializeField] private InputAction _interactInput;
        [SerializeField] private InputAction _jumpInput;
        [SerializeField] private InputAction _crouchInput;
        [SerializeField] private InputAction _leanInput;
        [SerializeField] private InputAction _cameraPerspectiveInput;
        [SerializeField] private InputAction _playerInventoryInput;
        [SerializeField] private InputAction _playerActionbarInput;
        [SerializeField] private InputAction _playerActionbarScrollInput;

        [Header("Runtime")]
        [SerializeField] private bool _setup = false;
        [SerializeField] private bool _canInteractInput = true;
        [SerializeField] private float _interactInputCurrentCooldown = 0f;
        [SerializeField] private Vector2 _currentCameraInput = Vector2.zero;
        public Vector2 CurrentCameraInput
        {
            get
            {
                return _currentCameraInput;
            }
        }
        [SerializeField] private Vector2 _currentMovementInput = Vector2.zero;
        public Vector2 CurrentMovementInput
        {
            get
            {
                return _currentMovementInput;
            }
        }
        [SerializeField] private float _currentLeanInput = 0f;
        public float CurrentLeanInput
        {
            get
            {
                return _currentLeanInput;
            }
        }
        [SerializeField] private bool _currentJumpInput = false;
        public bool CurrentJumpInput
        {
            get
            {
                return _currentJumpInput;
            }
        }
        [SerializeField] private bool _currentCrouchInput = false;
        public bool CurrentCrouchInput
        {
            get
            {
                return _currentCrouchInput;
            }
        }
        [SerializeField] private int _currentActionbarInput = -1;

        [Header("Events")]
        public UnityEvent<Vector2> OnCameraInputChanged = new UnityEvent<Vector2>();
        public UnityEvent<Vector2> OnMovementInputChanged = new UnityEvent<Vector2>();
        public UnityEvent OnInteractInputPerformed = new UnityEvent();
        public UnityEvent<float> OnLeanInputChanged = new UnityEvent<float>();
        public UnityEvent<bool> OnJumpInputChanged = new UnityEvent<bool>();
        public UnityEvent<bool> OnCrouchInputChanged = new UnityEvent<bool>();
        public UnityEvent OnCameraPerspectiveInputPerformed = new UnityEvent();
        public UnityEvent OnPlayerInventoryInputPerformed = new UnityEvent();
        public UnityEvent<int> OnPlayerActionbarInputPerformed = new UnityEvent<int>();
        public UnityEvent<int> OnPlayerActionbarScrollInputPerformed = new UnityEvent<int>();

        #region Setup/Unsetup/Update

        public void Setup()
        {
            if (_setup)
            {
                return;
            }
            _setup = true;

            InitializeCameraInput();
            ToggleCameraInput(true);

            InitializeInteractInput();
            ToggleInteractInput(true);

            InitializeCameraPerspectiveInput();
            ToggleCameraPerspectiveInput(true);

            InitializePlayerInventoryInput();
            TogglePlayerInventoryInput(true);

            InitializePlayerActionbarInput();
            TogglePlayerActionbarInput(true);
            TogglePlayerActionbarScrollInput(true);

            InitializeMovementInput();
            ToggleMovementInput(true);

            InitializeLeanInput();
            ToggleLeanInput(true);

            InitializeJumpInput();
            ToggleJumpInput(true);

            InitializeCrouchInput();
            ToggleCrouchInput(true);
        }

        public void Unsetup()
        {
            if (!_setup)
            {
                return;
            }
            _setup = false;

            ToggleCameraInput(false);
            DeinitializeCameraInput();

            ToggleInteractInput(false);
            DeinitializeInteractInput();

            ToggleCameraPerspectiveInput(false);
            DeinitializeCameraPerspectiveInput();

            TogglePlayerInventoryInput(false);
            DeinitializePlayerInventoryInput();

            TogglePlayerActionbarInput(false);
            TogglePlayerActionbarScrollInput(false);
            DeinitializePlayerActionbarInput();

            ToggleMovementInput(false);
            DeinitializeMovementInput();

            ToggleLeanInput(false);
            DeinitializeLeanInput();

            ToggleJumpInput(false);
            DeinitializeJumpInput();

            ToggleCrouchInput(false);
            DeinitializeCrouchInput();
        }

        private void FixedUpdate()
        {
            if (!_canInteractInput)
            {
                if (_interactInputCurrentCooldown > 0f)
                {
                    _interactInputCurrentCooldown -= Time.fixedDeltaTime;
                }
                else
                {
                    _canInteractInput = true;
                    _interactInputCurrentCooldown = 0f;
                }
            }
        }

        #endregion

        #region Camera Input

        private void InitializeCameraInput()
        {
            _cameraXInput.performed += CameraInputChanged;
            _cameraXInput.canceled += CameraInputChanged;

            _cameraYInput.performed += CameraInputChanged;
            _cameraYInput.canceled += CameraInputChanged;
        }

        private void DeinitializeCameraInput()
        {
            _cameraXInput.performed -= CameraInputChanged;
            _cameraXInput.canceled -= CameraInputChanged;

            _cameraYInput.performed -= CameraInputChanged;
            _cameraYInput.canceled -= CameraInputChanged;
        }

        public void ToggleCameraInput(bool toggle)
        {
            _currentCameraInput = Vector2.zero;

            if (toggle)
            {
                _cameraXInput.Enable();
                _cameraYInput.Enable();

                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                _cameraXInput.Disable();
                _cameraYInput.Disable();

                Cursor.lockState = CursorLockMode.None;
            }

            OnCameraInputChanged?.Invoke(_currentCameraInput);
        }

        private void CameraInputChanged(InputAction.CallbackContext context)
        {
            if (context.action == _cameraXInput)
            {
                _currentCameraInput.x = context.ReadValue<float>();
            }
            else if (context.action == _cameraYInput)
            {
                _currentCameraInput.y = context.ReadValue<float>();
            }

            OnCameraInputChanged?.Invoke(_currentCameraInput);
        }

        public void CameraInputChangeOverride(Vector2 inputOverride)
        {
            _currentCameraInput = inputOverride;

            OnCameraInputChanged?.Invoke(_currentCameraInput);
        }

        #endregion

        #region Movement Input

        private void InitializeMovementInput()
        {
            _movementInput.performed += MovementInputChanged;
            _movementInput.canceled += MovementInputChanged;
        }

        private void DeinitializeMovementInput()
        {
            _movementInput.performed -= MovementInputChanged;
            _movementInput.canceled -= MovementInputChanged;
        }

        public void ToggleMovementInput(bool toggle)
        {
            _currentMovementInput = Vector2.zero;

            if (toggle)
            {
                _movementInput.Enable();
            }
            else
            {
                _movementInput.Disable();
            }

            OnMovementInputChanged?.Invoke(_currentMovementInput);
        }

        private void MovementInputChanged(InputAction.CallbackContext context)
        {
            _currentMovementInput = context.ReadValue<Vector2>();

            OnMovementInputChanged?.Invoke(_currentMovementInput);
        }

        #endregion

        #region Lean Input

        private void InitializeLeanInput()
        {
            _leanInput.performed += LeanInputChanged;
            _leanInput.canceled += LeanInputChanged;
        }

        private void DeinitializeLeanInput()
        {
            _leanInput.performed -= LeanInputChanged;
            _leanInput.canceled -= LeanInputChanged;
        }

        public void ToggleLeanInput(bool toggle)
        {
            _currentLeanInput = 0f;

            if (toggle)
            {
                _leanInput.Enable();
            }
            else
            {
                _leanInput.Disable();
            }

            OnLeanInputChanged?.Invoke(_currentLeanInput);
        }

        private void LeanInputChanged(InputAction.CallbackContext context)
        {
            _currentLeanInput = context.ReadValue<float>();

            OnLeanInputChanged?.Invoke(_currentLeanInput);
        }

        #endregion

        #region Jump Input

        private void InitializeJumpInput()
        {
            _jumpInput.started += JumpInputChanged;
            _jumpInput.canceled += JumpInputChanged;
        }

        private void DeinitializeJumpInput()
        {
            _jumpInput.started -= JumpInputChanged;
            _jumpInput.canceled -= JumpInputChanged;
        }

        public void ToggleJumpInput(bool toggle)
        {
            _currentJumpInput = false;

            if (toggle)
            {
                _jumpInput.Enable();
            }
            else
            {
                _jumpInput.Disable();
            }

            OnJumpInputChanged?.Invoke(_currentJumpInput);
        }

        private void JumpInputChanged(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _currentJumpInput = true;
            }
            else if (context.canceled)
            {
                _currentJumpInput = false;
            }

            OnJumpInputChanged?.Invoke(_currentJumpInput);
        }

        #endregion

        #region Crouch Input

        private void InitializeCrouchInput()
        {
            _crouchInput.started += CrouchInputChanged;
            _crouchInput.canceled += CrouchInputChanged;
        }

        private void DeinitializeCrouchInput()
        {
            _crouchInput.started -= CrouchInputChanged;
            _crouchInput.canceled -= CrouchInputChanged;
        }

        public void ToggleCrouchInput(bool toggle)
        {
            _currentCrouchInput = false;

            if (toggle)
            {
                _crouchInput.Enable();
            }
            else
            {
                _crouchInput.Disable();
            }

            OnCrouchInputChanged?.Invoke(_currentCrouchInput);
        }

        private void CrouchInputChanged(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _currentCrouchInput = true;
            }
            else if (context.canceled)
            {
                _currentCrouchInput = false;
            }

            OnCrouchInputChanged?.Invoke(_currentCrouchInput);
        }

        #endregion

        #region Interact Input

        private void InitializeInteractInput()
        {
            _interactInput.performed += InteractInputPerformed;
        }

        private void DeinitializeInteractInput()
        {
            _interactInput.performed -= InteractInputPerformed;
        }

        public void ToggleInteractInput(bool toggle)
        {
            if (toggle && !_interactInput.enabled)
            {
                _interactInput.Enable();
            }
            else if (!toggle && _interactInput.enabled)
            {
                _interactInput.Disable();
            }
        }

        private void InteractInputPerformed(InputAction.CallbackContext context)
        {
            if (!_canInteractInput)
            {
                return;
            }

            OnInteractInputPerformed?.Invoke();
            _interactInputCurrentCooldown = _interactInputCooldown;
            _canInteractInput = false;
        }

        #endregion

        #region Camera Perspective Input

        private void InitializeCameraPerspectiveInput()
        {
            _cameraPerspectiveInput.performed += CameraPerspectiveInputPerformed;
        }

        private void DeinitializeCameraPerspectiveInput()
        {
            _cameraPerspectiveInput.performed -= CameraPerspectiveInputPerformed;
        }

        public void ToggleCameraPerspectiveInput(bool toggle)
        {
            if (toggle && !_cameraPerspectiveInput.enabled)
            {
                _cameraPerspectiveInput.Enable();
            }
            else if (!toggle && _cameraPerspectiveInput.enabled)
            {
                _cameraPerspectiveInput.Disable();
            }
        }

        private void CameraPerspectiveInputPerformed(InputAction.CallbackContext context)
        {
            OnCameraPerspectiveInputPerformed?.Invoke();
        }

        #endregion

        #region Player Inventory Input

        private void InitializePlayerInventoryInput()
        {
            _playerInventoryInput.performed += PlayerInventoryInputPerformed;
        }

        private void DeinitializePlayerInventoryInput()
        {
            _playerInventoryInput.performed -= PlayerInventoryInputPerformed;
        }

        public void TogglePlayerInventoryInput(bool toggle)
        {
            if (toggle && !_playerInventoryInput.enabled)
            {
                _playerInventoryInput.Enable();
            }
            else if (!toggle && _playerInventoryInput.enabled)
            {
                _playerInventoryInput.Disable();
            }
        }

        private void PlayerInventoryInputPerformed(InputAction.CallbackContext context)
        {
            OnPlayerInventoryInputPerformed?.Invoke();
        }

        #endregion

        #region Player Actionbar Input

        private void InitializePlayerActionbarInput()
        {
            _playerActionbarInput.performed += PlayerActionbarInputPerformed;

            _playerActionbarScrollInput.performed += PlayerActionbarScrollInputPerformed;
        }

        private void DeinitializePlayerActionbarInput()
        {
            _playerActionbarInput.performed -= PlayerActionbarInputPerformed;

            _playerActionbarScrollInput.performed -= PlayerActionbarScrollInputPerformed;
        }

        public void TogglePlayerActionbarInput(bool toggle)
        {
            if (toggle && !_playerActionbarInput.enabled)
            {
                _playerActionbarInput.Enable();
            }
            else if (!toggle && _playerActionbarInput.enabled)
            {
                _playerActionbarInput.Disable();
            }
        }

        public void TogglePlayerActionbarScrollInput(bool toggle)
        {
            if (toggle && !_playerActionbarScrollInput.enabled)
            {
                _playerActionbarScrollInput.Enable();
            }
            else if (!toggle && _playerActionbarScrollInput.enabled)
            {
                _playerActionbarScrollInput.Disable();
            }
        }

        public void ResetActionbarInput()
        {
            _currentActionbarInput = -1;

            OnPlayerActionbarInputPerformed?.Invoke(_currentActionbarInput);
        }

        private void PlayerActionbarInputPerformed(InputAction.CallbackContext context)
        {
            var inputIndex = Mathf.RoundToInt(context.ReadValue<float>());
            inputIndex -= 1;

            if (_currentActionbarInput == inputIndex)
            {
                _currentActionbarInput = -1;
            }
            else
            {
                _currentActionbarInput = inputIndex;
            }

            OnPlayerActionbarInputPerformed?.Invoke(_currentActionbarInput);
        }

        private void PlayerActionbarScrollInputPerformed(InputAction.CallbackContext context)
        {
            var inputValue = context.ReadValue<float>();
            var inputIndex = 0;
            if (inputValue > 0f)
            {
                inputIndex = 1;
            }
            else if (inputValue < 0f)
            {
                inputIndex = -1;
            }

            _currentActionbarInput += inputIndex;

            var maxActionbarInput = Constants.PLAYER_ACTIONBAR_MAX_CAPACITY - 1;

            if (_currentActionbarInput < 0)
            {
                _currentActionbarInput = maxActionbarInput;
            }
            else if (_currentActionbarInput > maxActionbarInput)
            {
                _currentActionbarInput = 0;
            }

            OnPlayerActionbarInputPerformed?.Invoke(_currentActionbarInput);
        }

        #endregion
    }
}
