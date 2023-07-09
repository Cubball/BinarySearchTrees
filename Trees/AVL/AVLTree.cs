namespace Trees;

internal class AVLTree : ITree<AVLNode>
{
    private AVLNode? _root;

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

    public AVLNode? Minimum() => (_root == null) ? null : Minimum(_root);
    public AVLNode? Maximum() => (_root == null) ? null : Maximum(_root);

    public AVLNode? Search(int value)
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

    public AVLNode? Predecessor(int value)
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

    public AVLNode? Successor(int value)
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
        Insert(new AVLNode(value));
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

    private AVLNode Minimum(AVLNode node)
    {
        while (node.Left != null)
        {
            node = node.Left;
        }
        return node;
    }

    private AVLNode Maximum(AVLNode node)
    {
        while (node.Right != null)
        {
            node = node.Right;
        }
        return node;
    }

    private void Insert(AVLNode node)
    {
        AVLNode? parent = null;
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
            return;
        }
        else if (parent.Data < node.Data)
        {
            parent.Right = node;
        }
        else
        {
            parent.Left = node;
        }
        if (parent.BalanceFactor != 0)
        {
            Rebalance(parent);
        }
    }

    private void LeftRotate(AVLNode node)
    {
        var rightChild = node.Right;
        node.Right = rightChild!.Left;
        if (node.Right != null)
        {
            node.Right.Parent = node;
        }
        if (node.Parent == null)
        {
            _root = rightChild;
        }
        else if (node.Parent.Left == node)
        {
            node.Parent.Left = rightChild;
        }
        else
        {
            node.Parent.Right = rightChild;
        }
        rightChild.Parent = node.Parent;
        rightChild.Left = node;
        node.Parent = rightChild;
    }

    private void RightRotate(AVLNode node)
    {
        var leftChild = node.Left;
        node.Left = leftChild!.Right;
        if (node.Left != null)
        {
            node.Left.Parent = node;
        }
        if (node.Parent == null)
        {
            _root = leftChild;
        }
        else if (node.Parent.Left == node)
        {
            node.Parent.Left = leftChild;
        }
        else
        {
            node.Parent.Right = leftChild;
        }
        leftChild.Parent = node.Parent;
        leftChild.Right = node;
        node.Parent = leftChild;
    }

    private void Delete(AVLNode node)
    {
        var violatingNode = node.Parent;
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
            violatingNode = successor;
            if (successor != node.Right)
            {
                violatingNode = successor.Parent;
                Transplant(successor, successor.Right);
                successor.Right = node.Right;
                successor.Right.Parent = successor;
            }
            Transplant(node, successor);
            successor.Left = node.Left;
            successor.Left.Parent = successor;
        }
        Rebalance(violatingNode);
    }

    private void Transplant(AVLNode nodeToReplace, AVLNode? nodeReplacing)
    {
        if (nodeToReplace.Parent == null)
        {
            _root = nodeReplacing;
        }
        else if (nodeToReplace.Parent!.Left == nodeToReplace)
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

    private void Rebalance(AVLNode? node)
    {
        while (node != null)
        {
            if (node.BalanceFactor > 1)
            {
                if (node.Left!.BalanceFactor < 0)
                {
                    LeftRotate(node.Left);
                    node.Left.Left!.UpdateHeight();
                    node.Left.UpdateHeight();
                }
                RightRotate(node);
                node.UpdateHeight();
                node = node.Parent!;
            }
            else if (node.BalanceFactor < -1)
            {
                if (node.Right!.BalanceFactor > 0)
                {
                    RightRotate(node.Right);
                    node.Right.Right!.UpdateHeight();
                    node.Right.UpdateHeight();
                }
                LeftRotate(node);
                node.UpdateHeight();
                node = node.Parent!;
            }
            node.UpdateHeight();
            node = node.Parent;
        }
    }
}
