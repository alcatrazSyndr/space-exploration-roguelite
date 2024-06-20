using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    [CreateAssetMenu(fileName = "ItemDataSO_", menuName = "SpaceExplorationRoguelite/New ItemDataSO")]
    public class ItemDataSO : ScriptableObject
    {
        public string ItemID;
        public string ItemName;
        public Sprite ItemSprite;
        public Enums.ItemType ItemType;
        public bool Stackable = false;
        public GameObject ViewModelPrefab;
    }
}
