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
        private Camera mainCamera;
        public GameObject heldObject;

        void Start () {
            mainCamera = Camera.main;
        }

        private Rigidbody target;

        public override void AfterUpdate()
        {
            if (Input.GetButtonDown(PickUpButton)) // If pick up button was pressed
            {
                if (target) // If we already have a target rigidbody, set it to null, thus dropping/throwing it.
                {
                    PutDown();
                }
                else
                {
                    Ray r = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(r, out hit, MaxPickupDistance)) // Otherwise, if target was null, shoot a ray where we are aiming.
                    {
                        Rigidbody body = hit.collider.attachedRigidbody;
                        if (body && !body.isKinematic) // Retreive the rigidbody and assure that it is not kinematic.
                        {
                            PickUp(body);
                        }
                    }

                    //check if the object is already selected, remove it from list
                    if (heldObject && heldObject.GetComponent<ItemProperties>().selected)
                    {
                        RaycastManager rm = GetComponent<RaycastManager>();
                        rm.RemoveFromList(target.gameObject);
                        rm.selectedObjs.Remove(target.gameObject);
                    }
                }
            }

            if (target) // If target is not null, move the target in front of the camera at max pickup distance.
            {
                Vector3 floatingPosition = mainCamera.transform.position + mainCamera.transform.forward * MaxPickupDistance;
                target.angularVelocity *= 0.5f;
                target.velocity = ((floatingPosition - target.transform.position) * 10f);

                //AVOIDING FLYING ON TOP OF HELD OBJECT GLITCH:
                //checks rotation of player so that it's down (the between the 67 and 91f)
                //checks that the player is actually looking at OBJECT, (red crosshair), rather than looking at ground while object is distant
                if ((playerCam.transform.localEulerAngles.x > 65f) && (playerCam.transform.localEulerAngles.x < 91f) && GetComponent<RaycastManager>().crossHair.color == Color.red)
                {
                    PutDown();
                }
                // target.velocity = Vector3.zero;
                // target.AddForce((floatingPosition - target.transform.position) * 20f, ForceMode.VelocityChange);
            }
        }

        void PickUp (Rigidbody body) {
            target = body; // Set the target
            heldObject = target.gameObject;
            target.GetComponent<Rigidbody>().useGravity = false;
            // target.transform.parent = playerCam;

            target.freezeRotation = true;
        }

        void PutDown () {
            target.freezeRotation = false;
            // target.transform.parent = null;
            target.GetComponent<Rigidbody>().useGravity = true;
            heldObject = null; //remove from the list
            target = null;
        }
    }
}
