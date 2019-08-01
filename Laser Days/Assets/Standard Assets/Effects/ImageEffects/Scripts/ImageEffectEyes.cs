using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [RequireComponent(typeof (Camera))]
    [AddComponentMenu("")]
    public class ImageEffectEyes : PostEffectsBase
    {
        /// Provides a shader property that is set in the inspector
        /// and a material instantiated from the shader
        /// 
        /// 
        public Shader effectShader;
        public Color vignetteColor;
        public float intensity;
        private Material m_Material;

        public float intensityChangeTime = 0.4f;

        public bool isSeen;

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
            if (isSeen)
            {
                currentIntensity += (intensityChangeTime * Time.deltaTime);
            }
            else
            {
                currentIntensity -= (intensityChangeTime * Time.deltaTime);
            }

            currentIntensity = Mathf.Clamp(currentIntensity, 0f, intensity);
        }

    }
}
