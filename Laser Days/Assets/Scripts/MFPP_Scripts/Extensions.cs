using System;
using UnityEngine;

namespace MFPP
{
    public static class Extensions
    {
        /// <summary>
        /// Draws a capsule gizmo.
        /// </summary>
        /// <param name="center">The center of the capsule.</param>
        /// <param name="height">The total height of the capsule.</param>
        /// <param name="radius">The radius of the capsule.</param>
        /// <param name="visualRadius">The visual radius of the capsule.</param>
        public static void GizmosDrawCapsule(Vector3 center, float height, float radius, float visualRadius)
        {
            float length = Mathf.Clamp(height - radius * 2f, 0f, float.MaxValue);

            Vector3 p1 = center + Vector3.up * (length / 2f);
            Vector3 p2 = center - Vector3.up * (length / 2f);

            Gizmos.DrawWireSphere(p1, visualRadius);
            Gizmos.DrawWireSphere(p2, visualRadius);

            Gizmos.DrawLine(p1 + Vector3.left * visualRadius, p2 + Vector3.left * visualRadius);
            Gizmos.DrawLine(p1 + Vector3.forward * visualRadius, p2 + Vector3.forward * visualRadius);
            Gizmos.DrawLine(p1 + Vector3.right * visualRadius, p2 + Vector3.right * visualRadius);
            Gizmos.DrawLine(p1 + Vector3.back * visualRadius, p2 + Vector3.back * visualRadius);
        }

        /// <summary>
        /// Determines if a layer mask contains a certain layer.
        /// </summary>
        /// <param name="mask">The layer mask.</param>
        /// <param name="layer">The layer.</param>
        /// <returns>True if the layer mask contains that layer.</returns>
        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }

        public static bool IsNullOrWhitespace(this string s)
        {
            return s == null || s.Trim().Length <= 0;
        }
    }
}