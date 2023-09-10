using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GhumGhamNepal.Core.Models.DbEntity
{
    [Table("CommonAttachment")]
    public class CommonAttachment
    {
        [Key]
        public long AttachmentID { get; set; }

        [Required]
        [StringLength(150)]
        public string? ParentTableName { get; set; }

        [Required]
        [StringLength(150)]
        public string? ParentRecordID { get; set; }

        [Required]
        [StringLength(150)]
        public string? ServerFileName { get; set; }

        [StringLength(150)]
        public string? ServerThumbnailFileName { get; set; }

        [Required]
        [StringLength(150)]
        public string? UserFileName { get; set; }

        [StringLength(50)]
        public string? FileFormat { get; set; }

        [StringLength(150)]
        public string? FileType { get; set; }

        [StringLength(150)]
        public string? FileLocation { get; set; }

        [StringLength(50)]
        public string? Size { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }

}
