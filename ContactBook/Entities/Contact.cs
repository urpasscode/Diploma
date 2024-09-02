using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebApplication1.CustomAttributes;

namespace WebApplication1.Entities
{
    [Table("contact")]
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int elem_id { get; set; }
        [Required]
        public string fio { get; set; }
        [Required]
        public DateOnly cont_birth { get; set; }
        public string? address { get; set; }
        public string? cont_work { get; set; }
        public string? other { get; set; }
    }

    public class ContactAndInformation
    {
        public Contact contact { get; set; }
        public List<PhoneNumber> phoneNumbers { get; set; }
        public List<Device> devices { get; set; }
        public List<Email> emails { get; set; }
        public List<SocialNetwork> socialNetworks { get; set; }
        public List<SN_Name> socialNetworkNames { get; set; }

    }

    public class ContactModel
    {
        [Required]
        [RegularExpression(@"^[а-яА-ЯёЁa-zA-Z_ ]+ [а-яА-ЯёЁa-zA-Z_ ]+ ?[а-яА-ЯёЁa-zA-Z_ ]+$", ErrorMessage = "Некорректное ФИО для контакта. Поле обязательно" +
            "должно содержать фамилию и имя русскими или латинскими буквами")]
        [MinLength(1, ErrorMessage = "Минимальная длина ФИО - 1 символ")]
        [MaxLength(100, ErrorMessage = "Максимальная длина ФИО - 100 символов")]
        public string fio { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        [DateRange("1950-01-01", "2023-01-01", ErrorMessage = "Дата рождения " +
            "должна быть в диапазоне от {0} до {1}")]
        [DataType(DataType.Date)]
        public string cont_birth { get; set; }
        [RegularExpression(@"^[а-яА-ЯёЁa-zA-Z0-9\.\-\,\s]+$", ErrorMessage = "Некорректный адрес контакта. Адрес " +
           "может содержать буквы русского и латинского алфавитов, цифры и знаки . , - , ,")]
        [MaxLength(100, ErrorMessage = "Максимальная длина адреса - 100 символов")]
        public string? address { get; set; }
        [RegularExpression(@"^[а-яА-ЯёЁa-zA-Z0-9_ ]+$", ErrorMessage = "Некорректное место работы контакта. Место работы " +
           "может содержать буквы русского и латинского алфавитов и цифры")]
        [MaxLength(100, ErrorMessage = "Максимальная длина места работы - 100 символов")]
        public string? cont_work { get; set; }
        [RegularExpression(@"^[а-яА-ЯёЁa-zA-Z0-9\.\-\,\s ]+$", ErrorMessage = "Некорректная дополнительная информация о контакте. Дополнительная информация " +
           "может содержать буквы русского и латинского алфавитов, цифры и знаки . , - , ,")]
        [MaxLength(200, ErrorMessage = "Максимальная длина дополнительной информации - 200 символов")]
        public string? other { get; set; }
    }
}
