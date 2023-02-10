using System.ComponentModel.DataAnnotations;

namespace analysistools.api.Models.Authentication
{
    /// <summary>
    /// User login dto, this is what the API recieves from the client. (Only username and password).
    /// </summary>
    public class UserLogin
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
