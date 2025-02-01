using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class MouseManager : MonoBehaviour
{
    public LayerMask layerMask;

    private Camera Camera;

    [SerializeField] List<GameObject> preVizWall;
    [SerializeField] List<GameObject> preVizRoof;

    [SerializeField] GameObject preVizGO;
    [SerializeField] GameObject finalGO;

    [SerializeField] GameObject testGO;
    [SerializeField] GameObject testCornerGO;
    [SerializeField] GameObject roofGO;
    [SerializeField] GameObject roofCornerGO;
    [SerializeField] GridSystem gridSystem;

    private GridPosition.TilePos[,] roofTiles;
    /*    [SerializeField] GridPosition.TilePos startTile;
        [SerializeField] GridPosition.TilePos endTile;*/

    GridPosition.TilePos startTile;
    GridPosition.TilePos endTile;

    public HouseBuilding houseBuildingState;

    private bool preVizWallDown = false;
    private bool preVizRoofDown = false;
    private bool startIsZero = false;

    private int roofLevel;



    private void Start()
    {
        gridSystem = GetComponent<GridSystem>();
        houseBuildingState = HouseBuilding.None;

        
    }
    void Update()
    {
        switch(houseBuildingState)
        {
            case HouseBuilding.MouseDown:
                ClickSetStartTile();
                testGO = preVizGO;
                break;
            case HouseBuilding.MouseDragged:
                PreVizBuilding();
                break;
            case HouseBuilding.MouseUp:
                testGO = finalGO;
                break;
            case HouseBuilding.None:
                break;
        }

        if (Input.GetMouseButtonDown(0))
        {
            houseBuildingState = HouseBuilding.MouseDown;
            
        }
        if (Input.GetMouseButtonUp(0) && houseBuildingState == HouseBuilding.MouseDragged)
        {
            houseBuildingState = HouseBuilding.MouseUp;
            preVizWallDown = false;
            preVizRoofDown= false;
            
            DestroyPreVizBlocks(preVizWall);
            DestroyPreVizBlocks(preVizRoof);
        }

        if (houseBuildingState == HouseBuilding.MouseDown)
        {

        }



    }

    private void ClickSetStartTile()
    {
        
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit, 1000f, layerMask))
        {
            if (raycastHit.transform != null)
            {
                GridPosition hitGridPosition = raycastHit.transform.GetComponent<GridPosition>();
                GridPosition.TilePos tilePose = hitGridPosition.GetTilePos();
                startTile = tilePose;
                Debug.Log(tilePose.X + "," + tilePose.Y);
                

                houseBuildingState = HouseBuilding.MouseDragged;
            }
        }
    }

    private void ClickSetEndTile(GridPosition.TilePos endTile)
    {
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit, 100f))
        {
            if (raycastHit.transform != null)
            {
                GridPosition hitGridPosition = raycastHit.transform.GetComponent<GridPosition>();
                GridPosition.TilePos tilePose = hitGridPosition.GetTilePos();
                endTile = tilePose;
                if (endTile.X == startTile.X || endTile.Y == startTile.Y)
                {
                    return;
                }

                //Testing
                GetSelectedTiles(startTile, endTile);
                CreateWalls(GetSelectedTiles(startTile, endTile));

            }
        }


    }

    private void PreVizBuilding()
    {
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit, 1000f, layerMask))
        {
            float lastEndX = -1;
            float lastEndZ = -1;

            if (raycastHit.transform != null)
            {
                //Our custom method. 
                //Debug.Log(raycastHit.transform.gameObject.name);
                GridPosition hitGridPosition = raycastHit.transform.GetComponent<GridPosition>();
                GridPosition.TilePos tilePose = hitGridPosition.GetTilePos();
                endTile = tilePose;

                

                if (lastEndX != endTile.X && lastEndZ != endTile.Y)
                {
                    DestroyPreVizBlocks(preVizWall);
                    DestroyPreVizBlocks(preVizRoof);
                    preVizWallDown = false;
                    preVizRoofDown = false;
                }

                
                lastEndX = endTile.X;
                lastEndZ = endTile.Y;

                if ((startTile.X == 0 && startTile.Y == 0) || (endTile.X == 0 && endTile.Y == 0))
                {
                    startIsZero = true;
                }


                //Debug.Log(tilePose.X + "," + tilePose.Y);
                if (endTile.X == startTile.X && endTile.Y == startTile.Y)
                {
                    return;
                }

                if (Mathf.Abs(endTile.X - startTile.X) <= 1 || Mathf.Abs(endTile.Y - startTile.Y) <=1)
                {
                    return;
                }
                //Testing

                NormalizeTileBounds();

                CreateWalls(GetSelectedTiles(startTile, endTile));
                float roofHeight = 0;
                

                roofHeight = (Mathf.Min(Mathf.Abs(endTile.X - startTile.X), Mathf.Abs(endTile.Y - startTile.Y)));
                roofHeight = Mathf.CeilToInt(roofHeight / 2);


                startTile = new GridPosition.TilePos(startTile.X, startTile.Y);
                endTile = new GridPosition.TilePos(endTile.X, endTile.Y);

                preVizRoof = new List<GameObject>();
                roofLevel = 0;
                for (int i = 0; i <= roofHeight; i++)
                {
                    GridPosition.TilePos roofstart = new GridPosition.TilePos(startTile.X + i, startTile.Y + i);
                    GridPosition.TilePos roofend = new GridPosition.TilePos(endTile.X - i, endTile.Y - i);
                    preVizRoofDown = false;
                    roofLevel++;
                    PreVizRoof(GetSelectedTiles(roofstart, roofend));
                }
                preVizRoofDown = true;


                
                //Vector3 newGOSpawn = new Vector3(raycastHit.transform.position.x, raycastHit.transform.position.y + 1.5f, raycastHit.transform.position.z);
                //Instantiate(testGO, newGOSpawn, Quaternion.identity);
            }
        }

        //Find all tiles in the array
        //Find all edge tiles
        // place GO in the edge tiles
    }

    private void NormalizeTileBounds()
    {
        int startX = (int)Mathf.Min(startTile.X, endTile.X);
        int endX = (int)Mathf.Max(startTile.X, endTile.X);
        int startY = (int)Mathf.Min(startTile.Y, endTile.Y);
        int endY = (int)Mathf.Max(startTile.Y, endTile.Y);

        startTile = new GridPosition.TilePos(startX, startY);
        endTile = new GridPosition.TilePos(endX, endY);
    }

    private void PlaceBuilding()
    {
                    if(endTile.X == startTile.X ||  endTile.Y == startTile.Y)
                    {
                        return;
                    }

                    //Testing
                    GetSelectedTiles(startTile, endTile);
                    CreateWalls(GetSelectedTiles(startTile, endTile));

                    houseBuildingState = HouseBuilding.None;
                    

        //Find all tiles in the array
        //Find all edge tiles
        // place GO in the edge tiles
    }

    private GridPosition.TilePos[,] GetSelectedTiles(GridPosition.TilePos startTile, GridPosition.TilePos endTile)
    {
        int startx = Convert.ToInt32(Mathf.Min(startTile.X, endTile.X));
        int endx = Convert.ToInt32(Mathf.Max(startTile.X, endTile.X));
        int starty = Convert.ToInt32(Mathf.Min(startTile.Y, endTile.Y));
        int endy = Convert.ToInt32(Mathf.Max(startTile.Y, endTile.Y));

        //Debug.Log(startx +"," + starty);

        int arrayLengthX = endx - startx + 1;
        int arrayLengthZ = endy - starty + 1;

        int xpos = startx;
        int ypos = starty;
;

        // First, we need to clear out the array before filling it
        GridPosition.TilePos[,] selectedTiles = new GridPosition.TilePos[arrayLengthX, arrayLengthZ];

        // Top and Bottom Edges (Y stays constant)
        for (int i = 0; i < arrayLengthX; i++)
        {
            // Top edge: (i, 0)
            selectedTiles[i, 0] = gridSystem.GetTileInArray(startx + i, starty);  // starty is constant for top
                                                                                  // Bottom edge: (i, arrayLengthZ - 1)
            selectedTiles[i, arrayLengthZ - 1] = gridSystem.GetTileInArray(startx + i, starty + arrayLengthZ - 1); // starty + arrayLengthZ - 1 for the bottom
        }

        // Left and Right Edges (X stays constant)
        for (int i = 1; i < arrayLengthZ - 1; i++) // Avoid duplicate corners
        {
            // Left edge: (0, i)
            selectedTiles[0, i] = gridSystem.GetTileInArray(startx, starty + i);  // startx is constant for left
                                                                                  // Right edge: (arrayLengthX - 1, i)
            selectedTiles[arrayLengthX - 1, i] = gridSystem.GetTileInArray(startx + arrayLengthX - 1, starty + i);  // startx + arrayLengthX - 1 for the right
        }

        

        return selectedTiles;
    }

    private void CreateWalls(GridPosition.TilePos[,] selectedTiles)
    {
        if (preVizWallDown)
        {
            return;
        }

        
        

        if(selectedTiles == null)
        {
            return;
        }

        preVizWall = new List<GameObject>();
        
        int arrayLengthX = selectedTiles.GetLength(0);
        int arrayLengthZ = selectedTiles.GetLength(1);

        int roofArrayX = arrayLengthX - 1;
        int roofArrayZ = arrayLengthZ - 1;

        roofTiles = new GridPosition.TilePos[roofArrayX, roofArrayZ];
        /*if(arrayLengthX !> 2 ||  arrayLengthZ !> 2)
        {
            return;
        }*/



        for (int i = 0; i < arrayLengthX; i++)
        {
            for (int j = 0; j < arrayLengthZ; j++)
            {
                
                if (selectedTiles[i, j].X == 0 && selectedTiles[i, j].Y == 0 && !startIsZero )
                {
                    //Debug.Log("zero is end or start");
                    continue; // Skip invalid tiles
                    
                }
                // Place corner blocks with rotation
                if ((i == 0 && j == 0)) // Top-left corner
                {
                    Quaternion rotation = Quaternion.Euler(0, 180, 0); // No rotation
                    Vector3 newGOCornerSpawn = new Vector3(selectedTiles[i, j].Y * 1.5f, 1.5f, selectedTiles[i, j].X * 1.5f);
                    GameObject newCornerBlock = Instantiate(testCornerGO, newGOCornerSpawn, rotation);
                    preVizWall.Add(newCornerBlock);
                    continue;
                }
                else if ((i == 0 && j == arrayLengthZ - 1)) // Top-right corner
                {
                    Quaternion rotation = Quaternion.Euler(0, 90, 0); // Rotate 90 degrees around Y-axis
                    Vector3 newGOCornerSpawn = new Vector3(selectedTiles[i, j].Y * 1.5f, 1.5f, selectedTiles[i, j].X * 1.5f);
                    GameObject newCornerBlock = Instantiate(testCornerGO, newGOCornerSpawn, rotation);
                    preVizWall.Add(newCornerBlock);
                    continue;
                }
                else if ((i == arrayLengthX - 1 && j == 0)) // Bottom-left corner
                {
                    Quaternion rotation = Quaternion.Euler(0, -90, 0); // Rotate -90 degrees around Y-axis
                    Vector3 newGOCornerSpawn = new Vector3(selectedTiles[i, j].Y * 1.5f, 1.5f, selectedTiles[i, j].X * 1.5f);
                    GameObject newCornerBlock = Instantiate(testCornerGO, newGOCornerSpawn, rotation);
                    preVizWall.Add(newCornerBlock);
                    continue;
                }
                else if ((i == arrayLengthX - 1 && j == arrayLengthZ - 1)) // Bottom-right corner
                {
                    Quaternion rotation = Quaternion.Euler(0, 0, 0); // Rotate 180 degrees around Y-axis
                    Vector3 newGOCornerSpawn = new Vector3(selectedTiles[i, j].Y * 1.5f, 1.5f, selectedTiles[i, j].X * 1.5f);
                    GameObject newCornerBlock = Instantiate(testCornerGO, newGOCornerSpawn, rotation);
                    preVizWall.Add(newCornerBlock);
                    continue;
                }
                if(i == arrayLengthX -1 || i + arrayLengthZ -1 == arrayLengthZ - 1)
                {
                    float tileOffsetX = -.5f;
                    if(i == arrayLengthX -1)
                    {
                        tileOffsetX = .5f;
                    }

                    Quaternion rotation = Quaternion.Euler(0, 90, 0); // Rotate 180 degrees around Y-axis
                    Vector3 newGOSpawn = new Vector3(selectedTiles[i, j].Y * 1.5f, 1.5f, (selectedTiles[i, j].X * 1.5f) + tileOffsetX);
                    GameObject newblock = Instantiate(testGO, newGOSpawn, rotation);
                    preVizWall.Add(newblock);
                    preVizWallDown = true;
                    startIsZero = false;
                    continue;
                }
                else if (j == arrayLengthZ - 1 || j + arrayLengthX - 1 == arrayLengthX - 1)
                {
                    float tileOffsetY = -.5f;
                    if (j == arrayLengthZ - 1)
                    {
                        tileOffsetY = .5f;
                    }
                    Quaternion rotation = Quaternion.Euler(0,0, 0); // Rotate 180 degrees around Y-axis
                    Vector3 newGOSpawn = new Vector3((selectedTiles[i, j].Y * 1.5f) +tileOffsetY, 1.5f, selectedTiles[i, j].X * 1.5f);
                    GameObject newblock = Instantiate(testGO, newGOSpawn, rotation);
                    preVizWall.Add(newblock);
                    preVizWallDown = true;
                    startIsZero = false;
                    continue;
                }

                /*for (i = 0  ; i < arrayLengthX - 2; i++)
                {
                    for(j = 0  ; j < arrayLengthZ - 2; j++)
                    {
                        roofTiles[i,j] = selectedTiles[i,j];
                    }
                }*/
                

            }
        }
        //PreVizRoof(roofTiles);

    }

    private void PreVizRoof(GridPosition.TilePos[,] selectedTiles)
    {
        if (preVizRoofDown)
        {
            return;
        }




        if (selectedTiles == null)
        {
            Debug.Log("Reutrn lisst is null");
            return;
        }

        

        int arrayLengthX = selectedTiles.GetLength(0);
        int arrayLengthZ = selectedTiles.GetLength(1);

        int roofArrayX = arrayLengthX - 1;
        int roofArrayZ = arrayLengthZ - 1;


        /*if(arrayLengthX !> 2 ||  arrayLengthZ !> 2)
        {
            return;
        }*/

        float roofIntialOffset = 1.5f;
        float roofHeight = 1.5f;


        for (int i = 0; i < arrayLengthX; i++)
        {
            for (int j = 0; j < arrayLengthZ; j++)
            {

                /*if (selectedTiles[i, j].X == 0 && selectedTiles[i, j].Y == 0 && !startIsZero)
                {
                    //Debug.Log("zero is end or start");
                    continue; // Skip invalid tiles

                }*/
                // Place corner blocks with rotation
                if ((i == 0 && j == 0)) // Top-left corner
                {
                    Quaternion rotation = Quaternion.Euler(0, 180, 0); // No rotation
                    Vector3 newGOCornerSpawn = new Vector3(selectedTiles[i, j].Y * 1.5f, 1.5f + (roofIntialOffset * roofLevel), selectedTiles[i, j].X * 1.5f);
                    GameObject newCornerBlock = Instantiate(roofCornerGO, newGOCornerSpawn, rotation);
                    preVizRoof.Add(newCornerBlock);
                    continue;
                }
                else if ((i == 0 && j == arrayLengthZ - 1)) // Top-right corner
                {
                    Quaternion rotation = Quaternion.Euler(0, 90, 0); // Rotate 90 degrees around Y-axis
                    Vector3 newGOCornerSpawn = new Vector3(selectedTiles[i, j].Y * 1.5f, 1.5f + (roofIntialOffset * roofLevel), selectedTiles[i, j].X * 1.5f);
                    GameObject newCornerBlock = Instantiate(roofCornerGO, newGOCornerSpawn, rotation);
                    preVizRoof.Add(newCornerBlock);
                    continue;
                }
                else if ((i == arrayLengthX - 1 && j == 0)) // Bottom-left corner
                {
                    Quaternion rotation = Quaternion.Euler(0, -90, 0); // Rotate -90 degrees around Y-axis
                    Vector3 newGOCornerSpawn = new Vector3(selectedTiles[i, j].Y * 1.5f, 1.5f + (roofIntialOffset * roofLevel), selectedTiles[i, j].X * 1.5f);
                    GameObject newCornerBlock = Instantiate(roofCornerGO, newGOCornerSpawn, rotation);
                    preVizRoof.Add(newCornerBlock);
                    continue;
                }
                else if ((i == arrayLengthX - 1 && j == arrayLengthZ - 1)) // Bottom-right corner
                {
                    Quaternion rotation = Quaternion.Euler(0, 0, 0); // Rotate 180 degrees around Y-axis
                    Vector3 newGOCornerSpawn = new Vector3(selectedTiles[i, j].Y * 1.5f, 1.5f + (roofIntialOffset * roofLevel), selectedTiles[i, j].X * 1.5f);
                    GameObject newCornerBlock = Instantiate(roofCornerGO, newGOCornerSpawn, rotation);
                    preVizRoof.Add(newCornerBlock);
                    continue;
                }
                if (i == arrayLengthX - 1 || i + arrayLengthZ - 1 == arrayLengthZ - 1)
                {
                    float tileOffsetX = -.0f;
                    if (i == arrayLengthX - 1)
                    {
                        tileOffsetX = .0f;
                    }

                    Quaternion rotation = Quaternion.Euler(0, 90, 0); // Rotate 180 degrees around Y-axis
                    Vector3 newGOSpawn = new Vector3(selectedTiles[i, j].Y * 1.5f, 1.5f + (roofIntialOffset * roofLevel), (selectedTiles[i, j].X * 1.5f) + tileOffsetX);
                    GameObject newblock = Instantiate(roofGO, newGOSpawn, rotation);
                    preVizRoof.Add(newblock);
                    //preVizRoofDown = true;
                    startIsZero = false;
                    continue;
                }
                else if (j == arrayLengthZ - 1 || j + arrayLengthX - 1 == arrayLengthX - 1)
                {
                    float tileOffsetY = -.0f;
                    if (j == arrayLengthZ - 1)
                    {
                        tileOffsetY = .0f;
                    }
                    Quaternion rotation = Quaternion.Euler(0, 0, 0); // Rotate 180 degrees around Y-axis
                    Vector3 newGOSpawn = new Vector3((selectedTiles[i, j].Y * 1.5f) + tileOffsetY, 1.5f + (roofIntialOffset * roofLevel), selectedTiles[i, j].X * 1.5f);
                    GameObject newblock = Instantiate(roofGO, newGOSpawn, rotation);
                    preVizRoof.Add(newblock);
                    //preVizRoofDown = true;
                    startIsZero = false;
                    continue;
                }

                /*for (i = 0  ; i < arrayLengthX - 2; i++)
                {
                    for(j = 0  ; j < arrayLengthZ - 2; j++)
                    {
                        roofTiles[i,j] = selectedTiles[i,j];
                    }
                }*/


            }
        }
        //PreVizRoof(roofTiles);
    }

    private void DestroyPreVizBlocks(List<GameObject> goList)
    {
        foreach (GameObject go in goList)
        {
            Destroy(go);
        }
            
    }
    
}

public enum HouseBuilding
{
    None,
    MouseDown,
    MouseDragged,
    MouseUp,
}
