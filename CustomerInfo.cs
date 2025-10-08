// Models/CustomerInfo.cs
using System.ComponentModel.DataAnnotations;

namespace TubeShopBackend.Models
{
    public class CustomerInfo
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Inn { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Comment { get; set; }
    }
}