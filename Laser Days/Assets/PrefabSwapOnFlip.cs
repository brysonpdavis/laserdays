using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSwapOnFlip : FlipInteraction
{

    public GameObject destroyedPrefab;
    private GameObject replacedObject;
    public float minDestroyDistance;
    public float explosionForce = 500f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Interact()
    {
        float distanceFromPlayer = Vector3.Distance(Toolbox.Instance.GetPlayer().transform.position, transform.position);

        if (minDestroyDistance > distanceFromPlayer && Toolbox.Instance.PlayerInReal())
        {

           
           replacedObject = Instantiate(destroyedPrefab, transform.position, transform.rotation);
           

            Rigidbody[] rb = replacedObject.GetComponentsInChildren<Rigidbody>();

            Debug.Log(rb.Length);

            //Vector3 velocity = GetComponent<Rigidbody>().velocity;

            foreach (Rigidbody r in rb)
            {
                Vector3 toCenter = r.gameObject.transform.position - replacedObject.transform.position;
                toCenter *= -3f;

                //r.AddExplosionForce(explosionForce, replacedObject.transform.position, 5, 1.0f, ForceMode.Impulse);
                r.velocity = new Vector3(0, 10, 0);

            //r.useGravity = false;

            }


            Destroy(gameObject);
        }

    }
}