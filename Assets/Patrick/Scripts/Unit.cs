using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Troop troop;

    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.tag.Equals(this.gameObject.tag)) { //should be 'Allies'
            Debug.Log(collision.gameObject.name + " " + collision.gameObject.tag);
            Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
        } else
        {
            //Debug.Log(collision.gameObject.name + " " + collision.gameObject.tag);
        }
    }
    /*
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag.Equals(this.gameObject.tag))
        { //should be 'Allies'
            Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
        }
        else
        {
            Debug.Log(collision.gameObject.name + " " + collision.gameObject.tag);
        }
    }*/
}
