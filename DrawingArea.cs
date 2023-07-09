namespace Trees;

internal class DrawingArea : Panel
{
    private readonly Color _backColor;
    private readonly Color _foreColor;
    private readonly Color _highlightColor;
    private readonly Brush _foreBrush;
    private readonly Brush _backBrush;

    private readonly Dictionary<INode, Rectangle> _nodesCoords;

    private Font? _nodeTextFont;

    public DrawingArea()
    {
        DoubleBuffered = true;
        _backColor = Color.White;
        _foreColor = Color.Black;
        _highlightColor = Color.Lime;
        _foreBrush = new SolidBrush(_foreColor);
        _backBrush = new SolidBrush(_backColor);
        _nodesCoords = new Dictionary<INode, Rectangle>();
    }

    public ITree? Tree { get; set; }
    public INode? HighlightedNode { get; set; }

    protected override void OnPaint(PaintEventArgs e)
    {
        UpdateNodesCoords();
        if (_nodesCoords.Count == 0)
        {
            e.Graphics.Clear(FindForm()!.BackColor); 
        }
        else
        {
            DrawTree(e.Graphics);
        }
    }

    private void UpdateNodesCoords()
    {
        _nodesCoords.Clear();
        if (Tree == null || Tree.IsEmpty)
        {
            return;
        }

        var nodesByLevels = Tree.GetEnumeratedNodesByLevels()!;
        int maxTreeWidth = (int)Math.Pow(2, nodesByLevels.Count - 1);
        int nodeDiameter = (int)(Math.Min(Width, Height) * 0.9 / maxTreeWidth);
        var nodeSize = new Size(nodeDiameter, nodeDiameter);
        _nodeTextFont = new Font(Font.FontFamily, Math.Max(nodeDiameter / 4, 1));

        int heightPerNode = Height / nodesByLevels.Count;
        int maxNodesInLevel = 1;

        for (int level = 0; level < nodesByLevels.Count; level++)
        {
            int widthPerNode = Width / maxNodesInLevel;
            foreach (var node in nodesByLevels[level])
            {
                int x = widthPerNode * (node.Item2 - 1) + (widthPerNode - nodeDiameter) / 2;
                int y = heightPerNode * level + (heightPerNode - nodeDiameter) / 2;
                _nodesCoords[node.Item1] = new Rectangle(new Point(x, y), nodeSize);
            }
            maxNodesInLevel *= 2;
        }
    }

    private void DrawTree(Graphics graphics)
    {
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

        var penWidth = _nodesCoords.Values.FirstOrDefault().Width / 40F;
        var pen = new Pen(_foreColor, penWidth);

        if (_nodesCoords.Keys.First() is RBTNode)
        {
            foreach (var node in _nodesCoords.Keys)
            {
                DrawNodeChildrenEdges(node, graphics, pen);
                DrawRBTNode((RBTNode)node, graphics);
            }
        }
        else
        {
            foreach (var node in _nodesCoords.Keys)
            {
                DrawNodeChildrenEdges(node, graphics, pen);
                DrawNode(node, graphics, pen);
            }
        }

        if (HighlightedNode != null)
        {
            using var highlightPen = new Pen(_highlightColor, 3 * penWidth);
            graphics.DrawEllipse(highlightPen, _nodesCoords[HighlightedNode]);
        }
    }

    private void DrawNode(INode node, Graphics graphics, Pen pen)
    {
        var nodeRectangle = _nodesCoords[node];
        graphics.FillEllipse(_backBrush, nodeRectangle);
        graphics.DrawString
        (
            node.Data.ToString(),
            _nodeTextFont!,
            _foreBrush, nodeRectangle, new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.FitBlackBox,
            }
        );
        graphics.DrawEllipse(pen, nodeRectangle);
    }

    private void DrawRBTNode(RBTNode node, Graphics graphics) 
    {
        var nodeRectangle = _nodesCoords[node];
        Brush brush;
        if (node.IsBlack)
        {
            brush = Brushes.Black;
        }
        else
        {
            brush = Brushes.Red;
        }
        graphics.FillEllipse(brush, nodeRectangle);
        graphics.DrawString
        (
            node.Data.ToString(),
            _nodeTextFont!,
            Brushes.White, nodeRectangle, new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.FitBlackBox,
            }
        );
    }

    private void DrawNodeChildrenEdges(INode node, Graphics graphics, Pen pen)
    {
        var nodeRectangle = _nodesCoords[node];
        int startX = nodeRectangle.X + nodeRectangle.Width / 2;
        int startY = nodeRectangle.Y + nodeRectangle.Height / 2;
        if (node.Left != null && _nodesCoords.ContainsKey(node.Left))
        {
            var childRectangle = _nodesCoords[node.Left];
            int endX = childRectangle.X + childRectangle.Width / 2;
            int endY = childRectangle.Y + childRectangle.Height / 2;
            graphics.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));
        }
        if (node.Right != null && _nodesCoords.ContainsKey(node.Right))
        {
            var childRectangle = _nodesCoords[node.Right];
            int endX = childRectangle.X + childRectangle.Width / 2;
            int endY = childRectangle.Y + childRectangle.Height / 2;
            graphics.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));
        }
    }
}
