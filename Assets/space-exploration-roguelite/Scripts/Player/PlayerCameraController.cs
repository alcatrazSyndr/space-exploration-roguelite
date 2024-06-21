using System.Collections;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private float _artificialGravityResetTimer = 0.5f;
        [SerializeField] private float _rotationRate = 1f;
        public float RotationRate
        {
            get
            {
                return _rotationRate;
            }
        }

        [Header("Components")]
        [SerializeField] private Transform _cameraY;
        [SerializeField] private Transform _cameraX;
        [SerializeField] private Transform _cameraZ;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Camera _camera;
        public Camera Camera
        {
            get
            {
                return _camera;
            }
        }

        [Header("Runtime")]
        [SerializeField] private bool _setup = false;
        [SerializeField] private bool _setupForArtificialGravity = false;
        public bool SetupForArtificialGravity
        {
            get
            {
                return _setupForArtificialGravity;
            }
        }
        private IEnumerator _artificialGravityCameraResetCRT = null;

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

        public void RotateCameraOnLocalXAxis(float value)
        {
            var targetXLocalRotation = _cameraX.localRotation * Quaternion.Euler(new Vector3(-value * _rotationRate, 0f, 0f));
            var targetXLocalEuler = targetXLocalRotation.eulerAngles;

            if ((targetXLocalEuler.x >= 275f || targetXLocalEuler.x <= 85f) && targetXLocalEuler.y == 0f && targetXLocalEuler.z == 0f)
            {
                _cameraX.localRotation = targetXLocalRotation;
            }
        }

        public void ResetCameraOnLocalXAxis()
        {
            _cameraX.localRotation = Quaternion.identity;
        }

        public void SetupArtificialGravity()
        {
            if (_setupForArtificialGravity)
            {
                return;
            }

            _setupForArtificialGravity = true;

            if (_artificialGravityCameraResetCRT != null)
            {
                StopCoroutine(_artificialGravityCameraResetCRT);
                _artificialGravityCameraResetCRT = null;
            }
        }

        public void UnsetupArtificialGravity()
        {
            if (!_setupForArtificialGravity)
            {
                return;
            }

            _setupForArtificialGravity = false;

            if (_artificialGravityCameraResetCRT != null)
            {
                StopCoroutine(_artificialGravityCameraResetCRT);
                _artificialGravityCameraResetCRT = null;
            }

            _artificialGravityCameraResetCRT = ArtificialGravityCameraResetCRT();
            StartCoroutine(_artificialGravityCameraResetCRT);
        }

        private IEnumerator ArtificialGravityCameraResetCRT()
        {
            var xStartRot = _cameraX.localRotation;
            var xEndRot = Quaternion.identity;

            var timer = 0f;
            var interpolation = 0f;

            while (timer < _artificialGravityResetTimer)
            {
                interpolation = Mathf.Clamp01(timer / _artificialGravityResetTimer);

                _cameraX.localRotation = Quaternion.Lerp(xStartRot, xEndRot, interpolation);

                timer += Time.deltaTime;
                yield return null;
            }

            _cameraX.localRotation = xEndRot;

            _artificialGravityCameraResetCRT = null;
            yield break;
        }

        #endregion
    }
}
