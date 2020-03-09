using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Game specific utils and enums
public enum PolygonMod {
    Checkpoint,
    Gravity,
    Teleport,
    Bond,
    BehaviorRoot
}

public static class Game {
    public static void SetGravity(Vector3 dir) {
        float gravMagnitude = Physics.gravity.magnitude;
        Physics.gravity = dir.normalized * gravMagnitude;
    }
}
