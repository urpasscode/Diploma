
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    [Table("socialnetwork")]
    public class SocialNetwork
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sn_id { get; set; }
        [Required]
        public string sn_link { get; set; }
        [Required(ErrorMessage = "Идентификатор контакта для ссылки на соц. сеть - обязательное поле")]
        public int elem_id { get; set; }
        public int? snn_id { get; set; }
    }

    public class SocialNetworkModel
    {
        [Required(ErrorMessage = "Ссылка на социальную сеть - обязательное поле")]
        [MaxLength(150, ErrorMessage = "Максимальная длина ссылки на социальную сеть - 100 символов")]
        public string sn_link { get; set; }
        public int? snn_id { get; set; }
    }
}
