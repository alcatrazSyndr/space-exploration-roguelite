using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
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

        #endregion
    }
}
