using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class GameManagerSingleton : NetworkBehaviour
    {
        public static GameManagerSingleton Instance
        {
            get;
            private set;
        }

        [Header("Components")]
        [SerializeField] private Transform _playerPawnSpawnPosition;
        [SerializeField] private Transform _worldTransformOrigin;

        [Header("Prefabs")]
        [SerializeField] private GameObject _playerPawnPrefab;
        [SerializeField] private GameObject _playerPrefab;

        [Header("Runtime")]
        private readonly SyncDictionary<NetworkConnection, PlayerController> _playerControllerDictionary = new();

        #region Singleton Setup

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        #endregion

        #region Server Setup/Unsetup

        public override void OnStartServer()
        {
            base.OnStartServer();

            ServerManager.OnRemoteConnectionState += OnPlayerConnectionStateChanged;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            ServerManager.OnRemoteConnectionState -= OnPlayerConnectionStateChanged;
        }

        #endregion

        #region Player Connection

        [Server]
        private void OnPlayerConnectionStateChanged(NetworkConnection playerConnection, FishNet.Transporting.RemoteConnectionStateArgs connectionStateArgs)
        {
            if (connectionStateArgs.ConnectionState == RemoteConnectionState.Started && !_playerControllerDictionary.ContainsKey(playerConnection))
            {
                playerConnection.OnLoadedStartScenes += PlayerConnectionLoadedStartScenes;
            }
            else if (connectionStateArgs.ConnectionState == RemoteConnectionState.Stopped && _playerControllerDictionary.ContainsKey(playerConnection))
            {
                var ownedObjectList = new List<NetworkObject>(playerConnection.Objects);
                for (int i = ownedObjectList.Count - 1; i >= 0; i--)
                {
                    if (ownedObjectList[i].GetComponent<SpaceshipController>())
                    {
                        ownedObjectList[i].RemoveOwnership();
                    }
                }

                var playerController = _playerControllerDictionary[playerConnection];

                if (playerController != null)
                {
                    if (playerController.PlayerPawnController.Value != null)
                    {
                        ServerManager.Despawn(playerController.PlayerPawnController.Value.gameObject);
                    }

                    ServerManager.Despawn(playerController.gameObject);
                }

                _playerControllerDictionary.Remove(playerConnection);
            }
        }

        [Server]
        private void PlayerConnectionLoadedStartScenes(NetworkConnection playerConnection, bool asServer)
        {
            if (!asServer)
            {
                return;
            }

            var playerControllerInstance = Instantiate(_playerPrefab, _worldTransformOrigin);
            playerControllerInstance.transform.position = Vector3.zero;
            var playerController = playerControllerInstance.GetComponent<PlayerController>();

            ServerManager.Spawn(playerControllerInstance, playerConnection);

            _playerControllerDictionary.Add(playerConnection, playerController);

            playerConnection.OnLoadedStartScenes -= PlayerConnectionLoadedStartScenes;

            if (playerController.PlayerPawnController.Value != null)
            {
                return;
            }

            var playerPawnInstance = Instantiate(_playerPawnPrefab, _worldTransformOrigin);
            playerPawnInstance.transform.position = _playerPawnSpawnPosition.position;
            var playerPawnController = playerPawnInstance.GetComponent<PlayerPawnController>();

            ServerManager.Spawn(playerPawnInstance, playerConnection);

            playerController.PlayerPawnController.Value = playerPawnController;
        }

        public PlayerController GetPlayerConnectionPlayerController(NetworkConnection playerConnection)
        {
            if (_playerControllerDictionary.ContainsKey(playerConnection))
            {
                if (_playerControllerDictionary[playerConnection] != null)
                {
                    return _playerControllerDictionary[playerConnection];
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

        #endregion

        #region Player Inventory Management

        [ServerRpc(RequireOwnership = false)]
        public void RequestInitialInventoryDataFromServer(NetworkConnection playerConnection)
        {
            if (_playerControllerDictionary.ContainsKey(playerConnection) && _playerControllerDictionary[playerConnection] != null)
            {
                DeliverInventoryDataToClientPlayerController(_playerControllerDictionary[playerConnection]);
                DeliverActionbarDataToClientPlayerController(_playerControllerDictionary[playerConnection]);
            }
        }

        [Server]
        private void DeliverInventoryDataToClientPlayerController(PlayerController playerController)
        {
            var itemSlotList = new List<ItemSlot>();

            // DEBUG

            for (int i = 0; i < Constants.PLAYER_INVENTORY_MAX_CAPACITY; i++)
            {
                var itemSlot = new ItemSlot();
                itemSlot.SlotIndex = i;

                itemSlotList.Add(itemSlot);
            }

            // DEBUG

            var itemSlotJson = JsonConvert.SerializeObject(itemSlotList);

            if (playerController != null)
            {
                playerController.InventoryItemSlotDataReceiveFromServer(playerController.Owner, itemSlotJson);
            }
        }

        [Server]
        private void DeliverActionbarDataToClientPlayerController(PlayerController playerController)
        {
            var itemSlotList = new List<ItemSlot>();

            // DEBUG

            for (int i = 0; i < Constants.PLAYER_ACTIONBAR_MAX_CAPACITY; i++)
            {
                var itemSlot = new ItemSlot();
                itemSlot.SlotIndex = i;

                if (i == 0)
                {
                    itemSlot.ItemID = "Item_Weapon_LaserRifle";
                    itemSlot.ItemCount = 1;
                }

                itemSlotList.Add(itemSlot);
            }

            // DEBUG

            var itemSlotJson = JsonConvert.SerializeObject(itemSlotList);

            if (playerController != null)
            {
                playerController.ActionbarItemSlotDataReceiveFromServer(playerController.Owner, itemSlotJson);
            }
        }

        #endregion
    }
}
