using System.ComponentModel.DataAnnotations;

namespace MongoCore.WebApi.Models.Users
{
    public class AppUserLoginDto
    {
        [Required]
        [MaxLength(12)]
        public string UserName { get; set; }
        
        [Required]
        [MaxLength(40)]
        public string UserPassword { get; set; }

    }
}