using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smasher : MonoBehaviour {

    public GameObject destroyedVersionReal;
    public GameObject destroyedVersionLaser;

    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log(collision.gameObject.layer);

        if (collision.gameObject.tag.Equals("Real"))
        {
            Instantiate(destroyedVersionReal, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        if (collision.gameObject.tag.Equals("Laser"))
        {
            Instantiate(destroyedVersionLaser, transform.position, transform.rotation);
            Destroy(gameObject);
        }

    }


}
