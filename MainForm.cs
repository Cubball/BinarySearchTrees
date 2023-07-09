namespace Trees;

public partial class MainForm : Form
{
    private readonly DrawingArea _drawingArea;
    private readonly Label _treeSelectionLabel;
    private readonly ComboBox _treeSelectionInput;
    private readonly Label _operationSelectionLabel;
    private readonly ComboBox _operationSelectionInput;
    private readonly Label _valueInputLabel;
    private readonly NumericUpDown _valueInput;
    private readonly Button _button;

    private readonly Operation[] _operations;
    private readonly Dictionary<string, ITreeFactory> _treeTypes;

    private ITree? _tree;
    private string? _selectedTreeName;
    private Action<ITree, int>? _action;
    private IEnumerator<INode>? _treeWalk;

    public MainForm()
    {
        InitializeComponent();
        Text = "Бінарні дерева пошуку";
        MinimumSize = new Size(Screen.PrimaryScreen!.WorkingArea.Width / 3, Screen.PrimaryScreen.WorkingArea.Height / 3);
        DoubleBuffered = true;
        _drawingArea = new DrawingArea();
        _treeSelectionLabel = new Label();
        _treeSelectionInput = new ComboBox();
        _operationSelectionLabel = new Label();
        _operationSelectionInput = new ComboBox();
        _valueInputLabel = new Label();
        _valueInput = new NumericUpDown();
        _button = new Button();

        _operations = new Operation[]
        {
            new Operation("Додати елемент", true, "Додати", (tree, value) =>
            {
                tree.Insert(value);
            }),
            new Operation("Видалити елемент", true, "Видалити", (tree, value) =>
            {
                try
                {
                    tree.Delete(value);
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show($"Елемента зі значенням {value} не існує в дереві", "Помилка");
                }
            }),
            new Operation("Знайти елемент", true, "Знайти", (tree, value) =>
            {
                var foundNode = tree.Search(value);
                _drawingArea.HighlightedNode = foundNode;
                if (foundNode == null)
                {
                    MessageBox.Show($"Елемента зі значенням {value} немає у дереві", "Увага");
                }
            }),
            new Operation("Мінімум", false, "Знайти", (tree, value) =>
            {
                _drawingArea.HighlightedNode = tree.Minimum();
            }),
            new Operation("Максимум", false, "Знайти", (tree, value) =>
            {
                _drawingArea.HighlightedNode = tree.Maximum();
            }),
            new Operation("Попередник", true, "Знайти", (tree, value) =>
            {
                try
                {
                    _drawingArea.HighlightedNode = tree.Predecessor(value);
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show($"Елемента зі значенням {value} не існує в дереві", "Помилка");
                }
            }),
            new Operation("Наступник", true, "Знайти", (tree, value) =>
            {
                try
                {
                    _drawingArea.HighlightedNode = tree.Successor(value);
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show($"Елемента зі значенням {value} не існує в дереві", "Помилка");
                }
            }),
            new Operation("Прямий обхід", false, "Далі", (tree, value) =>
            {
                _treeWalk ??= tree.PreorderTreeWalk().GetEnumerator();
                if (_treeWalk.MoveNext())
                {
                    _drawingArea.HighlightedNode = _treeWalk.Current;
                }
                else
                {
                    _drawingArea.HighlightedNode = null;
                    _treeWalk = null;
                }
            }),
            new Operation("Центрований обхід", false, "Далі", (tree, value) =>
            {
                _treeWalk ??= tree.InorderTreeWalk().GetEnumerator();
                if (_treeWalk.MoveNext())
                {
                    _drawingArea.HighlightedNode = _treeWalk.Current;
                }
                else
                {
                    _drawingArea.HighlightedNode = null;
                    _treeWalk = null;
                }
            }),
            new Operation("Зворотній обхід", false, "Далі", (tree, value) =>
            {
                _treeWalk ??= tree.PostorderTreeWalk().GetEnumerator();
                if (_treeWalk.MoveNext())
                {
                    _drawingArea.HighlightedNode = _treeWalk.Current;
                }
                else
                {
                    _drawingArea.HighlightedNode = null;
                    _treeWalk = null;
                }
            }),
        };
        _treeTypes = new Dictionary<string, ITreeFactory>
        {
            ["Звичайне"] = new BSTFactory(),
            ["Червоно-чорне"] = new RBTFactory(),
            ["AVL-дерево"] = new AVLFactory(),
        };

        SetupControls();
    }

    private void SetupControls()
    {
        Controls.Add(_drawingArea);

        _treeSelectionLabel.Text = "Оберіть тип бінарного дерева пошуку:";
        _treeSelectionLabel.TextAlign = ContentAlignment.BottomCenter;
        Controls.Add(_treeSelectionLabel);

        _treeSelectionInput.DropDownStyle = ComboBoxStyle.DropDownList;
        _treeSelectionInput.Items.AddRange(_treeTypes.Keys.ToArray());
        _treeSelectionInput.SelectedIndexChanged += (source, e) =>
        {
            if (_treeSelectionInput.SelectedIndex == -1)
            {
                return;
            }
            var newTreeName = (string)_treeSelectionInput.SelectedItem;
            if (newTreeName == _selectedTreeName)
            {
                return;
            }
            _selectedTreeName = newTreeName;
            _tree = _treeTypes[_selectedTreeName].GetTree();
            _treeWalk = null;
            _drawingArea.Tree = _tree;
            _operationSelectionLabel.Visible = true;
            _operationSelectionInput.Visible = true;
            _drawingArea.Invalidate();
        };
        Controls.Add(_treeSelectionInput);

        _operationSelectionLabel.Text = "Оберіть операцію:";
        _operationSelectionLabel.TextAlign = ContentAlignment.BottomCenter;
        _operationSelectionLabel.Visible = false;
        Controls.Add(_operationSelectionLabel);

        _operationSelectionInput.DropDownStyle = ComboBoxStyle.DropDownList;
        _operationSelectionInput.Items.AddRange(_operations);
        _operationSelectionInput.SelectedIndexChanged += (source, e) =>
        {
            if (_operationSelectionInput.SelectedIndex == -1)
            {
                return;
            }
            var operation = (Operation)_operationSelectionInput.SelectedItem;
            if (operation.Action == _action)
            {
                return;
            }
            if (operation.NeedsValueInput)
            {
                _valueInput.Visible = true;
                _valueInputLabel.Visible = true;
            }
            else
            {
                _valueInput.Visible = false;
                _valueInputLabel.Visible = false;
            }
            _button.Text = operation.ButtonText;
            _button.Visible = true;
            _action = operation.Action;
            _treeWalk = null;
            RepositionSidebar();
        };
        _operationSelectionInput.Visible = false;
        Controls.Add(_operationSelectionInput);

        _valueInputLabel.Text = "Введіть значення елемента:";
        _valueInputLabel.TextAlign = ContentAlignment.BottomCenter;
        _valueInputLabel.Visible = false;
        Controls.Add(_valueInputLabel);

        _valueInput.Minimum = -1000;
        _valueInput.Maximum = 1000;
        _valueInput.Visible = false;
        Controls.Add(_valueInput);

        AcceptButton = _button;
        _button.Click += (source, e) =>
        {
            if (_tree == null || _action == null)
            {
                return;
            }
            _drawingArea.HighlightedNode = null;
            _action(_tree, (int)_valueInput.Value);
            _drawingArea.Invalidate();
        };
        _button.Visible = false;
        Controls.Add(_button);

        ClientSizeChanged += (source, e) =>
        {
            RepositionSidebar();
        };

        RepositionSidebar();
    }

    private void RepositionSidebar()
    {
        int drawingAreaWidth = ClientSize.Width * 4 / 5;
        int controlsWidth = (ClientSize.Width - drawingAreaWidth) * 4 / 5;
        int labelHeight = ClientSize.Height / 6;
        var labelSize = new Size(controlsWidth, labelHeight);
        var font = new Font(Font.FontFamily, Math.Max(1F, Math.Min(labelHeight / 8f, controlsWidth / 16f)));

        _drawingArea.ClientSize = new Size(drawingAreaWidth, ClientSize.Height);
        _drawingArea.Invalidate();

        _treeSelectionLabel.Size = labelSize;
        _treeSelectionLabel.Font = font;
        _treeSelectionLabel.Location = new Point(drawingAreaWidth, 0);

        _treeSelectionInput.Width = controlsWidth;
        _treeSelectionInput.Font = font;
        _treeSelectionInput.Location = new Point(drawingAreaWidth, _treeSelectionLabel.Bottom + _treeSelectionInput.Height / 2);

        _operationSelectionLabel.Size = labelSize;
        _operationSelectionLabel.Font = font;
        _operationSelectionLabel.Location = new Point(drawingAreaWidth, _treeSelectionInput.Bottom);

        _operationSelectionInput.Width = controlsWidth;
        _operationSelectionInput.Font = font;
        _operationSelectionInput.Location = new Point(drawingAreaWidth, _operationSelectionLabel.Bottom + _operationSelectionInput.Height / 2);

        _valueInputLabel.Size = labelSize;
        _valueInputLabel.Font = font;
        _valueInputLabel.Location = new Point(drawingAreaWidth, _operationSelectionInput.Bottom);

        _valueInput.Width = controlsWidth;
        _valueInput.Font = font;
        _valueInput.Location = new Point(drawingAreaWidth, _valueInputLabel.Bottom + _valueInput.Height / 2);

        _button.Size = new Size(controlsWidth, labelHeight / 2);
        _button.Font = font;
        if (_valueInput.Visible)
        {
            _button.Location = new Point(drawingAreaWidth, _valueInput.Bottom + _button.Height / 2);
        }
        else
        {
            _button.Location = new Point(drawingAreaWidth, _operationSelectionInput.Bottom + _button.Height / 2);
        }
    }
}
