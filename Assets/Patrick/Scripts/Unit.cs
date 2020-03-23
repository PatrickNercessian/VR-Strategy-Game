using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Troop troop;

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == this.gameObject.tag) { //should be 'Allies'
            Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
        }
    }
}
