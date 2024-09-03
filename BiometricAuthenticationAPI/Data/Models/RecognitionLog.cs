using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BiometricAuthenticationAPI.Data.Models
{
    [Table("recognitionLogs")]
    public class RecognitionLog
    {
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("RecognitionTime")]
        public DateTime RecognitionTime { get; set; }

        [Column("ConfidenceLevel")]
        public float ConfidenceLevel { get; set; }

        [Column("Status")]
        public string? Status { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
