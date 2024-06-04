namespace SpaceExplorationRoguelite
{
    public class PlayerMenuHUDController : PlayerMenuController
    {
        public void ToggleInteractText(bool toggle, string interactText = "")
        {
            (_view as PlayerMenuHUDView).ToggleInteractText(toggle, interactText);
        }
    }
}
