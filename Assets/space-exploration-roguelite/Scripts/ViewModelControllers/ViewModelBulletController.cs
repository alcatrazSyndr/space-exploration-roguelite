using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class ViewModelBulletController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private float _flightSpeedLimit = 1f;
        [SerializeField] private float _flightSpeedAcceleration = 1f;
        [SerializeField] private float _flightSpeedStart = 1f;
        [SerializeField] private float _existTimer = 5f;

        [Header("Runtime")]
        [SerializeField] private bool _setup = false;
        [SerializeField] private float _currentExistTimer = 0f;
        [SerializeField] private float _currentFlightSpeed = 0f;

        private void Update()
        {
            if (!_setup)
            {
                return;
            }

            transform.position += (transform.forward * _currentFlightSpeed * Time.deltaTime);
            if (_currentFlightSpeed < _flightSpeedLimit)
            {
                _currentFlightSpeed += _flightSpeedAcceleration;
            }

            _currentExistTimer += Time.deltaTime;

            if (_currentExistTimer >= _existTimer)
            {
                Unsetup();
            }
        }

        public void Setup(Vector3 targetPosition)
        {
            transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);

            _currentFlightSpeed = _flightSpeedStart;

            _setup = true;
        }

        private void Unsetup()
        {
            _setup = false;

            Destroy(gameObject);
        }
    }
}
