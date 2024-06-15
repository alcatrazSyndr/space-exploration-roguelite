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
        [SerializeField] private float _rotationAccelerationRate;
        [SerializeField] private float _rotationVelocityRate;

        [Header("Components")]
        [SerializeField] private InteractableObjectController _pilotSeatInteractableObjectController;
        [SerializeField] private ControllableObjectController _pilotSeatControllableObjectController;

        [Header("Runtime")]
        [SerializeField] private float _tickRate = 0f;
        [SerializeField] private Vector3 _currentMovementInput = Vector3.zero;
        [SerializeField] private Vector3 _currentMovementVector = Vector3.zero;
        [SerializeField] private Vector3 _currentRotationInput = Vector3.zero;
        [SerializeField] private Vector3 _currentRotationVector = Vector3.zero;

        #region Setup/OnTick

        public override void OnStartServer()
        {
            base.OnStartServer();

            _tickRate = (float)TimeManager.TickDelta;

            //TimeManager.OnTick += OnServerTick;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            //TimeManager.OnTick -= OnServerTick;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            _tickRate = (float)TimeManager.TickDelta;

            TimeManager.OnTick += OnClientTick;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            TimeManager.OnTick -= OnClientTick;
        }

        private void OnClientTick()
        {
            if (!base.IsOwner)
            {
                return;
            }

            var currentRotationInput = _pilotSeatControllableObjectController.GetCurrentTargetRotationInput();
            if (_currentRotationInput != currentRotationInput)
            {
                _currentRotationInput = currentRotationInput;
            }

            if ((_currentRotationInput - _currentRotationVector).sqrMagnitude <= 0.00001f)
            {
                _currentRotationVector = _currentRotationInput;
            }
            else
            {
                _currentRotationVector = Vector3.Lerp(_currentRotationVector, _currentRotationInput, _rotationAccelerationRate * _tickRate);
            }

            if (_currentRotationVector != Vector3.zero)
            {
                transform.rotation *= Quaternion.Euler(_currentRotationVector * _rotationAccelerationRate);
            }

            var currentMovementInput = _pilotSeatControllableObjectController.GetCurrentTargetMovementInput();
            if (_currentMovementInput != currentMovementInput)
            {
                _currentMovementInput = currentMovementInput;
            }

            if ((_currentMovementInput - _currentMovementVector).sqrMagnitude <= 0.00001f)
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

        /*
        [ServerRpc]
        public void MovementInputChange(NetworkConnection playerConnection, Vector3 movementInput)
        {
            if (base.Owner == playerConnection)
            {
                _currentMovementInput = movementInput;
            }
        }

        [Server]
        private void OnServerTick()
        {
            if ((_currentMovementInput - _currentMovementVector).sqrMagnitude <= 0.00001f)
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
        */

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
                _currentMovementInput = Vector3.zero;

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
