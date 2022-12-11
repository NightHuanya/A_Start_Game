using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodes
{
    public bool Can_Walk;
    public Vector3 WorldPosition;
    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    public Nodes Parent;
    public Nodes(bool can_Walk,Vector3 worldPosition,int _gridX,int _gridY)
    {
        Can_Walk = can_Walk;
        WorldPosition = worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
