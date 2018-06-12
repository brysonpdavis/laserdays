using UnityEngine;

namespace MFPP.Modules
{
    [HelpURL("https://ashkoredracson.github.io/MFPP/#pick-up-module")]
    public class PickUpModule1 : PlayerModule
    {
        /// <summary>
        /// The maximum pickup distance.
        /// </summary>
        [Space]
        [Tooltip("The maximum pickup distance.")]
        public float MaxPickupDistance = 2f;
        /// <summary>
        /// Pick up button.
        /// </summary>
        [Tooltip("Pick up button.")]
        public string PickUpButton = "Pick Up";

        private Rigidbody target;

        public override void AfterUpdate()
        {
            if (Input.GetButtonDown(PickUpButton)) // If pick up button was pressed
            {
                if (target != null) // If we already have a target rigidbody, set it to null, thus dropping/throwing it.
                {
                    target = null;
                }
                else
                {
                    Ray r = new Ray(Camera.transform.position, Camera.transform.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(r, out hit, MaxPickupDistance)) // Otherwise, if target was null, shoot a ray where we are aiming.
                    {
                        Rigidbody body = hit.collider.attachedRigidbody;
                        if (body != null && !body.isKinematic) // Retreive the rigidbody and assure that it is not kinematic.
                        {
                            target = body; // Set the target
                        }
                    }
                }
            }

            if (target != null) // If target is not null, move the target in front of the camera at max pickup distance.
            {
                Vector3 floatingPosition = Camera.transform.position + Camera.transform.forward * MaxPickupDistance;
                target.angularVelocity *= 0.5f;
                target.velocity = Vector3.zero;
                target.AddForce((floatingPosition - target.transform.position) * 20f, ForceMode.VelocityChange);
            }
        }
    }
}