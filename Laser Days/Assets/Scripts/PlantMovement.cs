using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantMovement : MonoBehaviour {

    bool active = false;
    public Vector2 growthMinMax;
    public float speed;
    float startingY;
    float elapsed;

    SpawnableMutation spawnable;

    float dif;

    private void Start()
    {
        dif = growthMinMax[1] - growthMinMax[0];
        spawnable = GetComponent<SpawnableMutation>();
    }

    public void Activate()
    {
        active = true;
        startingY = transform.localScale.y;
        elapsed = 0;
    }

    public void Deactivate()
    {
        active = false;
    }


    // Update is called once per frame
    void Update() {
		
        if (active)
        {
            elapsed += Time.deltaTime;
            float amount = ((Mathf.Sin(elapsed * speed) + 1f) / 2f);

            float newYValue = Mathf.Lerp(growthMinMax[0], growthMinMax[1], amount);

            transform.localScale = new Vector3(transform.localScale.x, startingY + newYValue , transform.localScale.z);

        }


	}
}
