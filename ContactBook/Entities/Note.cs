using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebApplication1.Entities
{
    [Table("note")]
    public class Note
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int elem_id { get; set; }
        public string note_name { get; set; }
        public DateOnly note_create { get; set; }
        public string? description { get; set; }
    }

    public class NoteCreateModel
    {
        [Required(ErrorMessage = "Название заметки - обязательное поле")]
        [RegularExpression(@"^^[а-яА-ЯёЁa-zA-Z0-9\.\-\,\s ]+$", ErrorMessage = "Некорректное название заметки. Название " +
            "заметки может содержать буквы русского и латинского алфавитов, цифры и знаки . , - , ,")]
        [MinLength(1, ErrorMessage = "Минимальная длина названия заметки - 1 символ")]
        [MaxLength(50, ErrorMessage = "Максимальная длина названия заметки - 50 символов")]
        public string note_name { get; set; }
        [RegularExpression(@"^^[а-яА-ЯёЁa-zA-Z0-9\.\-\,\s ]+$", ErrorMessage = "Некорректное описание заметки. Описание " +
            "заметки может содержать буквы русского и латинского алфавитов, цифры и знаки . , - , ,")]
        [MaxLength(150, ErrorMessage = "Максимальная длина описания заметки - 150 символов")]
        public string? description { get; set; }
    }
}
