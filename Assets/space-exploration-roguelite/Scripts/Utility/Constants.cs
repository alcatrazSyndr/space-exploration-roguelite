using UnityEngine;

namespace SpaceExplorationRoguelite
{
    public static class Constants
    {
        public static float PLAYERPAWN_NO_GRAVITY_MOVE_RATE = 0.05f;
        public static float PLAYERPAWN_NO_GRAVITY_LEAN_RATE = 1f;
        public static float PLAYERPAWN_NO_GRAVITY_ROTATE_RATE = 0.01f;

        public static float PLAYERPAWN_ARTIFICIAL_GRAVITY_MOVE_RATE = 0.07f;
        public static float PLAYERPAWN_ARTIFICIAL_GRAVITY_ROTATE_RATE = 0.01f;
        public static float PLAYERPAWN_ARTIFICIAL_GRAVITY_ENTRY_UP_FIRECTION_FIX_TIMER = 0.5f;
        public static float PLAYERPAWN_ARTIFICIAL_GRAVITY_CHECK_LINE_LENGTH = 2f;
        public static float PLAYERPAWN_ARTIFICIAL_GRAVITY_CHECK_LINE_TARGET_LENGTH_MAX = 0.52f;
        public static float PLAYERPAWN_ARTIFICIAL_GRAVITY_CHECK_LINE_TARGET_LENGTH_MIN = 0.51f;
        public static float PLAYERPAWN_ARTIFICIAL_GRAVITY_CHECK_LINE_OFFSET = 1.5f;
        public static float PLAYERPAWN_ARTIFICIAL_GRAVITY_FORCE = 12f;

        public static float PLAYERPAWN_ANIMATION_MOVEMENT_ANIMATION_TRANSITION_TIME = 0.15f;

        public static int PLAYER_INVENTORY_MAX_CAPACITY = 24;
        public static int PLAYER_ACTIONBAR_MAX_CAPACITY = 6;

        public static string DEBUG_LOG_CLIENT_MESSAGE_COLOR_TAG = "#009CFF";
        public static string DEBUG_LOG_SERVER_MESSAGE_COLOR_TAG = "#CE00FF";

        public static string VIEWMODEL_FIRE_ANIMATION_NAME = "Fire";
    }
}
