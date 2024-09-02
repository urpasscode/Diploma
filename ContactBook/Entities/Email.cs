using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    [Table("email")]
    public class Email
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int email_id { get; set; }
        [Required]
        public string email_address { get; set; }
        [Required(ErrorMessage ="Идентификатор контакта обязателен для электронной почты")]
        public int elem_id { get; set; }
    }

    public class EmailModel
    {
        [Required]
        [RegularExpression(@"^[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+[.][A-Za-z]+$", ErrorMessage = "Адрес электронный почты введен некорректно")]
        [MaxLength(30, ErrorMessage ="Максимальная длина адреса электронной почты - 30 символов")]
        public string email_address { get; set; }
    }
}