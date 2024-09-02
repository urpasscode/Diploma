using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.CustomAttributes;
namespace WebApplication1.Entities
{
    [Table("fulluser")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        public string? user_name { get; set; }
        public string user_email { get; set; }
        public string user_password { get; set; }
        public string? user_pn { get; set; }
        
        public DateOnly? user_birth { get; set; }
    }

    public class UserLoginModel
    {
        [Required(ErrorMessage = "Введите адрес электронной почты!")]
        [RegularExpression(@"^[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+[.][A-Za-z]+$", ErrorMessage = "Некорректный адрес электронной почты")]
        [MaxLength(35, ErrorMessage = "Максимальная длина электронной почты - 35 символов")]
        public string user_email { get; set; }
        [Required(ErrorMessage = "Введите пароль!")]
        [MaxLength(35, ErrorMessage = "Максимальная длина пароля - 35 символов")]
        [MinLength(6, ErrorMessage = "Минимальная длина пароля - 6 символов")]
        public string user_password { get; set; }
    }

    public class UserModel
    {
        [RegularExpression(@"^[а-яА-Я_ ]+$", ErrorMessage = "Некорректное имя пользователя. Имя " +
            "пользователя может содержать только буквы русского алфавита")]
        [MaxLength(100, ErrorMessage = "Максимальная длина имени пользователя - 100 символов")]
        public string? user_name { get; set; }
        [Required(ErrorMessage = "Введите адрес электронной почты!")]
        [RegularExpression(@"^[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+[.][A-Za-z]+$", ErrorMessage = "Некорректный адрес электронной почты.")]
        [MaxLength(35, ErrorMessage = "Максимальная длина электронной почты - 35 символов")]
        public string user_email { get; set; }
        [Required(ErrorMessage = "Введите пароль!")]
        [MaxLength(35, ErrorMessage = "Максимальная длина пароля - 35 символов")]
        [MinLength(6, ErrorMessage = "Минимальная длина пароля - 6 символов")]
        public string user_password { get; set; }
        [RegularExpression(@"^((\+7|7|8)+([0-9]){10})$", ErrorMessage = "Некорректный номер телефона.")]
        [MaxLength(12, ErrorMessage = "Максимальная длина номера телефона - 12 символов")]
        public string? user_pn { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        [DateRange("1950-01-01", "2023-01-01", ErrorMessage = "Дата рождения " +
            "должна быть в диапазоне от {0} до {1}")]
        [DataType(DataType.Date)]
        public string? user_birth {  get; set; }
    }
}
