
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bai4._16
{
    public partial class Form1 : Form
    {
        private Button btnStart = new Button();
        private Button btnPause = new Button();

        private Label lblBTCPrice = new Label();
        private DataGridView dgvHistoryLog = new DataGridView();

        private ToolStripStatusLabel lblLastUpdate =
            new ToolStripStatusLabel();

        private ToolStripStatusLabel lblSuccessCount =
            new ToolStripStatusLabel();

        private CancellationTokenSource cts;

        private decimal currentPrice = 65000m;
        private int successCount = 0;
        private bool isMonitoring = false;

        private readonly Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            CreateDesign();
        }

        private void CreateDesign()
        {
            Text = "Bai 4.16 - Giam sat ty gia BTC/USD";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1000, 650);
            MinimumSize = new Size(800, 500);

            // TIEU DE
            Label lblTitle = new Label();
            lblTitle.Text = "Giam sat ty gia BTC/USD thoi gian thuc";
            lblTitle.Font = new Font("Arial", 17, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 60;
            Controls.Add(lblTitle);

            // KHU VUC DIEU KHIEN
            GroupBox grpControl = new GroupBox();
            grpControl.Text = "Dieu khien giam sat";
            grpControl.SetBounds(20, 70, 940, 100);
            grpControl.Anchor = AnchorStyles.Top |
                                AnchorStyles.Left |
                                AnchorStyles.Right;
            Controls.Add(grpControl);

            btnStart.Text = "Bat dau giam sat";
            btnStart.SetBounds(30, 35, 160, 40);
            btnStart.Click += async (s, e) =>
                await StartMonitoringAsync();
            grpControl.Controls.Add(btnStart);

            btnPause.Text = "Tam dung";
            btnPause.SetBounds(210, 35, 130, 40);
            btnPause.Enabled = false;
            btnPause.Click += (s, e) => PauseMonitoring();
            grpControl.Controls.Add(btnPause);

            // KHU VUC HIEN THI GIA
            GroupBox grpPrice = new GroupBox();
            grpPrice.Text = "Gia Bitcoin hien tai";
            grpPrice.SetBounds(20, 180, 940, 120);
            grpPrice.Anchor = AnchorStyles.Top |
                              AnchorStyles.Left |
                              AnchorStyles.Right;
            Controls.Add(grpPrice);

            lblBTCPrice.Text = "BTC/USD: $65,000.00";
            lblBTCPrice.Font = new Font("Arial", 26, FontStyle.Bold);
            lblBTCPrice.ForeColor = Color.Green;
            lblBTCPrice.TextAlign = ContentAlignment.MiddleCenter;
            lblBTCPrice.Dock = DockStyle.Fill;
            grpPrice.Controls.Add(lblBTCPrice);

            // LICH SU GIA
            GroupBox grpHistory = new GroupBox();
            grpHistory.Text = "Nhat ky bien dong gia";
            grpHistory.SetBounds(20, 310, 940, 250);
            grpHistory.Anchor = AnchorStyles.Top |
                                AnchorStyles.Bottom |
                                AnchorStyles.Left |
                                AnchorStyles.Right;
            Controls.Add(grpHistory);

            dgvHistoryLog.Dock = DockStyle.Fill;
            dgvHistoryLog.ReadOnly = true;
            dgvHistoryLog.AllowUserToAddRows = false;
            dgvHistoryLog.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistoryLog.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvHistoryLog.RowHeadersVisible = false;

            dgvHistoryLog.Columns.Add("Time", "Thoi gian");
            dgvHistoryLog.Columns.Add("Price", "Gia BTC/USD");
            dgvHistoryLog.Columns.Add("Change", "Bien dong");
            dgvHistoryLog.Columns.Add("Status", "Trang thai");

            grpHistory.Controls.Add(dgvHistoryLog);

            // STATUS STRIP
            StatusStrip statusStrip = new StatusStrip();

            lblLastUpdate.Text = "Cap nhat cuoi: Chua co";
            lblSuccessCount.Text = "So lan dong bo: 0";

            statusStrip.Items.Add(lblLastUpdate);
            statusStrip.Items.Add(
                new ToolStripStatusLabel(" | "));
            statusStrip.Items.Add(lblSuccessCount);

            Controls.Add(statusStrip);
        }

        private async Task StartMonitoringAsync()
        {
            if (isMonitoring)
                return;

            isMonitoring = true;

            btnStart.Enabled = false;
            btnPause.Enabled = true;

            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            try
            {
                await Task.Run(async () =>
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();

                        // Cho chuong trinh doi 2 giay
                        await Task.Delay(2000, token);

                        token.ThrowIfCancellationRequested();

                        decimal oldPrice = currentPrice;

                        // Sinh gia moi ngau nhien
                        int change = random.Next(-500, 501);
                        decimal newPrice = oldPrice + change;

                        if (newPrice <= 0)
                            newPrice = oldPrice + 100;

                        currentPrice = newPrice;

                        string direction;

                        if (newPrice > oldPrice)
                            direction = "Tang";
                        else if (newPrice < oldPrice)
                            direction = "Giam";
                        else
                            direction = "Khong doi";

                        // Chuyen viec cap nhat giao dien
                        // ve Main UI Thread bang Invoke.
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() =>
                            {
                                UpdateInterface(
                                    newPrice,
                                    oldPrice,
                                    direction);
                            }));
                        }
                        else
                        {
                            UpdateInterface(
                                newPrice,
                                oldPrice,
                                direction);
                        }
                    }
                }, token);
            }
            catch (OperationCanceledException)
            {
                // Tac vu da duoc tam dung
            }
            catch (ObjectDisposedException)
            {
                // Form da dong
            }
            catch (InvalidOperationException)
            {
                // Form khong con san sang de Invoke
            }
            finally
            {
                isMonitoring = false;

                if (!IsDisposed && IsHandleCreated)
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            btnStart.Enabled = true;
                            btnPause.Enabled = false;
                        }));
                    }
                    else
                    {
                        btnStart.Enabled = true;
                        btnPause.Enabled = false;
                    }
                }

                if (cts != null)
                {
                    cts.Dispose();
                    cts = null;
                }
            }
        }

        private void UpdateInterface(
            decimal newPrice,
            decimal oldPrice,
            string direction)
        {
            if (IsDisposed)
                return;

            // Cap nhat gia va mau chu
            lblBTCPrice.Text =
                "BTC/USD: $" + newPrice.ToString("N2");

            if (newPrice > oldPrice)
                lblBTCPrice.ForeColor = Color.Green;
            else if (newPrice < oldPrice)
                lblBTCPrice.ForeColor = Color.Red;

            // Cap nhat so lan dong bo
            successCount++;

            string time = DateTime.Now.ToString("HH:mm:ss");
            decimal difference = newPrice - oldPrice;

            lblLastUpdate.Text = "Cap nhat cuoi: " + time;
            lblSuccessCount.Text =
                "So lan dong bo: " + successCount;

            // Them dong lich su
            int rowIndex = dgvHistoryLog.Rows.Add(
                time,
                "$" + newPrice.ToString("N2"),
                (difference >= 0 ? "+" : "") +
                    difference.ToString("N2"),
                direction);

            if (difference > 0)
            {
                dgvHistoryLog.Rows[rowIndex].DefaultCellStyle.ForeColor =
                    Color.Green;
            }
            else if (difference < 0)
            {
                dgvHistoryLog.Rows[rowIndex].DefaultCellStyle.ForeColor =
                    Color.Red;
            }

            // Dua dong moi nhat len vi tri dang hien thi
            if (dgvHistoryLog.Rows.Count > 0)
            {
                dgvHistoryLog.FirstDisplayedScrollingRowIndex =
                    dgvHistoryLog.Rows.Count - 1;
            }
        }

        private void PauseMonitoring()
        {
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
            }

            btnPause.Enabled = false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
            }

            base.OnFormClosing(e);
        }
    }
}
