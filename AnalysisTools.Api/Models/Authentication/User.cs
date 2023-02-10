using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace analysistools.api.Models.Authentication
{
    /// <summary>
    /// User model for authentication, stored in the sqlite database.
    /// </summary>
    public class User
    {
        [JsonIgnore]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
