using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    [Table("event")]
    public class Event
    {
        [Key]
        public int event_id { get; set; }
        [Required(ErrorMessage ="Категория мероприятия - обязательное поле")]
        public string event_name { get; set; }
    }
}
