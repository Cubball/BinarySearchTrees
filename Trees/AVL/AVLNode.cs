namespace Trees;

internal class AVLNode : Node<AVLNode>
{
    public int Height { get; private set; }
    public int BalanceFactor => (Left?.Height ?? -1) - (Right?.Height ?? -1);

    public AVLNode(int data) : base(data)
    { 
        Height = 0;
    }

    public void UpdateHeight()
    {
        Height = Math.Max(Left?.Height ?? -1, Right?.Height ?? -1) + 1;
    }
}
