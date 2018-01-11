namespace MongoCore.WebApi.Models.Users
{
    public class AppUserTaskDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Done { get; set; }
    }
}