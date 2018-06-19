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
        private Camera mainCamera;

        void Start () {
            mainCamera = Camera.main;
        }


        private Rigidbody target;

        public override void AfterUpdate()
        {
            if (Input.GetButtonDown(PickUpButton)) // If pick up button was pressed
            {
                if (target != null) // If we already have a target rigidbody, set it to null, thus dropping/throwing it.
                {
                    target.transform.parent = null;
                    target.GetComponent<Rigidbody>().useGravity = true;
                    GetComponentInParent<RaycastManager>().heldObject = null; //remove from the list
                    target = null;
                }
                else
                {
                    
                    Ray r = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
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
                        rm.GetComponent<Rigidbody>().useGravity = true;
                        rm.heldObject = null;
                    }

                    //check if the object is already selected, remove it from list otherwise
                    if (rm.heldObject && rm.heldObject.GetComponent<ItemProperties>().selected)
                    {
                        rm.RemoveFromList(target.gameObject);
                        rm.selectedObjs.Remove(target.gameObject);
                    }

                }
            }

            if (target != null) // If target is not null, move the target in front of the camera at max pickup distance.
            {
               target.transform.parent = playerCam;
                target.GetComponent<Rigidbody>().useGravity = false;
                Vector3 floatingPosition = mainCamera.transform.position + mainCamera.transform.forward * MaxPickupDistance;
                target.angularVelocity *= 0.5f;
                target.velocity = Vector3.zero;
                target.AddForce((floatingPosition - target.transform.position) * 20f, ForceMode.VelocityChange);
            }
        }
    }
}