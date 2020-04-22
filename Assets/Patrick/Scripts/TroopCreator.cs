using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TroopData {
    public List<GameObject> troopPrefabs;
    public int troopId;

    public string name;
    public float movementSpeed;

    public int numX, numZ;
    public float spaceBetween;
}

public static class TroopCreator
{

    //public List<GameObject> knights;
    public static List<TroopData> troopDatas;

    public static void SetUpData() { //TO DO: Make this more generic so that files can be used for knight data
        TroopData knightData = new TroopData {
            troopPrefabs = new List<GameObject>() {
                (GameObject)Resources.Load("Character_Knight_Black"),
                (GameObject)Resources.Load("Character_Knight_White"),
                (GameObject)Resources.Load("Character_Knight_Brown")
            },
            troopId = 1,
            name = "Knights",
            movementSpeed = 5,
            numX = 5,
            numZ = 3,
            spaceBetween = 2.5f
        };
        troopDatas = new List<TroopData>() {
            knightData
        };
        if (troopDatas[0].troopPrefabs[0] == null)
            Debug.Log("null");
    }

    public static GameObject CreateTroop(TroopData troopData, Vector3 position) {
        GameObject troop = new GameObject(troopData.name, typeof(Troop), typeof(BoxCollider), typeof(Rigidbody));
        BoxCollider boxCollider = troop.GetComponent<BoxCollider>();
        boxCollider.center = new Vector3(0, 1, 0);
        boxCollider.size = new Vector3(12, 2, 6);
        boxCollider.isTrigger = true;
        troop.GetComponent<Troop>().movementSpeed = troopData.movementSpeed;
        troop.GetComponent<Rigidbody>().useGravity = true;

        troop.tag = "Allies"; //TO DO: MAKE ALLIES NOT COLLIDE WITH OTHER ALLIES
        troop.transform.position = new Vector3(position.x, 0.3f, position.z); //1 unit above ground

        float xPos, zPos;
        for (int z = 0; z < troopData.numZ; z++) {
            zPos = -(((troopData.numZ - 1) * troopData.spaceBetween) / 2) + (z * troopData.spaceBetween);
            for (int x = 0; x < troopData.numX; x++) {
                xPos = -(((troopData.numX - 1) * troopData.spaceBetween) / 2) + (x * troopData.spaceBetween);
                GameObject obj = Object.Instantiate(troopData.troopPrefabs[Random.Range(0, troopData.troopPrefabs.Count)], position + new Vector3(xPos, troop.transform.position.y, zPos),
                    Quaternion.identity, troop.transform);
                obj.tag = "Allies";
                obj.AddComponent<Unit>().troop = troop.GetComponent<Troop>(); //adds Unit script for each unit
                
            }
        }

        //Create Selected Sphere Above
        GameObject selectSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        float height = 3;

        Hover hoverComponent = selectSphere.AddComponent<Hover>();
        hoverComponent.medianHeight = height;
        hoverComponent.maxVelocity = 30;
        selectSphere.transform.parent = troop.transform;
        selectSphere.name = "Select Highlight";

        selectSphere.transform.localPosition = new Vector3(0, height+1, 0);
        selectSphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        selectSphere.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/SelectHighlight");
        selectSphere.SetActive(false);
        return troop;
    }

    public static TroopData Find(int troopId) {
        foreach (TroopData data in troopDatas) {
            if (data.troopId == troopId)
                return data;
        }
        return troopDatas[0]; //return first troop in list if not found
    }

    public static TroopData Find(string name) {
        foreach (TroopData data in troopDatas) {
            if (data.name.Equals(name))
                return data;
        }
        return troopDatas[0]; //return first troop in list if not found
    }

    /*
    public static void CreateTroops(List<GameObject> troops, int numX, int numZ, float spaceBetween, Vector3 position) {
        GameObject knightTroop = new GameObject("Knight Troop");
        knightTroop.transform.position = position;

        float xPos, zPos;
        for (int z = 0; z < numZ; z++) {
            zPos = -(((numZ - 1) * spaceBetween) / 2) + (z * spaceBetween);
            for (int x = 0; x < numX; x++) {
                xPos = -(((numX - 1) * spaceBetween) / 2) + (x * spaceBetween);
                Instantiate(troops[Random.Range(0, troops.Count)], position + new Vector3(xPos, 0, zPos),
                    Quaternion.identity, knightTroop.transform);
            }
        }
    }

    public void CreateKnights(int numX, int numZ, float spaceBetween, Vector3 position) {
        GameObject knightTroop = new GameObject("Knight Troop", typeof(Troop), typeof(BoxCollider));
        BoxCollider boxCollider = knightTroop.GetComponent<BoxCollider>();
        boxCollider.center = new Vector3(0, 1, 0);
        boxCollider.size = new Vector3(12, 2, 6);
        boxCollider.isTrigger = true;
        knightTroop.GetComponent<Troop>().movementSpeed = 5.0f;

        knightTroop.transform.position = new Vector3(position.x, 1, position.z); //always 1 unit above ground

        float xPos, zPos;
        for (int z = 0; z < numZ; z++) {
            zPos = -(((numZ - 1) * spaceBetween) / 2) + (z * spaceBetween);
            for (int x = 0; x < numX; x++) {
                xPos = -(((numX - 1) * spaceBetween) / 2) + (x * spaceBetween);
                Instantiate(knights[Random.Range(0, knights.Count)], position + new Vector3(xPos, 0, zPos),
                    Quaternion.identity, knightTroop.transform);
            }
        }
    }
    */
}
