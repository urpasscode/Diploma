using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    [Table("phonenumber")]
    public class PhoneNumber
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int phone_id { get; set; }
        [Required]
        public string pn { get; set; }
        [Required(ErrorMessage = "Идентификатор контакта для номера телефона - обязательное поле")]
        public int elem_id { get; set; }
        public int? device_id { get; set; }
    }

    public class PhoneNumberCreateModel
    {
        [Required(ErrorMessage = "Номер телефона - обязательное поле")]
        [RegularExpression(@"^((\+7|7|8)+([0-9]){10})$", ErrorMessage = "Номер телефона введен некорректно")]
        [MaxLength(12, ErrorMessage ="максимальная длина номера телефона - 12 символов")]
        public string pn { get; set; }
        public int? device_id { get; set; }
    }
}
