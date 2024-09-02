
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    [Table("tag")]
    public class Tag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tag_id { get; set; }

        [Required]
        [RegularExpression(@"^[а-яА-ЯёЁ_ ]", ErrorMessage = "Некорректное название цвета тега. Название " +
            "цвета тега может содержать только буквы русского алфавита")]
        public string tag_name { get; set; }
        public string? color { get; set; }

        [Required]
        public int user_id { get; set; }

        [Required]
        public bool tag_type { get; set; }

        //добавить id пользователя
    }

    public class TagAndElements<T>
    {
        public Tag tag { get; set; }
        public List<T> elements { get; set; }
    }

    public class TagModel
    {
        [Required(ErrorMessage = "Название тега - обязательное поле")]
        [MaxLength(30, ErrorMessage ="Максимальная длина названия тега - 30 символов")]
        public string tag_name { get; set; }
        [RegularExpression(@"^[а-яА-ЯёЁ_ ]", ErrorMessage = "Некорректное название цвета тега. Название цвета" +
           "заметки может содержать только буквы русского алфавита")]
        public string? color { get; set; }
        [Required(ErrorMessage = "Выберите элементы для тега")]
        public List<int> indexes { get; set; }
    }
}
