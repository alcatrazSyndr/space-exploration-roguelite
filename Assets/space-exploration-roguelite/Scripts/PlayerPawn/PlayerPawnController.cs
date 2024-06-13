using FishNet.Object;
using FishNet.Object.Prediction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerPawnController : NetworkBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private float _noGravityMoveRate = 0f;
        [SerializeField] private float _noGravityRotateRate = 0f;
        [SerializeField] private float _noGravityLeanRate = 0f;
        [SerializeField] private LayerMask _artificialGravityCheckLayerMask;
        [SerializeField] private float _artificialGravityForce = 0f;
        [SerializeField] private float _artificialGravityMoveRate = 0f;
        [SerializeField] private float _artificialGravityRotateRate = 0f;
        [SerializeField] private float _artificialGravityEntryUpDirectionFixTimer = 0f;
        [SerializeField] private float _artificialGravityCheckLineOffset = 0f;
        [SerializeField] private float _artificialGravityCheckLineLength = 0f;
        [SerializeField] private float _artificialGravityCheckLineTargetLengthMax = 0f;
        [SerializeField] private float _artificialGravityCheckLineTargetLengthMin = 0f;
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private bool _setup = false;
        [SerializeField] private Vector2 _currentMovementInput = Vector2.zero;
        [SerializeField] private Vector2 _currentRotationInput = Vector2.zero;
        [SerializeField] private float _currentLeanInput = 0f;
        [SerializeField] private bool _currentJumpInput = false;
        [SerializeField] private bool _currentCrouchInput = false;
        [SerializeField] private float _tickRate = 0f;
        [SerializeField] private ArtificialGravityController _artificialGravityController = null;
        public ArtificialGravityController ArtificialGravityController
        {
            get
            {
                return _artificialGravityController;
            }
        }
        private IEnumerator _fixPlayerUpDirectionCRT = null;

        #region Setup/Unsetup/OnTick

        public void Setup(PlayerController playerController)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_setup)
            {
                return;
            }

            _playerController = playerController;

            _setup = true;

            _noGravityMoveRate = Constants.PLAYERPAWN_NO_GRAVITY_MOVE_RATE;
            _noGravityRotateRate = Constants.PLAYERPAWN_NO_GRAVITY_ROTATE_RATE;
            _noGravityLeanRate = Constants.PLAYERPAWN_NO_GRAVITY_LEAN_RATE;

            _artificialGravityMoveRate = Constants.PLAYERPAWN_ARTIFICIAL_GRAVITY_MOVE_RATE;
            _artificialGravityRotateRate = Constants.PLAYERPAWN_ARTIFICIAL_GRAVITY_ROTATE_RATE;
            _artificialGravityEntryUpDirectionFixTimer = Constants.PLAYERPAWN_ARTIFICIAL_GRAVITY_ENTRY_UP_FIRECTION_FIX_TIMER;
            _artificialGravityCheckLineOffset = Constants.PLAYERPAWN_ARTIFICIAL_GRAVITY_CHECK_LINE_OFFSET;
            _artificialGravityCheckLineLength = Constants.PLAYERPAWN_ARTIFICIAL_GRAVITY_CHECK_LINE_LENGTH;
            _artificialGravityCheckLineTargetLengthMax = Constants.PLAYERPAWN_ARTIFICIAL_GRAVITY_CHECK_LINE_TARGET_LENGTH_MAX;
            _artificialGravityCheckLineTargetLengthMin = Constants.PLAYERPAWN_ARTIFICIAL_GRAVITY_CHECK_LINE_TARGET_LENGTH_MIN;
            _artificialGravityForce = Constants.PLAYERPAWN_ARTIFICIAL_GRAVITY_FORCE;
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

        private void OnPreTick()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (!_setup)
            {
                return;
            }

            if (_artificialGravityController != null)
            {
            }
            else
            {
            }
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

            if (_artificialGravityController != null)
            {
                var gravityValue = 0f;
                var gravityModifier = 1f;

                var gravityLinecastStartPosition = transform.position + (transform.up.normalized);
                var gravityLinecastOffsetPosition = gravityLinecastStartPosition - (transform.up.normalized * _artificialGravityCheckLineOffset);
                var gravityLinecastEndPosition = gravityLinecastOffsetPosition - (_artificialGravityController.transform.up * _artificialGravityCheckLineLength);
                RaycastHit gravityLinecastHit;
                RaycastHit gravityLinecastSourceHit;
                var gravityHit = Physics.Linecast(gravityLinecastOffsetPosition, gravityLinecastEndPosition, out gravityLinecastHit, _artificialGravityCheckLayerMask);
                var gravityOffsetHit = Physics.Linecast(gravityLinecastStartPosition, gravityLinecastOffsetPosition, out gravityLinecastSourceHit, _artificialGravityCheckLayerMask);

                if (gravityHit && !gravityOffsetHit)
                {
                    var distanceToFloor = gravityLinecastHit.distance;

                    if (distanceToFloor > _artificialGravityCheckLineTargetLengthMax)
                    {
                        var maxDistance = _artificialGravityCheckLineLength - _artificialGravityCheckLineTargetLengthMin;
                        gravityModifier = (distanceToFloor - _artificialGravityCheckLineTargetLengthMin) / maxDistance;

                        gravityValue = -_artificialGravityForce;
                    }
                    else if (distanceToFloor < _artificialGravityCheckLineTargetLengthMin)
                    {
                        gravityValue = _artificialGravityForce;
                    }
                }
                else if (gravityOffsetHit)
                {
                    gravityValue = _artificialGravityForce;
                }
                else if (!gravityHit && !gravityOffsetHit)
                {
                    gravityValue = -_artificialGravityForce;
                }

                if (gravityValue != 0f)
                {
                    var gravityMovementPosition = new Vector3(0f, gravityValue, 0f);
                    MovePlayerPawnArtificialGravity(gravityMovementPosition, gravityModifier);
                }

                var targetMovementPosition = new Vector3(_currentMovementInput.x, 0f, _currentMovementInput.y).normalized;
                MovePlayerPawnArtificialGravity(targetMovementPosition);
            }
            else
            {
                var targetLeanRotation = Quaternion.Euler(0f, 0f, -_currentLeanInput * _noGravityLeanRate);
                RotatePlayerPawnNoGravity(targetLeanRotation);

                var targetMovementPosition = new Vector3(_currentMovementInput.x, (_currentJumpInput ? 1f : 0f) + (_currentCrouchInput ? -1f : 0f), _currentMovementInput.y).normalized;
                MovePlayerPawnNoGravity(targetMovementPosition);
            }
        }

        private void OnPostTick()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (!_setup)
            {
                return;
            }

            if (_artificialGravityController != null)
            {
            }
            else
            {
            }
        }

        #endregion

        #region Player Pawn Manipulation No Gravity

        private void RotatePlayerPawnNoGravity(Quaternion rotationDelta)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (!_setup)
            {
                return;
            }

            transform.rotation *= rotationDelta;
        }

        private void MovePlayerPawnNoGravity(Vector3 positionDelta)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (!_setup)
            {
                return;
            }

            var forward = transform.forward * positionDelta.z;
            var right = transform.right * positionDelta.x;
            var up = transform.up * positionDelta.y;

            var direction = (forward + right + up).normalized;

            transform.position += (direction * _noGravityMoveRate);
        }

        #endregion

        #region Player Pawn Manipulation Artificial Gravity

        private void RotatePlayerPawnArtificialGravity(Quaternion rotationDelta)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (!_setup)
            {
                return;
            }

            transform.localRotation *= rotationDelta;
        }

        private void MovePlayerPawnArtificialGravity(Vector3 positionDelta, float modifier = 1f)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (!_setup)
            {
                return;
            }

            var forward = Vector3.ProjectOnPlane(transform.forward, _artificialGravityController.transform.up) * positionDelta.z;
            var right = Vector3.ProjectOnPlane(transform.right, _artificialGravityController.transform.up) * positionDelta.x;
            var up = _artificialGravityController.transform.up * positionDelta.y;

            var direction = (forward + right + up).normalized;

            transform.position += (direction * _artificialGravityMoveRate * modifier);
        }

        private void StartFixPlayerUpDirectionProcess()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (!_setup)
            {
                return;
            }

            ResetFixPlayerUpDirectionProcess();

            _fixPlayerUpDirectionCRT = FixPlayerUpDirectionCRT();
            StartCoroutine(_fixPlayerUpDirectionCRT);
        }

        private void ResetFixPlayerUpDirectionProcess()
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (!_setup)
            {
                return;
            }

            if (_fixPlayerUpDirectionCRT != null)
            {
                StopCoroutine(_fixPlayerUpDirectionCRT);
                _fixPlayerUpDirectionCRT = null;
            }
        }

        private IEnumerator FixPlayerUpDirectionCRT()
        {
            if (!base.IsOwner)
            {
                yield break;
            }

            if (!_setup)
            {
                yield break;
            }

            float timer = 0f;
            float interpolation = 0f;

            var endRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, _artificialGravityController.transform.up));
            var startRotation = transform.rotation;

            while (timer < _artificialGravityEntryUpDirectionFixTimer)
            {
                interpolation = Mathf.Clamp01(timer / _artificialGravityEntryUpDirectionFixTimer);

                endRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, _artificialGravityController.transform.up));

                transform.rotation = Quaternion.Lerp(transform.rotation, endRotation, interpolation);

                timer += _tickRate;

                yield return new WaitForSecondsRealtime(_tickRate);
            }

            endRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, _artificialGravityController.transform.up));
            transform.rotation = endRotation;

            _fixPlayerUpDirectionCRT = null;
            yield break;
        }

        #endregion

        #region Input

        public void JumpInputChanged(bool jumpInput)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (!_setup)
            {
                return;
            }

            if (_currentJumpInput != jumpInput)
            {
                _currentJumpInput = jumpInput;
            }
        }

        public void CrouchInputChanged(bool crouchInput)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (!_setup)
            {
                return;
            }

            if (_currentCrouchInput != crouchInput)
            {
                _currentCrouchInput = crouchInput;
            }
        }

        public void MovementInputChange(Vector2 movementInput)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (!_setup)
            {
                return;
            }

            if (_currentMovementInput != movementInput)
            {
                _currentMovementInput = movementInput;
            }
        }

        public void RotationInputChange(Vector2 rotationInput)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (!_setup)
            {
                return;
            }

            if (_currentRotationInput != rotationInput)
            {
                _currentRotationInput = rotationInput;

                if (_artificialGravityController != null)
                {
                    var targetRotationDelta = Quaternion.Euler(0f, rotationInput.x * _artificialGravityRotateRate, 0f);

                    RotatePlayerPawnArtificialGravity(targetRotationDelta);
                }
                else
                {
                    var targetRotationDelta = Quaternion.Euler(-rotationInput.y * _noGravityRotateRate, rotationInput.x * _noGravityRotateRate, 0f);

                    RotatePlayerPawnNoGravity(targetRotationDelta);
                }
            }
        }

        public void LeanInputChange(float leanInput)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (!_setup)
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

            if (!_setup)
            {
                return;
            }

            if (_artificialGravityController == artificialGravityController)
            {
                return;
            }

            _artificialGravityController = artificialGravityController;

            _playerController.ArtificialGravityControllerChanged();

            if (_artificialGravityController != null)
            {
                StartFixPlayerUpDirectionProcess();
                transform.SetParent(_artificialGravityController.transform);
            }
            else
            {
                ResetFixPlayerUpDirectionProcess();
                transform.SetParent(null);
            }
        }

        #endregion

        private void OnDrawGizmos()
        {
            if (_artificialGravityController != null)
            {
                var gravityLinecastStartPosition = transform.position;
                var gravityLinecastOffsetPosition = gravityLinecastStartPosition - (transform.up.normalized * _artificialGravityCheckLineOffset);
                var gravityLinecastEndPosition = gravityLinecastOffsetPosition - (_artificialGravityController.transform.up * _artificialGravityCheckLineLength);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(gravityLinecastStartPosition, gravityLinecastOffsetPosition);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(gravityLinecastOffsetPosition, gravityLinecastEndPosition);
            }
        }
    }
}
