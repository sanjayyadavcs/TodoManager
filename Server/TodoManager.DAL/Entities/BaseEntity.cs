using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TodoManager.DAL.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        [Column("Id")]
        public virtual int Id { get; set; }

        [Required]
        public virtual DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Required]
        public  virtual DateTime UpdatedOn {  get; set; } = DateTime.UtcNow;

        [AllowNull]
        public virtual DateTime? DeletedOn { get; set; }
    }
}
