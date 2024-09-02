using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;

namespace WebApplication1.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class DeviceController : Controller
    {
        private readonly DeviceRepository _devices;

        public DeviceController(DeviceRepository devices)
        {
            _devices = devices;
        }

        // Получение всех типов телефонов
        [HttpGet("GetAllDevices")]
        public IActionResult GetDevices()
        {
            var devices = _devices.Devices.ToList();
            return new JsonResult(devices) { StatusCode = 200 };
        }

        //получение конкретного типа телефона
        [HttpGet("GetOneDevice/{id}")]
        public IActionResult GetOneDevice(int id)
        {
            Device device = _devices.Devices.FirstOrDefault(e => e.device_id == id);
            if (device == null)
            {
                List<string> errors = new List<string> { "Тип номера телефона не найден" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            return new JsonResult(device) { StatusCode = 200 };
        }
    }
}
