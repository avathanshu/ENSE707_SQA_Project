namespace StoreSimGUI
{
    partial class CheckoutForm
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
            dgvCartItems = new System.Windows.Forms.DataGridView();
            btnRemoveItem = new System.Windows.Forms.Button();
            btnCancelOrder = new System.Windows.Forms.Button();
            btnCompleteCheckout = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(dgvCartItems)).BeginInit();
            SuspendLayout();
            // 
            // dgvCartItems
            // 
            dgvCartItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCartItems.Location = new System.Drawing.Point(237, 60);
            dgvCartItems.Name = "dgvCartItems";
            dgvCartItems.RowHeadersWidth = 72;
            dgvCartItems.Size = new System.Drawing.Size(1038, 485);
            dgvCartItems.TabIndex = 0;
            // 
            // btnRemoveItem
            // 
            btnRemoveItem.Location = new System.Drawing.Point(256, 623);
            btnRemoveItem.Name = "btnRemoveItem";
            btnRemoveItem.Size = new System.Drawing.Size(170, 82);
            btnRemoveItem.TabIndex = 1;
            btnRemoveItem.Text = "Remove Selected Items";
            btnRemoveItem.UseVisualStyleBackColor = true;
            btnRemoveItem.Click += new System.EventHandler(this.btnRemoveItem_Click);
            // 
            // btnCancelOrder
            // 
            btnCancelOrder.Location = new System.Drawing.Point(461, 623);
            btnCancelOrder.Name = "btnCancelOrder";
            btnCancelOrder.Size = new System.Drawing.Size(174, 82);
            btnCancelOrder.TabIndex = 2;
            btnCancelOrder.Text = "Cancel Order (Clear Cart)";
            btnCancelOrder.UseVisualStyleBackColor = true;
            btnCancelOrder.Click += new System.EventHandler(this.btnCancelOrder_Click);
            // 
            // btnCompleteCheckout
            // 
            btnCompleteCheckout.Location = new System.Drawing.Point(894, 594);
            btnCompleteCheckout.Name = "btnCompleteCheckout";
            btnCompleteCheckout.Size = new System.Drawing.Size(381, 140);
            btnCompleteCheckout.TabIndex = 3;
            btnCompleteCheckout.Text = "Complete Checkout";
            btnCompleteCheckout.UseVisualStyleBackColor = true;
            btnCompleteCheckout.Click += new System.EventHandler(this.btnCompleteCheckout_Click);
            // 
            // CheckoutForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1476, 786);
            Controls.Add(btnCompleteCheckout);
            Controls.Add(btnCancelOrder);
            Controls.Add(btnRemoveItem);
            Controls.Add(dgvCartItems);
            Name = "CheckoutForm";
            Text = "CheckoutForm - StoreSim";
            ((System.ComponentModel.ISupportInitialize)(dgvCartItems)).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvCartItems;
        private System.Windows.Forms.Button btnRemoveItem;
        private System.Windows.Forms.Button btnCancelOrder;
        private System.Windows.Forms.Button btnCompleteCheckout;
    }
}