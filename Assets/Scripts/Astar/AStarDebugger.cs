using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarDebugger : MonoBehaviour
{
    private TileScript goal, start;
    [SerializeField]
    private GameObject Arrow;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //void Update()
    //{
    //    ClickTile();

    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        AStar.GetPath(start.GridPosition, goal.GridPosition);
    //    }
    //}

    private void ClickTile()
    {
        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.collider != null)
            {
                TileScript tmp = hit.collider.GetComponent<TileScript>();
                if(tmp!=null)
                {
                    if(start == null)
                    {
                        start = tmp;
                        start.SpriteRenderer.color = new Color32(255, 255, 0, 255);
                    }
                    else if (goal == null)
                    {
                        goal = tmp;
                        goal.SpriteRenderer.color = new Color32(255, 132, 255, 255);
                    }
                    else
                    {
                        start.SpriteRenderer.color = Color.white;
                        goal.SpriteRenderer.color = Color.white;
                        start = tmp;
                        start.SpriteRenderer.color = new Color32(255, 255, 0, 255);
                        goal = null;
                    }
                }
            }
        }
    }

    public void DebugPath(HashSet<Node> openList, HashSet<Node> closedList, Stack<Node> path)
    {
        foreach (Node node in openList)
        {
            if(node.TileRef != start && node.TileRef != goal && !path.Contains(node))
            {
                node.TileRef.SpriteRenderer.color = Color.cyan;
            }

            //PointToParent(node, node.TileRef.WorldPosition);
            
        }
        foreach (Node node in closedList)
        {
            if (node.TileRef != start && node.TileRef != goal && !path.Contains(node))
            {
                node.TileRef.SpriteRenderer.color = Color.blue;
            }
        }
        foreach (Node node in path)
        {
            if (node.TileRef != start && node.TileRef != goal)
            {
                node.TileRef.SpriteRenderer.color = Color.red;
            }
        }
    }

    private void PointToParent(Node node, Vector2 position)
    {
        if (node.Parent != null)
        {
            GameObject arrow = (GameObject)Instantiate(Arrow, position, Quaternion.identity);
            if (node.GridPosition.X < node.Parent.GridPosition.X && node.GridPosition.Y == node.Parent.GridPosition.Y)
            {
                arrow.transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
        

    }
}
