using UnityEngine;
using System.Collections;

public class WallBuilderScript : MonoBehaviour {
    public GameObject w1h1;
    public GameObject w2h1;
    public GameObject w2h2;
    public GameObject w3h2;
    public GameObject w3h3;
    public GameObject w4h3;
    public GameObject w16h2;
    public int width=1;
    public int height=1;

    private GameObject[] blockArray;

    private GameObject[,] field;
    private int[,] idents;

    public void KillChildren()
    {
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                if(idents[w,h]!=-1)
                {
                    DestroyImmediate(field[w, h]);
                    idents[w, h] = -1;
                }
            }
        }
    }

    public void BuildObject()
    {
        blockArray = new GameObject[] {w1h1, w2h1, w2h2, w3h2, w3h3, w4h3, w16h2 };

        field = new GameObject[width, height];
        idents = new int[width, height];
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                int rand = Random.Range(0, 7);
                //int rand = 6;
                if (rand == 0)
                {
                    Vector3 v = new Vector3(.5f+ w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                    field[w, h] = Instantiate(blockArray[rand]) as GameObject;
                    field[w, h].transform.position = v;
                    field[w, h].transform.parent = transform;
                    idents[w, h] = rand;
                }
                else if (rand == 1)
                {
                    Vector3 v = new Vector3(1 + w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
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
                    Vector3 v = new Vector3(1.5f + w + transform.position.x - width / 2f, +h + transform.position.y - height / 2f, 0);
                    field[w, h] = Instantiate(blockArray[rand]) as GameObject;
                    field[w, h].transform.position = v;
                    field[w, h].transform.parent = transform;
                    idents[w, h] = rand;
                }
                else if (rand == 4)
                {
                    Vector3 v = new Vector3(1.5f + w + transform.position.x - width / 2f, +h + transform.position.y - height / 2f, 0);
                    field[w, h] = Instantiate(blockArray[rand]) as GameObject;
                    field[w, h].transform.position = v;
                    field[w, h].transform.parent = transform;
                    idents[w, h] = rand;
                }
                else if (rand == 5)
                {
                    Vector3 v = new Vector3(2f + w + transform.position.x - width / 2f, +h + transform.position.y - height / 2f, 0);
                    field[w, h] = Instantiate(blockArray[rand]) as GameObject;
                    field[w, h].transform.position = v;
                    field[w, h].transform.parent = transform;
                    idents[w, h] = rand;
                }
                else if (rand == 6)
                {
                    Vector3 v = new Vector3(8f + w + transform.position.x - width / 2f, +h + transform.position.y - height / 2f, 0);
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
                if(idents[w, h]==1)
                {
                    if(w==(width-1))
                    {
                        DestroyImmediate(field[w, h]);
                        Vector3 v = new Vector3(.5f + + w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                        field[w, h] = Instantiate(blockArray[0]) as GameObject;
                        field[w, h].transform.position = v;
                        field[w, h].transform.parent = transform;
                        idents[w, h] = 0;
                    }
                    else
                    {
                        if (idents[w + 1, h ] == -1)
                        {
                            DestroyImmediate(field[w, h]);
                            Vector3 v = new Vector3(.5f + +w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                            field[w, h] = Instantiate(blockArray[0]) as GameObject;
                            field[w, h].transform.position = v;
                            field[w, h].transform.parent = transform;
                            idents[w, h] = 0;

                        }
                        else
                        {
                            DestroyImmediate(field[w + 1, h ]);
                            field[w + 1, h ] = null;
                            idents[w + 1, h ] = -1;
                        }
                    }
                    
                }
                else if(idents[w, h] == 2)
                {
                    if (h == (height - 1))
                    {
                        DestroyImmediate(field[w, h]);
                        Vector3 v = new Vector3(1 + w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                        field[w, h] = Instantiate(blockArray[1]) as GameObject;
                        field[w, h].transform.position = v;
                        field[w, h].transform.parent = transform;
                        idents[w, h] = 1;
                        h--;
                    }
                    else if (w == (width - 1))
                    {
                        DestroyImmediate(field[w, h]);
                        Vector3 v = new Vector3(.5f + +w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                        field[w, h] = Instantiate(blockArray[0]) as GameObject;
                        field[w, h].transform.position = v;
                        field[w, h].transform.parent = transform;
                        idents[w, h] = 0;
                    }
                    else
                    {
                        for (int w2 = 0; w2 < 2; w2++)
                        {
                            for (int h2 = 0; h2 < 2; h2++)
                            {
                                if (w2 + h2 != 0)
                                {
                                    if (idents[w + w2, h + h2] == -1)
                                    {
                                        DestroyImmediate(field[w, h]);
                                        Vector3 v = new Vector3(1 + w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                                        field[w, h] = Instantiate(blockArray[1]) as GameObject;
                                        field[w, h].transform.position = v;
                                        field[w, h].transform.parent = transform;
                                        idents[w, h] = 1;
                                        h--;
                                        w2 = 3;
                                        h2 = 2;
                                    }
                                    else
                                    {
                                        DestroyImmediate(field[w + w2, h + h2]);
                                        field[w + w2, h + h2] = null;
                                        idents[w + w2, h + h2] = -1;
                                    }
                                }

                            }
                        }
                    }
                }
                else if (idents[w, h] == 3)
                {
                    if (h == (height - 1)||w+3>width)
                    {
                        DestroyImmediate(field[w, h]);
                        Vector3 v = new Vector3(1 + w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                        field[w, h] = Instantiate(blockArray[2]) as GameObject;
                        field[w, h].transform.position = v;
                        field[w, h].transform.parent = transform;
                        idents[w, h] = 2;
                        h--;
                    }
                    else
                    {
                        for (int w2 = 0; w2 < 3; w2++)
                        {
                            for (int h2 = 0; h2 < 2; h2++)
                            {
                                if(w2+h2!=0)
                                {
                                    if(idents[w + w2, h + h2]==-1)
                                    {
                                        DestroyImmediate(field[w, h]);
                                        Vector3 v = new Vector3(1 + w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                                        field[w, h] = Instantiate(blockArray[2]) as GameObject;
                                        field[w, h].transform.position = v;
                                        field[w, h].transform.parent = transform;
                                        idents[w, h] = 2;
                                        h--;
                                        w2 = 3;
                                        h2 = 2;
                                    }
                                    else
                                    {
                                            DestroyImmediate(field[w + w2, h + h2]);
                                            field[w + w2, h + h2] = null;
                                            idents[w + w2, h + h2] = -1;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (idents[w, h] == 4)
                {
                    if (h +3> height || w + 3 > width)
                    {
                        DestroyImmediate(field[w, h]);
                        Vector3 v = new Vector3(1.5f + w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                        field[w, h] = Instantiate(blockArray[3]) as GameObject;
                        field[w, h].transform.position = v;
                        field[w, h].transform.parent = transform;
                        idents[w, h] = 3;
                        h--;
                    }
                    else
                    {
                        for (int w2 = 0; w2 < 3; w2++)
                        {
                            for (int h2 = 0; h2 < 3; h2++)
                            {
                                if (w2 + h2 != 0)
                                {
                                    if (idents[w + w2, h + h2] == -1)
                                    {
                                        DestroyImmediate(field[w, h]);
                                        Vector3 v = new Vector3(1.5f + w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                                        field[w, h] = Instantiate(blockArray[3]) as GameObject;
                                        field[w, h].transform.position = v;
                                        field[w, h].transform.parent = transform;
                                        idents[w, h] = 3;
                                        h--;
                                        w2 = 3;
                                        h2 = 3;
                                    }
                                    else
                                    {
                                        DestroyImmediate(field[w + w2, h + h2]);
                                        field[w + w2, h + h2] = null;
                                        idents[w + w2, h + h2] = -1;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (idents[w, h] == 5)
                {
                    if (h + 3 > height || w + 4 > width)
                    {
                        DestroyImmediate(field[w, h]);
                        Vector3 v = new Vector3(1.5f + w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                        field[w, h] = Instantiate(blockArray[4]) as GameObject;
                        field[w, h].transform.position = v;
                        field[w, h].transform.parent = transform;
                        idents[w, h] = 4;
                        h--;
                    }
                    else
                    {
                        for (int w2 = 0; w2 < 4; w2++)
                        {
                            for (int h2 = 0; h2 < 3; h2++)
                            {
                                if (w2 + h2 != 0)
                                {
                                    if (idents[w + w2, h + h2] == -1)
                                    {
                                        DestroyImmediate(field[w, h]);
                                        Vector3 v = new Vector3(1.5f + w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                                        field[w, h] = Instantiate(blockArray[4]) as GameObject;
                                        field[w, h].transform.position = v;
                                        field[w, h].transform.parent = transform;
                                        idents[w, h] = 4;
                                        h--;
                                        w2 = 4;
                                        h2 = 3;
                                    }
                                    else
                                    {
                                        DestroyImmediate(field[w + w2, h + h2]);
                                        field[w + w2, h + h2] = null;
                                        idents[w + w2, h + h2] = -1;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (idents[w, h] == 6)
                {
                    if (h + 2 > height || w + 16 > width)
                    {
                        DestroyImmediate(field[w, h]);
                        Vector3 v = new Vector3(2f + w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                        field[w, h] = Instantiate(blockArray[5]) as GameObject;
                        field[w, h].transform.position = v;
                        field[w, h].transform.parent = transform;
                        idents[w, h] = 5;
                        h--;
                    }
                    else
                    {
                        for (int w2 = 0; w2 < 16; w2++)
                        {
                            for (int h2 = 0; h2 < 2; h2++)
                            {
                                if (w2 + h2 != 0)
                                {
                                    if (idents[w + w2, h + h2] == -1)
                                    {
                                        DestroyImmediate(field[w, h]);
                                        Vector3 v = new Vector3(2f + w + transform.position.x - width / 2f, h + transform.position.y - height / 2f, 0);
                                        field[w, h] = Instantiate(blockArray[5]) as GameObject;
                                        field[w, h].transform.position = v;
                                        field[w, h].transform.parent = transform;
                                        idents[w, h] = 5;
                                        h--;
                                        w2 = 16;
                                        h2 = 2;
                                    }
                                    else
                                    {
                                        DestroyImmediate(field[w + w2, h + h2]);
                                        field[w + w2, h + h2] = null;
                                        idents[w + w2, h + h2] = -1;
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }
    }
}
