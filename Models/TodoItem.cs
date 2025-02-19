using Newtonsoft.Json;
using SQLite;

namespace ToDo_LIst.Models
{
    /// <summary>
    /// Represents a to-do item
    /// The Tags property is stored as a JSON string in the database via the SerializedTags column.
    /// </summary>

    public class TodoItem : IIdentifiable
    {
        [PrimaryKey] 
        public string Identifier { get;  set; } 

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get;set; }
        public string Priority { get; set; } 
        public DateTime CreationDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string SerializedTags { get; set; } = "[]";  // A JSON representation of the Tags. This is stored in the database.
        [Ignore] public List<string> Tags  // A List of tags for the TodoItem. This property is not mapped to the database.
        {
            get => JsonConvert.DeserializeObject<List<string>>(SerializedTags) ?? [];
            set => SerializedTags = JsonConvert.SerializeObject(value); 
        } 
        public string? Parent { get; set; } 
        public string? AssignedTo { get; set; }
        public string Status { get; set; } 
        public string? Notes { get; set; }
        public int Points { get; set; }
        

        // Default constructor for a new TodoItem
        public TodoItem()
        {
            Identifier = IdHelper.GetNewID();
            Name = "none";
            Description = "none";
            Priority = "Normal";
            CreationDate = DateTime.Now;
            Tags = [];
            Parent = null;
            Status = "New";
            Points = 1;
        }

        // Parameterized constructor for a new TodoItem
        public TodoItem(
            string name, string description, // requiered 
            DateTime? dueDate = null, string? priority = null, string? assignedTo = null,
            string? notes = null, int points = 1,List<string>? tags = null, string? parent = null, 
            string? identifier = null, string? status = null)
        {
            Identifier = identifier ?? IdHelper.GetNewID();
            Name = name;
            Description = description;
            DueDate = dueDate;
            Priority = priority ?? "Normal";
            CreationDate = DateTime.Now;
            Tags = tags ?? new List<string>();
            Parent = parent;
            AssignedTo = assignedTo;
            if (status == null) { Status = assignedTo != null ? "Assigned" : "New"; }
            else {  Status = status; }
            Notes = notes;
            Points = points;
        }

    }
}