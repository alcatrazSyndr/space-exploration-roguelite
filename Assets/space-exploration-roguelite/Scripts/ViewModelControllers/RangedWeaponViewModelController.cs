using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class RangedWeaponViewModelController : ViewModelController
    {
        [Header("Data")]
        [SerializeField] private LayerMask _bulletRaycastLayerMask;

        [Header("Components")]
        [SerializeField] private Transform _bulletOriginPoint;

        [Header("Runtime")]
        [SerializeField] protected GameObject _bulletPrefab = null;
        [SerializeField] protected Enums.RangedWeaponFiringType _firingType = Enums.RangedWeaponFiringType.SingleAction;
        [SerializeField] protected float _firingCooldownTime = 0f;
        [SerializeField] protected bool _canFire = false;
        private IEnumerator _currentFiringCRT = null;

        #region Setup/Unsetup

        public override void Setup(PlayerController playerController, ItemDataSO itemDataSO)
        {
            base.Setup(playerController, itemDataSO);

            var weaponDataSO = itemDataSO as WeaponDataSO;

            if (weaponDataSO != null)
            {
                _firingType = weaponDataSO.FiringType;
                _bulletPrefab = weaponDataSO.BulletPrefab;
                _firingCooldownTime = weaponDataSO.FiringCooldown;

                _canFire = true;
            }
        }

        public override void Unsetup()
        {
            if (_currentFiringCRT != null && _setup && _canFire)
            {
                ResetFiringCRT();
            }

            base.Unsetup();

            _canFire = false;
            _bulletPrefab = null;
        }

        #endregion

        #region Input

        public override void PrimaryActionInputChanged(bool input)
        {
            base.PrimaryActionInputChanged(input);

            if (!_canFire)
            {
                return;
            }

            if (_currentPrimaryActionInput)
            {
                if (_currentFiringCRT == null)
                {
                    if (_firingType == Enums.RangedWeaponFiringType.SemiAutomatic && _bulletPrefab != null && _bulletOriginPoint != null && _currentPrimaryActionInput)
                    {
                        FireSemiAutomatic();
                    }
                }
            }
            else
            {

            }
        }

        #endregion

        #region Weapon Logic

        private RaycastHit? BulletRaycast()
        {
            var cameraTransform = _playerController != null ? _playerController.CameraVisionTransform != null ? _playerController.CameraVisionTransform : null : null;

            if (cameraTransform != null)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, cameraTransform.forward.normalized, out hit, 100f, _bulletRaycastLayerMask))
                {
                    return hit;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private void ResetFiringCRT()
        {
            if (_currentFiringCRT != null)
            {
                StopCoroutine(_currentFiringCRT);
                _currentFiringCRT = null;
            }
        }

        private void FireSemiAutomatic()
        {
            _currentFiringCRT = FireSemiAutomaticCRT();
            StartCoroutine(_currentFiringCRT);
        }

        private IEnumerator FireSemiAutomaticCRT()
        {
            var bulletGO = Instantiate(_bulletPrefab, null);
            bulletGO.transform.position = _bulletOriginPoint.position;
            bulletGO.transform.rotation = _bulletOriginPoint.rotation;

            var bulletController = bulletGO.GetComponent<ViewModelBulletController>();
            var bulletRaycast = BulletRaycast();
            if (bulletController != null)
            {
                var targetPos = Vector3.zero;
                if (bulletRaycast != null)
                {
                    targetPos = bulletRaycast.Value.point;
                }
                else
                {
                    if (_playerController != null && _playerController.CameraVisionTransform != null)
                    {
                        targetPos = transform.position + (_playerController.CameraVisionTransform.forward * 100f);
                    }
                    else
                    {
                        targetPos = transform.position + (transform.forward * 100f);
                    }
                }

                bulletController.Setup(targetPos);
                _playerController.WeaponBulletFired(targetPos, _itemDataSO.ItemID);
            }
            else
            {
                Destroy(bulletGO);
            }

            yield return new WaitForSecondsRealtime(_firingCooldownTime);

            _currentFiringCRT = null;

            yield break;
        }

        #endregion
    }
}
