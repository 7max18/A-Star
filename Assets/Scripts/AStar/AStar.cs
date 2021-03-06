using UnityEngine;
using System.Collections;

public class AStar
{
    #region List fields

    public static PriorityQueue closedList, openList;

    #endregion

    /// <summary>
    /// Calculate the final path in the path finding
    /// </summary>
    private static ArrayList CalculatePath(Node node)
    {
        ArrayList list = new ArrayList();
        while (node != null)
        {
            list.Add(node);
            node = node.parent;
        }
        list.Reverse();
        return list;
    }

    /// <summary>
    /// Calculate the estimated Heuristic cost to the goal
    /// </summary>
    private static float HeuristicEstimateCost(Node curNode, Node goalNode)
    {
        Vector3 vecCost = curNode.position - goalNode.position;
        return vecCost.magnitude;
    }

    /// <summary>
    /// Find the path between start node and goal node using AStar Algorithm
    /// </summary>
    public static ArrayList FindPath(Node start, Node goal)
    {
        //Start Finding the path
        openList = new PriorityQueue();
        openList.Push(start);
        start.nodeTotalCost = 0.0f;
        start.estimatedCost = HeuristicEstimateCost(start, goal);

        closedList = new PriorityQueue();
        Node node = null;

        //Keep running steps until the goal is found or the open list is exhausted
        while (openList.Length != 0)
        {
            //openList is always sorted with the lowest estimated cost first (see Node.cs),
            //so after each round of checking neighbors, the one with the lowest cost is automatically made the focus of the next round
            node = openList.First();

            if (node.position == goal.position)
            {
                return CalculatePath(node);
            }
			
            ArrayList neighbours = new ArrayList();
            GridManager.instance.GetNeighbours(node, neighbours);

            #region CheckNeighbours

            //Get the Neighbours
            for (int i = 0; i < neighbours.Count; i++)
            {
                //Cost between neighbour nodes
                Node neighbourNode = (Node)neighbours[i];

                if (!closedList.Contains(neighbourNode))
                {					
					//Cost from current node to this neighbour node
	                float cost = HeuristicEstimateCost(node, neighbourNode);	
	                
					//Total Cost So Far from start to this neighbour node (G)
	                float totalCost = node.nodeTotalCost + cost;
					
					//Estimated cost for neighbour node to the goal (H)
	                float neighbourNodeEstCost = HeuristicEstimateCost(neighbourNode, goal);					
					
					//Assign neighbour node properties
	                neighbourNode.nodeTotalCost = totalCost; //G
	                neighbourNode.parent = node;
	                neighbourNode.estimatedCost = totalCost + neighbourNodeEstCost; //F
	
	                //Add the neighbour node to the list if not already existed in the list
	                if (!openList.Contains(neighbourNode))
	                {
	                    openList.Push(neighbourNode);
	                }
                }
            }
			
            #endregion
            
            //remove the focused-on node from the open list once the round of neighbor-checking is done
            closedList.Push(node);
            openList.Remove(node);
        }

        //If finished looping and cannot find the goal then return null
        if (node.position != goal.position)
        {
            Debug.LogError("Goal Not Found");
            return null;
        }

        //Calculate the path based on the final node
        return CalculatePath(node);
    }
}
