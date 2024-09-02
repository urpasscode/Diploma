using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities
{
    [Table("elem")]
    public class Element
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int elem_id { get; set; }
        [Required(ErrorMessage = "Тип элемента - обязательное поле")]
        public bool elem_type { get; set; }
        [Required(ErrorMessage = "Идентификатор пользователя - обязательное поле")]
        public int user_id { get; set; }
    }
}
