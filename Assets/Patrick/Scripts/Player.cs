using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public GameObject selectedObj;

    // Start is called before the first frame update
    void Start()
    {
        TroopCreator.SetUpData();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            PrimaryClick();
        } else if (Input.GetMouseButton(1)) {
            SecondaryClick();
        }
    }

    private RaycastHit[] RayClick() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 100);

        return Physics.RaycastAll(ray);
    }

    private void PrimaryClick() {
        RaycastHit[] hits = RayClick();

        foreach (RaycastHit hit in hits) {
            string hitTag = hit.collider.tag;
            if (hitTag.Equals("Allies") || hitTag.Equals("Ground")) {
                if (hit.collider.GetComponent<Troop>() != null) { //if clicked a troop
                    selectedObj = hit.collider.gameObject; //select that troop
                } else {
                    if (selectedObj && selectedObj.GetComponent<Troop>() != null) { //if clicking while troop is selected
                        selectedObj = null; //deselect troop
                    }
                    else if (!selectedObj) {
                        selectedObj = TroopCreator.CreateTroop(TroopCreator.Find(1), hit.point);
                    }
                }
            }
        }
    }

    private void SecondaryClick() {
        RaycastHit[] hits = RayClick();

        foreach (RaycastHit hit in hits) {
            if (hit.collider.tag.Equals("Ground")) {
                if (selectedObj && selectedObj.GetComponent<Troop>() != null) { //if clicking while troop is selected
                    selectedObj.GetComponent<Troop>().targetLocation = hit.point; //set troop target location
                }
            }
        }
    }

}
