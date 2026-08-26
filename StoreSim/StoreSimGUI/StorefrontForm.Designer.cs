namespace StoreSimGUI
{
    partial class StorefrontForm
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
            dgvProducts = new System.Windows.Forms.DataGridView();
            cmbCategories = new System.Windows.Forms.ComboBox();
            lblCategory = new System.Windows.Forms.Label();
            txtQuantity = new System.Windows.Forms.TextBox();
            lblQuantity = new System.Windows.Forms.Label();
            btnAddToCart = new System.Windows.Forms.Button();
            btnGoToCheckout = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(dgvProducts)).BeginInit();
            SuspendLayout();
            // 
            // dgvProducts
            // 
            dgvProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProducts.Location = new System.Drawing.Point(30, 80);
            dgvProducts.Name = "dgvProducts";
            dgvProducts.RowHeadersWidth = 72;
            dgvProducts.Size = new System.Drawing.Size(950, 500);
            dgvProducts.TabIndex = 0;
            // 
            // cmbCategories
            // 
            cmbCategories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbCategories.FormattingEnabled = true;
            cmbCategories.Location = new System.Drawing.Point(30, 30);
            cmbCategories.Name = "cmbCategories";
            cmbCategories.Size = new System.Drawing.Size(250, 38);
            cmbCategories.TabIndex = 1;
            // Explicitly hooked here to guarantee it fires when changed
            cmbCategories.SelectedIndexChanged += new System.EventHandler(this.cmbCategories_SelectedIndexChanged);
            // 
            // lblCategory
            // 
            lblCategory.AutoSize = true;
            lblCategory.Location = new System.Drawing.Point(30, 0);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new System.Drawing.Size(161, 30);
            lblCategory.TabIndex = 2;
            lblCategory.Text = "Filter by Category:";
            // 
            // txtQuantity
            // 
            txtQuantity.Location = new System.Drawing.Point(1030, 120);
            txtQuantity.Name = "txtQuantity";
            txtQuantity.Size = new System.Drawing.Size(150, 35);
            txtQuantity.TabIndex = 3;
            txtQuantity.Text = "1";
            // 
            // lblQuantity
            // 
            lblQuantity.AutoSize = true;
            lblQuantity.Location = new System.Drawing.Point(1030, 80);
            lblQuantity.Name = "lblQuantity";
            lblQuantity.Size = new System.Drawing.Size(97, 30);
            lblQuantity.TabIndex = 4;
            lblQuantity.Text = "Quantity:";
            // 
            // btnAddToCart
            // 
            btnAddToCart.Location = new System.Drawing.Point(1030, 180);
            btnAddToCart.Name = "btnAddToCart";
            btnAddToCart.Size = new System.Drawing.Size(200, 60);
            btnAddToCart.TabIndex = 5;
            btnAddToCart.Text = "Add to Cart";
            btnAddToCart.UseVisualStyleBackColor = true;
            btnAddToCart.Click += new System.EventHandler(this.btnAddToCart_Click);
            // 
            // btnGoToCheckout
            // 
            btnGoToCheckout.Location = new System.Drawing.Point(1030, 500);
            btnGoToCheckout.Name = "btnGoToCheckout";
            btnGoToCheckout.Size = new System.Drawing.Size(200, 80);
            btnGoToCheckout.TabIndex = 6;
            btnGoToCheckout.Text = "Go to Checkout";
            btnGoToCheckout.UseVisualStyleBackColor = true;
            btnGoToCheckout.Click += new System.EventHandler(this.btnGoToCheckout_Click);
            // 
            // StorefrontForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1280, 640);
            Controls.Add(btnGoToCheckout);
            Controls.Add(btnAddToCart);
            Controls.Add(lblQuantity);
            Controls.Add(txtQuantity);
            Controls.Add(lblCategory);
            Controls.Add(cmbCategories);
            Controls.Add(dgvProducts);
            Name = "StorefrontForm";
            Text = "StorefrontForm";
            ((System.ComponentModel.ISupportInitialize)(dgvProducts)).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvProducts;
        private System.Windows.Forms.ComboBox cmbCategories;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.Button btnAddToCart;
        private System.Windows.Forms.Button btnGoToCheckout;
    }
}