using UnityEngine;

namespace MFPP.Modules
{
    [HelpURL("https://ashkoredracson.github.io/MFPP/#bob-module")]
    public class BobModule : PlayerModule
    {
        /// <summary>
        /// The amplitude of the bobbing.
        /// </summary>
        [Space]
        [Tooltip("The amplitude of the bobbing.")]
        public float BobAmplitude = 0.1f;
        /// <summary>
        /// The speed of the bobbing.
        /// </summary>
        [Tooltip("The speed of the bobbing.")]
        public float BobSpeed = 3f;
        /// <summary>
        /// The X axis animation curve of the bobbing.
        /// </summary>
        [Space]
        [Tooltip("The X axis animation curve of the bobbing.")]
        public AnimationCurve BobX = AnimationCurve.Constant(0, 1, 0);
        /// <summary>
        /// The Y axis animation curve of the bobbing.
        /// </summary>
        [Tooltip("The Y axis animation curve of the bobbing.")]
        public AnimationCurve BobY = AnimationCurve.Constant(0, 1, 0);
        /// <summary>
        /// The amplitude of the landing bob.
        /// </summary>
        [Space]
        [Tooltip("The amplitude of the landing bob.")]
        public float LandingBobAmplitude = 5f;
        /// <summary>
        /// The speed recover of the landing bob.
        /// </summary>
        [Tooltip("The speed recover of the landing bob.")]
        public float LandingBobSpeed = 2f;
        /// <summary>
        /// The speed recover lerp (smoothing) of the landing bob.
        /// </summary>
        [Tooltip("The speed recover lerp (smoothing) of the landing bob.")]
        public float LandingBobLerpSpeed = 5f;

        private float bobGlobalMult, bobTime, normalizedSpeed, curLandingBobY, targetLandingBobY;

        public override void AfterUpdate()
        {
            DoBob();
            DoLandingBob();
        }

        private void DoBob()
        {
            float magnitude = Player.PlanarVelocity.magnitude;

            // Calculate bob based on amplitude, animation curves, planar velocity magnitude, and time.
            normalizedSpeed = Mathf.InverseLerp(0, Player.Movement.Speed * Player.Movement.Sprint.SpeedMultiplier, magnitude);
            bobGlobalMult = Mathf.Lerp(bobGlobalMult, Player.IsGrounded ? 1f : 0f, Time.deltaTime * 10);

            Vector3 finalBob = new Vector3(BobX.Evaluate(bobTime) * BobAmplitude, BobY.Evaluate(bobTime) * BobAmplitude, 0) * bobGlobalMult;
            Vector3 translation = Vector3.Lerp(Vector3.zero, finalBob, normalizedSpeed);
            Camera.transform.Translate(translation); // Apply bob translation.

            bobTime += Time.deltaTime * normalizedSpeed * BobSpeed;
        }

        private void DoLandingBob()
        {
            if (Player.JustLanded && Player.OldVelocity.magnitude > 1f) // If landing magnitude is greater than 1, apply landing bob.
                targetLandingBobY = LandingBobAmplitude;

            // Apply landing bob and smooth it.
            curLandingBobY = Mathf.Lerp(curLandingBobY, targetLandingBobY, LandingBobLerpSpeed * Time.deltaTime);
            if (targetLandingBobY > 0)
                targetLandingBobY = Mathf.Clamp(targetLandingBobY - (LandingBobSpeed * LandingBobAmplitude * Time.deltaTime), 0, float.MaxValue);

            Camera.transform.localEulerAngles += new Vector3(curLandingBobY, 0, 0); // Apply landing bob current rotation.
        }
    }
}