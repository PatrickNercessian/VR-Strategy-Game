using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCreator : MonoBehaviour
{

    public List<GameObject> tiles;
    [Range(1, 9)]public int tileDiameter;

    // Start is called before the first frame update
    void Start()
    {
        //need odd tileDiamter in order to have a center tile
        if (tileDiameter % 2 == 0)
            tileDiameter--;

        int x = -(tileDiameter / 2), z = -(tileDiameter / 2); //start at top left
        for (int i = 0; i < (tileDiameter * tileDiameter); i++) {
            GameObject gb = new GameObject("Tile " + (int) x + " " + (int) z);
            gb.transform.SetParent(this.transform);
            gb.transform.position = this.transform.position + new Vector3(x * 10, 0, z * 10);


            //instantiate random tile prefab from list at x*10,0,z*10 position as a child of this
            GameObject tile = Instantiate(tiles[Random.Range(0, tiles.Count)], gb.transform.position, Quaternion.identity, gb.transform);

            int randomRot = Random.Range(0, 4);
            tile.transform.Rotate(0, randomRot * 90, 0);

            x++;
            if (x > (tileDiameter / 2)) {
                x = -(tileDiameter / 2);
                z++;
            }
        }
    }
}
