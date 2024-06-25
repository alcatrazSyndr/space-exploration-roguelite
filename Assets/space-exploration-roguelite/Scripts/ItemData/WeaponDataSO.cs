using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    [CreateAssetMenu(fileName = "WeaponDataSO_", menuName = "SpaceExplorationRoguelite/New WeaponDataSO")]
    public class WeaponDataSO : ToolDataSO
    {
        [Header("Weapon Data")]
        public Enums.WeaponType WeaponType;
        public Enums.RangedWeaponFiringType FiringType;
        public GameObject BulletPrefab;
        public float FiringCooldown;
    }
}
