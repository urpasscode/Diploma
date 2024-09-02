using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventController : Controller
    {
        private readonly EventRepository _events;

        public EventController(EventRepository events)
        {
            _events = events;
        }

        // Получение всех событий
        [HttpGet("GetEvents")]
        public IActionResult GetEvents()
        {
            var events = _events.Events.ToList();
            return Ok(events);
        }

        // Получение конкретного события
        [HttpGet("GetOneEvent/{id}")]
        public IActionResult GetOneEvent(int id)
        {
            var ev = _events.Events.FirstOrDefault(e => e.event_id == id);
            if (ev == null)
            {
                List<string> errors = new List<string> { "Тип события для уведомления не найден" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            return new JsonResult(ev) { StatusCode = 200 };
        }
    }
}
