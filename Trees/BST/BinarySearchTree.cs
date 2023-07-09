namespace Trees;

internal class BinarySearchTree : ITree<BSTNode>
{
    private BSTNode? _root;

    public bool IsEmpty => _root == null;

    public IEnumerable<INode> PreorderTreeWalk()
    {
        return PreorderTreeWalk(_root);
    }

    public IEnumerable<INode> InorderTreeWalk()
    {
        return InorderTreeWalk(_root);
    }

    public IEnumerable<INode> PostorderTreeWalk()
    {
        return PostorderTreeWalk(_root);
    }

    public BSTNode? Minimum() => (_root == null) ? null : Minimum(_root);
    public BSTNode? Maximum() => (_root == null) ? null : Maximum(_root);

    public BSTNode? Search(int value)
    {
        var current = _root;
        while (current != null && current.Data != value)
        {
            if (current.Data < value)
            {
                current = current.Right;
            }
            else
            {
                current = current.Left;
            }
        }
        return current;
    }

    public BSTNode? Predecessor(int value)
    {
        var node = Search(value);
        if (node == null)
        {
            throw new ArgumentOutOfRangeException($"Елемента зі значенням {value} немає в дереві");
        }

        if (node.Left != null)
        {
            return Maximum(node.Left);
        }

        var parent = node.Parent;
        while (parent != null && node == parent.Left)
        {
            node = parent;
            parent = parent.Parent;
        }
        return parent;
    }

    public BSTNode? Successor(int value)
    {
        var node = Search(value);
        if (node == null)
        {
            throw new ArgumentOutOfRangeException($"Елемента зі значенням {value} немає в дереві");
        }

        if (node.Right != null)
        {
            return Minimum(node.Right);
        }

        var parent = node.Parent;
        while (parent != null && node == parent.Right)
        {
            node = parent;
            parent = parent.Parent;
        }
        return parent;
    }

    public void Insert(int value)
    {
        Insert(new BSTNode(value));
    }

    public void Delete(int value)
    {
        var nodeToDelete = Search(value);
        if (nodeToDelete == null)
        {
            throw new ArgumentOutOfRangeException($"Елемента зі значенням {value} немає в дереві");
        }
        Delete(nodeToDelete);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>A list of lists of tuples, where the first item is the node, 
    /// and the second value is it's number in it's level from the left in a 
    /// full binary tree or null, if tree is empty</returns>
    public List<List<(INode, int)>>? GetEnumeratedNodesByLevels()
    {
        if (_root == null)
        {
            return null;
        }

        var levels = new List<List<(INode, int)>>();
        var queue = new Queue<(INode, int)>();
        queue.Enqueue((_root, 1));
        while (queue.Count > 0)
        {
            levels.Add(queue.ToList());
            int numberOfNodesInLevel = queue.Count;
            for (int i = 0; i < numberOfNodesInLevel; i++)
            {
                var nodeInfo = queue.Dequeue();
                if (nodeInfo.Item1.Left != null)
                {
                    queue.Enqueue((nodeInfo.Item1.Left, nodeInfo.Item2 * 2 - 1));
                }
                if (nodeInfo.Item1.Right != null)
                {
                    queue.Enqueue((nodeInfo.Item1.Right, nodeInfo.Item2 * 2));
                }
            }
        }
        return levels;
    }

    private IEnumerable<INode> PreorderTreeWalk(INode? node)
    {
        if (node == null)
        {
            yield break;
        }
        yield return node;
        foreach (var nextNode in PreorderTreeWalk(node.Left))
        {
            yield return nextNode;
        }
        foreach (var nextNode in PreorderTreeWalk(node.Right))
        {
            yield return nextNode;
        }
    }

    private IEnumerable<INode> InorderTreeWalk(INode? node)
    {
        if (node == null)
        {
            yield break;
        }
        foreach (var nextNode in InorderTreeWalk(node.Left))
        {
            yield return nextNode;
        }
        yield return node;
        foreach (var nextNode in InorderTreeWalk(node.Right))
        {
            yield return nextNode;
        }
    }

    private IEnumerable<INode> PostorderTreeWalk(INode? node)
    {
        if (node == null)
        {
            yield break;
        }
        foreach (var nextNode in PostorderTreeWalk(node.Left))
        {
            yield return nextNode;
        }
        foreach (var nextNode in PostorderTreeWalk(node.Right))
        {
            yield return nextNode;
        }
        yield return node;
    }

    private BSTNode Minimum(BSTNode node)
    {
        while (node.Left != null)
        {
            node = node.Left;
        }
        return node;
    }

    private BSTNode Maximum(BSTNode node)
    {
        while (node.Right != null)
        {
            node = node.Right;
        }
        return node;
    }

    private void Insert(BSTNode node)
    {
        BSTNode? parent = null;
        var current = _root;
        while (current != null)
        {
            parent = current;
            if (current.Data < node.Data)
            {
                current = current.Right;
            }
            else
            {
                current = current.Left;
            }
        }
        node.Parent = parent;
        if (parent == null)
        {
            _root = node;
        }
        else if (parent.Data < node.Data)
        {
            parent.Right = node;
        }
        else
        {
            parent.Left = node;
        }
    }

    private void Delete(BSTNode node)
    {
        if (node.Left == null)
        {
            Transplant(node, node.Right);
        }
        else if (node.Right == null)
        {
            Transplant(node, node.Left);
        }
        else
        {
            var successor = Minimum(node.Right);
            if (successor != node.Right)
            {
                Transplant(successor, successor.Right);
                successor.Right = node.Right;
                successor.Right.Parent = successor;
            }
            Transplant(node, successor);
            successor.Left = node.Left;
            successor.Left.Parent = successor;
        }
    }

    private void Transplant(BSTNode nodeToReplace, BSTNode? nodeReplacing)
    {
        if (nodeToReplace.Parent == null)
        {
            _root = nodeReplacing;
        }
        else if (nodeToReplace.Parent.Left == nodeToReplace)
        {
            nodeToReplace.Parent.Left = nodeReplacing;
        }
        else
        {
            nodeToReplace.Parent.Right = nodeReplacing;
        }
        if (nodeReplacing != null)
        {
            nodeReplacing.Parent = nodeToReplace.Parent;
        }
    }
}
