using UnityEngine;

namespace MFPP.Modules
{
    [HelpURL("https://ashkoredracson.github.io/MFPP/#pick-up-module")]
    public class PickUpModule : PlayerModule
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



        //OUR VARIABLES [will]
        public Transform player;
        public Transform playerCam;
        public bool selected = false;


        private Rigidbody target;

        public override void AfterUpdate()
        {
            if (Input.GetButtonDown(PickUpButton)) // If pick up button was pressed
            {
                if (target != null) // If we already have a target rigidbody, set it to null, thus dropping/throwing it.
                {
                    target.transform.parent = null;
                    GetComponentInParent<RaycastManager>().heldObject = null; //remove from the list
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


                    RaycastManager rm = GetComponent<RaycastManager>();
                    if (target) {
                        rm.heldObject = target.gameObject;
                    }
                    else {
                        rm.heldObject = null;
                    }

                    //check if the object is already selected, remove it from list otherwise
                    if (GetComponentInParent<RaycastManager>().heldObject.gameObject.GetComponent<ThrowObject>().selected)
                    {
                        rm.RemoveFromList(target.gameObject);
                        rm.selectedObjs.Remove(target.gameObject);

                    }

                }
            }

            if (target != null) // If target is not null, move the target in front of the camera at max pickup distance.
            {
                target.transform.parent = playerCam;
                Vector3 floatingPosition = Camera.transform.position + Camera.transform.forward * MaxPickupDistance;
                target.angularVelocity *= 0.5f;
                target.velocity = Vector3.zero;
                target.AddForce((floatingPosition - target.transform.position) * 20f, ForceMode.VelocityChange);
            }
        }
    }
}