namespace StoreSimGUI
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new System.Windows.Forms.Label();
            txtCustomerName = new System.Windows.Forms.TextBox();
            btnLogin = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(621, 316);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(175, 30);
            label1.TabIndex = 0;
            label1.Text = "Enter your Name:";
            label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtCustomerName
            // 
            txtCustomerName.Location = new System.Drawing.Point(601, 364);
            txtCustomerName.Name = "txtCustomerName";
            txtCustomerName.Size = new System.Drawing.Size(227, 35);
            txtCustomerName.TabIndex = 1;
            // 
            // btnLogin
            // 
            btnLogin.Location = new System.Drawing.Point(510, 463);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new System.Drawing.Size(412, 147);
            btnLogin.TabIndex = 2;
            btnLogin.Text = "Login / Register";
            btnLogin.UseVisualStyleBackColor = true;
            // Explicitly hooked here to guarantee it triggers your login event handler
            btnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1476, 786);
            Controls.Add(btnLogin);
            Controls.Add(txtCustomerName);
            Controls.Add(label1);
            Name = "LoginForm";
            Text = "LoginForm - StoreSim";
            Load += new System.EventHandler(this.LoginForm_Load);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Button btnLogin;
    }
}