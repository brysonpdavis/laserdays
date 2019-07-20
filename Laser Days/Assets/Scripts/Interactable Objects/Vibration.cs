using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibration : MonoBehaviour {

	public bool active;

    public float amount = 1.0f; //how much it shakes
    private int count;
    private int counter = 0;
    public float maxDistance = .05f;
    private Vector3 origin;

    private void Start()
    {
        count = 3*(int)Mathf.Floor(Random.Range(1, 5));
        origin = transform.position;
    }

    void FixedUpdate()
    {
		if (active)
			{
            counter += 1;
            if (counter == count)
            {
                counter = 0;
                Vector3 temp = Random.insideUnitSphere * amount;

                if (Vector3.Distance(transform.position + temp, origin) > maxDistance)
                {
                    transform.position -= temp;
                }

                else transform.position += temp;

            }

		    }
		    
       
    }
}
