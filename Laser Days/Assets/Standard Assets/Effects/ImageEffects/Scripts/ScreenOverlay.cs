using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent (typeof(Camera))]
    [AddComponentMenu ("Image Effects/Other/Screen Overlay")]
    public class ScreenOverlay : PostEffectsBase
	{
	    public enum OverlayBlendMode
		{
            Additive = 0,
            ScreenBlend = 1,
            Multiply = 2,
            Overlay = 3,
            AlphaBlend = 4,
        }

        public OverlayBlendMode blendMode = OverlayBlendMode.Overlay;
        public float intensity = 1.0f;
        private float currIntensity = 1.0f;
        public Texture2D texture = null;

        public Shader overlayShader = null;
        private Material overlayMaterial = null;

        private bool Animating;
        private float num = 0f;

        public float speed = 0.07f;


        public override bool CheckResources ()
		{
            CheckSupport (false);

            overlayMaterial = CheckShaderAndCreateMaterial (overlayShader, overlayMaterial);

            if	(!isSupported)
                ReportAutoDisable ();
            return isSupported;
        }

        private void Update()
        {
            if (Animating)
            {
                num += speed;
                Mathf.Clamp(num, 0f, 1f);
                currIntensity = (num) * intensity;
                if (num <= 0f || num >= 1f)
                {
                    Animating = false;
                }
            }
        }

        public void Shift(bool toLaser)
        {

            if (toLaser)
            {
                speed = Mathf.Abs(speed);
            }
            else
            {
                speed = -Mathf.Abs(speed);
            }

            Animating = true;

        }






        [ImageEffectOpaque]
        void OnRenderImage (RenderTexture source, RenderTexture destination)
		{
            if (CheckResources() == false)
			{
                Graphics.Blit (source, destination);
                return;
            }

            Vector4 UV_Transform = new  Vector4(1, 0, 0, 1);

			#if UNITY_WP8
	    	// WP8 has no OS support for rotating screen with device orientation,
	    	// so we do those transformations ourselves.
			if (Screen.orientation == ScreenOrientation.LandscapeLeft) {
				UV_Transform = new Vector4(0, -1, 1, 0);
			}
			if (Screen.orientation == ScreenOrientation.LandscapeRight) {
				UV_Transform = new Vector4(0, 1, -1, 0);
			}
			if (Screen.orientation == ScreenOrientation.PortraitUpsideDown) {
				UV_Transform = new Vector4(-1, 0, 0, -1);
			}
			#endif

            overlayMaterial.SetVector("_UV_Transform", UV_Transform);
            overlayMaterial.SetFloat ("_Intensity", currIntensity);
            overlayMaterial.SetTexture ("_Overlay", texture);
            Graphics.Blit (source, destination, overlayMaterial, (int) blendMode);
        }
    }
}
