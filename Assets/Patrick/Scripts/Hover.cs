using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    private bool accelerateUp;
    private float bobVelocity;

    public float maxVelocity;
    public float medianHeight;

    // Update is called once per frame
    void Update()
    {

        //Hover
        float yDiff = transform.localPosition.y - medianHeight;

        //Bobbing Effect
        if (yDiff < 0) {
            if (!accelerateUp) //crossed threshold
                bobVelocity = -1 * maxVelocity; //ensures constant bob size, not based on original ydiff
            accelerateUp = true;
        }
        else {
            if (accelerateUp) //crossed threshold
                bobVelocity = maxVelocity; //ensures constant bob size, not based on original ydiff
            accelerateUp = false;
        }

        if (accelerateUp) {
            transform.Translate(new Vector3(0, Time.deltaTime * ++bobVelocity / 25, 0)); //the / 25 slows it down
        }
        else {
            transform.Translate(new Vector3(0, Time.deltaTime * --bobVelocity / 25, 0)); //the / 25 slows it down
        }
    }
}
