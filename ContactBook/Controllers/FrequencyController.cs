using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;
using WebApplication1;

namespace ContactBook.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FrequencyController : Controller
    {
        private readonly FrequencyRepository _frequency;

        public FrequencyController(FrequencyRepository frequency)
        {
            _frequency = frequency;
        }

        // Получение всех частот напоминаний
        [HttpGet("GetAllFrequencies")]
        public IActionResult GetAllFrequencies()
        {
            var freq = _frequency.Frequencies.ToList();
            return new JsonResult(freq) { StatusCode = 200 };
        }

        //получение конкретной частоты напоминания
        [HttpGet("GetOneFrequency/{id}")]
        public IActionResult GetOneFrequency(int id)
        {
            var freq = _frequency.Frequencies.FirstOrDefault(e => e.freq_id == id);
            if (freq == null)
            {
                List<string> errors = new List<string> { "Переодичность для уведомлений некорректна" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            return new JsonResult(freq) { StatusCode = 200 };
        }
    }
}
