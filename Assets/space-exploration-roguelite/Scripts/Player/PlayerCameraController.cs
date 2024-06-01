using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform _cameraY;
        [SerializeField] private Transform _cameraX;
        [SerializeField] private Transform _cameraZ;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Camera _camera;

        [Header("Runtime")]
        [SerializeField] private bool _setup = false;

        #region Setup/Unsetup

        public void Setup()
        {
            if (_setup)
            {
                return;
            }
            _setup = true;
        }

        public void Unsetup()
        {
            if (!_setup)
            {
                return;
            }
            _setup = false;
        }

        #endregion

        #region Camera

        public Vector3 CameraRelativeMovementInput(Vector3 movementInput)
        {
            var forward = _cameraTransform.forward;
            var right = _cameraTransform.right;
            var up = _cameraTransform.up;

            var desiredInputDirection = forward.normalized * movementInput.z + right.normalized * movementInput.x + up.normalized * movementInput.y;
            return new Vector3(desiredInputDirection.x, desiredInputDirection.y, desiredInputDirection.z);
        }

        #endregion
    }
}
