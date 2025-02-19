using ToDo_LIst.Models;

namespace ToDo_LIst
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// The starting form and top level view of the todo items without parents
        /// </summary>
        private List<TodoItem> toDoItemTable = [];
        private DataGridView dataGridView1 = new();
        private Button btnAddTodo = new();
        private List<ToDoItemDisplay> displayToDoItems = [];
        private ContextMenuStrip contextMenu = new();
        private int RightClickIndex;

        public Form1()
        {
            InitializeComponent();
            LoadToDoItems();

            if (displayToDoItems != null)
            {
                BindDataGridView(displayToDoItems);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "What TO DO?";
            this.Width = 835;
            this.Height = 690;

            dataGridView1.Width = 800;
            dataGridView1.Height = 600;
            dataGridView1.Location = new System.Drawing.Point(10, 10);
            dataGridView1.AutoGenerateColumns = true;

            btnAddTodo.Text = "Add Something New To Do";
            btnAddTodo.Location = new System.Drawing.Point(10, 620);
            btnAddTodo.Width = 160;
            
            btnAddTodo.Click += BtnAddTodo_Click;

            // Setup context menu for deletion
            ToolStripMenuItem deleteOption = new ToolStripMenuItem("Delete");
            contextMenu.Items.Add(deleteOption);
            deleteOption.Click += DeleteOption_Click;

            // Add controls
            this.Controls.Add(dataGridView1);
            this.Controls.Add(btnAddTodo);

            // Event handlers for DataGridView
            dataGridView1.CellContentDoubleClick += DataGridView1_CellDoubleClick;
            dataGridView1.CellMouseClick += DataGridView1_CellMouseClick;
        }

        private void DeleteOption_Click(object? sender, EventArgs e)
        {
            DataManager<TodoItem> todoDM = new();
            var id = dataGridView1.Rows[RightClickIndex].Cells[0].Value.ToString();
            if (id != null) { 
                todoDM.DeleteItem(id);
                RefreshData();
            }        
        }

        private void DataGridView1_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.Button == MouseButtons.Right)
            {
                RightClickIndex = e.RowIndex;
                contextMenu.Show(Cursor.Position);
            }
        }

        private void DataGridView1_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].Cells["Id"].Value != null)
            {        
                var selectedRow = dataGridView1.Rows[e.RowIndex];
                string todoItemId = selectedRow.Cells["Id"].Value?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(todoItemId))
                {
                    using var todoForm = new ViewItemForm(todoItemId);
                    todoForm.ShowDialog();
                    RefreshData();
                }
            }
        }


        private void BtnAddTodo_Click(object? sender, EventArgs e)
        {
            using AddTodoForm todoForm = new();
            if (todoForm.ShowDialog() == DialogResult.OK && todoForm.CreatedItem != null)
            {
                DataManager<TodoItem> todoDM = new();
                todoDM.InsertItem(todoForm.CreatedItem);
                RefreshData();
            }
        }
        private void RefreshData()
        {
            LoadToDoItems();
            BindDataGridView(displayToDoItems);
        }
        private void LoadToDoItems()
        {
            DataManager<TodoItem> UserDM = new();

            toDoItemTable = UserDM.GetTable();
            var filteredToDoItems = toDoItemTable
                .Where(item => item.Status != "Complete" && item.Parent == null)
                .Select(item => new ToDoItemDisplay
                {
                    Id = item.Identifier,
                    Name = item.Name,
                    Priority = item.Priority,
                    Status = item.Status,
                    AssignedTo = item.AssignedTo,
                    Description = item.Description
                })
              .ToList();
            displayToDoItems = filteredToDoItems;
        }

        private void BindDataGridView<T>(List<T> table)
        {
            dataGridView1.DataSource = table;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (column.Name == "Description")
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                else { column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; }
                if (column.Name == "Id")
                {
                    column.Visible = false;
                }
            }
        }

        // Display model for the DataGridView
        public class ToDoItemDisplay
        {
            public string Id { get; set; } = "" ;
            public string Name { get; set; } = "" ;
            public string Priority { get; set; } = "Normal" ;
            public string Status { get; set; } = "" ;
            public string? AssignedTo { get; set; }
            public string Description { get; set; } = "" ;
        }

    }
}
