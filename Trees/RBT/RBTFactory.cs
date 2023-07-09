namespace Trees;

internal class RBTFactory : ITreeFactory
{
    public ITree GetTree()
    {
        return new RedBlackTree();
    }
}
