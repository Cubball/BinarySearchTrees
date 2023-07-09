namespace Trees;

internal interface ITree<T> : ITree where T : Node<T>
{
    new T? Minimum();
    new T? Maximum();
    new T? Search(int value);
    new T? Predecessor(int value);
    new T? Successor(int value);
    new void Insert(int value);
    new void Delete(int value);

    INode? ITree.Minimum() => Minimum();
    INode? ITree.Maximum() => Maximum();
    INode? ITree.Search(int value) => Search(value);
    INode? ITree.Predecessor(int value) => Predecessor(value);
    INode? ITree.Successor(int value) => Successor(value);
    void ITree.Insert(int value) => Insert(value);
    void ITree.Delete(int value) => Delete(value);
}
