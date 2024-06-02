using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class DebugLightSwitchController : NetworkBehaviour
    {
        [Header("Components")]
        [SerializeField] private InteractableObjectController _interactableSwitch;
        [SerializeField] private MeshRenderer _lightMeshRenderer;

        public override void OnStartServer()
        {
            base.OnStartServer();

            _interactableSwitch.OnInteracted.AddListener(LightSwitchInteraction);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            _interactableSwitch.OnInteracted.RemoveListener(LightSwitchInteraction);
        }

        [Server]
        private void LightSwitchInteraction()
        {
            var color = Random.ColorHSV();
            LightSwitchColorChange(color);
        }

        [ObserversRpc]
        private void LightSwitchColorChange(Color color)
        {
            _lightMeshRenderer.material.color = color;
        }
    }
}
