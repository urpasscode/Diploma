using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WebApplication1.Entities;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly EmailRepository _emails;
        private readonly ContactRepository _contacts;
        private readonly ElementRepository _elements;
        public EmailController(ContactRepository contacts, ElementRepository elements, EmailRepository emails)
        {
            _contacts = contacts;
            _elements = elements;
            _emails = emails ?? throw new ArgumentNullException(nameof(emails));
        }

        //только для использования в контроллере контакта
        [HttpGet("GetListEmailsForContact")]
        public List<Email> GetListEmailsForContact([Required] int contactId)
        {
            var emails = _emails.Emails.Where(e => e.elem_id == contactId).ToList();
            return emails;
        }

        //получить все электронные почты конкретного контакта
        [HttpGet("GetListEmails")]
        public IActionResult GetListEmails([Required] int contactId)
        {
            var emails = _emails.Emails.Where(e => e.elem_id == contactId).ToList();
            return new JsonResult(emails) { StatusCode = 200 };
        }

        //получить конкретную электронную почту
        [HttpGet("GetOneEmail/{id}")]
        public IActionResult GetOneEmail([Required] int id)
        {
            Email email = _emails.Emails.FirstOrDefault(e => e.email_id == id);
            if (email == null)
            {
                List<string> errors = new List<string> { "Адрес электронной почты не найден" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            return new JsonResult(email) { StatusCode = 200 };
        }

        // Создание новой почты
        [HttpPost("CreateEmail")]
        public IActionResult CreateEmail(int contactId, [FromForm] EmailModel model)
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
                    var email = new Email();
                    email.elem_id = contactId;
                    email.email_address = model.email_address;
                    _emails.Emails.Add(email);
                    _emails.SaveChanges();
                    return new JsonResult(email) { StatusCode = 200 };
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

        // Изменение почты 
        [HttpPut("UpdateEmail/{id}")]
        public IActionResult UpdateEmail(int id, [FromForm] EmailModel model)
        {
            if (ModelState.IsValid)
            {
                Email email = _emails.Emails.FirstOrDefault(e => e.email_id == id);
                if (email == null)
                {
                    List<string> errors = new List<string> { "Адрес электронной почты не найден" };
                    return new JsonResult(errors)
                    { StatusCode = 404 }; // NotFound
                }
                email.email_address = model.email_address;
                _emails.SaveChanges();
                return new JsonResult(email) { StatusCode = 200 };
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

        // Удаление электронной почты
        [HttpDelete("DeleteEmail/{id}")]
        public IActionResult DeleteEmail(int id)
        {
            Email email = _emails.Emails.FirstOrDefault(e => e.email_id == id);
            if (email == null)
            {
                List<string> errors = new List<string> { "Адрес электронной почты не найден" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            _emails.Emails.Remove(email);
            _emails.SaveChanges();
            return new JsonResult("Электронная почта успешно удалена") { StatusCode = 200 };
        }
    }
}
