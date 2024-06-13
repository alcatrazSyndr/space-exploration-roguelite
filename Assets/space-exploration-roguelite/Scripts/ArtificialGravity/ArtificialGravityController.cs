using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class ArtificialGravityController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private List<ArtificialGravityBounds> _artificialGravityBounds = new List<ArtificialGravityBounds>();

        public void Start()
        {
            foreach (var artificialGravityBounds in _artificialGravityBounds)
            {
                artificialGravityBounds.Setup(this);
            }
        }

        public void ColliderLeftGravityBounds(ArtificialGravityBounds bounds, Collider collider)
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
