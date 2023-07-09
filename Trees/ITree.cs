namespace Trees;

internal interface ITree
{
    bool IsEmpty { get; }
    IEnumerable<INode> PreorderTreeWalk();
    IEnumerable<INode> InorderTreeWalk();
    IEnumerable<INode> PostorderTreeWalk();
    INode? Minimum();
    INode? Maximum();
    INode? Search(int value);
    INode? Predecessor(int value);
    INode? Successor(int value);
    void Insert(int value);
    void Delete(int value);
    List<List<(INode, int)>>? GetEnumeratedNodesByLevels();
}
