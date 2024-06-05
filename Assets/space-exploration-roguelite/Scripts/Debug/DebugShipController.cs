using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class DebugShipController : NetworkBehaviour
    {
        [Header("Data")]
        [SerializeField] private Vector3 _shipVelocity = Vector3.zero;
        [SerializeField] private Vector3 _shipAngularVelocity = Vector3.zero;
        [SerializeField] private float _shipMoveRate = 1f;
        [SerializeField] private float _shipRotationRate = 1f;

        [Header("Components")]
        [SerializeField] private InteractableObjectController _interactableSwitch;
        [SerializeField] private Rigidbody _shipRigidbody;

        [Header("Runtime")]
        [SerializeField] private bool _shipToggle = false;
        [SerializeField] private Vector3 _currentShipVelocity = Vector3.zero;
        [SerializeField] private Vector3 _currentShipAngularVelocity = Vector3.zero;
        [SerializeField] private float _tickDelta = 0f;

        public override void OnStartServer()
        {
            base.OnStartServer();

            _tickDelta = (float)TimeManager.TickDelta;
            TimeManager.OnTick += OnTick;

            _interactableSwitch.OnInteracted.AddListener(ShipToggleInteraction);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            TimeManager.OnTick -= OnTick;

            _interactableSwitch.OnInteracted.RemoveListener(ShipToggleInteraction);
        }

        [Server]
        private void ShipToggleInteraction()
        {
            _shipToggle = !_shipToggle;
        }

        [Server]
        private void OnTick()
        {
            var targetVelocityVector = _shipToggle ? _shipVelocity : Vector3.zero;
            var targetAngularVelocityVector = _shipToggle ? _shipAngularVelocity : Vector3.zero;

            _currentShipVelocity = Vector3.Lerp(_currentShipVelocity, targetVelocityVector, _tickDelta * _shipMoveRate);
            _currentShipAngularVelocity = Vector3.Lerp(_currentShipAngularVelocity, targetAngularVelocityVector, _tickDelta * _shipRotationRate);

            var shipForward = transform.forward.normalized;
            var shipRight = transform.right.normalized;
            var shipUp = transform.up.normalized;

            var shipHeadingVector = shipForward * _currentShipVelocity.z + shipRight * _currentShipVelocity.x + shipUp * _currentShipVelocity.y;

            _shipRigidbody.velocity = shipHeadingVector;
            _shipRigidbody.angularVelocity = _currentShipAngularVelocity;
        }
    }
}
