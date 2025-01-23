using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class MouseManager : MonoBehaviour
{
    private Camera Camera;

    [SerializeField] GameObject preVizGO;
    [SerializeField] GameObject finalGO;

    [SerializeField] GameObject testGO;
    [SerializeField] GridSystem gridSystem;
    /*    [SerializeField] GridPosition.TilePos startTile;
        [SerializeField] GridPosition.TilePos endTile;*/

    GridPosition.TilePos startTile;
    GridPosition.TilePos endTile;

    public HouseBuilding houseBuildingState;
    



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
                PlaceBuilding();
                break;
            case HouseBuilding.None:
                break;
        }

        if (Input.GetMouseButtonDown(0))
        {
            houseBuildingState = HouseBuilding.MouseDown;
            
        }
        if (Input.GetMouseButtonUp(0))
        {
            houseBuildingState = HouseBuilding.MouseUp;
        }

        if (houseBuildingState == HouseBuilding.MouseDown)
        {

        }



    }

    private void ClickSetStartTile()
    {
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit, 100f))
        {
            if (raycastHit.transform != null)
            {
                //Our custom method. 
                Debug.Log(raycastHit.transform.gameObject.name);
                GridPosition hitGridPosition = raycastHit.transform.GetComponent<GridPosition>();
                GridPosition.TilePos tilePose = hitGridPosition.GetTilePos();
                startTile = tilePose;
                //Debug.Log(tilePose.X + "," + tilePose.Y);

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
        if (Physics.Raycast(ray, out raycastHit, 100f))
        {
            if (raycastHit.transform != null)
            {
                //Our custom method. 
                //Debug.Log(raycastHit.transform.gameObject.name);
                GridPosition hitGridPosition = raycastHit.transform.GetComponent<GridPosition>();
                GridPosition.TilePos tilePose = hitGridPosition.GetTilePos();

                if(endTile.X != startTile.X && endTile.Y != startTile.Y)
                {
                    endTile = tilePose;
                }
                else
                {
                    return;
                }
                
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
        GridPosition.TilePos[,] selectedTiles = new GridPosition.TilePos[Convert.ToInt32(MathF.Abs(startTile.X - endTile.X) + 1),Convert.ToInt32(MathF.Abs(startTile.Y - endTile.Y) + 1)];

        int arrayLengthX = selectedTiles.GetLength(0) - 1;
        int arrayLengthZ = selectedTiles.GetLength(0) - 1;

        

        //selectedTiles[0,0] = startTile;
        //Debug.Log(arrayLengthX + "," + arrayLengthZ);
        //selectedTiles[arrayLengthX, arrayLengthZ] = endTile;
        int startx = Convert.ToInt32(startTile.X);
        int starty = Convert.ToInt32(startTile.Y);
        
        //Debug.Log(startx + "," + starty);

        if (/*startTile.X < endTile.X && startTile.Y < endTile.Y*/ 1 == 1)
        {

            

            /*if (startTile.X < endTile.X && startTile.Y < endTile.Y)
            {
                startx = Convert.ToInt32(startTile.X);
                starty = Convert.ToInt32(startTile.Y);
            }
            else
            {
                startx = Convert.ToInt32(endTile.X);
                starty = Convert.ToInt32(endTile.Y);
            }*/

            if (startTile.Y < endTile.Y && startTile.X > endTile.X)
            {
                startx = Convert.ToInt32(startTile.Y);
                starty = Convert.ToInt32(endTile.X);
            }
            else
            {
                startx = Convert.ToInt32(startTile.X);
                starty = Convert.ToInt32(endTile.Y);
            }

            int xpos = startx;
            int ypos = starty;



            for (int i = 0; i <= arrayLengthX; i++)
            {
                
                //Debug.Log(gridSystem.GetTileInArray(xpos, ypos).X + ","  + gridSystem.GetTileInArray(xpos, ypos).Y);
                //Debug.Log(gridSystem.GetTileInArray(xpos, ypos).X);
                selectedTiles[0, i] = gridSystem.GetTileInArray(xpos, ypos);
                selectedTiles[arrayLengthX, i] = gridSystem.GetTileInArray(xpos + arrayLengthX, ypos);
                ypos++;

            }
            ypos = starty;
            for (int j = 1; j <= arrayLengthZ; j++)
            {
                selectedTiles[j, 0] = gridSystem.GetTileInArray(xpos + 1, ypos);
                selectedTiles[j, arrayLengthX] = gridSystem.GetTileInArray(xpos + 1, ypos + arrayLengthX);
                xpos++;

            }


            for (int i = 0; i <= arrayLengthX; i++)
            {
                for (int j = 0; j <= arrayLengthZ; j++)
                {
                    Debug.Log(selectedTiles[i, j].X + "." + selectedTiles[i, j].Y);

                }
            }



            return selectedTiles;
        }
        

        



    }

    private void CreateWalls(GridPosition.TilePos[,] selectedTiles)
    {
        

        int arrayLengthX = selectedTiles.GetLength(0) - 1;
        int arrayLengthZ = selectedTiles.GetLength(1) - 1;

        

        for (int i = 0; i <= arrayLengthX; i++)
        {
            for (int j = 0; j <= arrayLengthZ; j++)
            {
                Vector3 newGOSpawn = new Vector3(selectedTiles[i,j].X * 1.5f, 1.5f, selectedTiles[i,j].Y * 1.5f);
                Instantiate(testGO, newGOSpawn, Quaternion.identity);

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
    
}

public enum HouseBuilding
{
    None,
    MouseDown,
    MouseDragged,
    MouseUp,
}
