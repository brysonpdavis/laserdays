using UnityEngine;

[CreateAssetMenu(fileName = "New WorldSettings", menuName = "World Settings", order = 51)]
public class WorldSettings : ScriptableObject
{
    [SerializeField] private Material skyBoxMaterial;

    [SerializeField] private Color laserFog;

    [SerializeField] private Color laserAmbient;

    [SerializeField] private Color realFog;

    [SerializeField] private Color realAmbient;

    [SerializeField] private bool playerCanSwitch;

    [SerializeField] private Material realGlobalParticle;

    [SerializeField] private Material laserGlobalParticle;

    [SerializeField] private MFPP.FlipClipAsset flipClip;

    [SerializeField] private bool soundtrackEnabled = true;

    [SerializeField] private bool muteOpeningSoundtrack = true;


    public Material GetSkyBoxMaterial()
    {
        return skyBoxMaterial;
    }

    public Color GetLaserFog()
    {
        return laserFog;
    }

    public Color GetLaserAmbient()
    {
        return laserAmbient;
    }

    public Color GetRealFog()
    {
        return realFog;
    }

    public Color GetRealAmbient()
    {
        return realAmbient;
    }

    public bool GetPlayerCanSwitch()
    {
        return playerCanSwitch;
    }

    public Material GetRealGlobalParticle()
    {
        return realGlobalParticle;
    }

    public Material GetLaserGlobalParticle()
    {
        return laserGlobalParticle;
    }

    public MFPP.FlipClipAsset GetFlipClip()
    {
        return flipClip;
    }

    public bool GetSoundtrackEnabled()
    {
        return soundtrackEnabled;
    }

    public bool MuteOpeningSoundtrack()
    {
        return muteOpeningSoundtrack;
    }
}
