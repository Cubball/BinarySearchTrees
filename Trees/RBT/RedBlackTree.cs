namespace Trees;

internal class RedBlackTree : ITree<RBTNode>
{
    private RBTNode _root;
    private RBTNode _nil;

    public RedBlackTree()
    {
        _nil = new RBTNode(0)
        {
            IsBlack = true,
        };
        _root = _nil;
    }

    public bool IsEmpty => _root == _nil;

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

    public RBTNode? Minimum() => (_root == _nil) ? null : Minimum(_root!);
    public RBTNode? Maximum() => (_root == _nil) ? null : Maximum(_root!);

    public RBTNode? Search(int value)
    {
        var current = _root;
        while (current != _nil && current.Data != value)
        {
            if (current.Data < value)
            {
                current = current.Right!;
            }
            else
            {
                current = current.Left!;
            }
        }
        return (current == _nil) ? null : current;
    }

    public RBTNode? Predecessor(int value)
    {
        var node = Search(value);
        if (node == null)
        {
            throw new ArgumentOutOfRangeException($"Елемента зі значенням {value} немає в дереві");
        }

        if (node.Left != _nil)
        {
            return Maximum(node.Left!);
        }

        var parent = node.Parent;
        while (parent != _nil && node == parent!.Left)
        {
            node = parent;
            parent = parent.Parent;
        }
        return parent;
    }

    public RBTNode? Successor(int value)
    {
        var node = Search(value);
        if (node == null)
        {
            throw new ArgumentOutOfRangeException($"Елемента зі значенням {value} немає в дереві");
        }

        if (node.Right != _nil)
        {
            return Minimum(node.Right!);
        }

        var parent = node.Parent;
        while (parent != _nil && node == parent!.Right)
        {
            node = parent;
            parent = parent.Parent;
        }
        return parent;
    }

    public void Insert(int value)
    {
        Insert(new RBTNode(value));
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
        if (_root == _nil)
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
                if (nodeInfo.Item1.Left != _nil)
                {
                    queue.Enqueue((nodeInfo.Item1.Left!, nodeInfo.Item2 * 2 - 1));
                }
                if (nodeInfo.Item1.Right != _nil)
                {
                    queue.Enqueue((nodeInfo.Item1.Right!, nodeInfo.Item2 * 2));
                }
            }
        }
        return levels;
    }

    private IEnumerable<INode> PreorderTreeWalk(INode? node)
    {
        if (node == _nil)
        {
            yield break;
        }
        yield return node!;
        foreach (var nextNode in PreorderTreeWalk(node!.Left))
        {
            yield return nextNode;
        }
        foreach (var nextNode in PreorderTreeWalk(node!.Right))
        {
            yield return nextNode;
        }
    }

    private IEnumerable<INode> InorderTreeWalk(INode? node)
    {
        if (node == _nil)
        {
            yield break;
        }
        foreach (var nextNode in InorderTreeWalk(node!.Left))
        {
            yield return nextNode;
        }
        yield return node;
        foreach (var nextNode in InorderTreeWalk(node!.Right))
        {
            yield return nextNode;
        }
    }

    private IEnumerable<INode> PostorderTreeWalk(INode? node)
    {
        if (node == _nil)
        {
            yield break;
        }
        foreach (var nextNode in PostorderTreeWalk(node!.Left))
        {
            yield return nextNode;
        }
        foreach (var nextNode in PostorderTreeWalk(node!.Right))
        {
            yield return nextNode;
        }
        yield return node;
    }

    private RBTNode Minimum(RBTNode node)
    {
        while (node.Left != _nil)
        {
            node = node.Left!;
        }
        return node;
    }

    private RBTNode Maximum(RBTNode node)
    {
        while (node.Right != _nil)
        {
            node = node.Right!;
        }
        return node;
    }

    private void Insert(RBTNode node)
    {
        var parent = _nil;
        var current = _root;
        while (current != _nil)
        {
            parent = current!;
            if (current!.Data < node.Data)
            {
                current = current.Right;
            }
            else
            {
                current = current.Left;
            }
        }
        if (parent == _nil)
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
        node.Parent = parent;
        node.IsBlack = false;
        node.Left = _nil;
        node.Right = _nil;
        InsertFixup(node);
    }

    private void InsertFixup(RBTNode node)
    {
        while (!node.Parent!.IsBlack)
        {
            // випадок 1: дядько червоний -> перефарб. дядька, батька, діда, перекинути вказівник на діда
            // випадок 2: дядько чорний і елемент протилений нащадок порівняно з батьком -> поворот зводить до випадку 3
            // випадок 3: дядько чорний і елемент той же ж нащадок, що й батько -> 
            // перефарб. батька, діда і повернути діда від батька
            if (node.Parent == node.Parent.Parent!.Left)
            {
                var uncle = node.Parent.Parent.Right!;
                if (!uncle.IsBlack)                     // 1
                {
                    node.Parent.IsBlack = true;
                    uncle.IsBlack = true;
                    node.Parent.Parent.IsBlack = false;
                    node = node.Parent.Parent;
                }
                else
                {
                    if (node == node.Parent.Right)      // 2
                    {
                        node = node.Parent;
                        LeftRotate(node);
                    }
                    node.Parent!.IsBlack = true;        // 3
                    node.Parent.Parent!.IsBlack = false;
                    RightRotate(node.Parent.Parent);
                }
            }
            else
            {
                var uncle = node.Parent.Parent.Left!;
                if (!uncle.IsBlack)                     // 1
                {
                    node.Parent.IsBlack = true;
                    uncle.IsBlack = true;
                    node.Parent.Parent.IsBlack = false;
                    node = node.Parent.Parent;
                }
                else
                {
                    if (node == node.Parent.Left)      // 2
                    {
                        node = node.Parent;
                        RightRotate(node);
                    }
                    node.Parent!.IsBlack = true;        // 3
                    node.Parent.Parent!.IsBlack = false;
                    LeftRotate(node.Parent.Parent);
                }
            }

        }
        _root.IsBlack = true;
    }

    private void LeftRotate(RBTNode node)
    {
        var rightChild = node.Right!;
        node.Right = rightChild.Left!;
        if (node.Right != _nil)
        {
            node.Right.Parent = node;
        }
        if (node.Parent == _nil)
        {
            _root = rightChild;
        }
        else if (node.Parent!.Left == node)
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

    private void RightRotate(RBTNode node)
    {
        var leftChild = node.Left!;
        node.Left = leftChild.Right!;
        if (node.Left != _nil)
        {
            node.Left.Parent = node;
        }
        if (node.Parent == _nil)
        {
            _root = leftChild;
        }
        else if (node.Parent!.Left == node)
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

    private void Delete(RBTNode node) 
    {
        var isReplacedNodeBlack = node.IsBlack;
        RBTNode nodeReplacing;
        if (node.Left == _nil)
        {
            Transplant(node, node.Right!);
            nodeReplacing = node.Right!;
        }
        else if (node.Right == _nil)
        {
            Transplant(node, node.Left!);
            nodeReplacing = node.Left!;
        }
        else
        {
            var successor = Minimum(node.Right!);
            isReplacedNodeBlack = successor.IsBlack;
            nodeReplacing = successor.Right!;
            if (successor != node.Right)
            {
                Transplant(successor, successor.Right!);
                successor.Right = node.Right;
                successor.Right!.Parent = successor;
            }
            else
            {
                nodeReplacing.Parent = successor;
            }
            Transplant(node, successor);
            successor.Left = node.Left;
            successor.Left!.Parent = successor;
            successor.IsBlack = node.IsBlack;
        }
        if (isReplacedNodeBlack)
        {
            DeleteFixup(nodeReplacing);
        }
    }

    private void Transplant(RBTNode nodeToReplace, RBTNode nodeReplacing)
    {
        if (nodeToReplace.Parent == _nil)
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
        nodeReplacing.Parent = nodeToReplace.Parent;
    }

    private void DeleteFixup(RBTNode node)
    {
        // випадок 1: брат червоний -> перефарб. брата, батька і повернути батька в сторону елемента
        // випадок 2: брат і його 2 нащадки чорні -> перефарб. брата в червоний, перекинути вказівник на батька
        // випадок 3: брат чорний і його нащадок ззовні чорний -> перефарб. 
        // брата і нащадка всередині, повернути брата від даного елемента
        // випадок 4: брат чорний і його нащадок ззовні червоний -> перефарб. брата в колір батька, нащадка ззовні
        // та батька в чорний, повернути батька в сторону елемента
        while (node != _root && node.IsBlack)
        {
            if (node == node.Parent!.Left)
            {
                var sibling = node.Parent.Right!;
                if (!sibling.IsBlack)                                   // 1
                {
                    sibling.IsBlack = true;
                    node.Parent.IsBlack = false;
                    LeftRotate(node.Parent);
                    sibling = node.Parent.Right!;
                }
                if (sibling.Left!.IsBlack && sibling.Right!.IsBlack)    // 2
                {
                    sibling.IsBlack = false;
                    node = node.Parent!;
                }
                else
                {
                    if (sibling.Right!.IsBlack)                         // 3
                    {
                        sibling.Left.IsBlack = true;
                        sibling.IsBlack = false;
                        RightRotate(sibling);
                        sibling = node.Parent.Right!;
                    }
                    sibling.IsBlack = node.Parent.IsBlack;              // 4
                    node.Parent.IsBlack = true;
                    sibling.Right!.IsBlack = true;
                    LeftRotate(node.Parent);
                    node = _root;
                }
            }
            else
            {
                var sibling = node.Parent.Left!;
                if (!sibling.IsBlack)                                   // 1
                {
                    sibling.IsBlack = true;
                    node.Parent.IsBlack = false;
                    RightRotate(node.Parent);
                    sibling = node.Parent.Left!;
                }
                if (sibling.Left!.IsBlack && sibling.Right!.IsBlack)    // 2
                {
                    sibling.IsBlack = false;
                    node = node.Parent!;
                }
                else
                {
                    if (sibling.Left!.IsBlack)                          // 3
                    {
                        sibling.Right!.IsBlack = true;
                        sibling.IsBlack = false;
                        LeftRotate(sibling);
                        sibling = node.Parent.Left!;
                    }
                    sibling.IsBlack = node.Parent.IsBlack;              // 4
                    node.Parent.IsBlack = true;
                    sibling.Left!.IsBlack = true;
                    RightRotate(node.Parent);
                    node = _root;
                }
            }
        }
        node.IsBlack = true;
    }
}
