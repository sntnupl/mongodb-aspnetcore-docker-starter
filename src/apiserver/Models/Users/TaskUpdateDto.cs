using System.ComponentModel.DataAnnotations;

namespace MongoCore.WebApi.Models.Users
{
    public class TaskUpdateDto
    {
        [MaxLength(80, ErrorMessage = "Task Name can't be more than 80 chars in length")]
        public string Name { get; set; }

        [MaxLength(160, ErrorMessage = "Task Description can't be more than 160 chars in length")]
        public string Description { get; set; }

        public bool Done { get; set; }
    }
}