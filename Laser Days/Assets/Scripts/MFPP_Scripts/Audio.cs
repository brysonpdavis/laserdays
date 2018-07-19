using UnityEngine;
using UnityEngine.Audio;

namespace MFPP
{
    public static class Audio
    {
        /// <summary>
        /// The output audio mixer group to route the created sounds using this class.
        /// </summary>
        public static AudioMixerGroup OutputAudioMixerGroup { get; set; }

        /// <summary>
        /// The duration in seconds of the DSP buffer.
        /// </summary>
        public static float DSPBufferDuration
        {
            get
            {
                int bufferLength, numBuffers;
                AudioSettings.GetDSPBufferSize(out bufferLength, out numBuffers);

                return (float)bufferLength / AudioSettings.outputSampleRate;
            }
        }

        /// <summary>
        /// Plays an <see cref="AudioClip"/>.
        /// </summary>
        /// <param name="clip">The <see cref="AudioClip"/>.</param>
        /// <returns>The generated <see cref="AudioSource"/>.</returns>
        public static AudioSource Play(AudioClip clip)
        {
            if (clip == null)
                return null;

            GameObject go = new GameObject("AudioClip: " + clip.name);
            AudioSource a = go.AddComponent<AudioSource>();
            a.outputAudioMixerGroup = OutputAudioMixerGroup;
            a.clip = clip;
            a.Play();

            Object.Destroy(go, a.clip.length + DSPBufferDuration);
            return a;
        }
        /// <summary>
        /// Plays an <see cref="AudioClip"/>.
        /// </summary>
        /// <param name="clip">The <see cref="AudioClip"/>.</param>
        /// <param name="volume">The volume for the generated <see cref="AudioSource"/>.</param>
        /// <returns>The generated <see cref="AudioSource"/>.</returns>
        public static AudioSource Play(AudioClip clip, float volume)
        {
            if (clip == null)
                return null;

            GameObject go = new GameObject("AudioClip: " + clip.name);
            AudioSource a = go.AddComponent<AudioSource>();
            a.outputAudioMixerGroup = OutputAudioMixerGroup;
            a.clip = clip;
            a.volume = volume;
            a.Play();

            Object.Destroy(go, a.clip.length + DSPBufferDuration);
            return a;
        }
        /// <summary>
        /// Plays an <see cref="AudioClip"/>.
        /// </summary>
        /// <param name="clip">The <see cref="AudioClip"/>.</param>
        /// <param name="volume">The volume for the generated <see cref="AudioSource"/>.</param>
        /// <param name="pitch">The pitch for the generated <see cref="AudioSource"/>.</param>
        /// <returns>The generated <see cref="AudioSource"/>.</returns>
        public static AudioSource Play(AudioClip clip, float volume, float pitch)
        {
            if (clip == null)
                return null;

            GameObject go = new GameObject("AudioClip: " + clip.name);
            AudioSource a = go.AddComponent<AudioSource>();
            a.outputAudioMixerGroup = OutputAudioMixerGroup;
            a.clip = clip;
            a.volume = volume;
            a.pitch = pitch;
            a.Play();

            Object.Destroy(go, a.clip.length / pitch + DSPBufferDuration);
            return a;
        }

        /// <summary>
        /// Plays an <see cref="AudioClip"/> in 3D.
        /// </summary>
        /// <param name="clip">The <see cref="AudioClip"/>.</param>
        /// <param name="parent">The parent <see cref="Transform"/> the generated <see cref="AudioSource"/> will attach to.</param>
        /// <returns>The generated <see cref="AudioSource"/>.</returns>
        public static AudioSource Play3D(AudioClip clip, Transform parent)
        {
            if (clip == null)
                return null;

            GameObject go = new GameObject("AudioClip: " + clip.name);
            go.transform.parent = parent;
            go.transform.localPosition = Vector3.zero;
            AudioSource a = go.AddComponent<AudioSource>();
            a.outputAudioMixerGroup = OutputAudioMixerGroup;
            a.clip = clip;
            a.spatialBlend = 1f;
            a.Play();

            Object.Destroy(go, a.clip.length + DSPBufferDuration);
            return a;
        }
        /// <summary>
        /// Plays an <see cref="AudioClip"/> in 3D.
        /// </summary>
        /// <param name="clip">The <see cref="AudioClip"/>.</param>
        /// <param name="parent">The parent <see cref="Transform"/> the generated <see cref="AudioSource"/> will attach to.</param>
        /// <param name="volume">The volume for the generated <see cref="AudioSource"/>.</param>
        /// <returns>The generated <see cref="AudioSource"/>.</returns>
        public static AudioSource Play3D(AudioClip clip, Transform parent, float volume)
        {
            if (clip == null)
                return null;

            GameObject go = new GameObject("AudioClip: " + clip.name);
            go.transform.parent = parent;
            go.transform.localPosition = Vector3.zero;
            AudioSource a = go.AddComponent<AudioSource>();
            a.outputAudioMixerGroup = OutputAudioMixerGroup;
            a.clip = clip;
            a.spatialBlend = 1f;
            a.volume = volume;
            a.Play();

            Object.Destroy(go, a.clip.length + DSPBufferDuration);
            return a;
        }
        /// <summary>
        /// Plays an <see cref="AudioClip"/> in 3D.
        /// </summary>
        /// <param name="clip">The <see cref="AudioClip"/>.</param>
        /// <param name="parent">The parent <see cref="Transform"/> the generated <see cref="AudioSource"/> will attach to.</param>
        /// <param name="volume">The volume for the generated <see cref="AudioSource"/>.</param>
        /// <param name="pitch">The pitch for the generated <see cref="AudioSource"/>.</param>
        /// <returns>The generated <see cref="AudioSource"/>.</returns>
        public static AudioSource Play3D(AudioClip clip, Transform parent, float volume, float pitch)
        {
            if (clip == null)
                return null;

            GameObject go = new GameObject("AudioClip: " + clip.name);
            go.transform.parent = parent;
            go.transform.localPosition = Vector3.zero;
            AudioSource a = go.AddComponent<AudioSource>();
            a.outputAudioMixerGroup = OutputAudioMixerGroup;
            a.clip = clip;
            a.spatialBlend = 1f;
            a.volume = volume;
            a.pitch = pitch;
            a.Play();

            Object.Destroy(go, a.clip.length / pitch + DSPBufferDuration);
            return a;
        }
        /// <summary>
        /// Plays an <see cref="AudioClip"/> in 3D.
        /// </summary>
        /// <param name="clip">The <see cref="AudioClip"/>.</param>
        /// <param name="parent">The parent <see cref="Transform"/> the generated <see cref="AudioSource"/> will attach to.</param>
        /// <param name="volume">The volume for the generated <see cref="AudioSource"/>.</param>
        /// <param name="pitch">The pitch for the generated <see cref="AudioSource"/>.</param>
        /// <param name="range">The maximum hearing range for the generated <see cref="AudioSource"/>.</param>
        /// <returns>The generated <see cref="AudioSource"/>.</returns>
        public static AudioSource Play3D(AudioClip clip, Transform parent, float volume, float pitch, float range)
        {
            if (clip == null)
                return null;

            GameObject go = new GameObject("AudioClip: " + clip.name);
            go.transform.parent = parent;
            go.transform.localPosition = Vector3.zero;
            AudioSource a = go.AddComponent<AudioSource>();
            a.outputAudioMixerGroup = OutputAudioMixerGroup;
            a.rolloffMode = AudioRolloffMode.Linear;
            a.maxDistance = range;
            a.clip = clip;
            a.spatialBlend = 1f;
            a.volume = volume;
            a.pitch = pitch;
            a.Play();

            Object.Destroy(go, a.clip.length / pitch + DSPBufferDuration);
            return a;
        }

        /// <summary>
        /// Plays an <see cref="AudioClip"/> in 3D.
        /// </summary>
        /// <param name="clip">The <see cref="AudioClip"/>.</param>
        /// <param name="position">The position of the generated <see cref="AudioSource"/>.</param>
        /// <returns>The generated <see cref="AudioSource"/>.</returns>
        public static AudioSource Play3D(AudioClip clip, Vector3 position)
        {
            if (clip == null)
                return null;

            GameObject go = new GameObject("AudioClip: " + clip.name);
            go.transform.position = position;
            AudioSource a = go.AddComponent<AudioSource>();
            a.outputAudioMixerGroup = OutputAudioMixerGroup;
            a.clip = clip;
            a.spatialBlend = 1f;
            a.Play();

            Object.Destroy(go, a.clip.length + DSPBufferDuration);
            return a;
        }
        /// <summary>
        /// Plays an <see cref="AudioClip"/> in 3D.
        /// </summary>
        /// <param name="clip">The <see cref="AudioClip"/>.</param>
        /// <param name="position">The position of the generated <see cref="AudioSource"/>.</param>
        /// <param name="volume">The volume for the generated <see cref="AudioSource"/>.</param>
        /// <returns>The generated <see cref="AudioSource"/>.</returns>
        public static AudioSource Play3D(AudioClip clip, Vector3 position, float volume)
        {
            if (clip == null)
                return null;

            GameObject go = new GameObject("AudioClip: " + clip.name);
            go.transform.position = position;
            AudioSource a = go.AddComponent<AudioSource>();
            a.outputAudioMixerGroup = OutputAudioMixerGroup;
            a.clip = clip;
            a.spatialBlend = 1f;
            a.volume = volume;
            a.Play();

            Object.Destroy(go, a.clip.length + DSPBufferDuration);
            return a;
        }
        /// <summary>
        /// Plays an <see cref="AudioClip"/> in 3D.
        /// </summary>
        /// <param name="clip">The <see cref="AudioClip"/>.</param>
        /// <param name="position">The position of the generated <see cref="AudioSource"/>.</param>
        /// <param name="volume">The volume for the generated <see cref="AudioSource"/>.</param>
        /// <param name="pitch">The pitch for the generated <see cref="AudioSource"/>.</param>
        /// <returns>The generated <see cref="AudioSource"/>.</returns>
        public static AudioSource Play3D(AudioClip clip, Vector3 position, float volume, float pitch)
        {
            if (clip == null)
                return null;

            GameObject go = new GameObject("AudioClip: " + clip.name);
            go.transform.position = position;
            AudioSource a = go.AddComponent<AudioSource>();
            a.outputAudioMixerGroup = OutputAudioMixerGroup;
            a.clip = clip;
            a.spatialBlend = 1f;
            a.volume = volume;
            a.pitch = pitch;
            a.Play();

            Object.Destroy(go, a.clip.length / pitch + DSPBufferDuration);
            return a;
        }
        /// <summary>
        /// Plays an <see cref="AudioClip"/> in 3D.
        /// </summary>
        /// <param name="clip">The <see cref="AudioClip"/>.</param>
        /// <param name="position">The position of the generated <see cref="AudioSource"/>.</param>
        /// <param name="volume">The volume for the generated <see cref="AudioSource"/>.</param>
        /// <param name="pitch">The pitch for the generated <see cref="AudioSource"/>.</param>
        /// <param name="range">The maximum hearing range for the generated <see cref="AudioSource"/>.</param>
        /// <returns>The generated <see cref="AudioSource"/>.</returns>
        public static AudioSource Play3D(AudioClip clip, Vector3 position, float volume, float pitch, float range)
        {
            if (clip == null)
                return null;

            GameObject go = new GameObject("AudioClip: " + clip.name);
            go.transform.position = position;
            AudioSource a = go.AddComponent<AudioSource>();
            a.outputAudioMixerGroup = OutputAudioMixerGroup;
            a.rolloffMode = AudioRolloffMode.Linear;
            a.maxDistance = range;
            a.clip = clip;
            a.spatialBlend = 1f;
            a.volume = volume;
            a.pitch = pitch;
            a.Play();

            Object.Destroy(go, a.clip.length / pitch + DSPBufferDuration);
            return a;
        }
    }

}