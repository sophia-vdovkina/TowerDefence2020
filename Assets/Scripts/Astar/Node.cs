using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Point GridPosition { get; set; }

    public int G { get; set; }

    public int H { get; set; }

    public int F { get; set; }

    public Vector2 WorldPosition { get; set; }

    /// <summary>
    /// Ссылка на тайл на котором расположен узел
    /// </summary>
    public TileScript TileRef { get; private set; }

    public Node Parent { get; private set; }

    public Node(TileScript tileRef)
    {
        TileRef = tileRef;
        GridPosition = TileRef.GridPosition;
        WorldPosition = TileRef.WorldPosition;
    }

    public void CalcValues(Node parent, Node goal, int gCost)
    {
        Parent = parent;
        G = parent.G + gCost;
        H = (Math.Abs(GridPosition.X - goal.GridPosition.X) + Math.Abs(goal.GridPosition.Y - goal.GridPosition.Y)) * 10;
        F = G + H;
    }
}
