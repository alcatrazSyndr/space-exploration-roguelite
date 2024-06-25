using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public static class Enums
    {
        public enum PlayerMenuType
        {
            None,
            HUD,
            ShipHUD,
            Inventory
        }

        public enum ControllableObjectType
        {
            None,
            Ship
        }

        public enum DebugLogMessageType
        {
            Default,
            Error,
            Warning
        }

        public enum ItemType
        {
            None,
            Item,
            Tool
        }

        public enum ToolType
        {
            Tool,
            Weapon
        }

        public enum ToolAnimType
        {
            Unarmed,
            Rifle
        }

        public enum WeaponType
        {
            Ranged,
            Melee
        }

        public enum RangedWeaponFiringType
        {
            SingleAction,
            SemiAutomatic,
            Automatic,
            Beam
        }
    }

    public static class EnumsUtility
    {
        public static Color GetDebugMessageColorFromType(Enums.DebugLogMessageType type)
        {
            switch (type)
            {
                case Enums.DebugLogMessageType.Default:
                    return Color.green;
                case Enums.DebugLogMessageType.Error:
                    return Color.red;
                case Enums.DebugLogMessageType.Warning:
                    return Color.yellow;
                default:
                    return Color.green;
            }
        }
    }
}
