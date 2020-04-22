using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSelect : MonoBehaviour
{
    public Player player;
    public GameObject ground;

    /*
    private void OnTriggerEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Allies") && collision.gameObject.GetComponent<Troop>() != null)
        {
            player.selectedObjects.Add(collision.gameObject);
        }
    }
*/
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Allies") && other.gameObject.GetComponent<Troop>() != null)
        {
            Debug.Log("trigger entered");
            player.SelectTroop(other.gameObject);
        }
    }

    /*
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Allies") && other.gameObject.GetComponent<Troop>() != null)
        {
            if (player.selectedObjects.Contains(other.gameObject))
                player.selectedObjects.Remove(other.gameObject);
        }
    }*/
}
