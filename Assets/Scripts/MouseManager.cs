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




    private void Start()
    {
        gridSystem = GetComponent<GridSystem>();
        // Lock the mouse Pos
        // Cursor.lockState = CursorLockMode.Locked;
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
                //if(preVizWall.Count > 0)
                    //DestroyPreVizBlocks(preVizWall);
                //PlaceBuilding();
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
                //Our custom method. 
                GridPosition hitGridPosition = raycastHit.transform.GetComponent<GridPosition>();
                GridPosition.TilePos tilePose = hitGridPosition.GetTilePos();
                startTile = tilePose;
                Debug.Log(tilePose.X + "," + tilePose.Y);

                //Testing

                //Vector3 newGOSpawn = new Vector3(raycastHit.transform.position.x, raycastHit.transform.position.y + 1.5f, raycastHit.transform.position.z);
                //Instantiate(testGO, newGOSpawn, Quaternion.identity);
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
                //Our custom method. 
                //Debug.Log(raycastHit.transform.gameObject.name);
                GridPosition hitGridPosition = raycastHit.transform.GetComponent<GridPosition>();
                GridPosition.TilePos tilePose = hitGridPosition.GetTilePos();
                endTile = tilePose;
                //Debug.Log(tilePose.X + "," + tilePose.Y);
                if (endTile.X == startTile.X || endTile.Y == startTile.Y)
                {
                    return;
                }

                //Testing
                GetSelectedTiles(startTile, endTile);
                CreateWalls(GetSelectedTiles(startTile, endTile));
                //Vector3 newGOSpawn = new Vector3(raycastHit.transform.position.x, raycastHit.transform.position.y + 1.5f, raycastHit.transform.position.z);
                //Instantiate(testGO, newGOSpawn, Quaternion.identity);
            }
        }


    }

    private void PreVizBuilding()
    {/*
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    //Our custom method. 
                    //Debug.Log(raycastHit.transform.gameObject.name);
                    GridPosition hitGridPosition = raycastHit.transform.GetComponent<GridPosition>();
                    GridPosition.TilePos tilePose = hitGridPosition.GetTilePos();
                    startTile = tilePose;
                    //Debug.Log(tilePose.X + "," + tilePose.Y);

                    //Testing

                    //Vector3 newGOSpawn = new Vector3(raycastHit.transform.position.x, raycastHit.transform.position.y + 1.5f, raycastHit.transform.position.z);
                    //Instantiate(testGO, newGOSpawn, Quaternion.identity);
                }
            }
        }*/

        /*if (Input.GetMouseButtonUp(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    //Our custom method. 
                    //Debug.Log(raycastHit.transform.gameObject.name);
                    GridPosition hitGridPosition = raycastHit.transform.GetComponent<GridPosition>();
                    GridPosition.TilePos tilePose = hitGridPosition.GetTilePos();
                    endTile = tilePose;
                    //Debug.Log(tilePose.X + "," + tilePose.Y);
                    if (endTile.X == startTile.X || endTile.Y == startTile.Y)
                    {
                        return;
                    }

                    //Testing
                    GetSelectedTiles(startTile, endTile);
                    CreateWalls(GetSelectedTiles(startTile, endTile));
                    //Vector3 newGOSpawn = new Vector3(raycastHit.transform.position.x, raycastHit.transform.position.y + 1.5f, raycastHit.transform.position.z);
                    //Instantiate(testGO, newGOSpawn, Quaternion.identity);
                }
            }
        }*/

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
       /* if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    //Our custom method. 
                    //Debug.Log(raycastHit.transform.gameObject.name);
                    GridPosition hitGridPosition = raycastHit.transform.GetComponent<GridPosition>();
                    GridPosition.TilePos tilePose = hitGridPosition.GetTilePos();
                    startTile = tilePose;
                    //Debug.Log(tilePose.X + "," + tilePose.Y);

                    //Testing

                    //Vector3 newGOSpawn = new Vector3(raycastHit.transform.position.x, raycastHit.transform.position.y + 1.5f, raycastHit.transform.position.z);
                    //Instantiate(testGO, newGOSpawn, Quaternion.identity);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    //Our custom method. 
                    //Debug.Log(raycastHit.transform.gameObject.name);
                    GridPosition hitGridPosition = raycastHit.transform.GetComponent<GridPosition>();
                    GridPosition.TilePos tilePose = hitGridPosition.GetTilePos();
                    endTile = tilePose;*/
                    //Debug.Log(tilePose.X + "," + tilePose.Y);
                    if(endTile.X == startTile.X ||  endTile.Y == startTile.Y)
                    {
                        return;
                    }

                    //Testing
                    GetSelectedTiles(startTile, endTile);
                    CreateWalls(GetSelectedTiles(startTile, endTile));

                    houseBuildingState = HouseBuilding.None;
                    //Vector3 newGOSpawn = new Vector3(raycastHit.transform.position.x, raycastHit.transform.position.y + 1.5f, raycastHit.transform.position.z);
                    //Instantiate(testGO, newGOSpawn, Quaternion.identity);
                /*}
            }
        }*/

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


/*    private GridPosition.TilePos[,] GetSelectedTiles(GridPosition.TilePos startTile, GridPosition.TilePos endTile)
    {
        int arrayLengthX = Convert.ToInt32(MathF.Abs(startTile.X - endTile.X) + 1);
        int arrayLengthZ = Convert.ToInt32(MathF.Abs(startTile.Y - endTile.Y) + 1);


        GridPosition.TilePos[,] selectedTiles = new GridPosition.TilePos[arrayLengthX,arrayLengthZ];

        
         

        

        //selectedTiles[0,0] = startTile;
        Debug.Log(arrayLengthX + "," + arrayLengthZ);
        //selectedTiles[arrayLengthX, arrayLengthZ] = endTile;
        int startx = Convert.ToInt32(startTile.X);
        int starty = Convert.ToInt32(startTile.Y);
        
        Debug.Log(startx + "," + starty);



        *//*if (startTile.X < endTile.X)
        {
            startx = Convert.ToInt32(startTile.X);
        }
        else
        {
            startx = Convert.ToInt32(endTile.X);
        }

        if (startTile.Y < endTile.Y)
        {
            starty = Convert.ToInt32(startTile.X);
        }
        else
        {
            starty = Convert.ToInt32(endTile.X);
        }*/

        /*if (startTile.X < endTile.X && startTile.Y < endTile.Y)
        {
            startx = Convert.ToInt32(startTile.X);
            starty = Convert.ToInt32(startTile.Y);
        }
        if (startTile.X > endTile.X && startTile.Y > endTile.Y)
        {
            startx = Convert.ToInt32(endTile.X);
            starty = Convert.ToInt32(endTile.Y);
        }

        if (startTile.X < endTile.X && startTile.Y > endTile.Y)
        {
            startx = Convert.ToInt32(startTile.X);
            starty = Convert.ToInt32(endTile.Y);


        }
        if (startTile.X > endTile.X && startTile.Y < endTile.Y)
        {
            startx = Convert.ToInt32(endTile.X);
            starty = Convert.ToInt32(startTile.Y);
        }*//*

        int xpos = startx;
        int ypos = starty;
        
        Debug.Log(xpos + "," + ypos);
        Debug.Log(endTile.X + "," + endTile.Y);

        if (Mathf.Abs(startTile.X - endTile.X) <= 1 || Mathf.Abs(startTile.Y - endTile.Y) <= 1)
        {
            Debug.Log("this is smaller than a 3X3s");
        }


        for (int i = 0; i < arrayLengthZ; i++)
        {
            Debug.Log(gridSystem.GetTileInArray(xpos, ypos));
            selectedTiles[i,0] = gridSystem.GetTileInArray(xpos, ypos);
            selectedTiles[i, arrayLengthX - 1] = gridSystem.GetTileInArray(xpos + arrayLengthX - 1, ypos);
            ypos++;

        }
        ypos = starty;
        xpos++;
        for (int i = 1; i < arrayLengthX - 1; i++)
        {
            
            //Debug.Log(gridSystem.GetTileInArray(xpos, ypos).X + ","  + gridSystem.GetTileInArray(xpos, ypos).Y);
            //Debug.Log(gridSystem.GetTileInArray(xpos, ypos).X);
            Debug.Log(i);
            selectedTiles[0,i] = gridSystem.GetTileInArray(xpos, ypos);
            selectedTiles[arrayLengthZ - 1, i] = gridSystem.GetTileInArray(xpos, ypos + arrayLengthZ - 1);
            xpos++;


        }
        xpos = starty;
        

        for (int i = 0; i <= arrayLengthX; i++)
        {
            for (int j = 0; j <= arrayLengthZ; j++)
            {
                //Debug.Log(selectedTiles[i, j].X + "." + selectedTiles[i, j].Y);

            }
        }



        return selectedTiles;
        
        

        



    }*/

    private void CreateWalls(GridPosition.TilePos[,] selectedTiles)
    {
        

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
        


        if(preVizWallDown)
        {
            return;
        }
        for (int i = 0; i < arrayLengthX; i++)
        {
            for (int j = 0; j < arrayLengthZ; j++)
            {
                Vector3 newGOSpawn = new Vector3(selectedTiles[i,j].Y * 1.5f, 1.5f, selectedTiles[i,j].X * 1.5f);
                GameObject newBlock = Instantiate(testGO, newGOSpawn, Quaternion.identity);
                Debug.Log(newBlock.transform.name);
                preVizWall.Add(newBlock);
                preVizWallDown = true;

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
        {
            Debug.Log(go);
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
