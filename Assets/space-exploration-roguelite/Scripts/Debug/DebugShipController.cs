using FishNet.Connection;
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
        [SerializeField] private InteractableObjectController _pilotSeatInteractableObjectController;

        [Header("Runtime")]
        [SerializeField] private float _tickRate = 0f;

        public override void OnStartServer()
        {
            base.OnStartServer();

            _tickRate = (float)TimeManager.TickDelta;

            TimeManager.OnTick += OnTick;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            TimeManager.OnTick -= OnTick;
        }

        private void OnTick()
        {

        }

        public override void OnOwnershipServer(NetworkConnection prevOwner)
        {
            base.OnOwnershipServer(prevOwner);

            if (!base.IsServerStarted)
            {
                return;
            }

            if (_pilotSeatInteractableObjectController == null)
            {
                return;
            }

            if (base.OwnerId == -1)
            {
                _pilotSeatInteractableObjectController.SetInteractable(true);
            }
            else
            {
                _pilotSeatInteractableObjectController.SetInteractable(false);
            }
        }
    }
}
