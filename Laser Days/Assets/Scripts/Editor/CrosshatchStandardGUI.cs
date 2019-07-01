using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class CrosshatchStandardGUI : ShaderGUI {

    enum World
    {
        Shared, Real, Laser
    }

    enum Type
    {
        Static, Interactable, InverseInteractable, Terrain
    }

    enum GradientMode
    {
        None, Radial, Height
    }

    struct WorldSettings {
        public RenderQueue queue;
    }

    Material target;
    MaterialEditor editor;
    MaterialProperty[] properties;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        this.target =  materialEditor.target as Material;
        this.editor = materialEditor;
        this.properties = properties;

        SetWorld();
        SetType();
        BaseModule();
        TransitionModule();
        OutlinesModule();
        AccentModule();
        GradientModule();
        if(IsKeywordEnabled("INTERACTABLE") || IsKeywordEnabled("INVERSE_INTERACTABLE"))
        {
            InteractionModule();
        }


        editor.RenderQueueField();
        //base.OnGUI(materialEditor, properties);
    }

    void SetWorld()
    {
        World w = World.Shared;
        if(IsKeywordEnabled("REAL"))
        {
            w = World.Real;
        }
        if (IsKeywordEnabled("LASER"))
        {
            w = World.Laser;
        }

        EditorGUI.BeginChangeCheck();
        w = (World)EditorGUILayout.EnumPopup("World", w);
        if(EditorGUI.EndChangeCheck())
        {
            editor.RegisterPropertyChangeUndo("World");
            SetKeyword("REAL", w == World.Real);
            SetKeyword("LASER", w == World.Laser);
            SetKeyword("SHARED", w == World.Shared);
        }
    }

    void SetType()
    {
        Type t = Type.Static;
        if (IsKeywordEnabled("STATIC"))
        {
            t = Type.Static;
        }
        if (IsKeywordEnabled("INTERACTABLE"))
        {
            t = Type.Interactable;
        }
        if (IsKeywordEnabled("TERRAIN"))
        {
            t = Type.Terrain;
        }
        if (IsKeywordEnabled("INVERSE_INTERACTABLE"))
        {
            t = Type.InverseInteractable;
        }

        EditorGUI.BeginChangeCheck();
        t = (Type)EditorGUILayout.EnumPopup("Type", t);
        if (EditorGUI.EndChangeCheck())
        {
            editor.RegisterPropertyChangeUndo("Type");
            SetKeyword("STATIC", t == Type.Static);
            SetKeyword("INTERACTABLE", t == Type.Interactable);
            SetKeyword("TERRAIN", t == Type.Terrain);
            SetKeyword("INVERSE_INTERACTABLE", t == Type.InverseInteractable);
        }
    }

    void SetGradient()
    {
        GradientMode gm = GradientMode.None;
        if (IsKeywordEnabled("HEIGHT_GRADIENT"))
        {
            gm = GradientMode.Height;
        }
        if (IsKeywordEnabled("RADIAL_GRADIENT"))
        {
            gm = GradientMode.Radial;
        }

        EditorGUI.BeginChangeCheck();
        gm = (GradientMode)EditorGUILayout.EnumPopup("Gradient Mode", gm);
        if (EditorGUI.EndChangeCheck())
        {
            editor.RegisterPropertyChangeUndo("Gradient Mode");
            SetKeyword("HEIGHT_GRADIENT", gm == GradientMode.Height);
            SetKeyword("RADIAL_GRADIENT", gm == GradientMode.Radial);
            SetKeyword("NO_GRADIENT", gm == GradientMode.None);
        }
    }

    void BaseModule()
    {
        GUILayout.Label("Base", EditorStyles.boldLabel);

        TextureProperty("_MainTex", "Darkness map - Real (R), Laser (G)");

        SliderProperty("_MainTexContribution", "");

        SliderProperty("_Highlights", "");

        if (IsKeywordEnabled("REAL"))
        {
            ColorProperty("_RealBase");
        }

        if(IsKeywordEnabled("LASER"))
        {
            ColorProperty("_LaserBase");
        }

        if(IsKeywordEnabled("SHARED"))
        {
            ColorProperty("_RealBase");
            ColorProperty("_LaserBase");
        }

        if(IsKeywordEnabled("TERRAIN"))
        {
            FloatProperty("_TerrainScale", "Texture scale");
            FloatProperty("_BlendOffset", "Triplanar blending offset");
        }
        else 
        {
            editor.TextureScaleOffsetProperty(FindProperty("_MainTex"));
        }

  
    }

    void AccentModule()
    {
        GUILayout.Label("Accent", EditorStyles.boldLabel);

        TextureProperty("_AccentMap", "Accent Mask - Real (R), Laser (G)");

        SetKeyword("ACCENT_ON", FindProperty("_AccentMap").textureValue);

        if(FindProperty("_AccentMap").textureValue)
        {
            if (IsKeywordEnabled("REAL"))
            {
                ColorProperty("_RealAccent");
            }

            if (IsKeywordEnabled("LASER"))
            {
                ColorProperty("_LaserAccent");
            }

            if (IsKeywordEnabled("SHARED"))
            {
                ColorProperty("_RealAccent");
                ColorProperty("_LaserAccent");
            }
        }
    }

    void GradientModule()
    {
        GUILayout.Label("Gradient", EditorStyles.boldLabel);

        SetGradient();

        if(!IsKeywordEnabled("NO_GRADIENT"))
        {
            SliderProperty("_GradientScale", "");
            SliderProperty("_GradientOffset", "");

            if (IsKeywordEnabled("REAL"))
            {
                ColorProperty("_RealGradient");
            }

            if (IsKeywordEnabled("LASER"))
            {
                ColorProperty("_LaserGradient");
            }

            if (IsKeywordEnabled("SHARED"))
            {
                ColorProperty("_RealGradient");
                ColorProperty("_LaserGradient");
            }
        }
    }

    void TransitionModule()
    {
        GUILayout.Label("Transition", EditorStyles.boldLabel);

        TextureProperty("_EffectMap", "Transition map (R), Glow mask (G), Panning glow effect (B)");

        SliderProperty("_TransitionState", "");

        if(!IsKeywordEnabled("SHARED"))
        {
            SliderProperty("_AlphaCutoff", "");
        }

    }

    void OutlinesModule()
    {
        GUILayout.Label("Outlines", EditorStyles.boldLabel);

        TextureProperty("_ShadingMap", "Occlusion hatching (R)");



        SliderProperty("_LineA", "Outline ID");
        SliderProperty("_Smoothness", "Outline sensititivity reduction for normals");
        //SliderProperty("_Smoothness2", "Outline sensititivity reduction for depth");

        target.SetFloat("_LineA", Mathf.Floor(target.GetFloat("_LineA")));
        target.SetFloat("_Smoothness", Mathf.Floor(target.GetFloat("_Smoothness") * 10f) * 0.1f);
        //target.SetFloat("_Smoothness2", Mathf.Floor(target.GetFloat("_Smoothness2") * 10f) * 0.1f);

    }

    void InteractionModule()
    {
        GUILayout.Label("Interaction", EditorStyles.boldLabel);
        ColorProperty("_InteractColor");
        ColorProperty("_ShimmerColor");
        SliderProperty("_TransitionStateB", "");
        SliderProperty("_onHover", "");
        SliderProperty("_onHold", "");

    }


    MaterialProperty FindProperty(string name)
    {
        return FindProperty(name, properties);
    }

    void TextureProperty(string prop, string tip)
    {
        MaterialProperty tex = FindProperty(prop);
        GUIContent texLabel = new GUIContent(tex.displayName, tip);
        editor.TexturePropertySingleLine(texLabel, tex);
    }

    void FloatProperty(string prop, string tip)
    {
        MaterialProperty num = FindProperty(prop);
        GUIContent numLabel = new GUIContent(num.displayName, tip);
        editor.ShaderProperty(num, numLabel);
    }


    void ColorProperty(string prop)
    {
        MaterialProperty col = FindProperty(prop);
        editor.ColorProperty(col, col.displayName);
    }

    void SliderProperty(string prop, string tip)
    {
        MaterialProperty slide = FindProperty(prop);
        GUIContent slideLabel = new GUIContent(slide.displayName, tip);
        editor.ShaderProperty(slide, slideLabel);
    }

    void SetKeyword(string keyword, bool state)
    {
        if (state)
        {
            target.EnableKeyword(keyword);
        }
        else
        {
            target.DisableKeyword(keyword);
        }
    }

    bool IsKeywordEnabled(string keyword)
    {
        return target.IsKeywordEnabled(keyword);
    }
}
