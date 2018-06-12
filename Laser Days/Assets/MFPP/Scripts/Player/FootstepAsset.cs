using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MFPP
{
    [HelpURL("https://ashkoredracson.github.io/MFPP/#footstep-asset")]
    [CreateAssetMenu]
    public class FootstepAsset : ScriptableObject
    {
        [Header("Footstep Data")]
        [SerializeField]
        [Tooltip("The default footsteps name that we select from the Footsteps list.")]
        private string defaultFootstepsName;
        /// <summary>
        /// The default footsteps name that we select from the Footsteps list.
        /// </summary>
        public string DefaultFootstepsName
        {
            get { return defaultFootstepsName; }
            set { defaultFootstepsName = value; }
        }
        private Data defaultFootsteps;
        /// <summary>
        /// The default footsteps to use and also fallback to in the case that no matching footsteps were found.
        /// </summary>
        public Data DefaultFootsteps
        {
            get
            {
                if (defaultFootsteps == null || defaultFootsteps.Name != DefaultFootstepsName)
                    defaultFootsteps = GetData(DefaultFootstepsName);

                return defaultFootsteps;
            }
        }
        [SerializeField]
        [Tooltip("The list of footstep data.")]
        private List<Data> footsteps;
        /// <summary>
        /// The list of footstep data.
        /// </summary>
        public List<Data> Footsteps
        {
            get { return footsteps; }
            set { footsteps = value; }
        }

        [Header("Timers")]
        [SerializeField]
        [Tooltip("The walking timer interval. 1 = 1 second between footsteps, 0.5 = 0.5 seconds between footsteps and so on. Lower is faster.")]
        private float walkTimer = 0.5f;
        /// <summary>
        /// The walking timer interval. 1 = 1 second between footsteps, 0.5 = 0.5 seconds between footsteps and so on. Lower is faster.
        /// </summary>
        public float WalkTimer
        {
            get { return walkTimer; }
            set { walkTimer = value; }
        }
        [SerializeField]
        [Tooltip("Interval multiplier for the walking timer when crouching. Higher than 1 slows it down (e.g 1.4 = 1.4x interval length), lower and it speeds it up.")]
        private float crouchTimerMultiplier = 1.4f;
        /// <summary>
        /// Interval multiplier for the walking timer when crouching. Higher than 1 slows it down (e.g 1.4 = 1.4x interval length), lower and it speeds it up.
        /// </summary>
        public float CrouchTimerMultiplier
        {
            get { return crouchTimerMultiplier; }
            set { crouchTimerMultiplier = value; }
        }
        [SerializeField]
        [Tooltip("Interval multiplier for the walking timer when sprinting. Higher than 1 slows it down, lower and it speeds it up (e.g 0.6 = 0.6x interval length).")]
        private float sprintTimerMultiplier = 0.6f;
        /// <summary>
        /// Interval multiplier for the walking timer when sprinting. Higher than 1 slows it down, lower and it speeds it up (e.g 0.6 = 0.6x interval length).
        /// </summary>
        public float SprintTimerMultiplier
        {
            get { return sprintTimerMultiplier; }
            set { sprintTimerMultiplier = value; }
        }

        [Header("Volume")]
        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("Footstep volume when crouching.")]
        private float crouchVolume = 0.25f;
        /// <summary>
        /// Footstep volume when crouching.
        /// </summary>
        public float CrouchVolume
        {
            get { return crouchVolume; }
            set { crouchVolume = value; }
        }
        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("Footstep volume when walking.")]
        private float walkVolume = 0.5f;
        /// <summary>
        /// Footstep volume when walking.
        /// </summary>
        public float WalkVolume
        {
            get { return walkVolume; }
            set { walkVolume = value; }
        }
        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("Footstep volume when sprinting.")]
        private float sprintVolume = 1f;
        /// <summary>
        /// Footstep volume when sprinting.
        /// </summary>
        public float SprintVolume
        {
            get { return sprintVolume; }
            set { sprintVolume = value; }
        }

        [Space]
        [SerializeField]
        [Tooltip("The minimum landing velocity.")]
        private float landingMinimumVelocity = 1f;
        /// <summary>
        /// The minimum landing velocity.
        /// </summary>
        public float LandingMinimumVelocity
        {
            get { return landingMinimumVelocity; }
            set { landingMinimumVelocity = value; }
        }
        [SerializeField]
        [Tooltip("The maximum landing velocity.")]
        private float landingMaximumVelocity = 10f;
        /// <summary>
        /// The maximum landing velocity.
        /// </summary>
        public float LandingMaximumVelocity
        {
            get { return landingMaximumVelocity; }
            set { landingMaximumVelocity = value; }
        }
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The final volume multiplier of the landing sounds.")]
        private float landingVolumeMultiplier = 1f;
        /// <summary>
        /// The final volume multiplier of the landing sounds.
        /// </summary>
        public float LandingVolumeMultiplier
        {
            get { return landingVolumeMultiplier; }
            set { landingVolumeMultiplier = value; }
        }

        /// <summary>
        /// Gets the corresponding footstep <see cref="Data"/> with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The corresponding footstep <see cref="Data"/> with the specified name.</returns>
        public Data GetData(string name)
        {
            return footsteps.FirstOrDefault(f => f.Name == name);
        }
        /// <summary>
        /// Gets the corresponding footstep <see cref="Data"/> for the specified <see cref="Material"/>.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <returns>The corresponding footstep <see cref="Data"/> for the specified <see cref="Material"/>.</returns>
        public Data GetData(Material material)
        {
            return footsteps.FirstOrDefault(f => f.LinkedMaterials.Contains(material));
        }
        /// <summary>
        /// Gets the corresponding footstep <see cref="Data"/> for the specified <see cref="Texture"/>.
        /// </summary>
        /// <param name="texture">The material.</param>
        /// <returns>The corresponding footstep <see cref="Data"/> for the specified <see cref="Texture"/>.</returns>
        public Data GetData(Texture texture)
        {
            return footsteps.FirstOrDefault(f => f.LinkedTextures.Contains(texture));
        }
        /// <summary>
        /// Gets the corresponding footstep <see cref="Data"/> for the specified <see cref="RaycastHit"/>.
        /// </summary>
        /// <param name="hit"></param>
        /// <returns>The corresponding footstep <see cref="Data"/> for the specified <see cref="RaycastHit"/>.</returns>
        public Data GetData(RaycastHit hit)
        {
            Data data = null;

            // Check for footstep override
            FootstepOverride footstepOverride = hit.collider.GetComponent<FootstepOverride>();
            if (footstepOverride != null)
                data = GetData(footstepOverride.FootstepOverrideName);

            if (data != null)
                return data;

            // Check for meshes
            MeshRenderer mr = hit.collider.GetComponent<MeshRenderer>();
            MeshFilter mf = hit.collider.GetComponent<MeshFilter>();
            if (mr != null)
            {
                // Check for submeshes materials
                if (hit.collider is MeshCollider && mf != null && mf.mesh.subMeshCount > 1 && !mr.isPartOfStaticBatch)
                {
                    Mesh m = mf.sharedMesh;

                    int limit = hit.triangleIndex * 3;
                    int submesh;
                    for (submesh = 0; submesh < m.subMeshCount; submesh++)
                    {
                        int numIndices = m.GetTriangles(submesh).Length;
                        if (numIndices > limit)
                            break;

                        limit -= numIndices;
                    }

                    Material material = mr.sharedMaterials[submesh];
                    data = GetData(material);

                    if (data != null)
                        return data;
                }

                // Check for material
                data = GetData(mr.sharedMaterial);

                if (data != null)
                    return data;

                // Check for texture
                data = GetData(mr.sharedMaterial.mainTexture);

                if (data != null)
                    return data;
            }

            // Check for terrains
            Terrain t = hit.collider.GetComponent<Terrain>();
            if (t != null)
            {
                TerrainData TD = t.terrainData;

                if (TD.splatPrototypes.Length > 0)
                {
                    Texture finalTexture = null;

                    Vector3 position = hit.point;
                    Vector2 AS = new Vector2(TD.alphamapWidth, TD.alphamapHeight); // Control texture size
                    Vector3 TS = TD.size; // Terrain size

                    // Lookup texture we are standing on
                    int AX = (int)Mathf.Lerp(0, AS.x, Mathf.InverseLerp(t.transform.position.x, t.transform.position.x + TS.x, position.x));
                    int AY = (int)Mathf.Lerp(0, AS.y, Mathf.InverseLerp(t.transform.position.z, t.transform.position.z + TS.z, position.z));

                    float[,,] TerrCntrl = TD.GetAlphamaps(AX, AY, 1, 1);

                    for (int i = 0; i < TD.splatPrototypes.Length; i++)
                    {
                        if (TerrCntrl[0, 0, i] >= .5f)
                        {
                            finalTexture = TD.splatPrototypes[i].texture;
                            break;
                        }
                    }

                    // Check for terrain texture we're standing on
                    data = GetData(finalTexture);

                    if (data != null)
                        return data;
                }
            }

            return DefaultFootsteps;
        }

        private void OnValidate()
        {
            walkTimer = Mathf.Clamp(walkTimer, 0f, float.MaxValue);
            crouchTimerMultiplier = Mathf.Clamp(crouchTimerMultiplier, 0f, float.MaxValue);
            sprintTimerMultiplier = Mathf.Clamp(sprintTimerMultiplier, 0f, float.MaxValue);
        }

        [Serializable]
        public class Data
        {
            [SerializeField]
            [Tooltip("The name of the footstep data.")]
            protected string name;
            /// <summary>
            /// The name of the footstep data.
            /// </summary>
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            [Header("Linking")]
            [SerializeField]
            [Tooltip("Linked materials for this footstep data (If this belongs to the Default Footsteps, this is ignored.)")]
            protected List<Material> linkedMaterials;
            /// <summary>
            /// Linked materials for this footstep data.
            /// </summary>
            public List<Material> LinkedMaterials
            {
                get { return linkedMaterials; }
                set { linkedMaterials = value; }
            }
            [SerializeField]
            [Tooltip("Linked textures for this footstep data (If this belongs to the Default Footsteps, this is ignored.)")]
            protected List<Texture> linkedTextures;
            /// <summary>
            /// Linked textures for this footstep data.
            /// </summary>
            public List<Texture> LinkedTextures
            {
                get { return linkedTextures; }
                set { linkedTextures = value; }
            }

            [Header("Audioclips")]
            [SerializeField]
            [Tooltip("Footstep sounds.")]
            protected List<AudioClip> footstepClips;
            /// <summary>
            /// Footstep sounds.
            /// </summary>
            public List<AudioClip> FootstepClips
            {
                get { return footstepClips; }
                set { footstepClips = value; }
            }
            [SerializeField]
            [Tooltip("Jumping sounds.")]
            protected List<AudioClip> jumpingClips;
            /// <summary>
            /// Jumping sounds.
            /// </summary>
            public List<AudioClip> JumpingClips
            {
                get { return jumpingClips; }
                set { jumpingClips = value; }
            }
            [SerializeField]
            [Tooltip("Landing sounds.")]
            protected List<AudioClip> landingClips;
            /// <summary>
            /// Landing sounds.
            /// </summary>
            public List<AudioClip> LandingClips
            {
                get { return landingClips; }
                set { landingClips = value; }
            }

            [Header("Pitch")]
            [SerializeField]
            [Range(0f, 3f)]
            [Tooltip("Minimum random pitch of a footstep.")]
            private float minPitch;
            /// <summary>
            /// Minimum random pitch of a footstep.
            /// </summary>
            public float MinPitch
            {
                get { return minPitch; }
                set { minPitch = value; }
            }
            [SerializeField]
            [Range(0f, 3f)]
            [Tooltip("Maximum random pitch of a footstep.")]
            private float maxPitch;
            /// <summary>
            /// Maximum random pitch of a footstep.
            /// </summary>
            public float MaxPitch
            {
                get { return maxPitch; }
                set { maxPitch = value; }
            }

            public Data()
            {
                name = "Name me";
                minPitch = 0.95f;
                maxPitch = 1.05f;
            }

            /// <summary>
            /// Gets a random footstep sound from this <see cref="Data"/>.
            /// </summary>
            /// <returns>A random footstep sound from this <see cref="Data"/>.</returns>
            public AudioClip GetRandomFootstep()
            {
                if (footstepClips == null || footstepClips.Count <= 0)
                    return null;

                return footstepClips[Random.Range(0, footstepClips.Count)];
            }
            /// <summary>
            /// Gets a random jumping sound from this <see cref="Data"/>.
            /// </summary>
            /// <returns>A random jumping sound from this <see cref="Data"/>.</returns>
            public AudioClip GetRandomJumping()
            {
                if (jumpingClips == null || jumpingClips.Count <= 0)
                    return null;

                return jumpingClips[Random.Range(0, jumpingClips.Count)];
            }
            /// <summary>
            /// Gets a random landing sound from this <see cref="Data"/>.
            /// </summary>
            /// <returns>A random landing sound from this <see cref="Data"/>.</returns>
            public AudioClip GetRandomLanding()
            {
                if (landingClips == null || landingClips.Count <= 0)
                    return null;

                return landingClips[Random.Range(0, landingClips.Count)];
            }

            /// <summary>
            /// Gets a random pitch from this <see cref="Data"/>.
            /// </summary>
            /// <returns>A random pitch from this <see cref="Data"/>.</returns>
            public float GetRandomPitch()
            {
                return Random.Range(minPitch, maxPitch);
            }
        }
    }
}