using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class DebugShipController : NetworkBehaviour
    {
        [Header("Components")]
        [SerializeField] private InteractableObjectController _doorInteractableObjectController;
        [SerializeField] private Transform _doorTransform;
        [SerializeField] private Transform _doorClosedTransform;
        [SerializeField] private Transform _doorOpenedTransform;

        [Header("Runtime")]
        private readonly SyncVar<bool> _doorOpened = new();

        public override void OnStartServer()
        {
            base.OnStartServer();

            _doorInteractableObjectController.OnInteracted.AddListener(ToggleShipDoor);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            _doorInteractableObjectController.OnInteracted.RemoveListener(ToggleShipDoor);
        }

        [Server]
        private void ToggleShipDoor()
        {
            _doorOpened.Value = !_doorOpened.Value;

            _doorTransform.position = _doorOpened.Value ? _doorOpenedTransform.position : _doorClosedTransform.position;
            _doorTransform.rotation = _doorOpened.Value ? _doorOpenedTransform.rotation : _doorClosedTransform.rotation;
        }
    }
}
