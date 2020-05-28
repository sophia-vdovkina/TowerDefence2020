using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class AStar
{
    private static Dictionary<Point, Node> nodes;

    /// <summary>
    /// Создает узел для каждого тайла в игре
    /// </summary>
    private static void CreateNodes()
    {
        // инициализируем словарь
        nodes = new Dictionary<Point, Node>();
        // пробегаем по всем тайлам в игре
        foreach(TileScript tile in LevelManager.Instance.Tiles.Values)
        {
            nodes.Add(tile.GridPosition, new Node(tile));

        }
    }

    public static Stack<Node> GetPath(Point start, Point goal)
    {
        // в первый раз мы создаем все узлы
        if (nodes == null)
        {
            CreateNodes();
        }
        //Open list для А* алгоритма
        HashSet<Node> openList = new HashSet<Node>();
        //Closed list для А* алгоритма
        HashSet<Node> closedList = new HashSet<Node>();
        // Путь
        Stack<Node> finalPath = new Stack<Node>();
        //Стартовая точка
        Node currentNode = nodes[start];
        // добавляем старт в open list
        openList.Add(currentNode);
        // пока не закончатся узлы
        while (openList.Count > 0)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Point neighbourPos = new Point(currentNode.GridPosition.X - x, currentNode.GridPosition.Y - y);

                    if (nodes.ContainsKey(neighbourPos) && neighbourPos != currentNode.GridPosition && LevelManager.Instance.Tiles[neighbourPos].Walkable)
                    {
                        int gCost = 0;

                        if (Math.Abs(x - y) == 1)
                        {
                            gCost = 10;
                        }
                        else
                        {
                            // чтобы не проходить сквозь препятствия
                            if (!(ConnectedDiagonally(currentNode, nodes[neighbourPos]))){
                                continue;
                            }
                            gCost = 14;
                        }
                        // добавляем соседний узел в open list
                        Node neighbour = nodes[neighbourPos];
                        if (openList.Contains(neighbour))
                        {
                            // смотрим является ли рассматриваемый узел лучшим родителем
                            if (currentNode.G + gCost < neighbour.G)
                            {
                                neighbour.CalcValues(currentNode, nodes[goal], gCost);
                            }
                        }
                        else if (!closedList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                            // подсчитываем для соседнего узла все значения
                            neighbour.CalcValues(currentNode, nodes[goal], gCost);
                        }

                    }

                }
            }
            // перемещаем просмотренный узел в closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            // Выбираем узел с самым маленьким F
            if (openList.Count > 0)
            {
                currentNode = openList.OrderBy(n => n.F).First();
            }
            // если дошли до конца
            if (currentNode == nodes[goal])
            {
                while (currentNode.GridPosition != start)
                {
                    finalPath.Push(currentNode);
                    currentNode = currentNode.Parent;
                }
                break;
            }
        }

        //****** debug
        //GameObject.Find("Debugger").GetComponent<AStarDebugger>().DebugPath(openList, closedList, finalPath);
        return finalPath;
    }


    /// <summary>
    /// Проверяем можно ли пройти по диагонали
    /// </summary>
    /// <param name="currentNode"></param>
    /// <param name="neighbor"></param>
    /// <returns></returns>
    private static bool ConnectedDiagonally(Node currentNode, Node neighbor)
    {
        Point direction = neighbor.GridPosition - currentNode.GridPosition;

        Point first = new Point(currentNode.GridPosition.X + direction.X, currentNode.GridPosition.Y);

        Point second = new Point(currentNode.GridPosition.X, currentNode.GridPosition.Y + direction.Y);

        if ((nodes.ContainsKey(first) && !LevelManager.Instance.Tiles[first].Walkable) || (nodes.ContainsKey(second) && !LevelManager.Instance.Tiles[second].Walkable))
        {
            return false;
        }
        return true;
    }
}
