namespace KoPilot_vA
{
    /// <summary>
    /// Lightweight autocomplete popup that floats over the editor.
    /// Driven by <see cref="RoslynIntelliSenseService"/> completion candidates.
    /// </summary>
    internal sealed class CompletionPopup : Form
    {
        private readonly ListBox _list;
        private List<CompletionCandidate> _items = new();
        private List<CompletionCandidate> _visibleItems = new();

        public event EventHandler<CompletionCandidate>? ItemCommitted;

        public CompletionPopup()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.FromArgb(40, 40, 40);
            Size = new Size(320, 180);

            _list = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.FromArgb(220, 220, 220),
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 10f),
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 20
            };
            _list.DrawItem += List_DrawItem;
            _list.MouseDoubleClick += (_, _) => CommitSelected();
            _list.KeyDown += List_KeyDown;

            Controls.Add(_list);
        }

        // ?? Public API ?????????????????????????????????????????????????????????

        public bool HasItems => _list.Items.Count > 0;

        public void SetItems(IReadOnlyList<CompletionCandidate> items)
        {
            _items = items.Take(50).ToList();
            _visibleItems = _items;
            _list.BeginUpdate();
            _list.Items.Clear();
            foreach (var item in _visibleItems)
                _list.Items.Add(item.DisplayText);
            if (_list.Items.Count > 0)
                _list.SelectedIndex = 0;
            _list.EndUpdate();

            Height = Math.Min(_visibleItems.Count * _list.ItemHeight + 4, 200);
        }

        public void FilterBy(string prefix)
        {
            _visibleItems = string.IsNullOrEmpty(prefix)
                ? _items
                : _items.Where(i => i.DisplayText.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();

            _list.BeginUpdate();
            _list.Items.Clear();
            foreach (var item in _visibleItems)
                _list.Items.Add(item.DisplayText);
            if (_list.Items.Count > 0)
                _list.SelectedIndex = 0;
            _list.EndUpdate();

            Height = Math.Min(_visibleItems.Count * _list.ItemHeight + 4, 200);
        }

        public void SelectNext()
        {
            if (_list.Items.Count == 0) return;
            _list.SelectedIndex = Math.Min(_list.SelectedIndex + 1, _list.Items.Count - 1);
        }

        public void SelectPrev()
        {
            if (_list.Items.Count == 0) return;
            _list.SelectedIndex = Math.Max(_list.SelectedIndex - 1, 0);
        }

        public void CommitSelected()
        {
            if (_list.SelectedIndex < 0 || _list.SelectedIndex >= _visibleItems.Count) return;
            ItemCommitted?.Invoke(this, _visibleItems[_list.SelectedIndex]);
            Hide();
        }

        // ?? Drawing ????????????????????????????????????????????????????????????

        private void List_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _visibleItems.Count) return;
            e.DrawBackground();

            bool selected = (e.State & DrawItemState.Selected) != 0;
            var backColor = selected ? Color.FromArgb(0, 122, 204) : Color.FromArgb(40, 40, 40);
            var foreColor = Color.FromArgb(220, 220, 220);

            using var backBrush = new SolidBrush(backColor);
            e.Graphics.FillRectangle(backBrush, e.Bounds);

            var item = _visibleItems[e.Index];
            var tagColor = GetTagColor(item.Kind);
            using var tagBrush = new SolidBrush(tagColor);
            e.Graphics.FillRectangle(tagBrush, new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 4, 3, 12));

            using var foreBrush = new SolidBrush(foreColor);
            e.Graphics.DrawString(item.DisplayText, e.Font ?? Font, foreBrush,
                new RectangleF(e.Bounds.X + 10, e.Bounds.Y + 2, e.Bounds.Width - 10, e.Bounds.Height));
        }

        private static Color GetTagColor(string kind) => kind switch
        {
            // WellKnownTags from CompletionService
            "Class" or "Structure" or "Delegate" or "NamedType"
                                 => Color.FromArgb(78,  201, 176),
            "Interface" or "Enum"
                                 => Color.FromArgb(184, 215, 163),
            "Method" or "ExtensionMethod"
                                 => Color.FromArgb(220, 220, 170),
            "Property" or "Field" or "Local" or "Parameter" or "Event"
                                 => Color.FromArgb(156, 220, 254),
            "Namespace"          => Color.FromArgb(220, 220, 220),
            "Keyword"            => Color.FromArgb(86,  156, 214),
            "Snippet"            => Color.FromArgb(200, 200, 140),
            _                    => Color.FromArgb(140, 140, 140)
        };

        // ?? Keyboard ???????????????????????????????????????????????????????????

        private void List_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true;
                CommitSelected();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Hide();
            }
        }

        // Prevent the popup from stealing focus from the editor
        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
                return cp;
            }
        }
    }
}
