using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class ArtificialGravityController : NetworkBehaviour
    {
        [Header("Components")]
        [SerializeField] private List<ArtificialGravityBounds> _artificialGravityBounds = new List<ArtificialGravityBounds>();

        public override void OnStartClient()
        {
            base.OnStartClient();

            foreach (var artificialGravityBounds in _artificialGravityBounds)
            {
                artificialGravityBounds.Setup(this);
                TimeManager.OnPostTick += artificialGravityBounds.OnPostTick;
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            foreach (var artificialGravityBounds in _artificialGravityBounds)
            {
                TimeManager.OnPreTick -= artificialGravityBounds.OnPostTick;
                artificialGravityBounds.Unsetup();
            }
        }

        public void ColliderLeftGravityBounds(ArtificialGravityBounds bounds, Collider collider)
        {
            if (collider == null)
            {
                return;
            }

            foreach (var gravityBounds in _artificialGravityBounds)
            {
                if (gravityBounds == bounds)
                {
                    continue;
                }

                if (gravityBounds.ContainsCollider(collider))
                {
                    return;
                }
            }

            var playerPawnController = collider.GetComponent<PlayerPawnController>();
            if (playerPawnController != null)
            {
                playerPawnController.SetArtificialGravityController(null);
            }
        }

        public void ColliderEnteredGravityBounds(ArtificialGravityBounds bounds, Collider collider)
        {
            foreach (var gravityBounds in _artificialGravityBounds)
            {
                if (gravityBounds == bounds)
                {
                    continue;
                }

                if (gravityBounds.ContainsCollider(collider))
                {
                    return;
                }
            }

            var playerPawnController = collider.GetComponent<PlayerPawnController>();
            if (playerPawnController != null)
            {
                playerPawnController.SetArtificialGravityController(this);
            }
        }
    }
}
