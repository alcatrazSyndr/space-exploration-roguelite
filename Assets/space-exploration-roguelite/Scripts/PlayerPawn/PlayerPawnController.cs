using FishNet.Object;
using FishNet.Object.Prediction;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerPawnController : NetworkBehaviour
    {
        [Header("Data")]
        [SerializeField] private float _moveRate = 1f;
        [SerializeField] private float _moveAcceleration = 1f;
        [SerializeField] private float _rotationRate = 1f;
        [SerializeField] private float _artificialGravityRotationAdaptRate = 1f;
        [SerializeField] private float _leanRate = 1f;
        [SerializeField] private LayerMask _physicsColliderLayerMasks;
        [SerializeField] private float _physicsCollisionCheckSphereRadius = 0.5f;
        [SerializeField] private float _physicsGravityRayCheckDistance = 1.01f;
        [SerializeField] private float _physicsGravityFallRate = 1f;

        [Header("Runtime")]
        [SerializeField] private bool _setup = false;
        [SerializeField] private Vector3 _currentMovementInput = Vector3.zero;
        [SerializeField] private Vector3 _currentMovementVector = Vector3.zero;
        [SerializeField] private Vector3 _currentRotationInput = Vector3.zero;
        [SerializeField] private float _currentLeanInput = 0f;
        [SerializeField] private float _tickRate = 0f;
        [SerializeField] private float _currentGravityVelocity = 0f;
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

            PlayerPawnPhysics();
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

                if (_artificialGravityController == null)
                {
                    PlayerPawnRotation(Quaternion.Euler(new Vector3(_currentRotationInput.x * _rotationRate, _currentRotationInput.y * _rotationRate, 0f) * _rotationRate * _tickRate));
                }
                else
                {
                    PlayerPawnRotation((Quaternion.Euler(new Vector3(0f, _currentRotationInput.y * _rotationRate, 0f) * _rotationRate * _tickRate)));
                }
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

        #region Physics

        private void PlayerPawnRotation(Quaternion rotation)
        {
            transform.rotation *= rotation;
        }

        private void PlayerPawnPhysics()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_artificialGravityController == null)
            {
                if (_currentGravityVelocity != 0f)
                {
                    _currentGravityVelocity = 0f;
                }

                if ((_currentMovementVector - _currentMovementInput).sqrMagnitude <= 0.01f)
                {
                    _currentMovementVector = _currentMovementInput;
                }
                else
                {
                    _currentMovementVector = Vector3.Lerp(_currentMovementVector, _currentMovementInput, _tickRate * _moveAcceleration);
                }

                if (_currentLeanInput != 0f)
                {
                    PlayerPawnRotation(Quaternion.Euler(new Vector3(0f, 0f, -_currentLeanInput) * _leanRate * _tickRate));
                }

                if (_currentMovementVector != Vector3.zero)
                {
                    //transform.position = transform.position + (_currentMovementVector * _tickRate * _moveRate);
                    MoveWithCollisionCheck((_currentMovementVector * _tickRate * _moveRate));
                }
            }
            else
            {
                var forwardProjection = Vector3.ProjectOnPlane(transform.forward, _artificialGravityController.transform.up);
                var rightProjection = Vector3.ProjectOnPlane(transform.right, _artificialGravityController.transform.up);
                var currentArtificialGravityControllerRotationOffset = Quaternion.LookRotation(forwardProjection, _artificialGravityController.transform.up);

                transform.rotation = Quaternion.Lerp(transform.rotation, currentArtificialGravityControllerRotationOffset, _tickRate * _rotationRate * _artificialGravityRotationAdaptRate);

                var gravityVector = _artificialGravityController.transform.up;

                RaycastHit hit;
                if (Physics.Raycast(transform.position, -transform.up, out hit, _physicsGravityRayCheckDistance, _physicsColliderLayerMasks))
                {
                    if (hit.distance <= (_physicsGravityRayCheckDistance - 0.02f))
                    {
                        if (_currentGravityVelocity > 0f)
                        {
                            _currentGravityVelocity = 0f;
                        }
                        _currentGravityVelocity -= _physicsGravityFallRate;
                    }
                    else if (_currentGravityVelocity != 0f)
                    {
                        _currentGravityVelocity = 0f;
                    }
                }
                else
                {
                    _currentGravityVelocity += _physicsGravityFallRate;
                }

                gravityVector *= (-_currentGravityVelocity);

                //transform.position = Vector3.Lerp(transform.position, transform.position + gravityVector, _tickRate * _moveRate);

                var currentMovementVector = forwardProjection.normalized * _currentMovementInput.z + rightProjection.normalized * _currentMovementInput.x;
                currentMovementVector.Normalize();

                if (_currentMovementVector != currentMovementVector)
                {
                    _currentMovementVector = currentMovementVector;
                }

                if (_currentMovementVector != Vector3.zero || gravityVector != Vector3.zero)
                {
                    //transform.position = transform.position + (_currentMovementVector * _tickRate * _moveRate) + (gravityVector * _tickRate * _moveRate);
                    MoveWithCollisionCheck((_currentMovementVector * _tickRate * _moveRate) + (gravityVector * _tickRate * _moveRate));
                }
            }
        }

        private void MoveWithCollisionCheck(Vector3 targetDirection, bool firstTry = true)
        {
            var hits = new List<RaycastHit>(Physics.SphereCastAll(transform.position, _physicsCollisionCheckSphereRadius + 0.2f, targetDirection.normalized, targetDirection.magnitude, _physicsColliderLayerMasks));

            for (int i = hits.Count - 1; i >= 0; i--)
            {
                if (hits[i].collider.gameObject == gameObject)
                {
                    hits.RemoveAt(i);
                }
            }

            if (hits.Count == 1)
            {
                var hit = hits[0];
                var projectedDirection = Vector3.ProjectOnPlane(targetDirection.normalized, hit.normal);

                transform.position += (projectedDirection * _tickRate * _moveRate * (_artificialGravityController == null ? _currentMovementVector.magnitude : 1f));
            }
            else if (hits.Count > 1 && firstTry)
            {
                var additiveProjectedDirection = Vector3.zero;

                foreach (var hit in hits)
                {
                    var projectedDirection = Vector3.ProjectOnPlane(targetDirection.normalized, hit.normal);
                    additiveProjectedDirection += projectedDirection;
                }

                additiveProjectedDirection.Normalize();
                additiveProjectedDirection *= _tickRate * _moveRate * (_artificialGravityController == null ? _currentMovementVector.magnitude : 1f);

                MoveWithCollisionCheck(additiveProjectedDirection, false);
                return;
            }
            else if (hits.Count == 0)
            {
                transform.position += targetDirection;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(transform.position, _physicsCollisionCheckSphereRadius);
        }

        #endregion
    }
}
