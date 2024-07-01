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
    }
}
