using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public Transform enemy, player;
    public float interval;
    private Node[] patrolNode;
    private Node randomNode;

    Grid grid;

    private void Start()
    {
        grid = GetComponent<Grid>();
        patrolNode = new Node[8];
        patrolNode[0] = grid.grid[12, 6];
        patrolNode[1] = grid.grid[16, 8];
        patrolNode[2] = grid.grid[17, 12];
        patrolNode[3] = grid.grid[16, 16];
        patrolNode[4] = grid.grid[12, 18];
        patrolNode[5] = grid.grid[8, 16];
        patrolNode[6] = grid.grid[7, 12];
        patrolNode[7] = grid.grid[8, 8];
        randomNode = patrolNode[0];
        PathFind();
    }

    private void Update()
    {
        if (grid.path != null && grid.path.Count <= 0)
        {
            StopCoroutine(PathFindRoutine());
            randomNode = patrolNode[Mathf.RoundToInt(Random.Range(0, 8))];
            PathFind();
        }
        else if (grid.NodeFromWorldPosition(player.position) != null)
        {
            StartCoroutine(PathFindRoutine());
        }
    }


    IEnumerator PathFindRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            PathFind();
        }
    }

    public void PathFind()
    {
        if (enemy == null) return;

        Node startNode = grid.NodeFromWorldPosition(enemy.position);
        Node targetNode = grid.NodeFromWorldPosition(player.position);

        if (targetNode == null)
        {
            targetNode = randomNode;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            // get lowest f cost
            for (int i = 1; i < openSet.Count; i++) 
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbor in grid.NeighboreNodes(currentNode))
            {
                /*if (neighbor == null) Debug.Log("neighbor null");
                if (targetNode == null) Debug.Log("target node null");*/
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
    }

    void RetracePath(Node startNode, Node targetNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        /*if (nodeA == null) Debug.Log("A null");
        if (nodeB == null) Debug.Log("B null");*/

        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);

        return 14 * distX + 10 * (distY - distX);
    }
}
