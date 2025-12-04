using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.Xml.Linq;
using Font = System.Drawing.Font;

namespace TitaniumMemberGift
{
    partial class Form1
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            btnStartSchedule = new Button();
            btnManualRun = new Button();
            lblLastRun = new Label();
            lblNextRun = new Label();
            txtLog = new TextBox();
            lblStartTime = new Label();
            SuspendLayout();

            // 🔹 btnManualRun
            btnManualRun.Location = new Point(435, 20);
            btnManualRun.Name = "btnManualRun";
            btnManualRun.Size = new Size(180, 30);
            btnManualRun.TabIndex = 1;
            btnManualRun.Text = "手動檢查當月鈦金會員獨享禮";
            btnManualRun.UseVisualStyleBackColor = true;
            btnManualRun.Click += btnManualRun_Click;

            // 🔹 btnStartSchedule
            btnStartSchedule.Location = new Point(620, 20);
            btnStartSchedule.Name = "btnStartSchedule";
            btnStartSchedule.Size = new Size(150, 30);
            btnStartSchedule.TabIndex = 0;
            btnStartSchedule.Text = "開始每月排程";
            btnStartSchedule.UseVisualStyleBackColor = true;
            btnStartSchedule.Click += btnStartSchedule_Click;

            // 🔹 lblLastRun
            lblLastRun.AutoSize = true;
            lblLastRun.Location = new Point(20, 30);
            lblLastRun.Name = "lblLastRun";
            lblLastRun.Size = new Size(91, 15);
            lblLastRun.TabIndex = 3;
            lblLastRun.Text = "上次執行時間：";

            // 🔹 lblNextRun
            lblNextRun.AutoSize = true;
            lblNextRun.Location = new Point(20, 50);
            lblNextRun.Name = "lblNextRun";
            lblNextRun.Size = new Size(91, 15);
            lblNextRun.TabIndex = 4;
            lblNextRun.Text = "下次執行時間：";

            // 🔹 txtLog
            txtLog.Font = new Font("Microsoft JhengHei UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 136);
            txtLog.Location = new Point(20, 70);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(750, 330);
            txtLog.TabIndex = 5;

            // 🔹 lblStartTime
            lblStartTime.AutoSize = true;
            lblStartTime.Location = new Point(20, 10);
            lblStartTime.Name = "lblStartTime";
            lblStartTime.Size = new Size(91, 15);
            lblStartTime.TabIndex = 6;
            lblStartTime.Text = "程式啟動時間：";
            lblStartTime.TextAlign = ContentAlignment.MiddleLeft;

            // 🔹 Form1
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnManualRun);
            Controls.Add(txtLog);
            Controls.Add(lblNextRun);
            Controls.Add(lblLastRun);
            Controls.Add(btnStartSchedule);
            Controls.Add(lblStartTime);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "Form1";
            Text = "鈦金會員獨享禮發送工具";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblStartTime;
        private Button btnStartSchedule;
        private Button btnManualRun;
        private Label lblLastRun;
        private Label lblNextRun;
        private TextBox txtLog;
    }
}
