using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class ControllableObjectController : NetworkBehaviour
    {
        public void ReleaseOwnership()
        {
            if (base.Owner.ClientId != ClientManager.Connection.ClientId)
            {
                return;
            }

            ReleaseOwnershipRPC(ClientManager.Connection);
        }

        [ServerRpc]
        private void ReleaseOwnershipRPC(NetworkConnection playerConnection)
        {
            if (base.Owner != playerConnection)
            {
                return;
            }

            base.RemoveOwnership();
        }

        public void ClaimOwnership()
        {
            if (base.Owner.ClientId != -1)
            {
                return;
            }

            ClaimOwnershipRPC(ClientManager.Connection);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ClaimOwnershipRPC(NetworkConnection playerConnection)
        {
            if (!base.HasAuthority)
            {
                return;
            }

            base.GiveOwnership(playerConnection);
        }

        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            base.OnOwnershipClient(prevOwner);

            var myPlayerController = GameManagerSingleton.Instance.GetPlayerConnectionPlayerController(ClientManager.Connection);

            if (myPlayerController != null)
            {
                if (base.IsOwner)
                {
                    if (myPlayerController.CurrentControlledObject == null)
                    {
                        myPlayerController.CurrentControlledObjectChanged(this);
                    }
                }
                else if (myPlayerController.CurrentControlledObject == this)
                {
                    myPlayerController.CurrentControlledObjectChanged(null);
                }
            }
        }
    }
}
