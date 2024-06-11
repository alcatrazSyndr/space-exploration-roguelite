using FishNet.Object;
using FishNet.Object.Prediction;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerPawnController : NetworkBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;

        [Header("Data")]
        [SerializeField] private float _groundCheckPositionOffset = 1f;
        [SerializeField] private float _groundCheckSize = 0.1f;
        [SerializeField] private LayerMask _groundCheckLayerMask;
        [SerializeField] private float _gravityModifier = 1f;
        [SerializeField] private float _moveRate = 100f;
        [SerializeField] private float _leanRate = 100f;
        [SerializeField] private float _gravityRotationFixRate = 10;
        [SerializeField] private float _zeroGravityXRotationRate = 0.1f;
        [SerializeField] private float _zeroGravityYRotationRate = 0.1f;

        [Header("Runtime")]
        [SerializeField] private bool _setup = false;
        [SerializeField] private Vector2 _currentMovementInput = Vector2.zero;
        [SerializeField] private Vector2 _currentRotationInput = Vector2.zero;
        [SerializeField] private float _currentLeanInput = 0f;
        [SerializeField] private bool _currentJumpInput = false;
        [SerializeField] private bool _currentCrouchInput = false;
        [SerializeField] private float _tickRate = 0f;
        [SerializeField] private ArtificialGravityController _artificialGravityController = null;
        public ArtificialGravityController ArtificialGravityController
        {
            get
            {
                return _artificialGravityController;
            }
        }

        #region Setup/Unsetup/OnTick

        public void Setup()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_setup)
            {
                return;
            }
            _setup = true;
        }

        public void Unsetup()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (!_setup)
            {
                return;
            }
            _setup = false;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!base.IsOwner)
            {
                return;
            }

            _tickRate = (float)TimeManager.TickDelta;
            TimeManager.OnTick += OnTick;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (!base.IsOwner)
            {
                return;
            }

            TimeManager.OnTick -= OnTick;
        }

        private void OnTick()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (!_setup)
            {
                return;
            }

            if (_artificialGravityController != null)
            {
                var projectedForwardDirection = Vector3.ProjectOnPlane(_rigidbody.transform.forward, _artificialGravityController.transform.up).normalized;
                var projectedRightDirection = Vector3.ProjectOnPlane(_rigidbody.transform.right, _artificialGravityController.transform.up).normalized;

                var targetMovementDirection = projectedForwardDirection * _currentMovementInput.y + projectedRightDirection * _currentMovementInput.x;
                _rigidbody.AddForce(targetMovementDirection.normalized * _tickRate * _moveRate, ForceMode.Impulse);

                var targetRotation = Quaternion.LookRotation(projectedForwardDirection, _artificialGravityController.transform.up);
                var currentRotation = _rigidbody.transform.rotation;
                var quaternionProduct = targetRotation * Quaternion.Inverse(currentRotation);
                _rigidbody.AddTorque(new Vector3(quaternionProduct.x, quaternionProduct.y, quaternionProduct.z) * (_leanRate * _gravityRotationFixRate) * _tickRate, ForceMode.Impulse);

                var grounded = Physics.CheckSphere(_rigidbody.transform.position + (-_rigidbody.transform.up.normalized * _groundCheckPositionOffset), _groundCheckSize, _groundCheckLayerMask);

                if (!grounded)
                {
                    var gravityVector = -_artificialGravityController.transform.up.normalized;
                    _rigidbody.AddForce(gravityVector * _gravityModifier, ForceMode.Impulse);
                }
            }
            else
            {
                _rigidbody.AddRelativeTorque(new Vector3(-_currentRotationInput.y * _zeroGravityXRotationRate, _currentRotationInput.x * _zeroGravityYRotationRate, -_currentLeanInput) * _leanRate * _tickRate, ForceMode.Impulse);

                _rigidbody.AddRelativeForce(new Vector3(_currentMovementInput.x, (_currentJumpInput ? 1f : 0f) + (_currentCrouchInput ? -1f : 0f), _currentMovementInput.y) * _tickRate * _moveRate, ForceMode.Impulse);
            }
        }

        #endregion

        #region Input

        public void JumpInputChanged(bool jumpInput)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentJumpInput != jumpInput)
            {
                _currentJumpInput = jumpInput;
            }
        }

        public void CrouchInputChanged(bool crouchInput)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentCrouchInput != crouchInput)
            {
                _currentCrouchInput = crouchInput;
            }
        }

        public void MovementInputChange(Vector2 movementInput)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentMovementInput != movementInput)
            {
                _currentMovementInput = movementInput;
            }
        }

        public void RotationInputChange(Vector2 rotationInput)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentRotationInput != rotationInput)
            {
                _currentRotationInput = rotationInput;
            }
        }

        public void LeanInputChange(float leanInput)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentLeanInput != leanInput)
            {
                _currentLeanInput = leanInput;
            }
        }

        #endregion

        #region Artificial Gravity

        public void SetArtificialGravityController(ArtificialGravityController artificialGravityController)
        {
            if (!base.IsOwner)
            {
                return;
            }

            _artificialGravityController = artificialGravityController;
        }

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(_rigidbody.transform.position + (-_rigidbody.transform.up.normalized * _groundCheckPositionOffset), _groundCheckSize);
        }
    }
}
