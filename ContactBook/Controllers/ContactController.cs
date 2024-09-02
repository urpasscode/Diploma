using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using WebApplication1.Entities;
using WebApplication1.JsonConverter;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContactController : Controller
    {
        private readonly ContactRepository _contacts;
        private readonly ElementRepository _elements;
        private readonly PhoneNumberRepository _phoneNumbers;
        private readonly SocialNetworkRepository _socialNetworks;
        private readonly EmailRepository _emails;

        public ContactController(ContactRepository contacts, ElementRepository elements, PhoneNumberRepository phoneNumbers, SocialNetworkRepository socialNetworks, EmailRepository emails)
        {
            _contacts = contacts;
            _elements = elements;
            _phoneNumbers = phoneNumbers;
            _socialNetworks = socialNetworks;
            _emails = emails;
        }

        //только для получения всех контактов, чтобы использовать данный код в других методах!!!
        [HttpGet("GetFullContact")]
        public IActionResult GetFullContact()
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
            var contacts = _contacts.Contacts.Where(n => elemsId.Contains(n.elem_id));
            var phones = new PhoneNumberController(_contacts, _elements, _phoneNumbers);
            var emails = new EmailController(_contacts, _elements, _emails);
            var networks = new SocialNetworkController(_contacts, _elements, _socialNetworks);
            List<ContactAndInformation> contactAndInformation = new List<ContactAndInformation>();
            foreach (var contact in contacts)
            {
                var currentPhones = phones.GetListPhonesForContact(contact.elem_id).ToList();
                var currentEmails = emails.GetListEmailsForContact(contact.elem_id).ToList();
                var currentNetworks = networks.GetListSocialNetworksForContact(contact.elem_id).ToList();
                var fullContact = new ContactAndInformation();
                fullContact.contact = contact;
                fullContact.phoneNumbers = currentPhones;
                fullContact.emails = currentEmails;
                fullContact.socialNetworks = currentNetworks;
                contactAndInformation.Add(fullContact);
            }
            return Ok(contactAndInformation);
        }

        // Получение всех контактов пользователя
        [HttpGet("GetAllContacts")]
        public IActionResult GetContacts()
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter() }
            };
            return new JsonResult(GetFullContact(), options) { StatusCode = 200 };
        }

        // Получение контакта по ID (с проверкой принадлежности пользователю)
        [HttpGet("GetContact/{id}")]
        public IActionResult GetContact(int id)
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
            var contacts = _contacts.Contacts.Where(n => elemsId.Contains(n.elem_id));
            var contact = contacts.FirstOrDefault(n => n.elem_id == id);
            var phones = new PhoneNumberController(_contacts, _elements, _phoneNumbers);
            var emails = new EmailController(_contacts, _elements, _emails);
            var networks = new SocialNetworkController(_contacts, _elements, _socialNetworks);
            if (contact == null)
            {
                List<string> errors = new List<string> { "Контакт не найден" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            var fullContact = new ContactAndInformation();
            fullContact.contact = contact;
            fullContact.phoneNumbers = phones.GetListPhonesForContact(contact.elem_id).ToList();
            fullContact.emails = emails.GetListEmailsForContact(contact.elem_id).ToList();
            fullContact.socialNetworks = networks.GetListSocialNetworksForContact(contact.elem_id).ToList();

            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter() }
            };

            return new JsonResult(fullContact, options) { StatusCode = 200 };
        }

        // Создание нового контакта
        [HttpPost("CreateContact")]
        public IActionResult CreateContact([FromForm] ContactModel contactModel)
        {
            if (ModelState.IsValid)
            {
                Element newElem = new Element();
                newElem.user_id = CurrentUser.UserId;
                newElem.elem_type = true;
                _elements.Elements.Add(newElem);
                _elements.SaveChanges();
                int newElemId = newElem.elem_id;

                var contact = new Contact();
                contact.elem_id = newElemId;
                contact.fio = contactModel.fio;
                DateOnly newBirth;
                if (DateOnly.TryParse(contactModel.cont_birth, out newBirth))
                {
                    contact.cont_birth = newBirth;
                }
                else
                {
                    List<string> errors = new List<string> { "Неверный формат ввода даты рождения" };
                    return new JsonResult(errors)
                    { StatusCode = 400 }; // BadRequest
                }
                contact.address = contactModel.address;
                contact.cont_work = contactModel.cont_work;
                contact.other = contactModel.other;
                _contacts.Contacts.Add(contact);

                var phones = new PhoneNumberController(_contacts, _elements, _phoneNumbers);
                var emails = new EmailController(_contacts, _elements, _emails);
                var networks = new SocialNetworkController(_contacts, _elements, _socialNetworks);
                _contacts.SaveChanges();

                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter() }
                };

                var fullContact = new ContactAndInformation();
                fullContact.contact = contact;
                fullContact.phoneNumbers = phones.GetListPhonesForContact(contact.elem_id).ToList();
                fullContact.emails = emails.GetListEmailsForContact(contact.elem_id).ToList();
                fullContact.socialNetworks = networks.GetListSocialNetworksForContact(contact.elem_id).ToList();

                return new JsonResult(fullContact, options) { StatusCode = 200 };
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

        // Изменение контакта (с проверкой принадлежности пользователю)
        [HttpPut("UpdateContact/{id}")]
        public IActionResult UpdateContact(int id, [FromForm] ContactModel contactModel)
        {
            if (ModelState.IsValid)
            {
                var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
                var contacts = _contacts.Contacts.Where(n => elemsId.Contains(n.elem_id));
                var contact = contacts.FirstOrDefault(n => n.elem_id == id);
                var phones = new PhoneNumberController(_contacts, _elements, _phoneNumbers);
                var emails = new EmailController(_contacts, _elements, _emails);
                var networks = new SocialNetworkController(_contacts, _elements, _socialNetworks);
                if (contact == null)
                {
                    List<string> errors = new List<string> { "Контакт не найден" };
                    return new JsonResult(errors)
                    { StatusCode = 404 }; // NotFound
                }

                contact.fio = contactModel.fio;
                DateOnly newBirth;
                if (DateOnly.TryParse(contactModel.cont_birth, out newBirth))
                {
                    contact.cont_birth = newBirth;
                }
                else
                {
                    List<string> errors = new List<string> { "Неверный формат ввода даты рождения" };
                    return new JsonResult(errors)
                    { StatusCode = 400 }; // BadRequest
                }
                contact.address = contactModel.address;
                contact.cont_work = contactModel.cont_work;
                contact.other = contactModel.other;
                _contacts.SaveChanges();

                var fullContact = new ContactAndInformation();
                fullContact.contact = contact;
                fullContact.phoneNumbers = phones.GetListPhonesForContact(contact.elem_id).ToList();
                fullContact.emails = emails.GetListEmailsForContact(contact.elem_id).ToList();
                fullContact.socialNetworks = networks.GetListSocialNetworksForContact(contact.elem_id).ToList();

                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter() }
                };
                return new JsonResult(fullContact, options) { StatusCode = 200 };
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

        // Удаление контакта (с проверкой принадлежности пользователю)
        [HttpDelete("DeleteContact/{id}")]
        public IActionResult DeleteContact(int id)
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
            var contacts = _contacts.Contacts.Where(n => elemsId.Contains(n.elem_id));
            var contact = contacts.FirstOrDefault(n => n.elem_id == id);
            if (contact == null)
            {
                List<string> errors = new List<string> { "Контакт не найден" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            _contacts.Contacts.Remove(contact);
            _contacts.SaveChanges();
            return new JsonResult("Контакт успешно удален") { StatusCode = 200 };
        }

        //поиск контактов по фио
        [HttpPost("FindContactByFIO")]
        public IActionResult FindContactsByFIO([Required] string request)
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
            var contacts = _contacts.Contacts.Where(n => elemsId.Contains(n.elem_id)).Where(c => c.fio.Contains(request)).ToList();
            var phones = new PhoneNumberController(_contacts, _elements, _phoneNumbers);
            var emails = new EmailController(_contacts, _elements, _emails);
            var networks = new SocialNetworkController(_contacts, _elements, _socialNetworks);
            List<ContactAndInformation> contactAndInformation = new List<ContactAndInformation>();
            foreach (var contact in contacts)
            {
                var fullContact = new ContactAndInformation();
                fullContact.contact = contact;
                fullContact.phoneNumbers = phones.GetListPhonesForContact(contact.elem_id).ToList();
                fullContact.emails = emails.GetListEmailsForContact(contact.elem_id).ToList();
                fullContact.socialNetworks = networks.GetListSocialNetworksForContact(contact.elem_id).ToList();
                contactAndInformation.Add(fullContact);
            }

            if (contactAndInformation.Count > 0)
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter() }
                };
                return new JsonResult(contactAndInformation, options) { StatusCode = 200 };
            }
            else
            {
                return new JsonResult("Контакты не найдены") { StatusCode = 200 };
            }
        }

        //сортировка контактов по ФИО
        [HttpGet("SortContactsByFIO")]
        public IActionResult SortContactsByFIO()
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
            var contacts = _contacts.Contacts.Where(n => elemsId.Contains(n.elem_id)).OrderBy(c => c.fio);
            var phones = new PhoneNumberController(_contacts, _elements, _phoneNumbers);
            var emails = new EmailController(_contacts, _elements, _emails);
            var networks = new SocialNetworkController(_contacts, _elements, _socialNetworks);
            List<ContactAndInformation> contactAndInformation = new List<ContactAndInformation>();
            foreach (var contact in contacts)
            {
                var fullContact = new ContactAndInformation();
                fullContact.contact = contact;
                fullContact.phoneNumbers = phones.GetListPhonesForContact(contact.elem_id).ToList();
                fullContact.emails = emails.GetListEmailsForContact(contact.elem_id).ToList();
                fullContact.socialNetworks = networks.GetListSocialNetworksForContact(contact.elem_id).ToList();
                contactAndInformation.Add(fullContact);
            }
            if (contactAndInformation.Count > 0)
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter() }
                };
                return new JsonResult(contactAndInformation, options) { StatusCode = 200 };
            }
            else
            {
                return new JsonResult("Контакты не найдены") { StatusCode = 200 };
            }
        }

        //сортировка контактов по дате рождения
        [HttpGet("SortContactsByDate")]
        public IActionResult SortContactsByDate()
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
            var contacts = _contacts.Contacts.Where(n => elemsId.Contains(n.elem_id)).OrderBy(c => c.cont_birth);
            var phones = new PhoneNumberController(_contacts, _elements, _phoneNumbers);
            var emails = new EmailController(_contacts, _elements, _emails);
            var networks = new SocialNetworkController(_contacts, _elements, _socialNetworks);
            List<ContactAndInformation> contactAndInformation = new List<ContactAndInformation>();
            foreach (var contact in contacts)
            {
                var fullContact = new ContactAndInformation();
                fullContact.contact = contact;
                fullContact.phoneNumbers = phones.GetListPhonesForContact(contact.elem_id).ToList();
                fullContact.emails = emails.GetListEmailsForContact(contact.elem_id).ToList();
                fullContact.socialNetworks = networks.GetListSocialNetworksForContact(contact.elem_id).ToList();
                contactAndInformation.Add(fullContact);
            }
            if (contactAndInformation.Count > 0)
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter() }
                };
                return new JsonResult(contactAndInformation, options) { StatusCode = 200 };
            }
            else
            {
                return new JsonResult("Контакты не найдены") { StatusCode = 200 };
            }
        }

        //фильтрация контактов по месяцу рождения
        //месяц считывается числом
        [HttpPost("FilterByBirth")]
        public IActionResult FilterByBirth([FromForm][Required] int request)
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
            var contacts = _contacts.Contacts.Where(n => elemsId.Contains(n.elem_id)).Where(c => c.cont_birth.Month.Equals(request));
            var phones = new PhoneNumberController(_contacts, _elements, _phoneNumbers);
            var emails = new EmailController(_contacts, _elements, _emails);
            var networks = new SocialNetworkController(_contacts, _elements, _socialNetworks);
            List<ContactAndInformation> contactAndInformation = new List<ContactAndInformation>();
            foreach (var contact in contacts)
            {
                var fullContact = new ContactAndInformation();
                fullContact.contact = contact;
                fullContact.phoneNumbers = phones.GetListPhonesForContact(contact.elem_id).ToList();
                fullContact.emails = emails.GetListEmailsForContact(contact.elem_id).ToList();
                fullContact.socialNetworks = networks.GetListSocialNetworksForContact(contact.elem_id).ToList();
                contactAndInformation.Add(fullContact);
            }
            if (contactAndInformation.Count > 0)
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter() }
                };
                return new JsonResult(contactAndInformation, options) { StatusCode = 200 };
            }
            else
            {
                return new JsonResult("Контакты не найдены") { StatusCode = 200 };
            }
        }
    }
}
