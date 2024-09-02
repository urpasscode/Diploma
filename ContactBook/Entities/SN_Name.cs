
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    [Table("sn_name")]
    public class SN_Name
    {
        [Key]
        public int snn_id { get; set; }
        [Required(ErrorMessage = "Название социальной сети - обязательное поле")]
        public string sn_name { get; set; }
    }
}
