using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace SpaceExplorationRoguelite
{
    public class PlayerInputController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private float _interactInputCooldown = 0.5f;

        [Header("Input Actions")]
        [SerializeField] private InputAction _movementInput;
        [SerializeField] private InputAction _cameraXInput;
        [SerializeField] private InputAction _cameraYInput;
        [SerializeField] private InputAction _interactInput;
        [SerializeField] private InputAction _jumpInput;
        [SerializeField] private InputAction _crouchInput;
        [SerializeField] private InputAction _leanInput;

        [Header("Runtime")]
        [SerializeField] private bool _setup = false;
        [SerializeField] private bool _canInteractInput = true;
        [SerializeField] private float _interactInputCurrentCooldown = 0f;
        [SerializeField] private Vector2 _currentCameraInput = Vector2.zero;
        [SerializeField] private Vector2 _currentMovementInput = Vector2.zero;
        public Vector2 CurrentMovementInput
        {
            get
            {
                return _currentMovementInput;
            }
        }
        [SerializeField] private float _currentLeanInput = 0f;

        [Header("Events")]
        public UnityEvent<Vector2> OnCameraInputChanged = new UnityEvent<Vector2>();
        public UnityEvent<Vector2> OnMovementInputChanged = new UnityEvent<Vector2>();
        public UnityEvent OnInteractInputPerformed = new UnityEvent();
        public UnityEvent<float> OnLeanInputChanged = new UnityEvent<float>();

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

            InitializeMovementInput();
            ToggleMovementInput(true);
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

            ToggleMovementInput(false);
            DeinitializeMovementInput();
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

        }

        private void DeinitializeLeanInput()
        {

        }

        public void ToggleLeanInput(bool toggle)
        {

        }

        private void LeanInputChanged(InputAction.CallbackContext context)
        {

        }

        #endregion

        #region Interact Input

        private void InitializeInteractInput()
        {
            _interactInput.performed += InteractInputPerformed;
        }

        private void DeinitializeInteractInput()
        {
            _interactInput.performed += InteractInputPerformed;
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
    }
}
