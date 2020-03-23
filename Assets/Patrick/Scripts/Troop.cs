using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Troop : MonoBehaviour
{

    public Vector3 targetLocation;
    public float movementSpeed;

    private bool isMoving;


    // Start is called before the first frame update
    void Start()
    {
        targetLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveStep();
    }
        

    private void MoveStep() {
        Vector3 difference = targetLocation - transform.position;
        float distance = Mathf.Sqrt(Mathf.Pow(difference.x, 2) + Mathf.Pow(difference.z, 2));
        if (distance > 0.5f) {
            //if total rotation is greater than 90 degrees and less than 270 degrees, flip troop
            float desiredRotationAmount = transform.eulerAngles.y - Quaternion.LookRotation(difference, Vector3.up).eulerAngles.y;
            if (Mathf.Abs(desiredRotationAmount) > 90 && Mathf.Abs(desiredRotationAmount) < 270) {
                Debug.Log(desiredRotationAmount);
                RotateWithoutChildren(180);
                FlipChildren();
            } else {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(difference, Vector3.up), Time.deltaTime * 3);
            }

            transform.Translate(0, 0, Mathf.Clamp(distance, 2.5f, movementSpeed) * Time.deltaTime, Space.Self); //move forward
            if (!isMoving) {
                isMoving = true;
                ChangeAnimations();
            }
        } else {
            if (isMoving) {
                isMoving = false;
                ChangeAnimations();
            }
        }
    }

    private void RotateWithoutChildren(float amount) {
        // Get all direct children
        Transform[] children = new Transform[this.transform.childCount];
        int i = 0;
        foreach (Transform child in this.transform) {
            children[i++] = child;
        }
        // Detach
        this.transform.DetachChildren();

        // Change parent transform
        this.transform.Rotate(0.0f, amount, 0.0f);

        // Reparent
        foreach (Transform child in children) {
            child.parent = this.transform;
        }
    }

    private void FlipChildren() {
        foreach (Transform child in this.transform) {
            child.Rotate(0.0f, 180, 0.0f);
        }
    }

    private void ChangeAnimations() {
        foreach (Transform child in transform) {
            Animator animator = child.GetComponent<Animator>();
            if (isMoving) {
                animator.SetBool("isMoving", true);
            } else {
                animator.SetBool("isMoving", false);
            }
        }
    }
}
