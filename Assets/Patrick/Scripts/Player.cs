using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool pcDebugging;

    public List<GameObject> selectedObjects;
    public OVRHand rightHand;
    public OVRHand leftHand;

    public OVRSkeleton rightHandSkeleton;
    public OVRSkeleton leftHandSkeleton;

    private List<OVRBone> rightFingerBones;
    private List<OVRBone> leftFingerBones;

    private LineRenderer lineRenderer;

    private bool usingRightHand;
    private bool inMultiSelectMode;
    private Vector3 firstMultiSelectPoint;
    private Vector3 secondMultiSelectPoint;
    private GameObject multiSelectBox;

    private float timeSinceLastRightIndexPinch; //EVENTUALLY REMOVE THIS: REPLACE WITH ONLY ALLOWING PLACEMENT AFTER PINCH STOPS AND THEN HAPPENS AGAIN
    private float timeSinceLastLeftIndexPinch;
    public GameObject ground; //FOR DEBUGGING, PLEASE REMOVE

    // Start is called before the first frame update
    void Start()
    {
        rightFingerBones = new List<OVRBone>(rightHandSkeleton.Bones);
        leftFingerBones = new List<OVRBone>(leftHandSkeleton.Bones);
        lineRenderer = GetComponent<LineRenderer>();
        inMultiSelectMode = false;
        usingRightHand = true;
        TroopCreator.SetUpData();
    }

    // Update is called once per frame
    void Update()
    {
        if (pcDebugging)
        {
            timeSinceLastLeftIndexPinch += Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                SelectTroopFromRay(RayClick());
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out RaycastHit hit);
                firstMultiSelectPoint = hit.point;
                timeSinceLastLeftIndexPinch = 0;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                PlaceTroop(RayClick());
            }
            else if (Input.GetMouseButtonDown(2))
            {
                MoveTroop(RayClick());
            }
            else if (Input.GetMouseButtonUp(0) && timeSinceLastLeftIndexPinch > 0.1f)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out RaycastHit hit);
                secondMultiSelectPoint = hit.point;
                MultiSelectTroops();
            }
        } else
        {
            Pinch();
        }
    }


    //VR Pinching
    private void Pinch()
    {
        OVRSkeleton.BoneId fingerTipId = OVRSkeleton.BoneId.Hand_IndexTip;
        OVRSkeleton.BoneId fingerTipJointId = OVRSkeleton.BoneId.Hand_Index3;
        bool fullPinch = false;

        RaycastHit[] hits = null;

        if (rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > 0.2f) //if starting to pinch right index
        {
            usingRightHand = true;
            fullPinch = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) >= 0.95f; //only draw if not fully pinching
            if (fullPinch)
            {
                hits = RayPinch(fingerTipId, fingerTipJointId);
                if (timeSinceLastRightIndexPinch > 0.08f && timeSinceLastRightIndexPinch < 0.6f)//if Double Pinch (should ignore holding pinch)
                {
                    //ground.SetActive(!ground.activeInHierarchy);
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider.tag.Equals("Ground"))
                            firstMultiSelectPoint = hit.point;
                    }
                    inMultiSelectMode = true;
                }
                if (inMultiSelectMode)
                {
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider.tag.Equals("Ground"))
                        {
                            secondMultiSelectPoint = hit.point;
                            MultiSelectTroops();
                        }
                    }
                }
                timeSinceLastRightIndexPinch = 0;
            }
        }
        else if (rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) > 0.2f) //if starting to pinch right middle
        {
            usingRightHand = true;
            fullPinch = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) >= 0.95f;
            fingerTipId = OVRSkeleton.BoneId.Hand_MiddleTip;
            fingerTipJointId = OVRSkeleton.BoneId.Hand_Middle3;
        }
        else if (leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > 0.2f) //if starting to pinch left index
        {
            usingRightHand = false;
            if (timeSinceLastLeftIndexPinch > 1)
                fullPinch = leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) >= 0.95f;
            if (fullPinch)
            {
                timeSinceLastLeftIndexPinch = 0;
            }
        }
        timeSinceLastRightIndexPinch += Time.deltaTime;
        timeSinceLastLeftIndexPinch += Time.deltaTime;


        if (inMultiSelectMode && rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) < 0.95f) //if you let go of pinch while in multiselectmode
        {
            inMultiSelectMode = false;
            Object.Destroy(this.multiSelectBox);
        }

        if (hits == null)
             hits = RayPinch(fingerTipId, fingerTipJointId);
        

        //If an action should occur (i.e. player fully pinched fingers)
        if (fullPinch)
        {
            if (usingRightHand)
            {
                if (!inMultiSelectMode && fingerTipId == OVRSkeleton.BoneId.Hand_IndexTip && fingerTipJointId == OVRSkeleton.BoneId.Hand_Index3)
                    SelectTroopFromRay(hits);
                else if (fingerTipId == OVRSkeleton.BoneId.Hand_MiddleTip && fingerTipJointId == OVRSkeleton.BoneId.Hand_Middle3)
                    MoveTroop(hits);
            } else
            {
                if (fingerTipId == OVRSkeleton.BoneId.Hand_IndexTip && fingerTipJointId == OVRSkeleton.BoneId.Hand_Index3)
                    PlaceTroop(hits);
            }
        }
    }

    private RaycastHit[] RayPinch(OVRSkeleton.BoneId fingerTipId, OVRSkeleton.BoneId fingerTipJointId)
    {
        OVRBone fingerTip = null;
        OVRBone fingerTipJoint = null;
        OVRBone thumbTip = null;
        OVRBone thumbTipJoint = null;

        List<OVRBone> fingerBones = (usingRightHand) ? rightFingerBones : leftFingerBones;
        foreach (OVRBone bone in fingerBones)
        {
            if (bone.Id == fingerTipId) fingerTip = bone;
            else if (bone.Id == fingerTipJointId) fingerTipJoint = bone;
            else if (bone.Id == OVRSkeleton.BoneId.Hand_ThumbTip) thumbTip = bone;
            else if (bone.Id == OVRSkeleton.BoneId.Hand_Thumb3) thumbTipJoint = bone;
        }

        //Drawing line from average b/t joints to average b/t tips and beyond (hence the * 10000)
        Vector3 tipAveragePoint = (fingerTip.Transform.position + thumbTip.Transform.position) / 2;
        Vector3 jointAveragePoint = (fingerTipJoint.Transform.position + thumbTipJoint.Transform.position) / 2;
        Vector3 diff = tipAveragePoint - jointAveragePoint;
        Vector3 point2 = jointAveragePoint + (diff * 10000);
        lineRenderer.SetPositions(new Vector3[] { jointAveragePoint, point2 });

        return Physics.RaycastAll(jointAveragePoint, diff, 10000);
    }

    //PC Clicking
    private RaycastHit[] RayClick() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 100);

        lineRenderer.SetPositions(new Vector3[] { ray.origin - new Vector3(0,0.1f,0), ray.GetPoint(100) });

        return Physics.RaycastAll(ray);
    }

    private void PlaceTroop(RaycastHit[] hits)
    {
        DeselectAllTroops();
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag.Equals("Ground"))
            {
                Debug.Log("Place Troops On Ground");
                SelectTroop(TroopCreator.CreateTroop(TroopCreator.Find(1), hit.point));
            }
        }
    }

    private void SelectTroopFromRay(RaycastHit[] hits) {
        DeselectAllTroops();
        foreach (RaycastHit hit in hits) {
            if (hit.collider.tag.Equals("Allies"))  //if clicked a troop
            {
                Debug.Log("hit ally");
                Troop troop = hit.collider.gameObject.GetComponent<Troop>();
                if (troop != null)
                    SelectTroop(troop.gameObject);
                return;
            }
        }
    }

    private void MultiSelectTroops()
    {
        DeselectAllTroops();

        if (!multiSelectBox)
        {
            multiSelectBox = new GameObject("MultiSelect Collider", typeof(BoxCollider), typeof(LineRenderer), typeof(MultiSelect), typeof(Rigidbody));
            multiSelectBox.GetComponent<Rigidbody>().isKinematic = true;
            multiSelectBox.GetComponent<MultiSelect>().ground = this.ground; //DEBUGGING
        }
        multiSelectBox.GetComponent<MultiSelect>().player = this;
        BoxCollider bc = multiSelectBox.GetComponent<BoxCollider>();
        bc.center = (firstMultiSelectPoint + secondMultiSelectPoint) / 2;
        bc.size = new Vector3(Mathf.Abs(bc.center.x - firstMultiSelectPoint.x) * 2, 1, Mathf.Abs(bc.center.z - secondMultiSelectPoint.z) * 2);
        bc.center = new Vector3(bc.center.x, bc.center.y + 0.5f, bc.center.z);
        bc.isTrigger = true;
        

        LineRenderer lr = multiSelectBox.GetComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.loop = true;
        lr.positionCount = 4;
        lr.SetPositions(new Vector3[] {
            firstMultiSelectPoint,
            new Vector3(firstMultiSelectPoint.x, firstMultiSelectPoint.y, secondMultiSelectPoint.z),
            secondMultiSelectPoint,
            new Vector3(secondMultiSelectPoint.x, secondMultiSelectPoint.y, firstMultiSelectPoint.z),
        });
    }

    private void MoveTroop(RaycastHit[] hits) {

        foreach (RaycastHit hit in hits) {
            if (hit.collider.tag.Equals("Ground")) {
                foreach (GameObject selectedObject in selectedObjects) {
                    if (selectedObject.GetComponent<Troop>() != null) {
                        selectedObject.GetComponent<Troop>().targetLocation = hit.point; //set troop target location
                    }
                }
            }
        }
    }

    public void SelectTroop(GameObject troopObj)
    {
        Debug.Log("nuh");
        if (!selectedObjects.Contains(troopObj))
        {
            Debug.Log("yuh");
            troopObj.transform.Find("Select Highlight").gameObject.SetActive(true);
            selectedObjects.Add(troopObj);
        }

    }

    private void DeselectAllTroops()
    {
        foreach (GameObject obj in selectedObjects)
        {
            obj.transform.Find("Select Highlight").gameObject.SetActive(false);
        }
        selectedObjects.Clear();
    }

}
