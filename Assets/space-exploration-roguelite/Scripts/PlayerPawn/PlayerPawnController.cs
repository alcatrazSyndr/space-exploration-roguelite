using FishNet.Object;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerPawnController : NetworkBehaviour
    {
        [Header("Data")]
        [SerializeField] private float _moveRate = 1f;
        [SerializeField] private float _accelerationRate = 1f;

        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;

        [Header("Runtime")]
        [SerializeField] private bool _setup = false;
        [SerializeField] private Vector3 _currentMovementInput = Vector3.zero;
        [SerializeField] private Vector3 _currentMovementVector = Vector3.zero;
        [SerializeField] private float _tickDelta = 0f;

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

                _currentMovementVector = Vector3.Lerp(_currentMovementVector, _currentMovementInput, _tickDelta * _accelerationRate);
            }

            _rigidbody.velocity = _currentMovementVector * _moveRate;
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
    }
}
