
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bai4._15
{
    public partial class Form1 : Form
    {
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private ComboBox cboBranch;
        private Button btnStatistics;
        private Button btnCancel;
        private ProgressBar pgbLoading;
        private Label lblProcessingStatus;
        private DataGridView dgvRevenueData;

        private CancellationTokenSource cts;

        public Form1()
        {
            InitializeComponent();
            CreateDesign();
        }

        private void CreateDesign()
        {
            Text = "Revenue Analytics - Thong ke doanh thu";
            Size = new Size(1000, 650);
            MinimumSize = new Size(850, 550);
            StartPosition = FormStartPosition.CenterScreen;

            Label lblTitle = new Label();
            lblTitle.Text = "BANG DIEU KHIEN THONG KE DOANH THU";
            lblTitle.Font = new Font("Arial", 16, FontStyle.Bold);
            lblTitle.Location = new Point(20, 15);
            lblTitle.Size = new Size(700, 35);
            Controls.Add(lblTitle);

            GroupBox grpCriteria = new GroupBox();
            grpCriteria.Text = "Tieu chi thong ke";
            grpCriteria.Location = new Point(20, 60);
            grpCriteria.Size = new Size(940, 125);
            Controls.Add(grpCriteria);

            Label lblFrom = new Label();
            lblFrom.Text = "Tu ngay:";
            lblFrom.Location = new Point(20, 30);
            lblFrom.AutoSize = true;
            grpCriteria.Controls.Add(lblFrom);

            dtpFromDate = new DateTimePicker();
            dtpFromDate.Format = DateTimePickerFormat.Short;
            dtpFromDate.Location = new Point(85, 25);
            dtpFromDate.Width = 130;
            dtpFromDate.Value = DateTime.Today.AddDays(-7);
            grpCriteria.Controls.Add(dtpFromDate);

            Label lblTo = new Label();
            lblTo.Text = "Den ngay:";
            lblTo.Location = new Point(240, 30);
            lblTo.AutoSize = true;
            grpCriteria.Controls.Add(lblTo);

            dtpToDate = new DateTimePicker();
            dtpToDate.Format = DateTimePickerFormat.Short;
            dtpToDate.Location = new Point(315, 25);
            dtpToDate.Width = 130;
            grpCriteria.Controls.Add(dtpToDate);

            Label lblBranch = new Label();
            lblBranch.Text = "Chi nhanh:";
            lblBranch.Location = new Point(470, 30);
            lblBranch.AutoSize = true;
            grpCriteria.Controls.Add(lblBranch);

            cboBranch = new ComboBox();
            cboBranch.DropDownStyle = ComboBoxStyle.DropDownList;
            cboBranch.Location = new Point(545, 25);
            cboBranch.Width = 160;
            cboBranch.Items.AddRange(new object[]
            {
                "Tat ca chi nhanh",
                "Ha Noi",
                "TP Ho Chi Minh",
                "Da Nang"
            });
            cboBranch.SelectedIndex = 0;
            grpCriteria.Controls.Add(cboBranch);

            btnStatistics = new Button();
            btnStatistics.Text = "Thong Ke Doanh Thu";
            btnStatistics.Location = new Point(85, 75);
            btnStatistics.Size = new Size(180, 32);
            btnStatistics.Click += async (s, e) =>
            {
                await RunStatisticsAsync();
            };
            grpCriteria.Controls.Add(btnStatistics);

            btnCancel = new Button();
            btnCancel.Text = "Huy Thao Tac";
            btnCancel.Location = new Point(280, 75);
            btnCancel.Size = new Size(140, 32);
            btnCancel.Enabled = false;
            btnCancel.Click += (s, e) => CancelStatistics();
            grpCriteria.Controls.Add(btnCancel);

            GroupBox grpStatus = new GroupBox();
            grpStatus.Text = "Trang thai xu ly";
            grpStatus.Location = new Point(20, 195);
            grpStatus.Size = new Size(940, 90);
            Controls.Add(grpStatus);

            lblProcessingStatus = new Label();
            lblProcessingStatus.Text = "San sang thong ke doanh thu.";
            lblProcessingStatus.Location = new Point(20, 25);
            lblProcessingStatus.Size = new Size(880, 25);
            grpStatus.Controls.Add(lblProcessingStatus);

            pgbLoading = new ProgressBar();
            pgbLoading.Location = new Point(20, 55);
            pgbLoading.Size = new Size(890, 22);
            pgbLoading.Minimum = 0;
            pgbLoading.Maximum = 100;
            pgbLoading.Value = 0;
            grpStatus.Controls.Add(pgbLoading);

            GroupBox grpResult = new GroupBox();
            grpResult.Text = "Danh sach giao dich";
            grpResult.Location = new Point(20, 300);
            grpResult.Size = new Size(940, 290);
            Controls.Add(grpResult);

            dgvRevenueData = new DataGridView();
            dgvRevenueData.Location = new Point(10, 25);
            dgvRevenueData.Size = new Size(915, 250);
            dgvRevenueData.Anchor = AnchorStyles.Top |
                                    AnchorStyles.Bottom |
                                    AnchorStyles.Left |
                                    AnchorStyles.Right;
            dgvRevenueData.AllowUserToAddRows = false;
            dgvRevenueData.ReadOnly = true;
            dgvRevenueData.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvRevenueData.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvRevenueData.RowHeadersVisible = false;

            dgvRevenueData.Columns.Add("Id", "Ma giao dich");
            dgvRevenueData.Columns.Add("Date", "Ngay giao dich");
            dgvRevenueData.Columns.Add("Branch", "Chi nhanh");
            dgvRevenueData.Columns.Add("Description", "Noi dung");
            dgvRevenueData.Columns.Add("Amount", "Doanh thu");

            grpResult.Controls.Add(dgvRevenueData);
        }

        private async Task RunStatisticsAsync()
        {
            if (cts != null)
                return;

            if (dtpFromDate.Value.Date > dtpToDate.Value.Date)
            {
                MessageBox.Show(
                    "Ngay bat dau khong duoc lon hon ngay ket thuc!",
                    "Thong bao",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DateTime fromDate = dtpFromDate.Value.Date;
            DateTime toDate = dtpToDate.Value.Date;
            string branch = cboBranch.SelectedItem?.ToString()
                            ?? "Tat ca chi nhanh";

            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            btnStatistics.Enabled = false;
            btnCancel.Enabled = true;
            dtpFromDate.Enabled = false;
            dtpToDate.Enabled = false;
            cboBranch.Enabled = false;

            pgbLoading.Value = 0;
            dgvRevenueData.Rows.Clear();
            lblProcessingStatus.Text = "Dang xu ly...";

            try
            {
                // Khong dung progress.Report()
                // Cap nhat giao dien bang BeginInvoke
                List<RevenueTransaction> data = await Task.Run(async () =>
                {
                    List<RevenueTransaction> result =
                        new List<RevenueTransaction>();

                    Random random = new Random();

                    // Gia lap xu ly trong 5 giay
                    for (int i = 1; i <= 50; i++)
                    {
                        token.ThrowIfCancellationRequested();

                        await Task.Delay(100, token);

                        int percent = i * 100 / 50;

                        // Dua viec cap nhat giao dien ve UI Thread
                        if (!IsDisposed && IsHandleCreated)
                        {
                            BeginInvoke(new Action(() =>
                            {
                                if (!IsDisposed && !token.IsCancellationRequested)
                                {
                                    pgbLoading.Value = percent;
                                    lblProcessingStatus.Text =
                                        "Dang xu ly: " + percent + "%";
                                }
                            }));
                        }
                    }

                    token.ThrowIfCancellationRequested();

                    string[] branches =
                    {
                        "Ha Noi",
                        "TP Ho Chi Minh",
                        "Da Nang"
                    };

                    int dayCount = (toDate - fromDate).Days + 1;

                    for (int i = 1; i <= 20; i++)
                    {
                        token.ThrowIfCancellationRequested();

                        string transactionBranch =
                            branch == "Tat ca chi nhanh"
                            ? branches[random.Next(branches.Length)]
                            : branch;

                        DateTime transactionDate =
                            fromDate.AddDays(random.Next(dayCount));

                        result.Add(new RevenueTransaction
                        {
                            Id = "GD" + i.ToString("000"),
                            Date = transactionDate,
                            Branch = transactionBranch,
                            Description = "Giao dich ban hang " + i,
                            Amount = random.Next(500000, 10000000)
                        });
                    }

                    return result;
                }, token);

                // Hien thi ket qua tren UI Thread
                foreach (RevenueTransaction item in data)
                {
                    dgvRevenueData.Rows.Add(
                        item.Id,
                        item.Date.ToString("dd/MM/yyyy"),
                        item.Branch,
                        item.Description,
                        item.Amount.ToString("N0") + " VND");
                }

                pgbLoading.Value = 100;
                lblProcessingStatus.Text =
                    "Thong ke thanh cong! Co " + data.Count + " giao dich.";
            }
            catch (OperationCanceledException)
            {
                pgbLoading.Value = 0;
                lblProcessingStatus.Text = "Đã hủy thao tác!";

                MessageBox.Show(
                    "Đã hủy thao tác!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblProcessingStatus.Text = "Co loi khi thong ke.";
                MessageBox.Show("Loi: " + ex.Message);
            }
            finally
            {
                btnStatistics.Enabled = true;
                btnCancel.Enabled = false;
                dtpFromDate.Enabled = true;
                dtpToDate.Enabled = true;
                cboBranch.Enabled = true;

                cts?.Dispose();
                cts = null;
            }
        }

        private void CancelStatistics()
        {
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
                btnCancel.Enabled = false;
                lblProcessingStatus.Text = "Dang huy thao tac...";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            cts?.Cancel();
            base.OnFormClosing(e);
        }

        private class RevenueTransaction
        {
            public string Id { get; set; }
            public DateTime Date { get; set; }
            public string Branch { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
