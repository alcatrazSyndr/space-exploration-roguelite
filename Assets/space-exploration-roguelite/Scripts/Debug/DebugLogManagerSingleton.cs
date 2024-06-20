using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace SpaceExplorationRoguelite
{
    public class DebugLogManagerSingleton : MonoBehaviour
    {
        public static DebugLogManagerSingleton Instance
        {
            get;
            private set;
        }

        [Header("Input")]
        [SerializeField] private InputAction _toggleConsoleInput;
        [SerializeField] private InputAction _clearConsoleInput;

        [Header("Prefabs")]
        [SerializeField] private GameObject _debugMessagePrefab;

        [Header("Components")]
        [SerializeField] private Canvas _debugCanvas;
        [SerializeField] private RectTransform _debugMessageRoot;

        [Header("Runtime")]
        [SerializeField] private List<GameObject> _debugMessageList = new List<GameObject>();

        #region Awake

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;

                DontDestroyOnLoad(this.gameObject);

                InitializeClearConsoleInput();
                InitializeToggleConsoleInput();
            }
        }

        #endregion

        #region Logging

        public void LogMessage(string message, Enums.DebugLogMessageType messageType, bool fromServer)
        {
            var color = EnumsUtility.GetDebugMessageColorFromType(messageType);

            var messageSenderColorTag = fromServer ? Constants.DEBUG_LOG_SERVER_MESSAGE_COLOR_TAG : Constants.DEBUG_LOG_CLIENT_MESSAGE_COLOR_TAG;
            var messageSenderNameTag = fromServer ? "[Server]: " : "[Client]: ";
            var messageString = $"<color={messageSenderColorTag}>" + messageSenderNameTag + "</color>" + message;

            var messageGO = Instantiate(_debugMessagePrefab, _debugMessageRoot);
            var messageText = messageGO.GetComponent<TextMeshProUGUI>();
            if (messageText != null)
            {
                messageText.color = color;
                messageText.text = messageString;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_debugMessageRoot);
        }

        private void ClearLog()
        {
            for (int i = _debugMessageList.Count - 1; i >= 0; i--)
            {
                Destroy(_debugMessageList[i]);
                _debugMessageList.RemoveAt(i);
            }
        }

        private void ToggleConsole()
        {
            _debugCanvas.enabled = !_debugCanvas.enabled;
            _debugCanvas.gameObject.SetActive(!_debugCanvas.gameObject.activeSelf);
        }

        #endregion

        #region Input

        private void InitializeToggleConsoleInput()
        {
            _toggleConsoleInput.Enable();
            _toggleConsoleInput.performed += ToggleConsoleInputPerformed;
        }

        private void DeinitializeToggleConsoleInput()
        {
            _toggleConsoleInput.Disable();
            _toggleConsoleInput.performed -= ToggleConsoleInputPerformed;
        }

        private void ToggleConsoleInputPerformed(InputAction.CallbackContext context)
        {
            ToggleConsole();
        }

        private void InitializeClearConsoleInput()
        {
            _clearConsoleInput.Enable();
            _clearConsoleInput.performed += ClearConsoleInputPerformed;
        }

        private void DeinitializeClearConsoleInput()
        {
            _clearConsoleInput.Disable();
            _clearConsoleInput.performed -= ClearConsoleInputPerformed;
        }

        private void ClearConsoleInputPerformed(InputAction.CallbackContext context)
        {
            ClearLog();
        }

        #endregion
    }
}
