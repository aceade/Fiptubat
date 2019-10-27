using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetPriority
{
    CLOSEST,
    HIGHEST_REMAINING_HEALTH,
    CLOSEST_TO_DEATH,
    HIGHEST_DAMAGE_OUTPUT,
    LAST_ATTACKER
}
