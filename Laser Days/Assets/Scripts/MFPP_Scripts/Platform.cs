using UnityEngine;

namespace MFPP
{
    [RequireComponent(typeof(Rigidbody))]
    public class Platform : MonoBehaviour
    {
        /// <summary>
        /// The sine wave movement of the platform.
        /// </summary>
        [Tooltip("The sine wave movement of the platform.")]
        public Vector3 Movement;
        /// <summary>
        /// The movement frequency of the platform.
        /// </summary>
        [Tooltip("The movement frequency of the platform.")]
        public float Frequency = 1f;
        /// <summary>
        /// The constant rotation of the platform.
        /// </summary>
        [Tooltip("The constant rotation of the platform.")]
        public Vector3 Rotation;

        private new Rigidbody rigidbody;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            Vector3 offset = Movement * Mathf.Sin(Time.time * Frequency * Mathf.PI) * Time.deltaTime * Frequency * Mathf.Sqrt(2);
            rigidbody.MovePosition(transform.position + offset);
            rigidbody.MoveRotation(Quaternion.Euler(transform.eulerAngles + Rotation * Time.deltaTime));
        }
    }
}