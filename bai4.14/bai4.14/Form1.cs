
using System;
using System.Drawing;
using System.Windows.Forms;

namespace bai4._14
{
    public partial class Form1 : Form
    {
        private ToolStripStatusLabel lblDoctor;
        private ToolStripStatusLabel lblStatus;
        private ToolStripProgressBar progressBar;

        public Form1()
        {
            InitializeComponent();
            CreateMainForm();
        }

        private void CreateMainForm()
        {
            Text = "He thong quan ly benh vien";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1200, 750);
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;

            // MENU STRIP
            MenuStrip menu = new MenuStrip();

            // Menu He thong
            ToolStripMenuItem systemMenu =
                new ToolStripMenuItem("He thong");

            systemMenu.DropDownItems.Add(
                "Dang xuat", null,
                (s, e) => Logout());

            systemMenu.DropDownItems.Add(
                "Nap lai du lieu", null,
                (s, e) => ReloadData());

            systemMenu.DropDownItems.Add(
                "Thoat", null,
                (s, e) => Close());

            // Menu Quan ly
            ToolStripMenuItem manageMenu =
                new ToolStripMenuItem("Quan ly");

            manageMenu.DropDownItems.Add(
                "Ho so benh nhan", null,
                (s, e) => OpenChild<frmPatientProfiles>());

            manageMenu.DropDownItems.Add(
                "Danh muc bac si", null,
                (s, e) => OpenChild<frmDoctorSchedule>());

            manageMenu.DropDownItems.Add(
                "Danh muc khoa/phong", null,
                (s, e) => OpenDepartment());

            // Menu Cua so
            ToolStripMenuItem windowMenu =
                new ToolStripMenuItem("Cua so");

            windowMenu.DropDownItems.Add(
                "Sap xep xep tang", null,
                (s, e) => LayoutMdi(MdiLayout.Cascade));

            windowMenu.DropDownItems.Add(
                "Xep ngang", null,
                (s, e) => LayoutMdi(MdiLayout.TileHorizontal));

            windowMenu.DropDownItems.Add(
                "Xep doc", null,
                (s, e) => LayoutMdi(MdiLayout.TileVertical));

            windowMenu.DropDownItems.Add(
                "Dong tat ca", null,
                (s, e) => CloseAllChildren());

            menu.Items.Add(systemMenu);
            menu.Items.Add(manageMenu);
            menu.Items.Add(windowMenu);

            MainMenuStrip = menu;
            Controls.Add(menu);

            // STATUS STRIP
            StatusStrip statusStrip = new StatusStrip();

            lblDoctor = new ToolStripStatusLabel();
            lblDoctor.Text = "Bac si truc: Nguyen Van A";

            lblStatus = new ToolStripStatusLabel();
            lblStatus.Text = "He thong san sang";

            progressBar = new ToolStripProgressBar();
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            progressBar.Width = 150;

            ToolStripStatusLabel spring =
                new ToolStripStatusLabel();
            spring.Spring = true;

            statusStrip.Items.Add(lblDoctor);
            statusStrip.Items.Add(new ToolStripStatusLabel("|"));
            statusStrip.Items.Add(lblStatus);
            statusStrip.Items.Add(spring);
            statusStrip.Items.Add(progressBar);

            Controls.Add(statusStrip);

            // Cap nhat ten cua so dang mo
            MdiChildActivate += (s, e) =>
            {
                if (ActiveMdiChild != null)
                {
                    lblStatus.Text =
                        "Dang mo: " + ActiveMdiChild.Text;
                }
                else
                {
                    lblStatus.Text = "He thong san sang";
                }
            };
        }

        // Mo Form con va ngan chan mo trung
        private void OpenChild<T>() where T : Form, new()
        {
            foreach (Form child in MdiChildren)
            {
                if (child is T)
                {
                    if (child.WindowState == FormWindowState.Minimized)
                    {
                        child.WindowState = FormWindowState.Normal;
                    }

                    child.Activate();
                    return;
                }
            }

            T form = new T();
            form.MdiParent = this;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Show();

            lblStatus.Text = "Dang mo: " + form.Text;
        }

        private void OpenDepartment()
        {
            MessageBox.Show(
                "Chuc nang quan ly khoa/phong co the bo sung sau.",
                "Quan ly khoa/phong",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void CloseAllChildren()
        {
            foreach (Form child in MdiChildren)
            {
                child.Close();
            }

            lblStatus.Text = "Da dong tat ca cua so con";
        }

        private void ReloadData()
        {
            progressBar.Value = 0;
            lblStatus.Text = "Dang nap lai du lieu...";

            // Mo phong thao tac nap du lieu
            for (int i = 0; i <= 100; i += 20)
            {
                progressBar.Value = i;
                Application.DoEvents();
            }

            lblStatus.Text = "Nap lai du lieu thanh cong";
        }

        private void Logout()
        {
            DialogResult result = MessageBox.Show(
                "Ban co chac muon dang xuat?",
                "Dang xuat",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                CloseAllChildren();
                lblDoctor.Text = "Bac si truc: Chua dang nhap";
                lblStatus.Text = "Da dang xuat";
            }
        }
    }

    // FORM CON 1: HO SO BENH NHAN
    public class frmPatientProfiles : Form
    {
        private DataGridView dgvPatients;

        public frmPatientProfiles()
        {
            CreateDesign();
        }

        private void CreateDesign()
        {
            Text = "Ho so benh nhan";
            Size = new Size(800, 450);
            MinimumSize = new Size(600, 350);

            Label title = new Label();
            title.Text = "QUAN LY HO SO BENH NHAN";
            title.Font = new Font("Arial", 15, FontStyle.Bold);
            title.TextAlign = ContentAlignment.MiddleCenter;
            title.Dock = DockStyle.Top;
            title.Height = 50;
            Controls.Add(title);

            dgvPatients = new DataGridView();
            dgvPatients.Dock = DockStyle.Fill;
            dgvPatients.AllowUserToAddRows = true;
            dgvPatients.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvPatients.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvPatients.RowHeadersVisible = false;

            dgvPatients.Columns.Add("PatientId", "Ma benh nhan");
            dgvPatients.Columns.Add("PatientName", "Ho ten");
            dgvPatients.Columns.Add("Gender", "Gioi tinh");
            dgvPatients.Columns.Add("BirthYear", "Nam sinh");
            dgvPatients.Columns.Add("Phone", "So dien thoai");
            dgvPatients.Columns.Add("Address", "Dia chi");

            dgvPatients.Rows.Add(
                "BN001", "Nguyen Van B", "Nam", "2000",
                "0912345678", "Ha Noi");

            dgvPatients.Rows.Add(
                "BN002", "Tran Thi C", "Nu", "1998",
                "0987654321", "Bac Ninh");

            Controls.Add(dgvPatients);
            dgvPatients.BringToFront();
        }
    }

    // FORM CON 2: LICH TRUC BAC SI
    public class frmDoctorSchedule : Form
    {
        private DataGridView dgvDoctors;

        public frmDoctorSchedule()
        {
            CreateDesign();
        }

        private void CreateDesign()
        {
            Text = "Danh muc bac si va lich truc";
            Size = new Size(800, 450);
            MinimumSize = new Size(600, 350);

            Label title = new Label();
            title.Text = "DANH SACH BAC SI VA LICH TRUC";
            title.Font = new Font("Arial", 15, FontStyle.Bold);
            title.TextAlign = ContentAlignment.MiddleCenter;
            title.Dock = DockStyle.Top;
            title.Height = 50;
            Controls.Add(title);

            dgvDoctors = new DataGridView();
            dgvDoctors.Dock = DockStyle.Fill;
            dgvDoctors.AllowUserToAddRows = true;
            dgvDoctors.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvDoctors.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvDoctors.RowHeadersVisible = false;

            dgvDoctors.Columns.Add("DoctorId", "Ma bac si");
            dgvDoctors.Columns.Add("DoctorName", "Ho ten bac si");
            dgvDoctors.Columns.Add("Department", "Khoa");
            dgvDoctors.Columns.Add("Shift", "Ca truc");
            dgvDoctors.Columns.Add("Date", "Ngay truc");
            dgvDoctors.Columns.Add("Phone", "So dien thoai");

            dgvDoctors.Rows.Add(
                "BS001", "Nguyen Van A", "Noi tong hop",
                "Sang", "18/10/2026", "0911111111");

            dgvDoctors.Rows.Add(
                "BS002", "Tran Van B", "Ngoai khoa",
                "Chieu", "18/10/2026", "0922222222");

            Controls.Add(dgvDoctors);
            dgvDoctors.BringToFront();
        }
    }
}
