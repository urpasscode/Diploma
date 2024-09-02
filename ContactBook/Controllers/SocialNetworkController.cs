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
    public class SocialNetworkController : Controller
    {
        private readonly SocialNetworkRepository _socialnetworks;
        private readonly ContactRepository _contacts;
        private readonly ElementRepository _elements;
        public SocialNetworkController(ContactRepository contacts, ElementRepository elements, SocialNetworkRepository socialnetworks)
        {
            _contacts = contacts;
            _elements = elements;
            _socialnetworks = socialnetworks ?? throw new ArgumentNullException(nameof(socialnetworks));
        }

        //только для использования в контроллере контакта!!!
        [HttpGet("GetListSocialNetworksForContact")]
        public List<SocialNetwork> GetListSocialNetworksForContact([Required] int contactId)
        {
            List<SocialNetwork> sn = _socialnetworks.SocialNetworks.Where(e => e.elem_id == contactId).ToList();
            return sn;
        }

        //получение всех ссылок на соц. сети конкретного контакта
        [HttpGet("GetListSocialNetworks")]
        public IActionResult GetListSocialNetworks([Required] int contactId)
        {
            var sn = _socialnetworks.SocialNetworks.Where(e => e.elem_id == contactId).ToList();
            return new JsonResult(sn) { StatusCode = 200 };
        }

        //получение одной ссылки на соц. сеть
        [HttpGet("GetOneSocialNetwork/{id}")]
        public IActionResult GetOneSocialNetwork([Required] int id)
        {
            SocialNetwork sn = _socialnetworks.SocialNetworks.FirstOrDefault(e => e.sn_id == id);
            if (sn == null)
            {
                List<string> errors = new List<string> { "Ссылка на социальную сеть не найдена" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            return new JsonResult(sn) { StatusCode = 200 };
        }

        // Создание новой ссылки на соц сеть
        [HttpPost("CreateSocialNetwork")]
        public IActionResult CreateSocialNetwork(int contactId, [FromForm] SocialNetworkModel model)
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
                    var sn = new SocialNetwork();
                    sn.elem_id = contactId;
                    sn.sn_link = model.sn_link;
                    sn.snn_id = model.snn_id;
                    _socialnetworks.SocialNetworks.Add(sn);

                    _socialnetworks.SaveChanges();
                    return new JsonResult(sn) { StatusCode = 200 };
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

        // Изменение соц сети (с проверкой принадлежности пользователю)
        [HttpPut("UpdateSocialNetwork/{id}")]
        public IActionResult UpdateSocialNetwork(int id, [FromForm] SocialNetworkModel model)
        {
            if (ModelState.IsValid)
            {
                SocialNetwork sn = _socialnetworks.SocialNetworks.FirstOrDefault(e => e.sn_id == id);
                if (sn == null)
                {
                    List<string> errors = new List<string> { "Ссылка на социальную сеть не найдена" };
                    return new JsonResult(errors)
                    { StatusCode = 404 }; // NotFound
                }
                sn.sn_link = model.sn_link;
                sn.snn_id = model.snn_id;
                _socialnetworks.SaveChanges();
                return new JsonResult(sn) { StatusCode = 200 };
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

        // Удаление соц сети (с проверкой принадлежности пользователю)
        [HttpDelete("DeleteSocialNetwork/{id}")]
        public IActionResult DeleteSocialNetwork(int id)
        {
            SocialNetwork sn = _socialnetworks.SocialNetworks.FirstOrDefault(e => e.sn_id == id);
            if (sn == null)
            {
                List<string> errors = new List<string> { "Ссылка на социальную сеть не найдена" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }

            _socialnetworks.SocialNetworks.Remove(sn);
            _socialnetworks.SaveChanges();
            return new JsonResult("Ссылка на социальную сеть успешно удалена") { StatusCode = 200 };
        }
    }
}
