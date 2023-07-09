namespace Trees;

internal class BSTFactory : ITreeFactory
{
    public ITree GetTree()
    {
        return new BinarySearchTree();
    }
}
