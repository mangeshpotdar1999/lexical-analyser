using System;

namespace InhaCC
{
    partial class MainForm
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

        #region Windows Form


        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.rtbLS = new System.Windows.Forms.RichTextBox();
            this.rtbLT = new System.Windows.Forms.RichTextBox();
            this.rtbLLD = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.bLT = new System.Windows.Forms.Button();
            this.bLG = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.rtbLS);
            this.tabPage2.Controls.Add(this.rtbLT);
            this.tabPage2.Controls.Add(this.rtbLLD);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.bLT);
            this.tabPage2.Controls.Add(this.bLG);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(779, 422);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Lexer Generator";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 182);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "Sample Code";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(689, 386);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "About Us";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // rtbLS
            // 
            this.rtbLS.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLS.BackColor = System.Drawing.Color.WhiteSmoke;
            this.rtbLS.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtbLS.Location = new System.Drawing.Point(404, 35);
            this.rtbLS.Name = "rtbLS";
            this.rtbLS.Size = new System.Drawing.Size(334, 325);
            this.rtbLS.TabIndex = 6;
            this.rtbLS.Text = "";
            // 
            // rtbLT
            // 
            this.rtbLT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rtbLT.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtbLT.Location = new System.Drawing.Point(31, 203);
            this.rtbLT.Name = "rtbLT";
            this.rtbLT.Size = new System.Drawing.Size(367, 156);
            this.rtbLT.TabIndex = 3;
            this.rtbLT.Text = "#include<stdio.h>\n#include<conio.h>\nvoid main(){\nint a =10,b=10,c;\nc = a+b;\nprint" +
    "f(\'addition = 20\');\ngetch();\n}";
            this.rtbLT.TextChanged += new System.EventHandler(this.rtbLT_TextChanged);
            // 
            // rtbLLD
            // 
            this.rtbLLD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.rtbLLD.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtbLLD.Location = new System.Drawing.Point(31, 35);
            this.rtbLLD.Name = "rtbLLD";
            this.rtbLLD.Size = new System.Drawing.Size(367, 101);
            this.rtbLLD.TabIndex = 1;
            this.rtbLLD.Text = resources.GetString("rtbLLD.Text");
            this.rtbLLD.TextChanged += new System.EventHandler(this.rtbLLD_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Malgun Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(401, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Status:";
            // 
            // bLT
            // 
            this.bLT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bLT.Font = new System.Drawing.Font("Malgun Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bLT.Location = new System.Drawing.Point(165, 373);
            this.bLT.Name = "bLT";
            this.bLT.Size = new System.Drawing.Size(95, 34);
            this.bLT.TabIndex = 4;
            this.bLT.Text = "Test!";
            this.bLT.UseVisualStyleBackColor = true;
            this.bLT.Click += new System.EventHandler(this.bLT_Click);
            // 
            // bLG
            // 
            this.bLG.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bLG.Font = new System.Drawing.Font("Malgun Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bLG.Location = new System.Drawing.Point(165, 152);
            this.bLG.Name = "bLG";
            this.bLG.Size = new System.Drawing.Size(95, 34);
            this.bLG.TabIndex = 2;
            this.bLG.Text = "Generate! ";
            this.bLG.UseVisualStyleBackColor = true;
            this.bLG.Click += new System.EventHandler(this.bLG_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Lexer Definition: ";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(787, 450);
            this.tabControl1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 474);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Malgun Gothic", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GUI - Compiler";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void rtbLLD_TextChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox rtbLS;
        private System.Windows.Forms.RichTextBox rtbLT;
        private System.Windows.Forms.RichTextBox rtbLLD;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bLT;
        private System.Windows.Forms.Button bLG;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
    }
}

#endregion