using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;



[Serializable]
[PostProcess(typeof(StylizedFogRenderer), PostProcessEvent.BeforeStack, "Custom/StylizedFog")]
public sealed class StylizedFog : PostProcessEffectSettings
{

    [Range(0f, 1f), Tooltip("")]
    public FloatParameter FogDensity = new FloatParameter { };

    [Space]
    [Space]

    public ColorParameter Color1 = new ColorParameter { };

    [Range(0f, 1f), Tooltip("Cut off value for first color.")]
    public FloatParameter Color1Pos = new FloatParameter { };

    [Range(1f, 10f), Tooltip("Sharpness of transition from first to second color.")]
    public FloatParameter Color1Sharp = new FloatParameter { };

    [Space]

    public ColorParameter Color2 = new ColorParameter { };

    [Range(0f, 1f), Tooltip("Cut off value for second color.")]
    public FloatParameter Color2Pos = new FloatParameter { };

    [Range(1f, 10f), Tooltip("Sharpness of transition from second to third color.")]
    public FloatParameter Color2Sharp = new FloatParameter { };

    [Space]

    public ColorParameter Color3 = new ColorParameter { };

    [Range(0f, 1f), Tooltip("Cut off value for third color.")]
    public FloatParameter Color3Pos = new FloatParameter { };

    [Range(1f, 10f), Tooltip("Sharpness of transition from third to fourth color.")]
    public FloatParameter Color3Sharp = new FloatParameter { };

    [Space]

    public ColorParameter Color4 = new ColorParameter { };

    [Range(0f, 1f), Tooltip("Cut off value for fourth color.")]
    public FloatParameter Color4Pos = new FloatParameter { };

    [Range(1f, 10f), Tooltip("Sharpness of transition from fourth to fifth color.")]
    public FloatParameter Color4Sharp = new FloatParameter { };

    [Space]

    public ColorParameter Color5 = new ColorParameter { };


    [Range(1f, 10f), Tooltip("")]
    public FloatParameter BlendAmount = new FloatParameter { };




    public sealed class StylizedFogRenderer : PostProcessEffectRenderer<StylizedFog>
    {
        public override void Render(PostProcessRenderContext context)
        {

            var sheet = context.propertySheets.Get(Shader.Find("Hidden/5ColorFog"));

            sheet.properties.SetColor("_Color1", settings.Color1);
            sheet.properties.SetFloat("_Cutoff1", settings.Color1Pos);
            sheet.properties.SetFloat("_Sharp1", settings.Color1Sharp);

            sheet.properties.SetColor("_Color2", settings.Color2);
            sheet.properties.SetFloat("_Cutoff2", settings.Color2Pos);
            sheet.properties.SetFloat("_Sharp2", settings.Color2Sharp);

            sheet.properties.SetColor("_Color3", settings.Color3);
            sheet.properties.SetFloat("_Cutoff3", settings.Color3Pos);
            sheet.properties.SetFloat("_Sharp3", settings.Color3Sharp);

            sheet.properties.SetColor("_Color4", settings.Color4);
            sheet.properties.SetFloat("_Cutoff4", settings.Color4Pos);
            sheet.properties.SetFloat("_Sharp4", settings.Color4Sharp);

            sheet.properties.SetColor("_Color5", settings.Color5);

            sheet.properties.SetFloat("_FogDensity", settings.FogDensity);
            sheet.properties.SetFloat("_BlendAmount", settings.BlendAmount);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}







