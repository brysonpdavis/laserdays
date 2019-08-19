using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("")]
    public class ImageEffectBoundary : PostEffectsBase
    {
        /// Provides a shader property that is set in the inspector
        /// and a material instantiated from the shader
        /// 
        /// 
        public Shader effectShader;
        public Color vignetteColor;
        public float intensity;
        private Material m_Material;

        public float instensityMultiplier = 0f;

        private float currentIntensity;

        public override bool CheckResources()
        {
            CheckSupport(false);
            m_Material = CheckShaderAndCreateMaterial(effectShader, m_Material);

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false)
            {
                Graphics.Blit(source, destination);
                return;
            }

            UpdateIntensity();

            m_Material.SetFloat("_Intensity", currentIntensity);
            m_Material.SetColor("_VignetteColor", vignetteColor);

            Graphics.Blit(source, destination, m_Material);
        }

        void UpdateIntensity()
        {

            currentIntensity = intensity * instensityMultiplier;
        }

    }

}

