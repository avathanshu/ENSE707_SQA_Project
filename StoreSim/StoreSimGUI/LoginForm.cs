using System;
using System.Windows.Forms;
using StoreSim;

namespace StoreSimGUI
{
    public partial class LoginForm : Form
    {
        private readonly Store _store;

        public LoginForm()
        {
            InitializeComponent();
            _store = new Store();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // leaving empty to stop win form error
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // leaving empty to stop error
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string name = txtCustomerName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Register customer into local db
            int customerId = _store.RegisterCustomer(name);

            // pass props to storefront
            StorefrontForm storeForm = new StorefrontForm(_store, customerId);
            storeForm.Show();

            // Hide the login window
            this.Hide();
        }
    }
}