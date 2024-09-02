using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Entities;

namespace WebApplication1.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class PhoneNumberController : Controller
    {
        private readonly PhoneNumberRepository _phonenumbers;
        private readonly ContactRepository _contacts;
        private readonly ElementRepository _elements;
        public PhoneNumberController(ContactRepository contacts, ElementRepository elements, PhoneNumberRepository phonenumbers)
        {
            _contacts = contacts;
            _elements = elements;
            _phonenumbers = phonenumbers ?? throw new ArgumentNullException(nameof(phonenumbers));
        }

        //только для использования в контроллере контакта!!!
        [HttpGet("GetListPhonesForContact")]
        public List<PhoneNumber> GetListPhonesForContact([Required] int contactId)
        {
            var phones = _phonenumbers.PhoneNumbers.Where(e => e.elem_id == contactId).ToList();
            return phones;
        }

        //получение списка номеров телефонов конкретного контакта
        [HttpGet("GetListPhones")]
        public IActionResult GetListPhones([Required] int contactId)
        {
            var phones = _phonenumbers.PhoneNumbers.Where(e => e.elem_id == contactId).ToList();
            return new JsonResult(phones) { StatusCode = 200 };
        }

        //получение конкретного номера телефона
        [HttpGet("GetOnePhone/{id}")]
        public IActionResult GetOnePhone([Required] int id)
        {
            PhoneNumber phone = _phonenumbers.PhoneNumbers.FirstOrDefault(e => e.phone_id == id);
            if (phone == null)
            {
                List<string> errors = new List<string> { "Номер телефона не найден" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            return new JsonResult(phone) { StatusCode = 200 };
        }

        // Создание нового номер телефона
        [HttpPost("CreatePhoneNumber")]
        public IActionResult CreatePhoneNumber(int contactId, [FromForm] PhoneNumberCreateModel model)
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
            var contacts = _contacts.Contacts.Where(n => elemsId.Contains(n.elem_id));
            var contact = contacts.FirstOrDefault(n => n.elem_id == contactId);
            if (contact == null)
            {
                List<string> errors = new List<string> { "Контакт не найден" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var phone = new PhoneNumber();
                    phone.elem_id = contactId;
                    phone.pn = model.pn;
                    phone.device_id = model.device_id;
                    _phonenumbers.PhoneNumbers.Add(phone);
                    _phonenumbers.SaveChanges();
                    return new JsonResult(phone) { StatusCode = 200 };
                }
                else
                {
                    List<string> errors = new List<string>();
                    foreach (var item in ModelState)
                    {
                        if (item.Value.ValidationState == ModelValidationState.Invalid)
                        {
                            foreach (var error in item.Value.Errors)
                            {
                                errors.Add(error.ErrorMessage);
                            }
                        }
                    }
                    return new JsonResult(errors)
                    { StatusCode = 400 }; // BadRequest
                }
            }
        }

        // Изменение номера телефона
        [HttpPut("UpdatePhoneNumber/{id}")]
        public IActionResult UpdatePhoneNumber(int id, [FromForm] PhoneNumberCreateModel model)
        {
            if (ModelState.IsValid)
            {
                PhoneNumber phone = _phonenumbers.PhoneNumbers.FirstOrDefault(e => e.phone_id == id);
                if (phone == null)
                {
                    List<string> errors = new List<string> { "Номер телефона не найден" };
                    return new JsonResult(errors)
                    { StatusCode = 404 }; // NotFound
                }
                phone.pn = model.pn;
                phone.device_id = model.device_id;
                _phonenumbers.SaveChanges();
                return new JsonResult(phone) { StatusCode = 200 };
            }
            else
            {
                List<string> errors = new List<string>();
                foreach (var item in ModelState)
                {
                    if (item.Value.ValidationState == ModelValidationState.Invalid)
                    {
                        foreach (var error in item.Value.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }
                }
                return new JsonResult(errors)
                { StatusCode = 400 }; // BadRequest
            }
        }

        // Удаление номера телефона
        [HttpDelete("DeletePhoneNumber/{id}")]
        public IActionResult DeletePhoneNumber(int id)
        {
            PhoneNumber phone = _phonenumbers.PhoneNumbers.FirstOrDefault(e => e.phone_id == id);
            if (phone == null)
            {
                List<string> errors = new List<string> { "Номер телефона не найден" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }

            _phonenumbers.PhoneNumbers.Remove(phone);
            _phonenumbers.SaveChanges();
            return new JsonResult("Номер телефона успешно удален") { StatusCode = 200 };
        }
    }
}
