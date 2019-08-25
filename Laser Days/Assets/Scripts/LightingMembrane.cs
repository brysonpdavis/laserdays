using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.AccessControl;

public class LightingMembrane : MonoBehaviour {

    public enum Direction { Up, Down, PosZ, PosX, NegZ, NegX };
    public Direction direction;
    private Vector3 axis;
    public float distance = 3f;
    public float ambMultiplier = .5f;
    public float fogMultiplier = .5f;
    public float density = .1f;

    SkyboxTransition skybox;
    UnityStandardAssets.ImageEffects.GlobalFog fog;

    public bool active;
    private Transform player;

    private float normalFogMultiplier;
    private float normalAmbMultipliet;
    private float normalDensity;

    private void Start()
    {
        skybox = Toolbox.Instance.GetPlayer().GetComponent<SkyboxTransition>();
        normalFogMultiplier = skybox.fogMultiplier;
        normalAmbMultipliet = skybox.ambientMultiplier;
        fog = Camera.main.GetComponent<UnityStandardAssets.ImageEffects.GlobalFog>();
        normalDensity = RenderSettings.fogDensity;
        player = Toolbox.Instance.GetPlayer().transform;
        active = false;
        skybox.AdMembrane(this);
    }

    private void FixedUpdate()
    {
        if(active)
        {
            DoLightingChange();
        }

    }

    void DoLightingChange()
    {
        SetAxis(direction);

        float ratio = PlayerAlongAxis(direction);

        float densityValue = Mathf.Lerp(normalDensity, density, ratio);
        RenderSettings.fogDensity = densityValue;

        float fogMultVal = Mathf.Lerp(normalFogMultiplier, fogMultiplier, ratio);
        skybox.fogMultiplier = fogMultVal;

        float ambientMultVal = Mathf.Lerp(normalAmbMultipliet, ambMultiplier, ratio);
        skybox.ambientMultiplier = ambientMultVal;
    }

    float PlayerAlongAxis(Direction d)
    {
        float dist = 0;

        switch (d)
        {
            case Direction.Up:
                dist = player.position.y - transform.position.y;
                break;
            case Direction.Down:
                dist = transform.position.y - player.position.y; 
                break;
            case Direction.NegX:
                dist = transform.position.x - player.position.x;
                break;
            case Direction.PosX:
                dist = player.position.x - transform.position.x;
                break;
            case Direction.NegZ:
                dist = transform.position.z - player.position.z;
                break;
            case Direction.PosZ:
                dist = player.position.z - transform.position.z;
                break;
        }

        return Mathf.Clamp01(dist / distance);
    }

    void SetAxis(Direction d)
    {
        switch (d)
        {
            case Direction.Up:
                axis = (Vector3.up);
                break;
            case Direction.Down:
                axis = Vector3.down;
                break;
            case Direction.NegX:
                axis = Vector3.left;
                break;
            case Direction.PosX:
                axis = Vector3.right;
                break;
            case Direction.NegZ:
                axis = Vector3.back;
                break;
            case Direction.PosZ:
                axis = Vector3.forward;
                break;
        }
    }

   
}
