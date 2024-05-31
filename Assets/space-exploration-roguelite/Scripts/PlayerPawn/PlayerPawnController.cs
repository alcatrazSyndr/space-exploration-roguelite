using FishNet.Object;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerPawnController : NetworkBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;

        [Header("Runtime")]
        [SerializeField] private bool _setup = false;
        [SerializeField] private Vector3 _currentMovementInput = Vector3.zero;

        #region Setup/Unsetup/Update

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

        private void FixedUpdate()
        {
            if (!base.IsOwner)
            {
                return;
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
    }
}
