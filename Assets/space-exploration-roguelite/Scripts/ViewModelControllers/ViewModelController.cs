using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class ViewModelController : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] protected bool _setup = false;
        [SerializeField] protected ItemDataSO _itemDataSO = null;
        [SerializeField] protected PlayerController _playerController = null;
        [SerializeField] protected bool _currentPrimaryActionInput = false;

        public virtual void Setup(PlayerController playerController, ItemDataSO itemDataSO)
        {
            if (_setup)
            {
                return;
            }

            _playerController = playerController;
            _itemDataSO = itemDataSO;

            _setup = true;
        }

        public virtual void Unsetup()
        {
            if (!_setup)
            {
                return;
            }

            _setup = false;

            _itemDataSO = null;
            _playerController = null;
        }

        public virtual void PrimaryActionInputChanged(bool input)
        {
            if (_playerController == null || _itemDataSO == null)
            {
                return;
            }

            _currentPrimaryActionInput = input;
        }
    }
}
