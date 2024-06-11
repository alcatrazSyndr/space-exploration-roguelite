using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class DebugShipController : NetworkBehaviour
    {
        [Header("Data")]
        [SerializeField] private float _movementRateModifier = 1000f;
        [SerializeField] private float _rotationRateModifier = 1000f;
        [SerializeField] private float _accelerationRateModifier = 1f;
        [SerializeField] private Vector3 _targetShipVelocity = Vector3.zero;
        [SerializeField] private Vector3 _targetShipAngularVelocity = Vector3.zero;

        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;

        [Header("Runtime")]
        [SerializeField] private Vector3 _currentShipVelocity = Vector3.zero;
        [SerializeField] private Vector3 _currentShipAngularVelocity = Vector3.zero;
        [SerializeField] private float _tickRate = 0f;

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
            _currentShipAngularVelocity = Vector3.Lerp(_currentShipAngularVelocity, _targetShipAngularVelocity, _tickRate * _accelerationRateModifier);
            _currentShipVelocity = Vector3.Lerp(_currentShipVelocity, _targetShipVelocity, _tickRate * _accelerationRateModifier);

            _rigidbody.AddRelativeForce(_currentShipVelocity * _tickRate * _movementRateModifier, ForceMode.Force);
            _rigidbody.AddRelativeTorque(_currentShipAngularVelocity * _tickRate * _rotationRateModifier, ForceMode.Force);
        }
    }
}
