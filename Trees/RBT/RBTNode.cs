namespace Trees;

internal class RBTNode : Node<RBTNode> 
{
    public bool IsBlack { get; set; }

	public RBTNode(int data) : base(data) { }
}
