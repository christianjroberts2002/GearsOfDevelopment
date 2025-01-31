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

    [SerializeField] GameObject preVizGO;
    [SerializeField] GameObject finalGO;

    [SerializeField] GameObject testGO;
    [SerializeField] GridSystem gridSystem;
    /*    [SerializeField] GridPosition.TilePos startTile;
        [SerializeField] GridPosition.TilePos endTile;*/

    GridPosition.TilePos startTile;
    GridPosition.TilePos endTile;

    public HouseBuilding houseBuildingState;

    private bool preVizWallDown = false;
    private bool startIsZero = false;



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
            
            DestroyPreVizBlocks(preVizWall);
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
            if (raycastHit.transform != null)
            {
                //Our custom method. 
                //Debug.Log(raycastHit.transform.gameObject.name);
                GridPosition hitGridPosition = raycastHit.transform.GetComponent<GridPosition>();
                GridPosition.TilePos tilePose = hitGridPosition.GetTilePos();

                if ((startTile.X == 0 && startTile.Y == 0) || (endTile.X == 0 && endTile.Y == 0))
                {
                    startIsZero = true;
                }

                if (endTile.X != null && endTile.X != tilePose.X && endTile.Y != null && endTile.Y != tilePose.X)
                {
                    DestroyPreVizBlocks(preVizWall);
                    preVizWallDown = false;
                }

                endTile = tilePose;
                
                
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

                CreateWalls(GetSelectedTiles(startTile, endTile));
                //Vector3 newGOSpawn = new Vector3(raycastHit.transform.position.x, raycastHit.transform.position.y + 1.5f, raycastHit.transform.position.z);
                //Instantiate(testGO, newGOSpawn, Quaternion.identity);
            }
        }

        //Find all tiles in the array
        //Find all edge tiles
        // place GO in the edge tiles
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

        preVizWall = new List<GameObject>();
        

        if(selectedTiles == null)
        {
            return;
        }

        int arrayLengthX = selectedTiles.GetLength(0);
        int arrayLengthZ = selectedTiles.GetLength(1);

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
                    Debug.Log("zero is end or start");
                    continue; // Skip invalid tiles
                    
                }
                Vector3 newGOSpawn = new Vector3(selectedTiles[i,j].Y * 1.5f, 1.5f, selectedTiles[i,j].X * 1.5f);
                GameObject newBlock = Instantiate(testGO, newGOSpawn, Quaternion.identity);
                
                preVizWall.Add(newBlock);
                preVizWallDown = true;
                startIsZero = false;

            }
        }
    }

    private void CreateRoof(GridPosition.TilePos[,] selectedTiles)
    {
        int arrayLengthX = selectedTiles.GetLength(0) - 1;
        int arrayLengthZ = selectedTiles.GetLength(1) - 1;



        for (int i = 0; i <= arrayLengthX; i++)
        {
            for (int j = 0; j <= arrayLengthZ; j++)
            {
                Vector3 newGOSpawn = new Vector3(selectedTiles[i, j].X * 1.5f, 1.5f, selectedTiles[i, j].Y * 1.5f);
                Instantiate(testGO, newGOSpawn, Quaternion.identity);

            }
        }
    }

    private void DestroyPreVizBlocks(List<GameObject> goList)
    {
        foreach (GameObject go in goList)
        {;
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
