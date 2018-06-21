using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smasher : MonoBehaviour {

    //bring in prefabs of the objects that you want your original object to break into
    public GameObject destroyedVersionReal;
    public GameObject destroyedVersionLaser;

    private void OnCollisionEnter(Collision collision)
    {
        //spawns fractured version of "real" object in place
        if (collision.gameObject.tag.Equals("Real"))
        {
            Instantiate(destroyedVersionReal, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        //spawns fractured version of "laser" object in place
        if (collision.gameObject.tag.Equals("Laser"))
        {
            Instantiate(destroyedVersionLaser, transform.position, transform.rotation);
            Destroy(gameObject);
        }

    }


}
