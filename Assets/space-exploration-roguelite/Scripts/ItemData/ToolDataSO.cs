using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceExplorationRoguelite
{
    [CreateAssetMenu(fileName = "ToolDataSO_", menuName = "SpaceExplorationRoguelite/New ToolDataSO")]
    public class ToolDataSO : ItemDataSO
    {
        [Header("Tool Data")]
        public Enums.ToolType ToolType;
        public Enums.ToolAnimType ToolAnimationType;
    }
}
