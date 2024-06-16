using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class ArtificialGravityBounds : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private LayerMask _collisionLayerMask;

        [Header("Components")]
        [SerializeField] private BoxCollider _collider;

        [Header("Runtime")]
        [SerializeField] private List<Collider> _collidersInBoxcastList = new List<Collider>();
        [SerializeField] private ArtificialGravityController _artificialGravityController = null;

        public void Setup(ArtificialGravityController artificialGravityController)
        {
            _artificialGravityController = artificialGravityController;
        }

        public void Unsetup()
        {
            _artificialGravityController = null;
        }

        public void OnPostTick()
        {
            if (_artificialGravityController == null)
            {
                return;
            }

            var hits = Physics.BoxCastAll(_collider.bounds.center, (new Vector3(_collider.size.x * transform.localScale.x, _collider.size.y * transform.localScale.y, _collider.size.z * transform.localScale.z)) * 0.5f, transform.forward, transform.rotation, 0f, _collisionLayerMask);
            var colliders = new List<Collider>();

            foreach (var hit in hits)
            {
                if (!colliders.Contains(hit.collider))
                {
                    colliders.Add(hit.collider);
                }
                if (!_collidersInBoxcastList.Contains(hit.collider))
                {
                    _collidersInBoxcastList.Add(hit.collider);

                    _artificialGravityController.ColliderEnteredGravityBounds(this, hit.collider);
                }
            }

            for (int i = _collidersInBoxcastList.Count - 1; i >= 0; i--)
            {
                var collider = _collidersInBoxcastList[i];

                if (!colliders.Contains(collider))
                {
                    _collidersInBoxcastList.RemoveAt(i);

                    _artificialGravityController.ColliderLeftGravityBounds(this, collider);
                }
            }
        }

        public bool ContainsCollider(Collider collider)
        {
            return _collidersInBoxcastList.Contains(collider);
        }
    }
}
