namespace Trees;

internal abstract class Node<T> : INode where T : Node<T>
{
    public int Data { get; set; }
    public T? Parent { get; set; }  
    public T? Left { get; set; }
    public T? Right { get; set; }

    INode? INode.Parent => Parent;
    INode? INode.Left => Left;
    INode? INode.Right => Right;

    public Node(int data)
    {
        Data = data; 
    }
}
