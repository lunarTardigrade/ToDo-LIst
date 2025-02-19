using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_LIst.Models;
using JetBrains.Annotations;

namespace ToDo_LIst
{
    /// <summary>
    /// A form for creating a new Todo item
    /// </summary>
    public partial class AddTodoForm : Form
    {
        public TodoItem? CreatedItem { get; set; }
        private TextBox txtName;
        private TextBox txtDescription;
        private ComboBox cmbPriority;
        private DateTimePicker dtpDueDate;
        private TextBox txtAssignedTo;
        private NumericUpDown numPoints;
        private Button btnSave;
        private Button btnCancel;
        public string? ParentItem;

        public AddTodoForm(string? parent = null)
        {
            ParentItem = parent;
            this.Text = "New Something To Do :)";
            this.Width = 400;
            this.Height = 400;

            // Create and configure controls.
            Label lblName = new()
            {
                Text = "Name:",
                Location = new System.Drawing.Point(10, 10)
            };
            txtName = new()
            {
                Location = new System.Drawing.Point(120, 10),
                Width = 250
            };
            Label lblDescription = new()
            {
                Text = "Description:",
                Location = new System.Drawing.Point(10, 50),

            };
            txtDescription = new()
            {
                Location = new System.Drawing.Point(120, 50),
                Width = 250,
                Height = 60,
                Multiline = true
            };
            Label lblPriority = new()
            {
                Text = "Priority:",
                Location = new System.Drawing.Point(10, 120)
            };
            cmbPriority = new() 
            { 
                Location = new System.Drawing.Point(120, 120),
                Width = 120 
            };
            cmbPriority.Items.AddRange(new string[] { "Low", "Normal", "High", "Critical" });
            cmbPriority.SelectedIndex = 1;

            Label lblDueDate = new() 
            {
                Text = "Due Date:",
                Location = new System.Drawing.Point(10, 160) 
            };
            dtpDueDate = new()
            { 
                Location = new System.Drawing.Point(120, 160),
                Width = 250 
            };

            Label lblAssignedTo = new()
            { 
                Text = "Assigned To:", Location = new System.Drawing.Point(10, 200)
            };
            txtAssignedTo = new()
            {
                Location = new System.Drawing.Point(120, 200), Width = 250 
            };

            Label lblPoints = new() 
            { 
                Text = "Points:",
                Location = new System.Drawing.Point(10, 240)
            };
            numPoints = new() 
            {
                Location = new System.Drawing.Point(120, 240),
                Minimum = 1,
                Maximum = 100,
                Value = 1
            };

            btnSave = new() 
            {
                Text = "Save", Location = new System.Drawing.Point(120, 300), Width = 80 
            };
            btnCancel = new()
            {
                Text = "Cancel", Location = new System.Drawing.Point(220, 300), Width = 80 
            };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.Close();

            // Add controls to the form
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
            this.Controls.Add(lblPriority);
            this.Controls.Add(cmbPriority);
            this.Controls.Add(lblDueDate);
            this.Controls.Add(dtpDueDate);
            this.Controls.Add(lblAssignedTo);
            this.Controls.Add(txtAssignedTo);
            this.Controls.Add(lblPoints);
            this.Controls.Add(numPoints);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);

        }
        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Validate required fields.
            if (string.IsNullOrWhiteSpace(this.txtName.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Name and Description are required.", "!!!! ERROR !!!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create the new TodoItem.
            CreatedItem = new TodoItem(
                txtName.Text,
                txtDescription.Text,
                dtpDueDate.Value,
                cmbPriority.SelectedItem != null ? cmbPriority.SelectedItem.ToString() : "Normal",
                string.IsNullOrWhiteSpace(txtAssignedTo.Text) ? null : txtAssignedTo.Text, null, (int)numPoints.Value,parent:ParentItem
             );
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        
    }
}
