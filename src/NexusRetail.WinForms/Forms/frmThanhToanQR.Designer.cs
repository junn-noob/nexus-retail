namespace NexusRetail.WinForms.Forms
{
    partial class frmThanhToanQR
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlHeaderAccent = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblMaDon = new System.Windows.Forms.Label();
            this.picQr = new System.Windows.Forms.PictureBox();
            this.pnlInfo = new System.Windows.Forms.Panel();
            this.lblSoTien = new System.Windows.Forms.Label();
            this.lblLabelSoTien = new System.Windows.Forms.Label();
            this.lblNoiDung = new System.Windows.Forms.Label();
            this.lblLabelNoiDung = new System.Windows.Forms.Label();
            this.lblChuTk = new System.Windows.Forms.Label();
            this.lblLabelChuTk = new System.Windows.Forms.Label();
            this.lblStk = new System.Windows.Forms.Label();
            this.lblLabelStk = new System.Windows.Forms.Label();
            this.lblNganHang = new System.Windows.Forms.Label();
            this.lblLabelNganHang = new System.Windows.Forms.Label();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnKiemTra = new System.Windows.Forms.Button();
            this.btnDong = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQr)).BeginInit();
            this.pnlInfo.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.pnlHeader.Controls.Add(this.pnlHeaderAccent);
            this.pnlHeader.Controls.Add(this.lblMaDon);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(520, 72);
            this.pnlHeader.TabIndex = 0;
            // 
            // pnlHeaderAccent
            // 
            this.pnlHeaderAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.pnlHeaderAccent.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlHeaderAccent.Location = new System.Drawing.Point(0, 68);
            this.pnlHeaderAccent.Name = "pnlHeaderAccent";
            this.pnlHeaderAccent.Size = new System.Drawing.Size(520, 4);
            this.pnlHeaderAccent.TabIndex = 2;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(18, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(345, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "KIOTVIET POS • THANH TOÁN VIETQR";
            // 
            // lblMaDon
            // 
            this.lblMaDon.AutoSize = true;
            this.lblMaDon.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblMaDon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblMaDon.Location = new System.Drawing.Point(20, 40);
            this.lblMaDon.Name = "lblMaDon";
            this.lblMaDon.Size = new System.Drawing.Size(107, 17);
            this.lblMaDon.TabIndex = 1;
            this.lblMaDon.Text = "Mã HĐ: HDB01";
            // 
            // picQr
            // 
            this.picQr.BackColor = System.Drawing.Color.White;
            this.picQr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picQr.Location = new System.Drawing.Point(140, 84);
            this.picQr.Name = "picQr";
            this.picQr.Size = new System.Drawing.Size(240, 240);
            this.picQr.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picQr.TabIndex = 1;
            this.picQr.TabStop = false;
            // 
            // pnlInfo
            // 
            this.pnlInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlInfo.Controls.Add(this.lblSoTien);
            this.pnlInfo.Controls.Add(this.lblLabelSoTien);
            this.pnlInfo.Controls.Add(this.lblNoiDung);
            this.pnlInfo.Controls.Add(this.lblLabelNoiDung);
            this.pnlInfo.Controls.Add(this.lblChuTk);
            this.pnlInfo.Controls.Add(this.lblLabelChuTk);
            this.pnlInfo.Controls.Add(this.lblStk);
            this.pnlInfo.Controls.Add(this.lblLabelStk);
            this.pnlInfo.Controls.Add(this.lblNganHang);
            this.pnlInfo.Controls.Add(this.lblLabelNganHang);
            this.pnlInfo.Location = new System.Drawing.Point(20, 336);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Size = new System.Drawing.Size(480, 185);
            this.pnlInfo.TabIndex = 2;
            // 
            // lblLabelNganHang
            // 
            this.lblLabelNganHang.AutoSize = true;
            this.lblLabelNganHang.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLabelNganHang.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblLabelNganHang.Location = new System.Drawing.Point(16, 14);
            this.lblLabelNganHang.Name = "lblLabelNganHang";
            this.lblLabelNganHang.Size = new System.Drawing.Size(68, 15);
            this.lblLabelNganHang.TabIndex = 0;
            this.lblLabelNganHang.Text = "Ngân hàng:";
            // 
            // lblNganHang
            // 
            this.lblNganHang.AutoSize = true;
            this.lblNganHang.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblNganHang.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.lblNganHang.Location = new System.Drawing.Point(135, 13);
            this.lblNganHang.Name = "lblNganHang";
            this.lblNganHang.Size = new System.Drawing.Size(252, 17);
            this.lblNganHang.TabIndex = 1;
            this.lblNganHang.Text = "MB Bank (Ngân hàng TMCP Quân đội)";
            // 
            // lblLabelStk
            // 
            this.lblLabelStk.AutoSize = true;
            this.lblLabelStk.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLabelStk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblLabelStk.Location = new System.Drawing.Point(16, 46);
            this.lblLabelStk.Name = "lblLabelStk";
            this.lblLabelStk.Size = new System.Drawing.Size(75, 15);
            this.lblLabelStk.TabIndex = 2;
            this.lblLabelStk.Text = "Số tài khoản:";
            // 
            // lblStk
            // 
            this.lblStk.AutoSize = true;
            this.lblStk.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblStk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.lblStk.Location = new System.Drawing.Point(135, 43);
            this.lblStk.Name = "lblStk";
            this.lblStk.Size = new System.Drawing.Size(108, 20);
            this.lblStk.TabIndex = 3;
            this.lblStk.Text = "09999999900";
            // 
            // lblLabelChuTk
            // 
            this.lblLabelChuTk.AutoSize = true;
            this.lblLabelChuTk.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLabelChuTk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblLabelChuTk.Location = new System.Drawing.Point(16, 78);
            this.lblLabelChuTk.Name = "lblLabelChuTk";
            this.lblLabelChuTk.Size = new System.Drawing.Size(83, 15);
            this.lblLabelChuTk.TabIndex = 4;
            this.lblLabelChuTk.Text = "Chủ tài khoản:";
            // 
            // lblChuTk
            // 
            this.lblChuTk.AutoSize = true;
            this.lblChuTk.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblChuTk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblChuTk.Location = new System.Drawing.Point(135, 76);
            this.lblChuTk.Name = "lblChuTk";
            this.lblChuTk.Size = new System.Drawing.Size(152, 19);
            this.lblChuTk.TabIndex = 5;
            this.lblChuTk.Text = "NGUYEN VAN HUNG";
            // 
            // lblLabelNoiDung
            // 
            this.lblLabelNoiDung.AutoSize = true;
            this.lblLabelNoiDung.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLabelNoiDung.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblLabelNoiDung.Location = new System.Drawing.Point(16, 110);
            this.lblLabelNoiDung.Name = "lblLabelNoiDung";
            this.lblLabelNoiDung.Size = new System.Drawing.Size(60, 15);
            this.lblLabelNoiDung.TabIndex = 6;
            this.lblLabelNoiDung.Text = "Nội dung:";
            // 
            // lblNoiDung
            // 
            this.lblNoiDung.AutoSize = true;
            this.lblNoiDung.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblNoiDung.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(65)))), ((int)(((byte)(12)))));
            this.lblNoiDung.Location = new System.Drawing.Point(135, 107);
            this.lblNoiDung.Name = "lblNoiDung";
            this.lblNoiDung.Size = new System.Drawing.Size(148, 20);
            this.lblNoiDung.TabIndex = 7;
            this.lblNoiDung.Text = "DUYMOBILE HDB01";
            // 
            // lblLabelSoTien
            // 
            this.lblLabelSoTien.AutoSize = true;
            this.lblLabelSoTien.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLabelSoTien.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblLabelSoTien.Location = new System.Drawing.Point(16, 145);
            this.lblLabelSoTien.Name = "lblLabelSoTien";
            this.lblLabelSoTien.Size = new System.Drawing.Size(67, 15);
            this.lblLabelSoTien.TabIndex = 8;
            this.lblLabelSoTien.Text = "Số tiền cần:";
            // 
            // lblSoTien
            // 
            this.lblSoTien.AutoSize = true;
            this.lblSoTien.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblSoTien.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.lblSoTien.Location = new System.Drawing.Point(135, 138);
            this.lblSoTien.Name = "lblSoTien";
            this.lblSoTien.Size = new System.Drawing.Size(147, 28);
            this.lblSoTien.TabIndex = 9;
            this.lblSoTien.Text = "25.000.000 ₫";
            // 
            // pnlStatus
            // 
            this.pnlStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(248)))), ((int)(((byte)(238)))));
            this.pnlStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Location = new System.Drawing.Point(20, 532);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(480, 48);
            this.pnlStatus.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(136)))), ((int)(((byte)(46)))));
            this.lblStatus.Location = new System.Drawing.Point(0, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(478, 46);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "⏳ Đang lắng nghe Webhook ngân hàng xác nhận...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnKiemTra
            // 
            this.btnKiemTra.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.btnKiemTra.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnKiemTra.FlatAppearance.BorderSize = 0;
            this.btnKiemTra.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKiemTra.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnKiemTra.ForeColor = System.Drawing.Color.White;
            this.btnKiemTra.Location = new System.Drawing.Point(20, 595);
            this.btnKiemTra.Name = "btnKiemTra";
            this.btnKiemTra.Size = new System.Drawing.Size(310, 44);
            this.btnKiemTra.TabIndex = 4;
            this.btnKiemTra.Text = "🔄 Kiểm Tra Thanh Toán (F5)";
            this.btnKiemTra.UseVisualStyleBackColor = false;
            this.btnKiemTra.Click += new System.EventHandler(this.btnKiemTra_Click);
            // 
            // btnDong
            // 
            this.btnDong.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnDong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDong.FlatAppearance.BorderSize = 0;
            this.btnDong.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDong.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnDong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnDong.Location = new System.Drawing.Point(340, 595);
            this.btnDong.Name = "btnDong";
            this.btnDong.Size = new System.Drawing.Size(160, 44);
            this.btnDong.TabIndex = 5;
            this.btnDong.Text = "Hủy Bỏ (Esc)";
            this.btnDong.UseVisualStyleBackColor = false;
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            // 
            // frmThanhToanQR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(520, 660);
            this.Controls.Add(this.btnDong);
            this.Controls.Add(this.btnKiemTra);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.pnlInfo);
            this.Controls.Add(this.picQr);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmThanhToanQR";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "KiotViet POS • Cổng Thanh Toán VietQR MBBank";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQr)).EndInit();
            this.pnlInfo.ResumeLayout(false);
            this.pnlInfo.PerformLayout();
            this.pnlStatus.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlHeaderAccent;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblMaDon;
        private System.Windows.Forms.PictureBox picQr;
        private System.Windows.Forms.Panel pnlInfo;
        private System.Windows.Forms.Label lblLabelNganHang;
        private System.Windows.Forms.Label lblNganHang;
        private System.Windows.Forms.Label lblLabelStk;
        private System.Windows.Forms.Label lblStk;
        private System.Windows.Forms.Label lblLabelChuTk;
        private System.Windows.Forms.Label lblChuTk;
        private System.Windows.Forms.Label lblLabelNoiDung;
        private System.Windows.Forms.Label lblNoiDung;
        private System.Windows.Forms.Label lblLabelSoTien;
        private System.Windows.Forms.Label lblSoTien;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnKiemTra;
        private System.Windows.Forms.Button btnDong;
    }
}
