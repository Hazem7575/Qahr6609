using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding.AStar
{
    public static class Calculate
    {
        public struct Coordonates
        {
            public ushort X, Y;
        }
        public static List<Coordonates> FindWay(ushort myX, ushort myY, ushort toX, ushort toY, Conquer6609.Role.GameMap map)
        {
            List<Coordonates> SolutionPathList = new List<Coordonates>();
            Node node_goal = new Node(null, null, 1, toX, toY, map);
            Node node_start = new Node(null, node_goal, 1, myX, myY, map);
            SortedCostNodeList OPEN = new SortedCostNodeList();
            SortedCostNodeList CLOSED = new SortedCostNodeList();
            OPEN.push(node_start);
            while (OPEN.Count > 0)
            {
                Node node_current = OPEN.pop();
                if (node_current.isMatch(node_goal))
                {
                    node_goal.parentNode = node_current.parentNode;
                    break;
                }
                ArrayList successors = node_current.GetSuccessors();
                foreach (Node node_successor in successors)
                {
                    int oFound = OPEN.IndexOf(node_successor);

                    if (oFound > 0)
                    {
                        Node existing_node = OPEN.NodeAt(oFound);
                        if (existing_node.CompareTo(node_current) <= 0) continue;
                    }
                    int cFound = CLOSED.IndexOf(node_successor);
                    if (cFound > 0)
                    {
                        Node existing_node = CLOSED.NodeAt(cFound);
                        if (existing_node.CompareTo(node_current) <= 0) continue;
                    }
                    if (oFound != -1)
                        OPEN.RemoveAt(oFound);
                    if (cFound != -1)
                        CLOSED.RemoveAt(cFound);
                    OPEN.push(node_successor);
                }
                CLOSED.push(node_current);
            }
            Node p = node_goal;
            while (p != null)
            {
                SolutionPathList.Add(new Coordonates() { X = p.x, Y = p.y });
                p = p.parentNode;
            }
            return SolutionPathList;
        }
    }
}