using FishNet.Object;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerPawnController : NetworkBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private bool _setup = false;

        #region Setup/Unsetup

        public void Setup()
        {
            if (_setup)
            {
                return;
            }
            _setup = true;
        }

        public void Unsetup()
        {
            if (!_setup)
            {
                return;
            }
            _setup = false;
        }

        #endregion
    }
}
