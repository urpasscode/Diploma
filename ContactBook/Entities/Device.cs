using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    [Table("device")]
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int device_id { get; set; }
        [Required(ErrorMessage ="Название типа устройства - обязательное поле")]
        public string phone_type { get; set; }
    }
}
