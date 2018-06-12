using UnityEngine;

namespace MFPP.Modules
{
    [HelpURL("https://ashkoredracson.github.io/MFPP/#rigidbody-push-module")]
    public class RigidbodyPushModule : PlayerModule
    {
        /// <summary>
        /// The pushing force.
        /// </summary>
        [Space]
        [Tooltip("The pushing force.")]
        public float PushForce = 10f;

        public override void Initialize()
        {
            Player.CollisionDetected += Player_CollisionDetected; // Register collision detected event.
        }

        private void Player_CollisionDetected(ControllerColliderHit hit)
        {
            if (!Enabled)
                return;

            Rigidbody r = hit.collider.attachedRigidbody; // Get rigidbody from collision.

            if (r != null && !r.isKinematic) // If rigidbody is not null and is not kinematic then
            {
                Vector3 playerVelocity = Player.FinalMovement; // Get the current player velocity (Final movement so that we can detect input pushing)
                playerVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z); // Nullify Y axis.

                hit.rigidbody.AddForceAtPosition(hit.moveDirection.normalized * playerVelocity.magnitude * PushForce, hit.point, ForceMode.Force); // Apply force
            }
        }
    }
}