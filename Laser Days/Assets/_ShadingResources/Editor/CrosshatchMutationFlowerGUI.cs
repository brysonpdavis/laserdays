using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class CrosshatchMutationFlowerGUI : ShaderGUI {

    enum World
    {
        Shared, Real, Laser
    }

    struct WorldSettings {
        public RenderQueue queue;
    }

    bool Emissive;

    Material target;
    MaterialEditor editor;
    MaterialProperty[] properties;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        this.target =  materialEditor.target as Material;
        this.editor = materialEditor;
        this.properties = properties;

        SetWorld();
        BaseModule();
        TransitionModule();
        OutlinesModule();
        AccentModule();


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

    void BaseModule()
    {
        GUILayout.Label("Base", EditorStyles.boldLabel);

        TextureProperty("_MainTex", "Mutation map - Mask (R), Growth shape (B)");

        SliderProperty("_Highlights", "");

        editor.TextureScaleOffsetProperty(FindProperty("_MainTex"));

        GUILayout.Label("Base", EditorStyles.boldLabel);

        ColorProperty("_BeginColor");
        ColorProperty("_BaseColor");
        ColorProperty("_DeathColor");
        ColorProperty("_TintColor");

        GUILayout.Label("LifeCycle", EditorStyles.boldLabel);
        FloatProperty("_BeginToBase", "");
        FloatProperty("_BaseToDeath", "");

    }

    void AccentModule()
    {
        GUILayout.Label("Accent", EditorStyles.boldLabel);

        TextureProperty("_AccentMap", "Accent Mask - Real (R), Laser (G)");

        SetKeyword("ACCENT_ON", FindProperty("_AccentMap").textureValue);

        if(FindProperty("_AccentMap").textureValue)
        {
            ColorProperty("_AccentColor");
        }
    }

    void TransitionModule()
    {
        GUILayout.Label("Transition", EditorStyles.boldLabel);

        TextureProperty("_EffectMap", "Transition map (R), Glow mask (G), Panning glow effect (B)");

        SliderProperty("_TransitionState", "");

         SliderProperty("_AlphaCutoff", "");


    }

    void OutlinesModule()
    {
        GUILayout.Label("Outlines", EditorStyles.boldLabel);

        SliderProperty("_LineA", "Outline ID");
        SliderProperty("_Smoothness", "Outline sensititivity reduction for normals");
        //SliderProperty("_Smoothness2", "Outline sensititivity reduction for depth");

        target.SetFloat("_LineA", Mathf.Floor(target.GetFloat("_LineA")));
        //target.SetFloat("_Smoothness", Mathf.Floor(target.GetFloat("_Smoothness") * 10f) * 0.1f);
        //target.SetFloat("_Smoothness2", Mathf.Floor(target.GetFloat("_Smoothness2") * 10f) * 0.1f);

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
