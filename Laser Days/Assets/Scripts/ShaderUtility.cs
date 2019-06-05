using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public static class ShaderUtility
{

    public static void ShaderWorldChange(Material mat, bool toLaser)
    {
        Debug.Log("Looking for " + mat.shader);

        // If using our standard shader, then use keyword swap approach
        if (mat.shader == Shader.Find("Crosshatch/Standard"))
        {
            if (toLaser)
            {
                mat.DisableKeyword("REAL");
                mat.EnableKeyword("LASER");
                return;
            }
            else
            {
                mat.DisableKeyword("LASER");
                mat.EnableKeyword("REAL");
                return;
            }
        }

        // If using the old shaders, swap shader to the proper world
        if (mat.shader == Shader.Find("Transition/RealOnly") || mat.shader == Shader.Find("Transition/LaserOnly"))
        {
            if (toLaser)
            {
                mat.shader = Shader.Find("Transition/LaserOnly");
                return;
            }
            else
            {
                mat.shader = Shader.Find("Transition/RealOnly");
                return;
            }
        }

        // If using the Core shaders, swap Core shader to the proper world
        if (mat.shader == Shader.Find("Transition/RealCore") || mat.shader == Shader.Find("Transition/LaserCore"))
        {
            if (toLaser)
            {
                mat.shader = Shader.Find("Transition/LaserCore");
                return;
            }
            else
            {
                mat.shader = Shader.Find("Transition/RealCore");
                return;
            }
        }

        // If using the slightly old shadrrs, swap to proper world
        if (mat.shader == Shader.Find("Crosshatch/Real-Standard") || mat.shader == Shader.Find("Crosshatch/Laser-Standard"))
        {
            if (toLaser)
            {
                mat.shader = Shader.Find("Crosshatch/Laser-Standard");
                return;
            }
            else
            {
                mat.shader = Shader.Find("Crosshatch/Real-Standard");
                return;
            }
        }

        // Transparent shaders use a float to determine world 
        if (mat.shader == Shader.Find("Crosshatch/Glass-Single"))
        {
            if (toLaser)
            {
                mat.SetFloat("_Real", 0f);
                return;
            }
            else
            {
                mat.SetFloat("_Real", 1f);
                return;
            }
        }

        Debug.Log("No shader found boyo!");
    }
}
