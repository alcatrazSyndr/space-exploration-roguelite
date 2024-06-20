using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class ItemDataManagerSingleton : MonoBehaviour
    {
        public static ItemDataManagerSingleton Instance
        {
            get;
            private set;
        }

        [Header("Item Data")]
        [SerializeField] private List<ItemDataSO> _allItemDataSOList = new List<ItemDataSO>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;

                DontDestroyOnLoad(gameObject);
            }
        }

        public ItemDataSO GetItemDataSOWithItemID(string itemID)
        {
            var itemDataSO = _allItemDataSOList.Find(t => t.ItemID.Equals(itemID));

            return itemDataSO;
        }
    }
}
