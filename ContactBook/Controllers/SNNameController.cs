using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WebApplication1.Entities;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SNNameController : Controller
    {
        private readonly SNNameRepository _snnames;

        public SNNameController(SNNameRepository snnames)
        {
            _snnames = snnames;
        }

        // Получение всех названий соц сетей
        [HttpGet("GetSNNames")]
        public IActionResult GetSNNames()
        {
            var snnames = _snnames.SN_Names.ToList();
            return new JsonResult(snnames) { StatusCode = 200 };
        }

        //получение конкретного названия соц. сети
        [HttpGet("GetOneSNNames/{id}")]
        public IActionResult GetOneSNNames([Required] int id)
        {
            SN_Name snname = _snnames.SN_Names.FirstOrDefault(e => e.snn_id == id);
            if (snname == null)
            {
                List<string> errors = new List<string> { "Название социальной сети не найдено" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            return new JsonResult(snname) { StatusCode = 200 };
        }
    }
}
