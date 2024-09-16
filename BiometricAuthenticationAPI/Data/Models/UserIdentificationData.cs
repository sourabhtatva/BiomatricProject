using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BiometricAuthenticationAPI.Data.Models
{
    [Table("userIdentificationData")]
    public class UserIdentificationData
    {
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Column("FirstName")]
        public string? FirstName { get; set; }
        
        [Column("LastName")]
        public string? LastName { get; set; }

        [Column("Email")]
        public string? Email { get; set; }

        [Column("PhoneNumber")]
        public string? PhoneNumber { get; set; }

        [Column("UserIdNumber")]
        public string? UserIdNumber { get; set; }

        [Column("UserIdType")]
        public int UserIdType { get; set; }
        
        [Column("UserImage")]
        public byte[]? UserImage { get; set; }

        [Column("IsBlacklistUser")]
        public bool IsBlacklistUser { get; set; } = false;

        [Column("Address")]
        public string? Address { get; set; }

        [Column("City")]
        public string? City { get; set; }

        [Column("State")]
        public string? State { get; set; }

        [Column("ZipCode")]
        public string? ZipCode { get; set; }

        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("UserIdType")]
        public static UserIdentificationType? UserIdentificationType { get; set; }
    }
}
