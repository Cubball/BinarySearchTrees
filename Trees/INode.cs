namespace Trees;

internal interface INode
{
    int Data { get; }
    INode? Parent { get; }
    INode? Left { get; }
    INode? Right { get; }
}
