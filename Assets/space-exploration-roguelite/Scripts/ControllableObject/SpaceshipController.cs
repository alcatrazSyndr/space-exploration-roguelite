using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class SpaceshipController : NetworkBehaviour
    {
        [Header("Data")]
        [SerializeField] private float _movementAccelerationRate;
        [SerializeField] private float _movementVelocityRate;

        [Header("Components")]
        [SerializeField] private InteractableObjectController _pilotSeatInteractableObjectController;
        [SerializeField] private ControllableObjectController _pilotSeatControllableObjectController;

        [Header("Runtime")]
        [SerializeField] private float _tickRate = 0f;
        [SerializeField] private Vector3 _currentMovementInput = Vector3.zero;
        [SerializeField] private Vector3 _currentMovementVector = Vector3.zero;

        #region Setup/OnTick

        public override void OnStartServer()
        {
            base.OnStartServer();

            _tickRate = (float)TimeManager.TickDelta;

            TimeManager.OnTick += OnTick;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            TimeManager.OnTick -= OnTick;
        }

        private void OnTick()
        {
            if (!base.IsOwner)
            {
                return;
            }

            _currentMovementInput = _pilotSeatControllableObjectController.GetCurrentTargetMovementInput();

            if ((_currentMovementInput - _currentMovementVector).sqrMagnitude <= 0.001f)
            {
                _currentMovementVector = _currentMovementInput;
            }
            else
            {
                _currentMovementVector = Vector3.Lerp(_currentMovementVector, _currentMovementInput, _movementAccelerationRate * _tickRate);
            }

            if (_currentMovementVector != Vector3.zero)
            {
                var forward = transform.forward * _currentMovementVector.z;
                var right = transform.right * _currentMovementVector.x;
                var up = transform.up * _currentMovementVector.y;

                var positionOffset = (forward + right + up) * _movementVelocityRate;

                transform.position += positionOffset;
            }
        }

        #endregion

        #region Ownership

        public override void OnOwnershipServer(NetworkConnection prevOwner)
        {
            base.OnOwnershipServer(prevOwner);

            if (!base.IsServerStarted)
            {
                return;
            }

            if (_pilotSeatInteractableObjectController == null)
            {
                return;
            }

            if (base.OwnerId == -1)
            {
                _pilotSeatInteractableObjectController.SetInteractable(true);
            }
            else
            {
                _pilotSeatInteractableObjectController.SetInteractable(false);
            }
        }

        #endregion
    }
}
