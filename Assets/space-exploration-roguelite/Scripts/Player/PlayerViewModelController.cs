using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerViewModelController : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private bool _setup = false;
        [SerializeField] private PlayerController _playerController = null;

        #region Setup/Unsetup

        public void Setup(PlayerController playerController)
        {
            if (_setup)
            {
                return;
            }

            _playerController = playerController;

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
