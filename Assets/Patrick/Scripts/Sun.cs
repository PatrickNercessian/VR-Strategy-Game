using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{

    private void Start() {
        this.transform.eulerAngles = new Vector3(10, 0, 0);
    }

    void FixedUpdate()
    {
        this.transform.Rotate(new Vector3(0.1f, 0, 0));
    }
}
