
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace bai4._12
{
    public class MemberModel
    {
        public string MemberId { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Package { get; set; } = "";
        public DateTime StartDate { get; set; } = DateTime.Today;
        public decimal Fee { get; set; }
    }

    public partial class Form1 : Form
    {
        private TextBox txtMemberId;
        private TextBox txtFullName;
        private TextBox txtPhone;
        private TextBox txtFee;

        private ComboBox cboPackage;
        private DateTimePicker dtpStartDate;
        private DataGridView dgvMembers;

        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;

        private ErrorProvider errorProvider;

        private BindingList<MemberModel> members;
        private BindingSource bindingSource;

        private bool isLoading = false;

        public Form1()
        {
            InitializeComponent();
            CreateDesign();
            InitData();
        }

        // Tao giao dien bang code
        private void CreateDesign()
        {
            Controls.Clear();

            Text = "QUAN LY HOI VIEN PHONG GYM";
            Size = new Size(1000, 680);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.WhiteSmoke;
            Font = new Font("Segoe UI", 10);

            Label title = new Label();
            title.Text = "HE THONG QUAN LY HOI VIEN GYM";
            title.Font = new Font("Segoe UI", 17, FontStyle.Bold);
            title.ForeColor = Color.DarkBlue;
            title.TextAlign = ContentAlignment.MiddleCenter;
            title.SetBounds(20, 10, 940, 40);
            Controls.Add(title);

            GroupBox grpInfo = new GroupBox();
            grpInfo.Text = "THONG TIN HOI VIEN";
            grpInfo.SetBounds(20, 60, 940, 210);
            Controls.Add(grpInfo);

            AddLabel(grpInfo, "Ma hoi vien:", 20, 30);
            txtMemberId = new TextBox();
            txtMemberId.SetBounds(150, 28, 250, 28);
            txtMemberId.ReadOnly = true;
            grpInfo.Controls.Add(txtMemberId);

            AddLabel(grpInfo, "Ho va ten:", 20, 75);
            txtFullName = new TextBox();
            txtFullName.SetBounds(150, 73, 250, 28);
            grpInfo.Controls.Add(txtFullName);

            AddLabel(grpInfo, "So dien thoai:", 20, 120);
            txtPhone = new TextBox();
            txtPhone.SetBounds(150, 118, 250, 28);
            grpInfo.Controls.Add(txtPhone);

            AddLabel(grpInfo, "Goi tap:", 480, 30);
            cboPackage = new ComboBox();
            cboPackage.SetBounds(600, 28, 280, 28);
            cboPackage.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPackage.Items.AddRange(new object[]
            {
                "Theo thang",
                "6 thang",
                "1 nam"
            });
            grpInfo.Controls.Add(cboPackage);

            AddLabel(grpInfo, "Ngay dang ky:", 480, 75);
            dtpStartDate = new DateTimePicker();
            dtpStartDate.SetBounds(600, 73, 280, 28);
            dtpStartDate.Format = DateTimePickerFormat.Short;
            dtpStartDate.MaxDate = DateTime.Today;
            grpInfo.Controls.Add(dtpStartDate);

            AddLabel(grpInfo, "Phi dich vu:", 480, 120);
            txtFee = new TextBox();
            txtFee.SetBounds(600, 118, 280, 28);
            grpInfo.Controls.Add(txtFee);

            btnAdd = CreateButton("Them moi", 20, 285);
            btnEdit = CreateButton("Sua thong tin", 190, 285);
            btnDelete = CreateButton("Xoa hoi vien", 360, 285);
            btnRefresh = CreateButton("Lam moi", 530, 285);

            Controls.AddRange(new Control[]
            {
                btnAdd, btnEdit, btnDelete, btnRefresh
            });

            GroupBox grpGrid = new GroupBox();
            grpGrid.Text = "DANH SACH HOI VIEN";
            grpGrid.SetBounds(20, 340, 940, 285);
            Controls.Add(grpGrid);

            dgvMembers = new DataGridView();
            dgvMembers.Name = "dgvMembers";
            dgvMembers.SetBounds(10, 25, 920, 250);

            // Cau hinh DataGridView
            dgvMembers.AutoGenerateColumns = false;
            dgvMembers.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvMembers.MultiSelect = false;
            dgvMembers.ReadOnly = true;
            dgvMembers.AllowUserToAddRows = false;
            dgvMembers.AllowUserToDeleteRows = false;
            dgvMembers.RowHeadersVisible = false;
            dgvMembers.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvMembers.BackgroundColor = Color.White;

            AddColumn("MemberId", "Ma HV");
            AddColumn("FullName", "Ten hoi vien");
            AddColumn("Phone", "SDT");
            AddColumn("Package", "Goi tap");
            AddColumn("StartDate", "Ngay DK", "dd/MM/yyyy");
            AddColumn("Fee", "Phi dich vu", "N0");

            grpGrid.Controls.Add(dgvMembers);

            errorProvider = new ErrorProvider();
            errorProvider.ContainerControl = this;

            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            dgvMembers.SelectionChanged += dgvMembers_SelectionChanged;
        }

        private void AddLabel(
            Control parent, string text, int x, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.SetBounds(x, y, 120, 28);
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            parent.Controls.Add(lbl);
        }

        private Button CreateButton(string text, int x, int y)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.SetBounds(x, y, 150, 38);
            btn.BackColor = Color.LightSteelBlue;
            btn.FlatStyle = FlatStyle.Flat;
            return btn;
        }

        private void AddColumn(
            string propertyName,
            string headerText,
            string format = "")
        {
            DataGridViewTextBoxColumn col =
                new DataGridViewTextBoxColumn();

            col.Name = propertyName;
            col.HeaderText = headerText;
            col.DataPropertyName = propertyName;
            col.SortMode = DataGridViewColumnSortMode.Automatic;

            if (format != "")
                col.DefaultCellStyle.Format = format;

            dgvMembers.Columns.Add(col);
        }

        // Khoi tao du lieu
        private void InitData()
        {
            members = new BindingList<MemberModel>();
            bindingSource = new BindingSource();
            bindingSource.DataSource = members;

            dgvMembers.DataSource = bindingSource;

            members.Add(new MemberModel
            {
                MemberId = "HV001",
                FullName = "Nguyen Van An",
                Phone = "0912345678",
                Package = "Theo thang",
                StartDate = DateTime.Today,
                Fee = 500000
            });

            members.Add(new MemberModel
            {
                MemberId = "HV002",
                FullName = "Tran Thi Binh",
                Phone = "0987654321",
                Package = "6 thang",
                StartDate = DateTime.Today,
                Fee = 2700000
            });

            ClearInput();
        }

        // Tu dong sinh ma hoi vien
        private string GenerateMemberId()
        {
            int maxId = 0;

            foreach (MemberModel m in members)
            {
                int number;

                if (int.TryParse(
                    m.MemberId.Replace("HV", ""), out number))
                {
                    if (number > maxId)
                        maxId = number;
                }
            }

            return "HV" + (maxId + 1).ToString("D3");
        }

        // Kiem tra du lieu
        private bool ValidateInput()
        {
            errorProvider.Clear();
            bool valid = true;

            string name = txtFullName.Text.Trim();

            if (name == "")
            {
                errorProvider.SetError(
                    txtFullName, "Ho ten khong duoc de trong!");
                valid = false;
            }
            else if (!Regex.IsMatch(name, @"^[\p{L}\s]+$"))
            {
                errorProvider.SetError(
                    txtFullName,
                    "Ho ten chi duoc chua chu cai va khoang trang!");
                valid = false;
            }

            if (!Regex.IsMatch(txtPhone.Text.Trim(), @"^\d{10}$"))
            {
                errorProvider.SetError(
                    txtPhone,
                    "So dien thoai phai gom dung 10 chu so!");
                valid = false;
            }

            decimal fee;

            if (!decimal.TryParse(txtFee.Text.Trim(), out fee)
                || fee <= 0
                || fee != decimal.Truncate(fee))
            {
                errorProvider.SetError(
                    txtFee,
                    "Phi dich vu phai la so nguyen duong!");
                valid = false;
            }

            if (cboPackage.SelectedIndex == -1)
            {
                errorProvider.SetError(
                    cboPackage, "Vui long chon goi tap!");
                valid = false;
            }

            if (dtpStartDate.Value.Date > DateTime.Today)
            {
                errorProvider.SetError(
                    dtpStartDate,
                    "Ngay dang ky khong duoc lon hon hom nay!");
                valid = false;
            }

            return valid;
        }

        // Lay thong tin tu cac o nhap
        private MemberModel GetInputMember(string id)
        {
            return new MemberModel
            {
                MemberId = id,
                FullName = txtFullName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Package = cboPackage.Text,
                StartDate = dtpStartDate.Value.Date,
                Fee = decimal.Parse(txtFee.Text.Trim())
            };
        }

        // Them hoi vien
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            MemberModel member =
                GetInputMember(GenerateMemberId());

            members.Add(member);
            bindingSource.ResetBindings(false);

            MessageBox.Show(
                "Them hoi vien thanh cong!",
                "Thong bao",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            ClearInput();
        }

        // Sua thong tin
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current == null ||
                dgvMembers.CurrentRow == null)
            {
                MessageBox.Show("Vui long chon hoi vien can sua!");
                return;
            }

            if (!ValidateInput())
                return;

            MemberModel selected =
                (MemberModel)bindingSource.Current;

            MemberModel updated =
                GetInputMember(selected.MemberId);

            selected.FullName = updated.FullName;
            selected.Phone = updated.Phone;
            selected.Package = updated.Package;
            selected.StartDate = updated.StartDate;
            selected.Fee = updated.Fee;

            bindingSource.ResetItem(bindingSource.Position);

            MessageBox.Show(
                "Cap nhat thong tin thanh cong!",
                "Thong bao",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // Xoa hoi vien
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current == null ||
                dgvMembers.CurrentRow == null)
            {
                MessageBox.Show("Vui long chon hoi vien can xoa!");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Ban co chac chan muon xoa hoi vien nay?",
                "Xac nhan xoa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                members.Remove(
                    (MemberModel)bindingSource.Current);

                ClearInput();
            }
        }

        // Lam moi cac o nhap
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearInput();
        }

        private void ClearInput()
        {
            isLoading = true;

            errorProvider.Clear();
            txtMemberId.Text = GenerateMemberId();
            txtFullName.Clear();
            txtPhone.Clear();
            txtFee.Clear();
            cboPackage.SelectedIndex = -1;
            dtpStartDate.Value = DateTime.Today;

            dgvMembers.ClearSelection();
            dgvMembers.CurrentCell = null;

            isLoading = false;
        }

        // Chon dong de hien thi thong tin
        private void dgvMembers_SelectionChanged(
            object sender, EventArgs e)
        {
            if (isLoading ||
                dgvMembers.CurrentRow == null ||
                dgvMembers.CurrentRow.Index < 0)
                return;

            MemberModel member =
                bindingSource.Current as MemberModel;

            if (member == null)
                return;

            isLoading = true;

            txtMemberId.Text = member.MemberId;
            txtFullName.Text = member.FullName;
            txtPhone.Text = member.Phone;
            cboPackage.SelectedItem = member.Package;
            dtpStartDate.Value = member.StartDate;
            txtFee.Text = member.Fee.ToString("0");

            errorProvider.Clear();

            isLoading = false;
        }
    }
}
