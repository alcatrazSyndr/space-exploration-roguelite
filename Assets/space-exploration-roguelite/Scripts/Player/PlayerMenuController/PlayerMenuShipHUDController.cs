using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public class PlayerMenuShipHUDController : PlayerMenuController
    {
        public void FlightTargetInputChange(Vector2 delta)
        {
            (_view as PlayerMenuShipHUDView).ChangeFlightTargetPosition(delta);
        }

        public Vector2 GetCurrentFlightTargetPosition()
        {
            return (_view as PlayerMenuShipHUDView).CurrentFlightTargetPosition;
        }

        public void ToggleShipHUD(bool toggle, Enums.ControllableObjectType type)
        {
            (_view as PlayerMenuShipHUDView).ToggleShipHUD(toggle, type);
        }
    }
}
