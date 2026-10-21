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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblMaDon = new System.Windows.Forms.Label();
            this.picQr = new System.Windows.Forms.PictureBox();
            this.pnlInfo = new System.Windows.Forms.Panel();
            this.lblSoTien = new System.Windows.Forms.Label();
            this.lblLabelSoTien = new System.Windows.Forms.Label();
            this.lblNoiDung = new System.Windows.Forms.Label();
            this.lblLabelNoiDung = new System.Windows.Forms.Label();
            this.lblStk = new System.Windows.Forms.Label();
            this.lblLabelStk = new System.Windows.Forms.Label();
            this.lblChuTk = new System.Windows.Forms.Label();
            this.lblLabelChuTk = new System.Windows.Forms.Label();
            this.lblNganHang = new System.Windows.Forms.Label();
            this.lblLabelNganHang = new System.Windows.Forms.Label();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnSimulate = new System.Windows.Forms.Button();
            this.btnDong = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQr)).BeginInit();
            this.pnlInfo.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.pnlHeader.Controls.Add(this.lblMaDon);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(460, 68);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(18, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(306, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "THANH TOÁN VIETQR (MBBANK POS)";
            // 
            // lblMaDon
            // 
            this.lblMaDon.AutoSize = true;
            this.lblMaDon.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblMaDon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblMaDon.Location = new System.Drawing.Point(20, 38);
            this.lblMaDon.Name = "lblMaDon";
            this.lblMaDon.Size = new System.Drawing.Size(107, 17);
            this.lblMaDon.TabIndex = 1;
            this.lblMaDon.Text = "Mã HĐ: HDB01";
            // 
            // picQr
            // 
            this.picQr.BackColor = System.Drawing.Color.White;
            this.picQr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picQr.Location = new System.Drawing.Point(100, 82);
            this.picQr.Name = "picQr";
            this.picQr.Size = new System.Drawing.Size(260, 260);
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
            this.pnlInfo.Location = new System.Drawing.Point(24, 348);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Size = new System.Drawing.Size(412, 172);
            this.pnlInfo.TabIndex = 2;
            // 
            // lblLabelNganHang
            // 
            this.lblLabelNganHang.AutoSize = true;
            this.lblLabelNganHang.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblLabelNganHang.Location = new System.Drawing.Point(15, 12);
            this.lblLabelNganHang.Name = "lblLabelNganHang";
            this.lblLabelNganHang.Size = new System.Drawing.Size(68, 15);
            this.lblLabelNganHang.TabIndex = 0;
            this.lblLabelNganHang.Text = "Ngân hàng:";
            // 
            // lblNganHang
            // 
            this.lblNganHang.AutoSize = true;
            this.lblNganHang.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblNganHang.Location = new System.Drawing.Point(130, 12);
            this.lblNganHang.Name = "lblNganHang";
            this.lblNganHang.Size = new System.Drawing.Size(232, 15);
            this.lblNganHang.TabIndex = 1;
            this.lblNganHang.Text = "MB Bank (Ngân hàng TMCP Quân đội)";
            // 
            // lblLabelStk
            // 
            this.lblLabelStk.AutoSize = true;
            this.lblLabelStk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblLabelStk.Location = new System.Drawing.Point(15, 40);
            this.lblLabelStk.Name = "lblLabelStk";
            this.lblLabelStk.Size = new System.Drawing.Size(75, 15);
            this.lblLabelStk.TabIndex = 2;
            this.lblLabelStk.Text = "Số tài khoản:";
            // 
            // lblStk
            // 
            this.lblStk.AutoSize = true;
            this.lblStk.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblStk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblStk.Location = new System.Drawing.Point(130, 38);
            this.lblStk.Name = "lblStk";
            this.lblStk.Size = new System.Drawing.Size(97, 19);
            this.lblStk.TabIndex = 3;
            this.lblStk.Text = "09999999900";
            // 
            // lblLabelChuTk
            // 
            this.lblLabelChuTk.AutoSize = true;
            this.lblLabelChuTk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblLabelChuTk.Location = new System.Drawing.Point(15, 68);
            this.lblLabelChuTk.Name = "lblLabelChuTk";
            this.lblLabelChuTk.Size = new System.Drawing.Size(83, 15);
            this.lblLabelChuTk.TabIndex = 8;
            this.lblLabelChuTk.Text = "Chủ tài khoản:";
            // 
            // lblChuTk
            // 
            this.lblChuTk.AutoSize = true;
            this.lblChuTk.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblChuTk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblChuTk.Location = new System.Drawing.Point(130, 67);
            this.lblChuTk.Name = "lblChuTk";
            this.lblChuTk.Size = new System.Drawing.Size(142, 17);
            this.lblChuTk.TabIndex = 9;
            this.lblChuTk.Text = "NGUYEN VAN HUNG";
            // 
            // lblLabelNoiDung
            // 
            this.lblLabelNoiDung.AutoSize = true;
            this.lblLabelNoiDung.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblLabelNoiDung.Location = new System.Drawing.Point(15, 96);
            this.lblLabelNoiDung.Name = "lblLabelNoiDung";
            this.lblLabelNoiDung.Size = new System.Drawing.Size(60, 15);
            this.lblLabelNoiDung.TabIndex = 4;
            this.lblLabelNoiDung.Text = "Nội dung:";
            // 
            // lblNoiDung
            // 
            this.lblNoiDung.AutoSize = true;
            this.lblNoiDung.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblNoiDung.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(65)))), ((int)(((byte)(12)))));
            this.lblNoiDung.Location = new System.Drawing.Point(130, 94);
            this.lblNoiDung.Name = "lblNoiDung";
            this.lblNoiDung.Size = new System.Drawing.Size(133, 19);
            this.lblNoiDung.TabIndex = 5;
            this.lblNoiDung.Text = "DUYMOBILE HDB01";
            // 
            // lblLabelSoTien
            // 
            this.lblLabelSoTien.AutoSize = true;
            this.lblLabelSoTien.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblLabelSoTien.Location = new System.Drawing.Point(15, 128);
            this.lblLabelSoTien.Name = "lblLabelSoTien";
            this.lblLabelSoTien.Size = new System.Drawing.Size(67, 15);
            this.lblLabelSoTien.TabIndex = 6;
            this.lblLabelSoTien.Text = "Số tiền cần:";
            // 
            // lblSoTien
            // 
            this.lblSoTien.AutoSize = true;
            this.lblSoTien.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSoTien.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblSoTien.Location = new System.Drawing.Point(130, 123);
            this.lblSoTien.Name = "lblSoTien";
            this.lblSoTien.Size = new System.Drawing.Size(126, 25);
            this.lblSoTien.TabIndex = 7;
            this.lblSoTien.Text = "25.000.000 ₫";
            // 
            // pnlStatus
            // 
            this.pnlStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(255)))));
            this.pnlStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Location = new System.Drawing.Point(24, 530);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(412, 46);
            this.pnlStatus.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.lblStatus.Location = new System.Drawing.Point(0, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(410, 44);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "⏳ Đang lắng nghe Webhook ngân hàng xác nhận...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSimulate
            // 
            this.btnSimulate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.btnSimulate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSimulate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSimulate.ForeColor = System.Drawing.Color.White;
            this.btnSimulate.Location = new System.Drawing.Point(24, 588);
            this.btnSimulate.Name = "btnSimulate";
            this.btnSimulate.Size = new System.Drawing.Size(256, 38);
            this.btnSimulate.TabIndex = 4;
            this.btnSimulate.Text = "⚡ Giả lập Webhook MBBank (F9)";
            this.btnSimulate.UseVisualStyleBackColor = false;
            this.btnSimulate.Click += new System.EventHandler(this.btnSimulate_Click);
            // 
            // btnDong
            // 
            this.btnDong.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnDong.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnDong.Location = new System.Drawing.Point(292, 588);
            this.btnDong.Name = "btnDong";
            this.btnDong.Size = new System.Drawing.Size(144, 38);
            this.btnDong.TabIndex = 5;
            this.btnDong.Text = "Hủy bỏ (Esc)";
            this.btnDong.UseVisualStyleBackColor = false;
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            // 
            // frmThanhToanQR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(460, 642);
            this.Controls.Add(this.btnDong);
            this.Controls.Add(this.btnSimulate);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.pnlInfo);
            this.Controls.Add(this.picQr);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmThanhToanQR";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cổng Thanh Toán VietQR MBBank";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQr)).EndInit();
            this.pnlInfo.ResumeLayout(false);
            this.pnlInfo.PerformLayout();
            this.pnlStatus.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlHeader;
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
        private System.Windows.Forms.Button btnSimulate;
        private System.Windows.Forms.Button btnDong;
    }
}
