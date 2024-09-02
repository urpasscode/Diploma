using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    [Table("frequency")]
    public class Frequency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int freq_id { get; set; }
        [Required(ErrorMessage ="Переодичность напоминания - обязательное поле")]
        public string period { get; set; }
    }
}
