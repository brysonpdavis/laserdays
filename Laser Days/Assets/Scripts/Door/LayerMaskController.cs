using UnityEngine;
using UnityEditor;

public static class LayerMaskController 
{
    public enum Layer 
    {
        Default = 0,
        TransparentFX = 1,
        IgnoreRaycast = 2,
        Water = 4,
        UI = 5,
        Ladder = 8,
        Global = 9,
        Laser = 10,
        Real = 11, 
        Charging = 12,
        MovingPlatform = 13,
        GuardShared = 14,
        PlayerLaser = 15, 
        PlayerReal = 16, 
        StaticObjects = 17,
        LadderReal = 18,
        LadderLaser = 19,
        SokobanBumper = 20,
        InactiveLaser = 21,
        InactiveReal = 22,
        GuardLaser = 23,
        GuardReal = 24,
        TriggerReal = 25, 
        TriggerLaser = 26,
        TransitionOnly = 27,
        EditorOverlay = 28,
        ObjectBlocker = 29,
        MapIcons = 30,
        PlayerOnly = 31
    }


    public static LayerMask Real = 1 << 0 | 1 << 11 | 1 << 17;

    public static LayerMask RealWithPlayer = 1 << 0 | 1 << 11 | 1 << 17 | 1 << 16;

    public static LayerMask Laser = 1 << 0 | 1 << 10 | 1 << 17;

    public static LayerMask LaserWithPlayer = 1 << 0 | 1 << 10 | 1 << 17 | 1 << 15;

    public static LayerMask Everything = 1 << 0 | 1 << 10 | 1 << 11 | 1 << 17 | 1 << 16 | 1 << 15;

    public static LayerMask SharedOnly = 1 << 0 | 1 << 17;

    public static LayerMask SpawnCast = 1 << 0 | 1 << 17 | 1 << 10 | 1 << 11 | 1 << 12;


    public static LayerMask GetLayerMaskForRaycast(int playerLayer)
    {
        if(playerLayer == 15)
        {
            return LaserWithPlayer;
        }

        if (playerLayer == 16)
        {
            return RealWithPlayer;
        }

        return Everything;
    }

}
