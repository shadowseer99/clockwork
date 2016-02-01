using UnityEngine;
using System.Collections;

public class WallBuilderScript : MonoBehaviour {
    public GameObject w2h1;
    public GameObject w2h2;
    public GameObject w3h2;
    public GameObject w3h3;
    public GameObject w4h3;
    public GameObject w16h2;
    public int width=2;
    public int height=2;

    private GameObject[] blockArray;

    private GameObject[,] field;
    private int[,] idents;

    public void BuildObject()
    {
        blockArray = new GameObject[] { w2h1, w2h2, w3h2, w3h3, w4h3, w16h2 };

        field = new GameObject[width, height];
        idents = new int[width, height];
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                int rand = Random.Range(0, 2);
                //int rand = 0;
                if (rand == 0)
                {
                    Vector3 v = new Vector3(1 + w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                    field[w, h] = Instantiate(blockArray[rand]) as GameObject;
                    field[w, h].transform.position = v;
                    field[w, h].transform.parent = transform;
                    idents[w, h] = rand;
                }
                else if (rand == 1)
                {
                    Vector3 v = new Vector3(1 + w + transform.position.x - width / 2f, +h + transform.position.y - height / 2f, 0);
                    field[w, h] = Instantiate(blockArray[rand]) as GameObject;
                    field[w, h].transform.position = v;
                    field[w, h].transform.parent = transform;
                    idents[w, h] = rand;
                }
                else if (rand == 2)
                {
                    Vector3 v = new Vector3(1 + w + transform.position.x - width / 2f, +h + transform.position.y - height / 2f, 0);
                    field[w, h] = Instantiate(blockArray[rand]) as GameObject;
                    field[w, h].transform.position = v;
                    field[w, h].transform.parent = transform;
                    idents[w, h] = rand;
                }
                else if (rand == 3)
                {
                    Vector3 v = new Vector3(1 + w + transform.position.x - width / 2f, +h + transform.position.y - height / 2f, 0);
                    field[w, h] = Instantiate(blockArray[rand]) as GameObject;
                    field[w, h].transform.position = v;
                    field[w, h].transform.parent = transform;
                    idents[w, h] = rand;
                }
                else if (rand == 4)
                {
                    Vector3 v = new Vector3(1 + w + transform.position.x - width / 2f, +h + transform.position.y - height / 2f, 0);
                    field[w, h] = Instantiate(blockArray[rand]) as GameObject;
                    field[w, h].transform.position = v;
                    field[w, h].transform.parent = transform;
                    idents[w, h] = rand;
                }
                else
                {
                    Vector3 v = new Vector3(1 + w + transform.position.x - width / 2f, +h + transform.position.y - height / 2f, 0);
                    field[w, h] = Instantiate(blockArray[rand]) as GameObject;
                    field[w, h].transform.position = v;
                    field[w, h].transform.parent = transform;
                    idents[w, h] = rand;
                }

            }
        }

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                if(idents[w, h]==0)
                {
                    DestroyImmediate(field[w+1, h]);
                    field[w+1, h] = null;
                    idents[w + 1, h] = -1;
                }
                else if(idents[w, h] == 1)
                {
                    if(h==(height-1))
                    {
                        DestroyImmediate(field[w , h]);
                        Vector3 v = new Vector3(1 + w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                        field[w, h] = Instantiate(blockArray[0]) as GameObject;
                        field[w, h].transform.position = v;
                        field[w, h].transform.parent = transform;
                        idents[w, h] = 0;
                    }
                    else
                    {
                        DestroyImmediate(field[w , h+1]);
                        field[w , h+1] = null;
                        idents[w , h+1] = -1;
                        DestroyImmediate(field[w + 1, h+1]);
                        field[w + 1, h+1] = null;
                        idents[w + 1, h+1] = -1;
                    }
                    DestroyImmediate(field[w + 1, h]);
                    field[w + 1, h] = null;
                    idents[w + 1, h] = -1;
                }

            }
        }
    }
	
}
