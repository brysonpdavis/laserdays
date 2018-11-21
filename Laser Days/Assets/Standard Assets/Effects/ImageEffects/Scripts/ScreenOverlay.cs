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
        public Texture2D texture = null;
        public Texture2D texture1 = null;
        public Texture2D texture2 = null;

        public Shader overlayShader = null;
        private Material overlayMaterial = null;

        private int boi = 0;
        private int frames = 0;


        public override bool CheckResources ()
		{
            CheckSupport (false);

            overlayMaterial = CheckShaderAndCreateMaterial (overlayShader, overlayMaterial);

            if	(!isSupported)
                ReportAutoDisable ();
            return isSupported;
        }

        void OnRenderImage (RenderTexture source, RenderTexture destination)
		{
            if (CheckResources() == false)
			{
                Graphics.Blit (source, destination);
                return;
            }


            frames++;

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


            if(frames%5 == 0)
            {
                if (boi == 0)
                    overlayMaterial.SetTexture("_Overlay", texture);

                if (boi == 1)
                    overlayMaterial.SetTexture("_Overlay", texture1);

                if (boi == 2)
                    overlayMaterial.SetTexture("_Overlay", texture2);

                boi++;

                if (boi >= 3)
                    boi = 0;
            }

           

            overlayMaterial.SetVector("_UV_Transform", UV_Transform);
            overlayMaterial.SetFloat ("_Intensity", intensity);

            Graphics.Blit (source, destination, overlayMaterial, (int) blendMode);
        }
    }
}
