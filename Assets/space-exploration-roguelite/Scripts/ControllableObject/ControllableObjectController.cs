using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class ControllableObjectController : NetworkBehaviour
    {
        [Header("Data")]
        [SerializeField] private Enums.ControllableObjectType _controllableObjectType;
        public Enums.ControllableObjectType ControllableObjectType
        {
            get
            {
                return _controllableObjectType;
            }
        }

        [Header("Components")]
        [SerializeField] private Transform _controllableObjectSeatTransform;
        public Transform ControllableObjectSeatTransform
        {
            get
            {
                return _controllableObjectSeatTransform;
            }
        }
        [SerializeField] private Transform _controllableObjectExitTransform;
        public Transform ControllableObjectExitTransform
        {
            get
            {
                return _controllableObjectExitTransform;
            }
        }

        [Header("Runtime")]
        [SerializeField] private Vector2 _currentMovementInput = Vector2.zero;
        public Vector2 CurrentMovementInput
        {
            get
            {
                return _currentMovementInput;
            }
        }
        [SerializeField] private Vector2 _currentRotationInput = Vector2.zero;
        public Vector2 CurrentRotationInput
        {
            get
            {
                return _currentRotationInput;
            }
        }
        [SerializeField] private float _currentLeanInput = 0f;
        public float CurrentLeanInput
        {
            get
            {
                return _currentLeanInput;
            }
        }
        [SerializeField] private bool _currentJumpInput = false;
        public bool CurrentJumpInput
        {
            get
            {
                return _currentJumpInput;
            }
        }
        [SerializeField] private bool _currentCrouchInput = false;
        public bool CurrentCrouchInput
        {
            get
            {
                return _currentCrouchInput;
            }
        }

        #region Input

        public Vector3 GetCurrentTargetMovementInput()
        {
            if (!base.IsOwner)
            {
                return Vector3.zero;
            }

            return new Vector3(_currentMovementInput.x, (_currentJumpInput ? 1f : 0f) + (_currentCrouchInput ? -1f : 0f), _currentMovementInput.y).normalized;
        }

        public Vector3 GetCurrentTargetRotationInput()
        {
            if (!base.IsOwner)
            {
                return Vector3.zero;
            }

            return new Vector3(-_currentRotationInput.y, _currentRotationInput.x, -_currentLeanInput);
        }

        public void MovementInputChanged(Vector2 input)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentMovementInput != input)
            {
                _currentMovementInput = input;
            }
        }

        public void RotationInputChanged(Vector2 input)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentRotationInput != input)
            {
                _currentRotationInput = input;
            }
        }

        public void LeanInputChanged(float input)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentLeanInput != input)
            {
                _currentLeanInput = input;
            }
        }

        public void JumpInputChanged(bool input)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentJumpInput != input)
            {
                _currentJumpInput = input;
            }
        }

        public void CrouchInputChanged(bool input)
        {
            if (!base.IsOwner)
            {
                return;
            }

            if (_currentCrouchInput != input)
            {
                _currentCrouchInput = input;
            }
        }

        #endregion

        #region Ownership

        public void ReleaseOwnership()
        {
            if (base.Owner.ClientId != ClientManager.Connection.ClientId)
            {
                return;
            }

            ReleaseOwnershipRPC(ClientManager.Connection);
        }

        [ServerRpc]
        private void ReleaseOwnershipRPC(NetworkConnection playerConnection)
        {
            if (base.Owner != playerConnection)
            {
                return;
            }

            base.RemoveOwnership();
        }

        public void ClaimOwnership()
        {
            if (base.Owner.ClientId != -1)
            {
                return;
            }

            ClaimOwnershipRPC(ClientManager.Connection);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ClaimOwnershipRPC(NetworkConnection playerConnection)
        {
            if (base.HasAuthority && base.IsServerInitialized)
            {
                base.GiveOwnership(playerConnection);
            }
        }

        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            base.OnOwnershipClient(prevOwner);

            var myPlayerController = GameManagerSingleton.Instance.GetPlayerConnectionPlayerController(ClientManager.Connection);

            if (myPlayerController != null)
            {
                if (base.IsOwner)
                {
                    if (myPlayerController.CurrentControlledObject == null)
                    {
                        myPlayerController.CurrentControlledObjectChanged(this);
                    }
                }
                else if (myPlayerController.CurrentControlledObject == this)
                {
                    myPlayerController.CurrentControlledObjectChanged(null);
                }
            }
        }

        #endregion
    }
}
