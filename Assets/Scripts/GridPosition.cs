using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class GridPosition : MonoBehaviour
{
    TilePos tilePos;

    public struct TilePos

    {

        public float X;

        public float Y;

        



        public TilePos(float x, float y)

        {

            X = x;

            Y = y;

        }

    }


    private void Awake()
    {
        tilePos = new TilePos(this.gameObject.transform.position.z / 1.5f, this.gameObject.transform.position.x / 1.5f);
    }

    public TilePos GetTilePos()
    {
        return tilePos;
    }


}
