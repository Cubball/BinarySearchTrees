namespace Trees;

internal class AVLFactory : ITreeFactory
{
    public ITree GetTree()
    {
        return new AVLTree();
    }
}
