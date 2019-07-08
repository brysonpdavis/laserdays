using UnityEngine;
using UnityEditor;

public static class LayerMaskController 
{
    public static LayerMask Real = 1 << 0 | 1 << 11 | 1 << 17;

    public static LayerMask RealWithPlayer = 1 << 0 | 1 << 11 | 1 << 17 | 1 << 16;

    public static LayerMask Laser = 1 << 0 | 1 << 10 | 1 << 17;

    public static LayerMask LaserWithPlayer = 1 << 0 | 1 << 10 | 1 << 17 | 1 << 15;

    public static LayerMask Everything = 1 << 0 | 1 << 10 | 1 << 11 | 1 << 17 | 1 << 16 | 1 << 15;

    public static LayerMask SharedOnly = 1 << 0 | 1 << 17;


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
