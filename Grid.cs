using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform Player;
    public LayerMask Wall;
    public Vector2 Grid_World_Size;
    public float Node_Radius;
    public List<Nodes> Path;
    Nodes[,] _Grid;
    float Node_Diameter;
    int GridSizeX;
    int GridSizeY;
    // Start is called before the first frame update
    void Start()
    {
        Node_Diameter = Node_Radius * 2;
        GridSizeX = Mathf.RoundToInt(Grid_World_Size.x / Node_Diameter);
        GridSizeY = Mathf.RoundToInt(Grid_World_Size.y / Node_Diameter);
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateGrid()
    {
        _Grid = new Nodes[GridSizeX, GridSizeY];
        Vector3 WorldBottomLeft = transform.position - Vector3.right * Grid_World_Size.x / 2 - Vector3.forward * Grid_World_Size.y / 2;
        for (int i = 0; i < GridSizeX; i++)
        {
            for (int j = 0; j < GridSizeY; j++)
            {
                Vector3 WorldPoint = WorldBottomLeft + Vector3.right *
                    (i * Node_Diameter + Node_Radius) + Vector3.forward * (j * Node_Diameter + Node_Radius);
                bool Can_Walk = !(Physics.CheckSphere(WorldPoint, Node_Radius,Wall));
                _Grid[i, j] = new Nodes(Can_Walk, WorldPoint,i,j);
            }
        }
    }
    public List<Nodes> GetNeighbours(Nodes Node)
    {
        List<Nodes> Neighbours = new List<Nodes>();
        for (int i = -1; i <= 1 ; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j ==0)
                {
                    continue;
                }
                int CheckX = Node.gridX + i;
                int CheckY = Node.gridY + j;
                if (CheckX >= 0 && CheckX < GridSizeX && CheckY >=0 && CheckY < GridSizeY)
                {
                    Neighbours.Add(_Grid[CheckX, CheckY]);
                }
            }
        }
        return Neighbours;
    }
    public Nodes NodeFromWorldPoint(Vector3 WorldPosition)
    {
        float PercentX = (WorldPosition.x + Grid_World_Size.x / 2) / Grid_World_Size.x;
        float PercentY = (WorldPosition.z + Grid_World_Size.y / 2) / Grid_World_Size.y;
        PercentX = Mathf.Clamp01(PercentX);
        PercentY = Mathf.Clamp01(PercentY);
        int x = Mathf.RoundToInt((GridSizeX - 1) * PercentX);
        int y = Mathf.RoundToInt((GridSizeY - 1) * PercentY);
        return _Grid[x, y];
    }

    
}
