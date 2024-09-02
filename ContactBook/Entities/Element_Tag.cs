using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    [Table("element_tag")]
    public class Element_Tag
    {
        [Required(ErrorMessage ="Идентификатор тега для связи с элементом обязателен")]
        public int elem_id { get; set; }
        [Required(ErrorMessage = "Идентификатор элемента для связи с тегом обязателен")]
        public int tag_id { get; set; }
    }
}
