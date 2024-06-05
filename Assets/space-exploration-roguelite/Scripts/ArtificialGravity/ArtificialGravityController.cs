using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class ArtificialGravityController : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private List<Collider> _playerPawnColliderList = new List<Collider>();

        private void OnTriggerStay(Collider other)
        {
            if (!_playerPawnColliderList.Contains(other))
            {
                _playerPawnColliderList.Add(other);

                var playerPawnController = other.GetComponent<PlayerPawnController>();
                if (playerPawnController != null)
                {
                    playerPawnController.SetArtificialGravityController(this);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_playerPawnColliderList.Contains(other))
            {
                _playerPawnColliderList.Remove(other);

                var playerPawnController = other.GetComponent<PlayerPawnController>();
                if (playerPawnController != null)
                {
                    playerPawnController.SetArtificialGravityController(null);
                }
            }
        }

        private void FixedUpdate()
        {
            for (int i = _playerPawnColliderList.Count - 1; i >= 0; i--)
            {
                if (_playerPawnColliderList[i] == null)
                {
                    _playerPawnColliderList.RemoveAt(i);
                }
            }
        }
    }
}
