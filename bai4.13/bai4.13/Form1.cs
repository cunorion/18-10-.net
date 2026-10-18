
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace bai4._13
{
    public class BookModel
    {
        public string BookId { get; set; } = "";
        public string BookName { get; set; } = "";
        public string Author { get; set; } = "";
        public string Category { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string CoverPath { get; set; } = "";
    }

    public partial class Form1 : Form
    {
        private BindingList<BookModel> books = new BindingList<BookModel>();
        private BindingSource source = new BindingSource();

        private TextBox txtId = new TextBox();
        private TextBox txtName = new TextBox();
        private TextBox txtAuthor = new TextBox();
        private TextBox txtPrice = new TextBox();
        private TextBox txtQuantity = new TextBox();
        private TextBox txtSearch = new TextBox();

        private ComboBox cboCategory = new ComboBox();
        private PictureBox picBookCover = new PictureBox();
        private DataGridView dgvBooks = new DataGridView();

        // Thanh trang thai phai dung ToolStripStatusLabel
        private ToolStripStatusLabel lblTotal = new ToolStripStatusLabel();
        private ToolStripStatusLabel lblValue = new ToolStripStatusLabel();
        private ToolStripStatusLabel lblTime = new ToolStripStatusLabel();

        private string coverPath = "";

        // Ghi ro namespace de tranh loi Timer trung ten
        private System.Windows.Forms.Timer timer =
            new System.Windows.Forms.Timer();

        public Form1()
        {
            InitializeComponent();

            CreateDesign();
            InitData();
            UpdateStatus();

            timer.Interval = 1000;
            timer.Tick += (s, e) =>
            {
                lblTime.Text = "Thoi gian: " +
                    DateTime.Now.ToString("HH:mm:ss");
            };
            timer.Start();
        }

        private void CreateDesign()
        {
            Text = "Bai 4.13 - Quan ly sach nha sach";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1150, 750);
            MinimumSize = new Size(950, 650);

            // MENU
            MenuStrip menu = new MenuStrip();

            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            fileMenu.DropDownItems.Add("Mo CSV", null,
                (s, e) => LoadCsv());
            fileMenu.DropDownItems.Add("Luu CSV", null,
                (s, e) => SaveCsv());
            fileMenu.DropDownItems.Add("Thoat", null,
                (s, e) => Close());

            ToolStripMenuItem editMenu = new ToolStripMenuItem("Edit");
            editMenu.DropDownItems.Add("Them sach", null,
                (s, e) => AddBook());
            editMenu.DropDownItems.Add("Sua sach", null,
                (s, e) => EditBook());
            editMenu.DropDownItems.Add("Xoa sach", null,
                (s, e) => DeleteBook());
            editMenu.DropDownItems.Add("Tim kiem", null,
                (s, e) => SearchBook());

            menu.Items.Add(fileMenu);
            menu.Items.Add(editMenu);
            MainMenuStrip = menu;
            Controls.Add(menu);

            // TOOL STRIP
            ToolStrip tool = new ToolStrip();
            tool.Dock = DockStyle.Top;

            tool.Items.Add(new ToolStripButton(
                "Mo CSV", SystemIcons.Application.ToBitmap(),
                (s, e) => LoadCsv()));

            tool.Items.Add(new ToolStripButton(
                "Luu CSV", SystemIcons.Shield.ToBitmap(),
                (s, e) => SaveCsv()));

            tool.Items.Add(new ToolStripButton(
                "Them sach", SystemIcons.Information.ToBitmap(),
                (s, e) => AddBook()));

            Controls.Add(tool);

            // VUNG NHAP THONG TIN
            GroupBox group = new GroupBox();
            group.Text = "Thong tin sach";
            group.SetBounds(10, 65, 1110, 235);
            Controls.Add(group);

            AddLabel(group, "Ma sach:", 20, 30);
            txtId.SetBounds(105, 27, 190, 27);
            txtId.ReadOnly = true;
            group.Controls.Add(txtId);

            AddLabel(group, "Ten sach:", 20, 70);
            txtName.SetBounds(105, 67, 190, 27);
            group.Controls.Add(txtName);

            AddLabel(group, "Tac gia:", 20, 110);
            txtAuthor.SetBounds(105, 107, 190, 27);
            group.Controls.Add(txtAuthor);

            AddLabel(group, "The loai:", 20, 150);
            cboCategory.SetBounds(105, 147, 190, 27);
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategory.Items.AddRange(new object[]
            {
                "Cong nghe", "Van hoc", "Kinh te",
                "Ky nang song", "Giao duc", "Khac"
            });
            cboCategory.SelectedIndex = 0;
            group.Controls.Add(cboCategory);

            AddLabel(group, "Don gia:", 320, 30);
            txtPrice.SetBounds(390, 27, 160, 27);
            group.Controls.Add(txtPrice);

            AddLabel(group, "So luong:", 320, 70);
            txtQuantity.SetBounds(390, 67, 160, 27);
            group.Controls.Add(txtQuantity);

            Button btnCover = new Button();
            btnCover.Text = "Chon anh bia";
            btnCover.SetBounds(320, 110, 115, 32);
            btnCover.Click += (s, e) => ChooseCover();
            group.Controls.Add(btnCover);

            picBookCover.SetBounds(445, 105, 105, 110);
            picBookCover.BorderStyle = BorderStyle.FixedSingle;
            picBookCover.SizeMode = PictureBoxSizeMode.Zoom;
            group.Controls.Add(picBookCover);

            Button btnAdd = new Button();
            btnAdd.Text = "Them sach";
            btnAdd.SetBounds(590, 30, 105, 32);
            btnAdd.Click += (s, e) => AddBook();
            group.Controls.Add(btnAdd);

            Button btnEdit = new Button();
            btnEdit.Text = "Sua sach";
            btnEdit.SetBounds(705, 30, 105, 32);
            btnEdit.Click += (s, e) => EditBook();
            group.Controls.Add(btnEdit);

            Button btnDelete = new Button();
            btnDelete.Text = "Xoa sach";
            btnDelete.SetBounds(820, 30, 105, 32);
            btnDelete.Click += (s, e) => DeleteBook();
            group.Controls.Add(btnDelete);

            Button btnClear = new Button();
            btnClear.Text = "Lam moi";
            btnClear.SetBounds(935, 30, 105, 32);
            btnClear.Click += (s, e) => ClearInput();
            group.Controls.Add(btnClear);

            txtSearch.SetBounds(590, 85, 220, 27);
            txtSearch.PlaceholderText = "Nhap ma hoac ten sach";
            group.Controls.Add(txtSearch);

            Button btnSearch = new Button();
            btnSearch.Text = "Tim kiem";
            btnSearch.SetBounds(820, 82, 105, 32);
            btnSearch.Click += (s, e) => SearchBook();
            group.Controls.Add(btnSearch);

            Button btnShowAll = new Button();
            btnShowAll.Text = "Hien tat ca";
            btnShowAll.SetBounds(935, 82, 105, 32);
            btnShowAll.Click += (s, e) =>
            {
                source.DataSource = books;
                dgvBooks.DataSource = source;
                txtSearch.Clear();
            };
            group.Controls.Add(btnShowAll);

            // BANG DANH SACH SACH
            dgvBooks.SetBounds(10, 310, 1110, 330);
            dgvBooks.Anchor = AnchorStyles.Top |
                              AnchorStyles.Bottom |
                              AnchorStyles.Left |
                              AnchorStyles.Right;

            dgvBooks.AutoGenerateColumns = false;
            dgvBooks.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvBooks.MultiSelect = false;
            dgvBooks.AllowUserToAddRows = false;
            dgvBooks.ReadOnly = true;
            dgvBooks.RowHeadersVisible = false;
            dgvBooks.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvBooks.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Ma sach",
                DataPropertyName = "BookId",
                FillWeight = 65
            });

            dgvBooks.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Ten sach",
                DataPropertyName = "BookName",
                FillWeight = 130
            });

            dgvBooks.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Tac gia",
                DataPropertyName = "Author",
                FillWeight = 100
            });

            dgvBooks.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "The loai",
                DataPropertyName = "Category",
                FillWeight = 90
            });

            dgvBooks.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Don gia",
                DataPropertyName = "Price",
                FillWeight = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0"
                }
            });

            dgvBooks.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "So luong",
                DataPropertyName = "Quantity",
                FillWeight = 65
            });

            dgvBooks.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Anh bia",
                DataPropertyName = "CoverPath",
                FillWeight = 120
            });

            Controls.Add(dgvBooks);
            dgvBooks.SelectionChanged +=
                (s, e) => ShowSelectedBook();

            // STATUS STRIP
            StatusStrip status = new StatusStrip();

            lblTotal.Text = "So dau sach: 0";
            lblValue.Text = "Tong gia tri kho: 0 VND";
            lblTime.Text = "Thoi gian: ";

            status.Items.Add(lblTotal);
            status.Items.Add(new ToolStripStatusLabel(" | "));
            status.Items.Add(lblValue);
            status.Items.Add(new ToolStripStatusLabel
            {
                Spring = true
            });
            status.Items.Add(lblTime);

            Controls.Add(status);
        }

        private void AddLabel(
            Control parent, string text, int x, int y)
        {
            Label label = new Label();
            label.Text = text;
            label.SetBounds(x, y, 80, 25);
            parent.Controls.Add(label);
        }

        private void InitData()
        {
            books.Add(new BookModel
            {
                BookId = "S001",
                BookName = "Lap trinh C#",
                Author = "Nguyen Van A",
                Category = "Cong nghe",
                Price = 85000,
                Quantity = 10
            });

            books.Add(new BookModel
            {
                BookId = "S002",
                BookName = "Dac nhan tam",
                Author = "Dale Carnegie",
                Category = "Ky nang song",
                Price = 75000,
                Quantity = 5
            });

            source.DataSource = books;
            dgvBooks.DataSource = source;
        }

        private string GenerateId()
        {
            int max = 0;

            foreach (BookModel b in books)
            {
                string number = b.BookId.Length > 1
                    ? b.BookId.Substring(1) : "0";

                if (int.TryParse(number, out int n) && n > max)
                    max = n;
            }

            return "S" + (max + 1).ToString("D3");
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Vui long nhap ten sach va tac gia!");
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) ||
                price < 0)
            {
                MessageBox.Show("Don gia khong hop le!");
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) ||
                quantity < 0)
            {
                MessageBox.Show("So luong khong hop le!");
                return false;
            }

            return true;
        }

        private BookModel ReadInput(string id)
        {
            decimal.TryParse(txtPrice.Text, out decimal price);
            int.TryParse(txtQuantity.Text, out int quantity);

            return new BookModel
            {
                BookId = id,
                BookName = txtName.Text.Trim(),
                Author = txtAuthor.Text.Trim(),
                Category = cboCategory.Text,
                Price = price,
                Quantity = quantity,
                CoverPath = coverPath
            };
        }

        private void AddBook()
        {
            if (!ValidateInput())
                return;

            books.Add(ReadInput(GenerateId()));

            ShowAllBooks();
            UpdateStatus();
            ClearInput();

            MessageBox.Show("Them sach thanh cong!");
        }

        private void EditBook()
        {
            if (dgvBooks.CurrentRow == null ||
                dgvBooks.CurrentRow.DataBoundItem is not BookModel selected)
            {
                MessageBox.Show("Vui long chon sach can sua!");
                return;
            }

            if (!ValidateInput())
                return;

            BookModel updated = ReadInput(selected.BookId);

            selected.BookName = updated.BookName;
            selected.Author = updated.Author;
            selected.Category = updated.Category;
            selected.Price = updated.Price;
            selected.Quantity = updated.Quantity;
            selected.CoverPath = updated.CoverPath;

            ShowAllBooks();
            UpdateStatus();
            ClearInput();

            MessageBox.Show("Cap nhat sach thanh cong!");
        }

        private void DeleteBook()
        {
            if (dgvBooks.CurrentRow == null ||
                dgvBooks.CurrentRow.DataBoundItem is not BookModel selected)
            {
                MessageBox.Show("Vui long chon sach can xoa!");
                return;
            }

            DialogResult answer = MessageBox.Show(
                "Ban co chac muon xoa sach nay?",
                "Xac nhan",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer == DialogResult.Yes)
            {
                books.Remove(selected);
                ShowAllBooks();
                UpdateStatus();
                ClearInput();
            }
        }

        private void SearchBook()
        {
            string keyword = txtSearch.Text.Trim();

            if (keyword == "")
            {
                ShowAllBooks();
                return;
            }

            var result = books.Where(b =>
                b.BookId.Contains(keyword,
                    StringComparison.OrdinalIgnoreCase) ||
                b.BookName.Contains(keyword,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            source.DataSource = new BindingList<BookModel>(result);
            dgvBooks.DataSource = source;
        }

        private void ShowAllBooks()
        {
            source.DataSource = books;
            dgvBooks.DataSource = source;
        }

        private void ChooseCover()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Chon anh bia sach";
                dialog.Filter =
                    "Anh (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    coverPath = dialog.FileName;

                    if (picBookCover.Image != null)
                    {
                        picBookCover.Image.Dispose();
                        picBookCover.Image = null;
                    }

                    using (Image img = Image.FromFile(coverPath))
                    {
                        picBookCover.Image = new Bitmap(img);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Khong mo duoc anh: " + ex.Message);
                }
            }
        }

        private void ShowSelectedBook()
        {
            if (dgvBooks.CurrentRow?.DataBoundItem
                is not BookModel book)
                return;

            txtId.Text = book.BookId;
            txtName.Text = book.BookName;
            txtAuthor.Text = book.Author;
            cboCategory.Text = book.Category;
            txtPrice.Text = book.Price.ToString(
                CultureInfo.InvariantCulture);
            txtQuantity.Text = book.Quantity.ToString();
            coverPath = book.CoverPath;

            if (picBookCover.Image != null)
            {
                picBookCover.Image.Dispose();
                picBookCover.Image = null;
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(coverPath) &&
                    File.Exists(coverPath))
                {
                    using (Image img = Image.FromFile(coverPath))
                    {
                        picBookCover.Image = new Bitmap(img);
                    }
                }
            }
            catch
            {
                picBookCover.Image = null;
            }
        }

        private void ClearInput()
        {
            txtId.Clear();
            txtName.Clear();
            txtAuthor.Clear();
            txtPrice.Clear();
            txtQuantity.Clear();
            cboCategory.SelectedIndex = 0;
            coverPath = "";

            if (picBookCover.Image != null)
            {
                picBookCover.Image.Dispose();
                picBookCover.Image = null;
            }

            dgvBooks.ClearSelection();
        }

        private void UpdateStatus()
        {
            lblTotal.Text = "So dau sach: " + books.Count;

            decimal total = books.Sum(b => b.Price * b.Quantity);

            lblValue.Text = "Tong gia tri kho: " +
                total.ToString("N0") + " VND";
        }

        private void SaveCsv()
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Luu danh sach sach";
                dialog.Filter = "CSV (*.csv)|*.csv";
                dialog.FileName = "DanhSachSach.csv";

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    using (StreamWriter writer = new StreamWriter(
                        dialog.FileName, false, Encoding.UTF8))
                    {
                        writer.WriteLine(
                            "BookId,BookName,Author,Category,Price,Quantity,CoverPath");

                        foreach (BookModel b in books)
                        {
                            writer.WriteLine(string.Join(",",
                                CsvEscape(b.BookId),
                                CsvEscape(b.BookName),
                                CsvEscape(b.Author),
                                CsvEscape(b.Category),
                                b.Price.ToString(
                                    CultureInfo.InvariantCulture),
                                b.Quantity.ToString(),
                                CsvEscape(b.CoverPath)));
                        }
                    }

                    MessageBox.Show("Luu CSV thanh cong!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Loi khi luu CSV: " + ex.Message);
                }
            }
        }

        private string CsvEscape(string value)
        {
            value ??= "";

            if (value.Contains(',') || value.Contains('"') ||
                value.Contains('\n') || value.Contains('\r'))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }

        private void LoadCsv()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Mo danh sach sach";
                dialog.Filter = "CSV (*.csv)|*.csv";

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    string[] lines = File.ReadAllLines(
                        dialog.FileName, Encoding.UTF8);

                    BindingList<BookModel> imported =
                        new BindingList<BookModel>();

                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(lines[i]))
                            continue;

                        string[] f = ParseCsvLine(lines[i]);

                        if (f.Length < 7)
                            continue;

                        if (!decimal.TryParse(f[4],
                            NumberStyles.Any,
                            CultureInfo.InvariantCulture,
                            out decimal price))
                            continue;

                        if (!int.TryParse(f[5], out int quantity))
                            continue;

                        imported.Add(new BookModel
                        {
                            BookId = f[0],
                            BookName = f[1],
                            Author = f[2],
                            Category = f[3],
                            Price = price,
                            Quantity = quantity,
                            CoverPath = f[6]
                        });
                    }

                    books = imported;
                    ShowAllBooks();
                    UpdateStatus();
                    ClearInput();

                    MessageBox.Show(
                        "Da nhap " + books.Count + " dau sach.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Loi doc CSV: " + ex.Message);
                }
            }
        }

        private string[] ParseCsvLine(string line)
        {
            var fields = new System.Collections.Generic.List<string>();
            StringBuilder field = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length &&
                        line[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(field.ToString());
                    field.Clear();
                }
                else
                {
                    field.Append(c);
                }
            }

            fields.Add(field.ToString());
            return fields.ToArray();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            timer.Stop();
            timer.Dispose();

            if (picBookCover.Image != null)
            {
                picBookCover.Image.Dispose();
                picBookCover.Image = null;
            }

            base.OnFormClosed(e);
        }
    }
}
