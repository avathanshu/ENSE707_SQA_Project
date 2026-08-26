using System;
using System.Collections.Generic;
using System.Windows.Forms;
using StoreSim;

namespace StoreSimGUI
{
    public partial class CheckoutForm : Form
    {
        private readonly Store _store;
        private readonly int _customerId;

        public CheckoutForm(Store store, int customerId)
        {
            InitializeComponent();
            _store = store;
            _customerId = customerId;
            this.Text = "Checkout & Cart Management";

            LoadCart();
        }

        private void LoadCart()
        {
            List<CartDisplayItem> cart = _store.GetCustomerCart(_customerId);
            dgvCartItems.DataSource = null;
            dgvCartItems.DataSource = cart;
        }

        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            if (dgvCartItems.CurrentRow?.DataBoundItem is CartDisplayItem selectedItem)
            {
                // Return the entire quantity of this item back to inventory
                bool success = _store.ReturnItemToInventory(_customerId, selectedItem.ProductId, selectedItem.Quantity);
                if (success)
                {
                    MessageBox.Show("Item removed from cart and inventory restored.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCart();
                }
                else
                {
                    MessageBox.Show("Failed to remove item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select an item to remove.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Are you sure you want to cancel your order and return all items to stock?",
                "Cancel Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                _store.CancelEntireOrder(_customerId);
                MessageBox.Show("Order cancelled. Cart cleared.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void btnCompleteCheckout_Click(object sender, EventArgs e)
        {
            bool success = _store.CompleteCheckout(_customerId);
            if (success)
            {
                MessageBox.Show("Checkout complete! Thank you for your order.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Your cart is empty or checkout failed.", "Checkout Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}