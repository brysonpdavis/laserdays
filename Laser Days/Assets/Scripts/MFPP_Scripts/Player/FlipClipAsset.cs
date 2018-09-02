﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MFPP
{
    [HelpURL("https://ashkoredracson.github.io/MFPP/#flip-asset")]
    [CreateAssetMenu]
    public class FlipClipAsset : ScriptableObject
    {
        [Header("FlipClip Data")]
        [SerializeField]
        [Tooltip("The default flips name that we select from the FlipClips list.")]
        /// <summary>
        /// The default flips name that we select from the FlipClips list.
        /// </summary>

        public Data defaultFlipClips;
        /// <summary>
        /// The default flips to use and also fallback to in the case that no matching flips were found.
        /// </summary>

        [SerializeField]
        [Tooltip("The list of flip data.")]


        [Header("Volume")]
        [Range(0f, 1f)]
        private float defaultVolume = 0.25f;
        /// <summary>
        /// FlipClip volume when crouching.
        /// </summary>

        [Serializable]
        public class Data
        {
            [SerializeField]
            [Tooltip("The name of the flip data.")]
            /// <summary>
            /// The name of the flip data.
            /// </summary>


            public List<UnityEngine.AudioClip> flips;
            public List<UnityEngine.AudioClip> flipsSecondary;

            /// <summary>
            /// The list of flip data.
            /// </summary>

            /// <summary>
            /// FlipClip sounds.
            /// </summary>


            /// <summary>
            /// Gets a random flip sound from this <see cref="Data"/>.
            /// </summary>
            /// <returns>A random flip sound from this <see cref="Data"/>.</returns>
            public UnityEngine.AudioClip GetRandomFlipClip()
            {
                if (flips == null || flips.Count <= 0)
                    return null;

                return flips[Random.Range(0, flips.Count)];
            }

            public UnityEngine.AudioClip GetRandomFlipSecondary()
            {
                if (flipsSecondary == null || flipsSecondary.Count <= 0)
                    return null;

                return flipsSecondary[Random.Range(0, flips.Count)];
            }


            /// <summary>
            /// Gets a random jumping sound from this <see cref="Data"/>.
            /// </summary>
            /// <returns>A random jumping sound from this <see cref="Data"/>.</returns>
        }
    }
}