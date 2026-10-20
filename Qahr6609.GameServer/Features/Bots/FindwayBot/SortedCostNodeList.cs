using System;
using System.Collections;

namespace Pathfinding.AStar
{
    internal class SortedCostNodeList
    {
        ArrayList _list;
        NodeComparer _nodeComparer;
        public int Count
        {
            get { return _list.Count; }
        }
        public SortedCostNodeList()
        {
            _list = new ArrayList();
            _nodeComparer = new NodeComparer();
        }
        public Node NodeAt(int i)
        {
            return (Node)_list[i];
        }
        public void RemoveAt(int i)
        {
            _list.RemoveAt(i);
        }
        public int IndexOf(Node n)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                Node nodeInTheList = (Node)_list[i];
                if (nodeInTheList.isMatch(n))
                    return i;
            }
            return -1;
        }
        public int push(Node n)
        {
            int k = _list.BinarySearch(n, _nodeComparer);
            if (k == -1)
                _list.Insert(0, n);
            else if (k < 0)
            {
                k = ~k;
                _list.Insert(k, n);
            }
            else if (k >= 0)
                _list.Insert(k, n);
            return k;
        }
        public Node pop()
        {
            Node r = (Node)_list[0];
            _list.RemoveAt(0);
            return r;
        }
    }
}