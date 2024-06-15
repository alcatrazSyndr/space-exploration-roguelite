using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceExplorationRoguelite
{
    public class InteractableObjectController : NetworkBehaviour
    {
        [Header("Components")]
        [SerializeField] private ControllableObjectController _controllableObject = null;
        public ControllableObjectController ControllableObject
        {
            get
            {
                return _controllableObject;
            }
        }

        [Header("Data")]
        [SerializeField] private bool _interactCooldown = false;
        [SerializeField] private float _interactCooldownTimer = 0f;
        [SerializeField] private string _defaultInteractActionText = "[F] Interact";

        [Header("Runtime")]
        private readonly SyncVar<bool> _interactable = new();
        public bool Interactable
        {
            get
            {
                return _interactable.Value;
            }
        }
        private readonly SyncVar<string> _interactActionText = new();
        public string InteractActionText
        {
            get
            {
                return _interactActionText.Value;
            }
        }

        [Header("Events")]
        public UnityEvent OnInteracted = new UnityEvent();

        public override void OnStartServer()
        {
            base.OnStartServer();

            _interactable.Value = true;

            SetInteractActionText(_defaultInteractActionText);
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

            SetInteractable(false);

            OnInteracted?.Invoke();

            if (_interactCooldown)
            {
                StartCoroutine(InteractCooldownCRT());
            }
        }

        private IEnumerator InteractCooldownCRT()
        {
            yield return new WaitForSecondsRealtime(_interactCooldownTimer);

            _interactable.Value = true;

            yield break;
        }

        [Server]
        public void SetInteractActionText(string text)
        {
            if (!base.IsServerStarted)
            {
                return;
            }

            _interactActionText.Value = text;
        }

        [Server]
        public void SetInteractable(bool interactable)
        {
            if (!base.IsServerStarted)
            {
                return;
            }

            _interactable.Value = interactable;
        }
    }
}
