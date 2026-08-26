using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using StoreSim;

namespace StoreSimGUI
{
    public partial class StorefrontForm : Form
    {
        private readonly Store _store;
        private readonly int _customerId;
        private List<Product> _allProducts;

        public StorefrontForm(Store store, int customerId)
        {
            InitializeComponent();
            _store = store;
            _customerId = customerId;

            this.Text = $"StoreSim - Logged in as Customer ID: {_customerId}";

            // Explicitly wire up the event handler here so it's 100% guaranteed to fire,
            // regardless of whether the visual designer wired it correctly or not.
            this.cmbCategories.SelectedIndexChanged += new System.EventHandler(this.cmbCategories_SelectedIndexChanged);

            LoadCatalog();
        }

        private void LoadCatalog()
        {
            _allProducts = _store.GetAllProducts();
            dgvProducts.DataSource = _allProducts;

            // Prevent event firing loop while resetting the datasource/items
            cmbCategories.SelectedIndexChanged -= cmbCategories_SelectedIndexChanged;

            // Fill category drop down
            var categories = _allProducts.Select(p => p.Category).Distinct().ToList();
            categories.Insert(0, "All Categories");
            cmbCategories.DataSource = categories;

            // Re-enable the event
            cmbCategories.SelectedIndexChanged += cmbCategories_SelectedIndexChanged;
        }

        private void cmbCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = cmbCategories.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selected) || selected == "All Categories")
            {
                dgvProducts.DataSource = _allProducts;
            }
            else
            {
                dgvProducts.DataSource = _allProducts.Where(p => p.Category == selected).ToList();
            }
        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow?.DataBoundItem is Product selectedProduct)
            {
                if (int.TryParse(txtQuantity.Text, out int qty) && qty > 0)
                {
                    bool success = _store.ReserveItem(_customerId, selectedProduct.Id, qty);
                    if (success)
                    {
                        MessageBox.Show("Item reserved and added to cart!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCatalog(); // Refresh stock count
                    }
                    else
                    {
                        MessageBox.Show("Failed to reserve. Insufficient stock.", "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid quantity.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnGoToCheckout_Click(object sender, EventArgs e)
        {
            CheckoutForm checkoutForm = new CheckoutForm(_store, _customerId);
            checkoutForm.ShowDialog();
            LoadCatalog(); // Refresh catalog after checkout clears the cart
        }
    }
}