namespace Trees;

internal class Operation
{
    public string Name { get; }
    public bool NeedsValueInput { get; }
    public string ButtonText { get; }
    public Action<ITree, int> Action { get; }

    public Operation(string name, bool needsValueInput, string buttonText, Action<ITree, int> action)
    {
        Name = name; 
        NeedsValueInput = needsValueInput;
        ButtonText = buttonText;
        Action = action;
    }

    public override string ToString() => Name;
}
