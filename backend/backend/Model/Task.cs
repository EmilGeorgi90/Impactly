namespace backend.Model
{
    public class Task : IModel
    {
        public int id { get; set; }
        public UserDTO? UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime PostTime { get; set; }
    }
}
