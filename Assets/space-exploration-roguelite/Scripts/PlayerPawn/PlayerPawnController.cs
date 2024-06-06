using FishNet.Object;
using FishNet.Object.Prediction;
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
        [SerializeField] private float _physicsCollisionCheckCapsuleRadius = 0.5f;
        [SerializeField] private float _physicsCollisionCheckCapsuleOffset = 0.5f;
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

                var oldRotation = transform.rotation;
                var oldPosition = transform.position;

                if (_artificialGravityController == null)
                {
                    PlayerPawnRotation(Quaternion.Euler(new Vector3(_currentRotationInput.x * _rotationRate, _currentRotationInput.y * _rotationRate, 0f) * _rotationRate * _tickRate));

                    //CollisionCheck(oldPosition, oldRotation);
                }
                else
                {
                    PlayerPawnRotation((Quaternion.Euler(new Vector3(0f, _currentRotationInput.y * _rotationRate, 0f) * _rotationRate * _tickRate)));

                    //CollisionCheck(oldPosition, oldRotation);
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

            var oldPosition = transform.position;
            var oldRotation = transform.rotation;

            if (_artificialGravityController == null)
            {
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
                    transform.position = transform.position + (_currentMovementVector * _tickRate * _moveRate);
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

                transform.position = Vector3.Lerp(transform.position, transform.position + gravityVector, _tickRate * _moveRate);

                var currentMovementVector = forwardProjection.normalized * _currentMovementInput.z + rightProjection.normalized * _currentMovementInput.x;
                currentMovementVector.Normalize();

                if (_currentMovementVector != currentMovementVector)
                {
                    _currentMovementVector = currentMovementVector;
                }

                if (_currentMovementVector != Vector3.zero)
                {
                    transform.position = transform.position + (_currentMovementVector * _tickRate * _moveRate);
                }
            }

            if (_currentLeanInput != 0f || _currentMovementVector != Vector3.zero)
            {
                //CollisionCheck(oldPosition, oldRotation);
            }
        }

        private void CollisionCheck(Vector3 previousPosition, Quaternion previousRotation)
        {
            var collision = Physics.CheckCapsule(transform.position + (transform.up * _physicsCollisionCheckCapsuleOffset), transform.position + (-transform.up * _physicsCollisionCheckCapsuleOffset), _physicsCollisionCheckCapsuleRadius, _physicsColliderLayerMasks);

            if (collision)
            {
                transform.position = previousPosition;
                transform.rotation = previousRotation;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(transform.position + (transform.up * _physicsCollisionCheckCapsuleOffset), _physicsCollisionCheckCapsuleRadius);
            Gizmos.DrawWireSphere(transform.position + (-transform.up * _physicsCollisionCheckCapsuleOffset), _physicsCollisionCheckCapsuleRadius);
        }

        #endregion
    }
}
