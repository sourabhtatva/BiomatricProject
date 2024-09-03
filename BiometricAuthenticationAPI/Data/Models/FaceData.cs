using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BiometricAuthenticationAPI.Data.Models
{
    [Table("faceData")]
    public class FaceData
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Column("userId")]
        public int UserId { get; set; }

        [Column("faceEncoding")]
        public byte[]? FaceEncoding { get; set; }

        [Column("image")]
        public string? Image { get; set; }

        [Column("capturedAt")]
        public DateTime CapturedAt { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
