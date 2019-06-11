using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public static class ShaderUtility
{
    // Set Shader to the Real 
    public static void ShaderToReal(Material mat)
    {
        Debug.Log("Looking for " + mat.shader);
        // If using our standard shader, then use keyword swap approach
        if (mat.shader == Shader.Find("Crosshatch/Standard"))
        {
            mat.DisableKeyword("LASER");
            mat.EnableKeyword("REAL");
            return;
        }

        // If using the old shaders, swap shader to real
        if (mat.shader == Shader.Find("Transition/RealOnly") || mat.shader == Shader.Find("Transition/LaserOnly"))
        {
            mat.shader = Shader.Find("Transition/RealOnly");
            return;    
        }

        // If using the Core shaders, swap Core shader to real
        if (mat.shader == Shader.Find("Transition/RealCore") || mat.shader == Shader.Find("Transition/LaserCore"))
        {
            mat.shader = Shader.Find("Transition/RealCore");
            return;
        }

        // If using the slightly oldel shaders, swap to proper world
        if (mat.shader == Shader.Find("Crosshatch/Real-Standard") || mat.shader == Shader.Find("Crosshatch/Laser-Standard"))
        {
            mat.shader = Shader.Find("Crosshatch/Real-Standard");
            return;
        }

        // Transparent shaders use a float to determine world 
        if (mat.shader == Shader.Find("Crosshatch/Glass-Single"))
        {
            mat.SetFloat("_Real", 1f);
            return;
        }

        Debug.LogError("No shader found. Looking for " + mat.shader);
    }

    // Set Shader to Laser 
    public static void ShaderToLaser(Material mat)
    {
        Debug.Log("Looking for " + mat.shader);
        // If using our standard shader, then use keyword swap approach
        if (mat.shader == Shader.Find("Crosshatch/Standard"))
        {
            mat.DisableKeyword("REAL");
            mat.EnableKeyword("LASER");
            return;
        }

        // If using the old shaders, swap shader to real
        if (mat.shader == Shader.Find("Transition/RealOnly") || mat.shader == Shader.Find("Transition/LaserOnly"))
        {
            mat.shader = Shader.Find("Transition/LaserOnly");
            return;
        }

        // If using the Core shaders, swap Core shader to real
        if (mat.shader == Shader.Find("Transition/RealCore") || mat.shader == Shader.Find("Transition/LaserCore"))
        {
            mat.shader = Shader.Find("Transition/LaserCore");
            return;
        }

        // If using the slightly oldel shaders, swap to proper world
        if (mat.shader == Shader.Find("Crosshatch/Real-Standard") || mat.shader == Shader.Find("Crosshatch/Laser-Standard"))
        {
            mat.shader = Shader.Find("Crosshatch/Laser-Standard");
            return;
        }

        // Transparent shaders use a float to determine world 
        if (mat.shader == Shader.Find("Crosshatch/Glass-Single"))
        {
            mat.SetFloat("_Real", 0f);
            return;
        }

        Debug.LogError("No shader found. Looking for " + mat.shader);
    }


    // Same as ShaderToLaser and ShaderToReal except takes a bool toLaser to determine
    public static void ShaderWorldChange(Material mat, bool toLaser)
    {

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

        // Preview
        if (mat.shader == Shader.Find("Transition/RealPreview") || mat.shader == Shader.Find("Transition/LaserPreview"))
        {
            if (toLaser)
            {
                //mat.shader = Shader.Find("Transition/LaserPreview");
                return;
            }
            else
            {
                //mat.shader = Shader.Find("Transition/RealPreview");
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

        Debug.LogError("No shader found. Looking for " + mat.shader);
    }


}
