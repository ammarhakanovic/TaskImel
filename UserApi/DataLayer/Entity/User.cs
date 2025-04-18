using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserApi.DataLayer.Entity
{
    public class User
    {
        //[JsonIgnore]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
    }
}
