using SQLite;
using Newtonsoft.Json;

namespace ToDo_LIst.Models
{
    public interface IIdentifiable
    {
        string Identifier { get; }
    }
    public static class IdHelper
    {
        public static string GetNewID()
        {
            var newID = Guid.NewGuid();
            return newID.ToString();
        }
    }
    public class Score(DateTime date, int points)
    {
        public DateTime Date { get; set; } = date;
        public int Points { get; set; } = points;
    }
    
    public class User : IIdentifiable
    {
        [PrimaryKey] public string Identifier { get; private set; } 
        public string WholeName { get; set; }
        public int Age { get; set; }
        [Ignore] public List<Score> ScoresList { get; set; }
        [Column("Scores")] public string SocresSerialized
        {
            get => JsonConvert.SerializeObject(ScoresList);
            set => ScoresList = JsonConvert.DeserializeObject<List<Score>>(value)?? [];
        }

        public User() 
        {
            Identifier = IdHelper.GetNewID();
            WholeName = string.Empty;
            Age = 0;
            ScoresList = [];
        }

        public User(string wholeName, int age, List<Score>? scoresList = null, string? identifier = null)
        {
            Identifier = identifier ?? IdHelper.GetNewID();
            WholeName = wholeName;
            Age = age;
            ScoresList = scoresList ?? ([]);
        }

        public void AwardPoints(int points)
        {
            var newScore = new Score(DateTime.Now, points);
            ScoresList.Add(newScore);
        }
    }
}
