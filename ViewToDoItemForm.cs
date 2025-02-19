
/*
 * ViewItemForm.cs
 * 
 * This form displays and manages the details of a selected to-do item.
 * It allows users to view and update key item properties (description, assigned user, due date, priority,
 * creation date, and points), manage tags, and handle subtasks (adding, marking as complete, and deleting).
 * 
 * Major Functions:
 * - InitalizeComponent: Loads the current to-do item's data and configures UI controls.
 * - RefreshStatusCombo: Dynamically updates the status ComboBox based on the current state.
 * - LoadSubTasks: Retrieves and displays subtasks associated with the current item.
 * - BindDataGridView: Binds a list of subtask display objects to the DataGridView.
 */

using ToDo_LIst.Models;
using static ToDo_LIst.Form1;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace ToDo_LIst
{
    public partial class ViewItemForm : Form
    {
        public string Identifier { get; set; } = "";
        public string ItemName { get; set; } = "";
        public string? AssignedTo { get; set; }
        public string Description { get; set; } = "";
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = "Normal";
        public DateTime? CreationDate { get; set; }
        public List<string> Tags { get; set; } = [];
        public string? TaskParent { get; set; }
        public List<ToDoItemDisplay> SubTaskDisplayList { get; set; } = [];
        public string Status { get; set; } = "New";
        public int Points { get; set; } = 0;
        private ComboBox cmbStatus=new();
        private TextBox textDescription=new();
        private TextBox textAssignedTo=new();
        private ComboBox cmbPriority=new();
        private DateTimePicker dtpDueDate=new();
        private Label lblCreationDate=new();
        private NumericUpDown numPoints=new();
        private Label lblTags=new();
        private Button btnAddTag=new();
        private TextBox textAddTag=new();
        private Button btnRemoveTag=new();
        private DataGridView dataGridSubTasks=new();
        private ContextMenuStrip contextMenu=new();
        private ToolStripMenuItem deleteOption= new ToolStripMenuItem("Delete");
        private ToolStripMenuItem completeOption= new ToolStripMenuItem("Complete");
        private int RightClickIndex=-1;
        private Button btnAddSubTask=new();
        private Button btnUpdateToDoItem=new();

        public ViewItemForm(string id)
        {
            InitalizeComponent(id);
            LoadSubTasks();

            if (SubTaskDisplayList != null)
            {
                BindDataGridView(SubTaskDisplayList);
                this.Controls.Add(dataGridSubTasks);
            }
            RefreshStatusCombo();
        }
        /// <summary>
        /// Initializes the form with data for the selected to-do item and sets up the UI controls.
        /// </summary>
        /// <param name="id">The identifier of the to-do item to display.</param>
        private void InitalizeComponent(string id)
        {
            // Retrieve the to-do item from the data manager using its ID.
            DataManager<TodoItem> toDoDM = new();
            TodoItem currentToDo = toDoDM.GetRecord(id);

            // Populate form properties with item details.
            Identifier = currentToDo.Identifier;
            AssignedTo = currentToDo.AssignedTo;
            ItemName = currentToDo.Name;
            Description = currentToDo.Description;
            DueDate = currentToDo.DueDate;
            Priority = currentToDo.Priority;
            CreationDate = currentToDo.CreationDate;
            Tags = currentToDo.Tags;
            TaskParent = currentToDo.Parent;
            Status = currentToDo.Status;
            Points = currentToDo.Points;

            // Set form dimensions and title
            this.Text = ItemName;
            this.Width = 625;
            this.Height = 570;

            // Configure and position UI controls.
            ConfigureTextBox(textDescription, Description, new System.Drawing.Point(5, 5), 395);
            textDescription.Multiline = true;
            textDescription.Height = 50;

            Label labelAssignedTo = new();
            ConfigureLabel(labelAssignedTo, "Assigned To:", new System.Drawing.Point(405, 5), 85);
            ConfigureTextBox(textAssignedTo, AssignedTo ?? "", new System.Drawing.Point(490, 5), 115);
            
            Label statusLabel = new();
            ConfigureLabel(statusLabel, "Status:", new System.Drawing.Point(405, 30), 50);
            cmbStatus.Location = new System.Drawing.Point(490, 30);
            cmbStatus.Width = 115;

            Label dueDateLabel = new();
            ConfigureLabel(dueDateLabel, "Due:", new System.Drawing.Point(405, 55), 40);
            dtpDueDate.Location = Location = new System.Drawing.Point(490, 55);
            dtpDueDate.Format = DateTimePickerFormat.Short;
            dtpDueDate.Width = 115;
            if (DueDate != null)
            {
                dtpDueDate.Value = (DateTime)DueDate;
            }

            Label priorityLabel = new();
            ConfigureLabel(priorityLabel, "Priority:",new System.Drawing.Point(405, 85),50);
            cmbPriority.Location = new System.Drawing.Point(490, 85);
            cmbPriority.Width = 115;
            cmbPriority.Items.AddRange(new string[] { "Low", "Normal", "High", "Critical" });
            cmbPriority.Text = Priority;

            Label creationDateLabel = new();
            ConfigureLabel(creationDateLabel, "Created:", new System.Drawing.Point(405, 110), 55);
            ConfigureLabel(lblCreationDate, CreationDate.ToString() ?? "", new System.Drawing.Point(485, 110), 100);

            Label pointsLabel = new();
            ConfigureLabel(pointsLabel, "Points:", new System.Drawing.Point(405, 135), 55);
            numPoints.Value = Points;
            numPoints.Minimum = 1;
            numPoints.Maximum = 100;
            numPoints.Location = new System.Drawing.Point(490, 135);
            numPoints.Width = 115;
            
            Label tagsLabel = new();
            ConfigureLabel(tagsLabel, "Tags:", new System.Drawing.Point(5, 60), 200);
            ConfigureLabel(lblTags,string.Join(",",Tags),new System.Drawing.Point(5, 85),500);
            RefreshTags();
            ConfigureButton(btnAddTag, "Add Tag", new System.Drawing.Point(5, 110), 75);
            ConfigureTextBox(textAddTag, "", new System.Drawing.Point(90, 110), 100);
            ConfigureButton(btnRemoveTag, "Remove Tag", new System.Drawing.Point(300, 110), 90);
            
            Label subtaskLabel = new();
            ConfigureLabel(subtaskLabel, "Subtasks:", new System.Drawing.Point(5, 135), 100);
            dataGridSubTasks.Location = Location = new System.Drawing.Point(5, 160);
            dataGridSubTasks.Width = 600;
            dataGridSubTasks.Height = 300;
            dataGridSubTasks.AutoGenerateColumns = true;            
            ConfigureButton(btnAddSubTask, "Add Subtask", new System.Drawing.Point(5, 465), 75);
            ConfigureButton(btnUpdateToDoItem, "Update", new System.Drawing.Point(245, 475), 75);
            btnUpdateToDoItem.Height = 50;

            this.Controls.Add(textDescription);
            this.Controls.Add(labelAssignedTo);
            this.Controls.Add(textAssignedTo);
            this.Controls.Add(statusLabel);
            this.Controls.Add(cmbStatus);
            this.Controls.Add(dueDateLabel);
            this.Controls.Add(dtpDueDate);
            this.Controls.Add(priorityLabel);
            this.Controls.Add(cmbPriority);
            this.Controls.Add(creationDateLabel);
            this.Controls.Add(lblCreationDate);
            this.Controls.Add(pointsLabel);
            this.Controls.Add(numPoints);
            this.Controls.Add(tagsLabel);
            this.Controls.Add(lblTags);
            this.Controls.Add(btnAddTag);
            this.Controls.Add(textAddTag);
            this.Controls.Add(btnRemoveTag);
            this.Controls.Add(subtaskLabel);
            this.Controls.Add(btnUpdateToDoItem);
            this.Controls.Add(btnAddSubTask);

            // Attach event handlers forr user interactions.
            deleteOption.Click += DeleteOption_Click;
            completeOption.Click += CompleteOption_Click;
            btnAddSubTask.Click += AddSubTaskButton_Click;
            btnAddTag.Click += BttnAddTag_Click;
            btnRemoveTag.Click += BttnRemoveTag_Click;
            btnUpdateToDoItem.Click += ButtonUpdateToDoItem_Click;
            dataGridSubTasks.CellMouseClick += GridSubTasks_CellMouseClick;
            dataGridSubTasks.CellContentDoubleClick += GridSubTasks_CellContentDoubleClick;
            cmbStatus.SelectedValueChanged += CmbStatus_SelectedValueChanged;
            textAssignedTo.TextChanged += TextAssignedTo_TextChanged;
        }


        /// <summary>
        /// Updates the status ComboBox based on the current status and subtask completion.
        /// </summary>
        private void RefreshStatusCombo()
        {
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("New");
            if (Status == "New")
            {
                // If the status is 'New', then 'New' is the only available option.
                // The status will only change to 'Assigned' once a value is provided for the assignee.
                cmbStatus.SelectedIndex = 0;
            }
            else
            {
                cmbStatus.Items.Add("Assigned");
                if (Status == "Complete")
                {
                    cmbStatus.Items.Add("Complete");
                    cmbStatus.SelectedIndex = 2;
                }
                else
                {
                    cmbStatus.SelectedIndex = 1;
                    // Check if all subtasks are complete. If so, offer the option to mark the item complete
                    bool subtasksComplete = true;
                    if (SubTaskDisplayList != null)
                    {
                        foreach (ToDoItemDisplay subtask in SubTaskDisplayList)
                        {
                            if (subtask.Status != "Complete")
                            {
                                subtasksComplete = false;
                                break;
                            }
                        }
                    }
                    if (subtasksComplete)
                    {
                        cmbStatus.Items.Add("Complete");
                    }
                }
            }
        }
        private void BttnRemoveTag_Click(object? sender, EventArgs e)
        {

            if (Tags.Count > 0)
            {
                Tags.RemoveAt(Tags.Count - 1);
                RefreshTags();
            }
        }

        /// <summary>
        /// Handles the "Complete" context menu option click for a subtask.
        /// Updates the subtask's status to "Complete" and refreshes the UI accordingly.
        /// </summary>
        private void CompleteOption_Click(object? sender, EventArgs e)
        {
            DataManager<TodoItem> todoDM = new();
                
                // Retrieve the subtask's ID from the DataGridView based on the right-clicked row.
                var id = dataGridSubTasks.Rows[RightClickIndex].Cells[0].Value.ToString();
                if (id != null)
                {
                    // Get the subtask from the data store and update its status
                    TodoItem? updatedItem = todoDM.GetRecord(id);
                    if (updatedItem != null)
                    {
                        updatedItem.Status = "Complete";
                        todoDM.UpdateItem(updatedItem);
                        
                        // Refresh the subtask list and update the status ComboBox.
                        LoadSubTasks();
                        BindDataGridView(SubTaskDisplayList);
                        RefreshStatusCombo();
                    }
                }
            
        }

        private void TextAssignedTo_TextChanged(object? sender, EventArgs e)
        {
            if (textAssignedTo.Text.Length > 0)
            {
                if (Status == "New") { Status = "Assigned"; }
                if (cmbStatus.SelectedIndex == 0)
                {
                    RefreshStatusCombo();
                }
            }
            else
            {             
                if (cmbStatus.SelectedIndex > 0)
                {
                    cmbStatus.SelectedIndex = 0;
                }
            }
        }

        private void CmbStatus_SelectedValueChanged(object? sender, EventArgs e)
        {
            if (cmbStatus.SelectedIndex == 0)
            {
                textAssignedTo.Text = string.Empty;
                for (int i = cmbStatus.Items.Count - 1; i > 0; i--)
                {
                    cmbStatus.Items.RemoveAt(i);
                }
            }
        }

        private void GridSubTasks_CellContentDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridSubTasks.Rows[e.RowIndex].Cells["Id"].Value != null)
            {
                var selectonRow = dataGridSubTasks.Rows[e.RowIndex];
                string todoItemID = selectonRow.Cells["Id"].Value?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(todoItemID))
                {
                    var todoForm = new ViewItemForm(todoItemID);
                    todoForm.ShowDialog();

                    LoadSubTasks();
                    BindDataGridView(SubTaskDisplayList);
                    RefreshStatusCombo();
                }
            }
        }

        private void GridSubTasks_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.Button == MouseButtons.Right)
            {
                RightClickIndex = e.RowIndex;
                ToDoItemDisplay row = SubTaskDisplayList[e.RowIndex];
                contextMenu.Items.Clear();
                contextMenu.Items.Add(deleteOption);

                if (row.Status == "Assigned")
                {
                    bool subTasksComplete = true;
                    DataManager<TodoItem> subTaskDM = new();
                    List<TodoItem> subTasks = subTaskDM.GetRecordsBySecondaryId("Parent", row.Id);
                    foreach (var subTask in subTasks)
                    {
                        if (subTask.Status != "Complete")
                        {
                            subTasksComplete = false;
                            break;
                        }
                    }
                    if (subTasksComplete)
                    {
                        contextMenu.Items.Add(completeOption);
                    }
                }

                contextMenu.Show(Cursor.Position);
            }
        }

        private void DeleteOption_Click(object? sender, EventArgs e)
        {
            DataManager<TodoItem> todoDM = new();
            var id = dataGridSubTasks.Rows[RightClickIndex].Cells[0].Value.ToString();
            if (id != null)
            {
                todoDM.DeleteItem(id);
                LoadSubTasks() ;
                BindDataGridView(SubTaskDisplayList);
                RefreshStatusCombo();
            }
        }

        private void ButtonUpdateToDoItem_Click(object? sender, EventArgs e)
        {
            TodoItem updatedToDo = new(ItemName, textDescription.Text, dtpDueDate.Value, cmbPriority.Text,
                textAssignedTo.Text, points: (int)numPoints.Value, tags:Tags, parent: TaskParent,
                identifier: Identifier, status: cmbStatus.Text);
            DataManager<TodoItem> dm = new();
            dm.UpdateItem(updatedToDo);
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void RefreshTags()
        {
            string tagString = string.Join(",", Tags);
            lblTags.Text = tagString;
        }
        private void BttnAddTag_Click(object? sender, EventArgs e)
        {
            if (textAddTag.Text.Length > 0)
            {
                Tags.Add(textAddTag.Text);
                textAddTag.Text = "";
                RefreshTags();
            }
        }

        private void LoadSubTasks()
        {
            DataManager<TodoItem> todoDM = new();
            var T = todoDM.GetRecordsBySecondaryId("Parent", Identifier);
            SubTaskDisplayList = T.Select(item => new ToDoItemDisplay {
                Id = item.Identifier,
                Name = item.Name,
                Priority = item.Priority,
                Status = item.Status,
                AssignedTo = item.AssignedTo,
                Description = item.Description,

            }).ToList();
        }
        private void BindDataGridView<T>(List<T> table)
        {
            Console.WriteLine(string.Join(",", table));

            if (dataGridSubTasks.Columns.Count == 0)
            {
                dataGridSubTasks.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", DataPropertyName = "Id", Visible = false, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
                dataGridSubTasks.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", DataPropertyName = "Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
                dataGridSubTasks.Columns.Add(new DataGridViewTextBoxColumn { Name = "Priority", DataPropertyName = "Priority", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
                dataGridSubTasks.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", DataPropertyName = "Status", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
                dataGridSubTasks.Columns.Add(new DataGridViewTextBoxColumn { Name = "AssignedTo", DataPropertyName = "AssignedTo", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
                dataGridSubTasks.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", DataPropertyName = "Description", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            }
            dataGridSubTasks.DataSource = table;

        }
        private void AddSubTaskButton_Click(object? sender, EventArgs e)
        {
            using AddTodoForm todoForm = new(Identifier);
            if (todoForm.ShowDialog() == DialogResult.OK && todoForm.CreatedItem != null)
            {
                DataManager<TodoItem> todoDM = new();
                todoDM.InsertItem(todoForm.CreatedItem);

                LoadSubTasks();
                BindDataGridView(SubTaskDisplayList);
                RefreshStatusCombo();

            }
        }
        /// <summary>
        /// Utility method to configure a Label control with the specified text, location, and width.
        /// </summary>
        private void ConfigureLabel(Label labelName, string text, Point location, int width)
        {
            labelName.Text = text;
            labelName.Location = location;
            labelName.Width = width;
        }
        /// <summary>
        /// Utility method to configure a Button control with the specified text, location, and width.
        /// </summary>
        private void ConfigureButton(Button buttonName, string text, Point location, int width)
        {
            buttonName.Text = text;
            buttonName.Location = location;
            buttonName.Width = width;
        }
        /// <summary>
        /// Utility method to configure a TextBox control with the specified text, location, and width.
        /// </summary>
        private void ConfigureTextBox(TextBox textBoxName, string text, Point location, int width)
        {
            textBoxName.Text = text;
            textBoxName.Location = location;
            textBoxName.Width = width;
        }
    }
}