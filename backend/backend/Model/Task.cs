namespace backend.Model
{
    public class Task : IModel
    {
        public int ID { get; set; }
        public UserDTO User { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PostTime { get; set; }
    }
}
