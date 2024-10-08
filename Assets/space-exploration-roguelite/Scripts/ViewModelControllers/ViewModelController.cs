using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class ViewModelController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected Animator _primaryAnimator = null;
        [SerializeField] protected Animator _secondaryAnimator = null;

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
            if (_playerController == null)
            {
                return;
            }

            if (_itemDataSO == null)
            {
                return;
            }

            if (!_setup)
            {
                return;
            }

            _currentPrimaryActionInput = input;
        }
    }
}
