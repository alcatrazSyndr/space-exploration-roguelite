using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PawnModelController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform _bulletOriginPoint;
        public Transform BulletOriginPoint
        {
            get
            {
                return _bulletOriginPoint;
            }
        }
        [SerializeField] private Transform _leftHandIKTarget;
        public Transform LeftHandIKTarget
        {
            get
            {
                return _leftHandIKTarget;
            }
        }
        [SerializeField] private Transform _rightHandIKTarget;
        public Transform RightHandIKTarget
        {
            get
            {
                return _rightHandIKTarget;
            }
        }
    }
}
