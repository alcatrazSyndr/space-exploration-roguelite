using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerPawnController : NetworkBehaviour
    {
        [Header("Data")]
        [SerializeField] private float _moveRate = 1f;
        [SerializeField] private float _movementAccelerationRate = 1f;
        [SerializeField] private float _rotationRate = 1f;
        [SerializeField] private float _rotationDeaccelerationRate = 1f;
        [SerializeField] private float _leanRate = 1f;

        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;

        [Header("Runtime")]
        [SerializeField] private bool _setup = false;
        [SerializeField] private Vector3 _currentMovementInput = Vector3.zero;
        [SerializeField] private Vector3 _currentMovementVector = Vector3.zero;
        [SerializeField] private Vector3 _currentRotationInput = Vector3.zero;
        [SerializeField] private float _currentLeanInput = 0f;
        [SerializeField] private float _tickDelta = 0f;

        #region Setup/Unsetup/OnTick/Update

        private void Update()
        {
            if (base.IsOwner)
            {
                return;
            }

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

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

            _tickDelta = (float)TimeManager.TickDelta;
            TimeManager.OnTick += OnTick;
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

            if (_currentMovementVector != _currentMovementInput)
            {
                if ((_currentMovementVector - _currentMovementInput).sqrMagnitude <= 0.01f)
                {
                    _currentMovementVector = _currentMovementInput;
                }

                _currentMovementVector = Vector3.Lerp(_currentMovementVector, _currentMovementInput, _tickDelta * _movementAccelerationRate);
            }

            _rigidbody.velocity = _currentMovementVector * _moveRate;

            if (_rigidbody.angularVelocity != Vector3.zero)
            {
                _rigidbody.angularVelocity = Vector3.Lerp(_rigidbody.angularVelocity, Vector3.zero, _tickDelta * _rotationDeaccelerationRate);
            }

            if (_currentLeanInput != 0f)
            {
                _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(new Vector3(0f, 0f, -_currentLeanInput) * _leanRate));
            }
        }

        #endregion

        #region Movement

        public void MovementInputChange(Vector3 movementInput)
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

        #endregion

        #region Rotation

        public void RotationInputChange(Vector3 rotationInput)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentRotationInput != rotationInput)
            {
                _currentRotationInput = rotationInput;

                _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(_currentRotationInput * _rotationRate));
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
    }
}
