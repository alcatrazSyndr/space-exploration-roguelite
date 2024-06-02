using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceExplorationRoguelite
{
    public class InteractableObjectController : NetworkBehaviour
    {
        [Header("Data")]
        [SerializeField] private float _interactCooldownTimer = 0f;

        [Header("Runtime")]
        private readonly SyncVar<bool> _interactable = new();
        public bool Interactable
        {
            get
            {
                return _interactable.Value;
            }
        }

        [Header("Events")]
        public UnityEvent OnInteracted = new UnityEvent();

        public override void OnStartServer()
        {
            base.OnStartServer();

            _interactable.Value = true;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            StopAllCoroutines();

            _interactable.Value = false;
        }

        [ServerRpc(RequireOwnership = false)]
        public void Interact()
        {
            if (!_interactable.Value)
            {
                return;
            }

            _interactable.Value = false;

            OnInteracted?.Invoke();

            StartCoroutine(InteractCooldownCRT());
        }

        private IEnumerator InteractCooldownCRT()
        {
            yield return new WaitForSecondsRealtime(_interactCooldownTimer);

            _interactable.Value = true;

            yield break;
        }
    }
}
