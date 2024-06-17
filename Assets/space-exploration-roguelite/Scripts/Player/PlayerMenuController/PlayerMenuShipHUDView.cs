using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SpaceExplorationRoguelite
{
    public class PlayerMenuShipHUDView : PlayerMenuView
    {
        [Header("Data")]
        [SerializeField] private float _flightTargetInnerBounds = 0f;
        [SerializeField] private float _flightTargetOuterBounds = 0f;
        [SerializeField] private float _flightTargetMoveSensitivity = 1f;
        [SerializeField] private float _flightTargetAlphaTweenOffset = 0.25f;
        [SerializeField] private float _flightTargetConnectorLengthOffset = 1f;

        [Header("Components")]
        [SerializeField] private Image _crosshairFlightTargetImage;
        [SerializeField] private RectTransform _crosshairFlightTargetRect;
        [SerializeField] private Image _crosshairFlightTargetConnectorImage;
        [SerializeField] private RectTransform _crosshairFlightTargetConnectorRect;
        [SerializeField] private RectTransform _crosshairFlightTargetConnectorRootRect;
        [SerializeField] private GameObject _shipHUDRootGO;

        [Header("Runtime")]
        [SerializeField] private bool _setup = false;
        [SerializeField] private bool _enableRotation = true;
        [SerializeField] private float _flightTargetBoundsRange = 0f;
        [SerializeField] private Vector2 _currentFlightTargetPosition = Vector2.zero;
        public Vector2 CurrentFlightTargetPosition
        {
            get
            {
                return _currentFlightTargetPosition;
            }
        }

        public override void Show()
        {
            base.Show();

            _crosshairFlightTargetRect.anchoredPosition = Vector2.zero;
            _flightTargetBoundsRange = _flightTargetOuterBounds - _flightTargetInnerBounds;

            _setup = true;

            ChangeFlightTargetPosition(Vector2.zero);
        }

        public override void Hide()
        {
            base.Hide();

            _setup = false;

            _crosshairFlightTargetRect.anchoredPosition = Vector2.zero;
        }

        public void ChangeFlightTargetPosition(Vector2 delta)
        {
            if (!_setup)
            {
                return;
            }

            if (!_enableRotation)
            {
                return;
            }

            _crosshairFlightTargetRect.anchoredPosition += (delta * _flightTargetMoveSensitivity);

            var distanceFromCenter = _crosshairFlightTargetRect.anchoredPosition.magnitude;

            if (distanceFromCenter > _flightTargetOuterBounds)
            {
                _crosshairFlightTargetRect.anchoredPosition = _crosshairFlightTargetRect.anchoredPosition.normalized * _flightTargetOuterBounds;

                distanceFromCenter = _crosshairFlightTargetRect.anchoredPosition.magnitude;
            }
            else if (distanceFromCenter < _flightTargetInnerBounds)
            {
                distanceFromCenter = _flightTargetInnerBounds;
            }

            var targetRotation = Quaternion.LookRotation(new Vector3(_crosshairFlightTargetRect.anchoredPosition.x, 0f, _crosshairFlightTargetRect.anchoredPosition.y), Vector3.up);
            _crosshairFlightTargetConnectorRootRect.rotation = Quaternion.Euler(0f, 0f, -targetRotation.eulerAngles.y);

            var connectorDistance = (distanceFromCenter - _flightTargetInnerBounds);
            var alphaDistance = connectorDistance / _flightTargetBoundsRange;
            var alpha = alphaDistance + (alphaDistance > 0.01f ? _flightTargetAlphaTweenOffset : 0f);

            _crosshairFlightTargetConnectorRect.sizeDelta = new Vector2(1f, connectorDistance - _flightTargetConnectorLengthOffset);

            var color = _crosshairFlightTargetImage.color;
            color.a = alpha;
            _crosshairFlightTargetImage.color = color;
            _crosshairFlightTargetConnectorImage.color = color;

            var flightTargetPosition = _crosshairFlightTargetRect.anchoredPosition.normalized * alphaDistance;
            if (_currentFlightTargetPosition != flightTargetPosition)
            {
                _currentFlightTargetPosition = flightTargetPosition;
            }
        }

        public void ToggleShipHUD(bool toggle, Enums.ControllableObjectType type)
        {
            _shipHUDRootGO.SetActive(toggle);

            if (!toggle)
            {
                _crosshairFlightTargetRect.anchoredPosition = Vector2.zero;
                ChangeFlightTargetPosition(Vector2.zero);
            }

            _enableRotation = toggle;
        }
    }
}