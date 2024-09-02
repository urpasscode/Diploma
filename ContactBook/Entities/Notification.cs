using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using WebApplication1.CustomAttributes;

namespace WebApplication1.Entities
{
    [Table("notification")]
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int notif_id { get; set; }
        [Required]
        public string notif_name { get; set; }
        [Required]
        public DateOnly notif_date { get; set; }
        [Required]
        public int elem_id { get; set; }
        public string? notif_desc { get; set; }
        [Required]
        public bool notif_type { get; set; }
        [Required]
        public int freq_id { get; set; }
        public int? ev_id { get; set; }
        [Required]
        public int user_id { get; set; }
        [Required]
        public TimeOnly notif_time { get; set; }
    }

    public class NotificationModel
    {
        [Required(ErrorMessage ="Введите название уведомления")]
        [RegularExpression(@"^[а-яА-ЯёЁa-zA-Z0-9\.\-\,\s ]+$", ErrorMessage = "Некорректное название уведомления. Название " +
           "уведомления может содержать буквы русского и латинского алфавитов, цифры и знаки . , - , ,")]
        [MaxLength(100, ErrorMessage = "Максимальная длина адреса - 100 символов")]
        public string notif_name { get; set; }
        [RegularExpression(@"^[а-яА-ЯёЁa-zA-Z0-9\.\-\,\s ]+$", ErrorMessage = "Некорректное описание уведомления. Описание " +
           "может содержать буквы русского и латинского алфавитов, цифры и знаки . , - , ,")]
        [MaxLength(100, ErrorMessage = "Максимальная длина адреса - 100 символов")]
        public string? notif_desc { get; set; }
        [Required(ErrorMessage = "Выберите тип напоминания (со звуком или без)")]
        public bool notif_type { get; set; }
        [Required(ErrorMessage = "Выберите частоту напоминания")]
        public int freq_id { get; set; }
        public int? ev_id { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        [DateRange("2023-01-01", "2026-01-01", ErrorMessage = "Дата напоминания " +
            "должна быть в диапазоне от {0} до {1}")]
        [DataType(DataType.Date)]
        public string? notif_date{ get; set; }
    }

    public class NotificationAndInformation
    {
        [Required]
        public Notification notification { get; set; }
        public Event? ev { get; set; }
        [Required]
        public Frequency frequency { get; set; }
    }
}
