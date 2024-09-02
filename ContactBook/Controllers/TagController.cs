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
    public class TagController : Controller
    {
        private readonly NoteRepository _notes;
        private readonly ContactRepository _contacts;
        private readonly ElementRepository _elements;
        private readonly TagRepository _tags;
        private readonly ElementTagRepository _elementtag;

        public TagController(NoteRepository notes, ElementRepository elements, TagRepository tags, ContactRepository contact, ElementTagRepository elementTag)
        {
            _notes = notes;
            _elements = elements;
            _tags = tags;
            _contacts = contact;
            _elementtag = elementTag;
        }

        // Получение всех тегов с заметками 
        [HttpGet("GetTagsWithNotes")]
        public IActionResult GetTagsWithNotes()
        {
            var tags = _tags.Tags.Where(e => e.user_id == CurrentUser.UserId && e.tag_type == false);
            List<TagAndElements<Note>> allTags = new List<TagAndElements<Note>>();
            foreach (var tag in tags)
            {
                var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == false).Select(e => e.elem_id).ToList();
                var idConnectedNotes = _elementtag.Elements_Tags.Where(e => elemsId.Contains(e.elem_id) && e.tag_id == tag.tag_id).Select(e => e.elem_id).ToList();
                var notes = _notes.Notes.Where(n => idConnectedNotes.Contains(n.elem_id)).ToList();
                TagAndElements<Note> newTag = new TagAndElements<Note>();
                newTag.tag = tag;
                newTag.elements = notes;
                allTags.Add(newTag);
            }
            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
            };
            return new JsonResult(allTags,options) { StatusCode = 200 };
        }

        // Получение всех тегов с контактами
        [HttpGet("GetTagsWithContacts")]
        public IActionResult GetTagsWithContacts()
        {
            var tags = _tags.Tags.Where(e => e.user_id == CurrentUser.UserId && e.tag_type == true);
            List<TagAndElements<Contact>> allTags = new List<TagAndElements<Contact>>();
            foreach (var tag in tags)
            {
                var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
                var idConnectedContacts = _elementtag.Elements_Tags.Where(e => elemsId.Contains(e.elem_id) && e.tag_id == tag.tag_id).Select(e => e.elem_id).ToList();
                var contacts = _contacts.Contacts.Where(n => idConnectedContacts.Contains(n.elem_id)).ToList();
                TagAndElements<Contact> newTag = new TagAndElements<Contact>();
                newTag.tag = tag;
                newTag.elements = contacts;
                allTags.Add(newTag);
            }
            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
            };
            return new JsonResult(allTags,options) { StatusCode = 200 };
        }

        // Получение тега с заметками по ID (с проверкой принадлежности пользователю)
        [HttpGet("GetOneTagForNote/{id}")]
        public IActionResult GetOneTagForNote(int id)
        {
            var tag = _tags.Tags.Where(e => e.user_id == CurrentUser.UserId && e.tag_type == false).FirstOrDefault(e => e.tag_id == id);
            if (tag == null)
            {
                List<string> errors = new List<string> { "Тег не найден" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            //var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == false).Select(e => e.elem_id).ToList();
            var connectionWithTag = _elementtag.Elements_Tags.Where(e => e.tag_id == tag.tag_id).Select(c => c.elem_id).ToList();
            var notes = _notes.Notes.Where(n => connectionWithTag.Contains(n.elem_id)).ToList();
            TagAndElements<Note> newTag = new TagAndElements<Note>();
            newTag.tag = tag;
            newTag.elements = notes;
            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
            };
            return new JsonResult(newTag,options) { StatusCode = 200 };
        }

        // Получение тега с контактами по ID (с проверкой принадлежности пользователю)
        [HttpGet("GetOneTagForContact/{id}")]
        public IActionResult GetOneTagForContact(int id)
        {
            var tag = _tags.Tags.Where(e => e.user_id == CurrentUser.UserId && e.tag_type == true).FirstOrDefault(e => e.tag_id == id);
            if (tag == null)
            {
                List<string> errors = new List<string> { "Тег не найден" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            //var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
            var connectionWithTag = _elementtag.Elements_Tags.Where(e => e.tag_id == tag.tag_id).Select(c => c.elem_id).ToList();
            var contacts = _contacts.Contacts.Where(n => connectionWithTag.Contains(n.elem_id)).ToList();
            TagAndElements<Contact> newTag = new TagAndElements<Contact>();
            newTag.tag = tag;
            newTag.elements = contacts;
            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
            };
            return new JsonResult(newTag,options) { StatusCode = 200 };
        }

        // Создание нового тега с заметками
        [HttpPost("CreateTagForNotes")]
        public IActionResult CreateTagForNotes([FromForm] TagModel newTagForNotes)
        {
            if (ModelState.IsValid)
            {
                Tag tag = new Tag();
                tag.tag_name = newTagForNotes.tag_name;
                tag.color = newTagForNotes.color;
                tag.user_id = CurrentUser.UserId;
                tag.tag_type = false;
                _tags.Tags.Add(tag);
                _tags.SaveChanges();
                int tagId = tag.tag_id;
                foreach (var elem in newTagForNotes.indexes)
                {
                    Element_Tag newConnection = new Element_Tag() { elem_id = elem, tag_id = tagId };
                    _elementtag.Add(newConnection);
                    _elementtag.SaveChanges();
                }
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
                };
                return new JsonResult(tag, options) { StatusCode = 200 };
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

        // Создание нового тега с контактами
        [HttpPost("CreateTagForContacts")]
        public IActionResult CreateTagForContacts([FromForm] TagModel newTagForContacts)
        {
            if (ModelState.IsValid)
            {
                Tag tag = new Tag();
                tag.tag_name = newTagForContacts.tag_name;
                tag.color = newTagForContacts.color;
                tag.user_id = CurrentUser.UserId;
                tag.tag_type = true;
                _tags.Tags.Add(tag);
                _tags.SaveChanges();
                int tagId = tag.tag_id;
                foreach (var elem in newTagForContacts.indexes)
                {
                    Element_Tag newConnection = new Element_Tag() { elem_id = elem, tag_id = tagId };
                    _elementtag.Add(newConnection);
                    _elementtag.SaveChanges();
                }
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
                };
                return new JsonResult(tag, options) { StatusCode = 200 };
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

        // Изменение тега для заметок (с проверкой принадлежности пользователю)
        [HttpPut("UpdateTagForNotes/{id}")]
        public IActionResult UpdateTagForNotes(int id, [FromForm] TagModel tagToUpdate)
        {
            if (ModelState.IsValid)
            {
                var tag = _tags.Tags.FirstOrDefault(t => t.tag_id == id);
                if (tag == null)
                {
                    List<string> errors = new List<string> { "Тег не найден" };
                    return new JsonResult(errors)
                    { StatusCode = 404 }; // NotFound
                }
                tag.tag_name = tagToUpdate.tag_name;
                tag.color = tagToUpdate.color;

                //здесь получаем старый список заметок, которые были в данном теге
                var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == false).Select(e => e.elem_id).ToList();
                var idConnectedNotes = _elementtag.Elements_Tags.Where(e => elemsId.Contains(e.elem_id) && e.tag_id == tag.tag_id).Select(e => e.elem_id).ToList();

                //далее нужно сравнить этот список с тем, который мы получили при изменении
                //сначала проверка, есть ли вообще что-то в списке
                if (tagToUpdate.indexes != null)
                {
                    foreach (var newListNotes in tagToUpdate.indexes)
                    {
                        //если в старом списке не было заметки из нового списка, то добавляем связь
                        //если была, то связь уже есть, удаляем ее из старого списка
                        if (!idConnectedNotes.Contains(newListNotes))
                        {
                            Element_Tag newConnection = new Element_Tag() { elem_id = newListNotes, tag_id = id };
                            _elementtag.Add(newConnection);
                            _elementtag.SaveChanges();
                        }
                        else
                        {
                            idConnectedNotes.Remove(newListNotes);
                        }
                    }
                    //далее удалем старые заметки, которые теперь не входят в обновленный тег
                    if (idConnectedNotes.Count > 0)
                    {
                        foreach (var oldNotes in idConnectedNotes)
                        {
                            Element_Tag connectionToDelete = new Element_Tag() { elem_id = oldNotes, tag_id = tag.tag_id };
                            _elementtag.Remove(connectionToDelete);
                            _elementtag.SaveChanges();
                        }
                    }
                }
                else
                {
                    List<string> errors = new List<string> { "Тег должен содержать хотя бы 1 заметку" };
                    return new JsonResult(errors)
                    { StatusCode = 400 }; // BadRequest
                }
                //отображаем созданный тег
                var connectionWithTag = _elementtag.Elements_Tags.Where(e => e.tag_id == tag.tag_id).Select(c => c.elem_id).ToList();
                var notes = _notes.Notes.Where(n => connectionWithTag.Contains(n.elem_id)).ToList();
                TagAndElements<Note> updatedTag = new TagAndElements<Note>() { tag = tag, elements = notes };

                _tags.SaveChanges();
                _elementtag.SaveChanges();
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
                };
                return new JsonResult(updatedTag,options) { StatusCode = 200 };
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

        // Изменение тега для контактов (с проверкой принадлежности пользователю)
        [HttpPut("UpdateTagForContacts/{id}")]
        public IActionResult UpdateTagForContacts(int id, [FromForm] TagModel tagToUpdate)
        {
            if (ModelState.IsValid)
            {
                var tag = _tags.Tags.FirstOrDefault(t => t.tag_id == id);
                if (tag == null)
                {
                    List<string> errors = new List<string> { "Тег не найден" };
                    return new JsonResult(errors)
                    { StatusCode = 404 }; // NotFound
                }
                tag.tag_name = tagToUpdate.tag_name;
                tag.color = tagToUpdate.color;

                //здесь получаем старый список заметок, которые были в данном теге
                var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
                var idConnectedContacts = _elementtag.Elements_Tags.Where(e => elemsId.Contains(e.elem_id) && e.tag_id == tag.tag_id).Select(e => e.elem_id).ToList();

                //далее нужно сравнить этот список с тем, который мы получили при изменении
                //сначала проверка, есть ли вообще что-то в списке
                if (tagToUpdate.indexes != null)
                {
                    foreach (var newListContacts in tagToUpdate.indexes)
                    {
                        //если в старом списке не было заметки из нового списка, то добавляем связь
                        //если была, то связь уже есть, удаляем ее из старого списка
                        if (!idConnectedContacts.Contains(newListContacts))
                        {
                            Element_Tag newConnection = new Element_Tag() { elem_id = newListContacts, tag_id = id };
                            _elementtag.Add(newConnection);
                            _elementtag.SaveChanges();
                        }
                        else
                        {
                            idConnectedContacts.Remove(newListContacts);
                        }
                    }
                    //далее удалем старые заметки, которые теперь не входят в обновленный тег
                    if (idConnectedContacts.Count > 0)
                    {
                        foreach (var oldContacts in idConnectedContacts)
                        {
                            Element_Tag connectionToDelete = new Element_Tag() { elem_id = oldContacts, tag_id = tag.tag_id };
                            _elementtag.Remove(connectionToDelete);
                            _elementtag.SaveChanges();
                        }
                    }
                }
                else
                {
                    List<string> errors = new List<string> { "Тег должен содержать хотя бы 1 контакт" };
                    return new JsonResult(errors)
                    { StatusCode = 400 }; // BadRequest
                }
                //var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
                var connectionWithTag = _elementtag.Elements_Tags.Where(e => e.tag_id == tag.tag_id).Select(c => c.elem_id).ToList();
                var contacts = _contacts.Contacts.Where(n => connectionWithTag.Contains(n.elem_id)).ToList();
                TagAndElements<Contact> updatedTag = new TagAndElements<Contact>() { tag = tag, elements = contacts };

                _tags.SaveChanges();
                _elementtag.SaveChanges();
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
                };
                return new JsonResult(updatedTag,options) { StatusCode = 200 };
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

        // Удаление тега для заметок (с проверкой принадлежности пользователю)
        [HttpDelete("DeleteTagForNotes/{id}")]
        public IActionResult DeleteTagForNotes(int id)
        {
            var tag = _tags.Tags.Where(e => e.user_id == CurrentUser.UserId && e.tag_type == false).FirstOrDefault(e => e.tag_id == id);
            if (tag == null)
            {
                List<string> errors = new List<string> { "Тег не найден" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            _tags.Tags.Remove(tag);
            _tags.SaveChanges();
            return new JsonResult("Тег успешно удален") { StatusCode = 200 };
        }

        // Удаление тега для контактов (с проверкой принадлежности пользователю)
        [HttpDelete("DeleteTagForContacts/{id}")]
        public IActionResult DeleteTagForContacts(int id)
        {
            var tag = _tags.Tags.Where(e => e.user_id == CurrentUser.UserId && e.tag_type == true).FirstOrDefault(e => e.tag_id == id);
            if (tag == null)
            {
                List<string> errors = new List<string> { "Тег не найден" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            _tags.Tags.Remove(tag);
            _tags.SaveChanges();
            return new JsonResult("Тег успешно удален") { StatusCode = 200 };
        }

        //поиск тегов для заметок по названию
        [HttpPost("FindTagForNotesByName")]
        public IActionResult FindTagForNotesByName([Required] string request)
        {
            var tags = _tags.Tags.Where(e => e.user_id == CurrentUser.UserId && e.tag_type == false && e.tag_name.Contains(request));
            List<TagAndElements<Note>> allTags = new List<TagAndElements<Note>>();
            foreach (var tag in tags)
            {
                var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == false).Select(e => e.elem_id).ToList();
                var idConnectedNotes = _elementtag.Elements_Tags.Where(e => elemsId.Contains(e.elem_id) && e.tag_id == tag.tag_id).Select(e => e.elem_id).ToList();
                var notes = _notes.Notes.Where(n => idConnectedNotes.Contains(n.elem_id)).ToList();
                TagAndElements<Note> newTag = new TagAndElements<Note>();
                newTag.tag = tag;
                newTag.elements = notes;
                allTags.Add(newTag);
            }
            if (allTags.Count > 0)
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
                };
                return new JsonResult(allTags, options) { StatusCode = 200 };
            }
            else
            {
                return new JsonResult("Теги не найдены") { StatusCode = 200 };
            }
        }

        //поиск тегов для контактов по названию
        [HttpPost("FindTagForContactsByName")]
        public IActionResult FindTagForContactsByName([Required] string request)
        {
            var tags = _tags.Tags.Where(e => e.user_id == CurrentUser.UserId && e.tag_type == true && e.tag_name.Contains(request));
            List<TagAndElements<Contact>> allTags = new List<TagAndElements<Contact>>();
            foreach (var tag in tags)
            {
                var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
                var idConnectedContacts = _elementtag.Elements_Tags.Where(e => elemsId.Contains(e.elem_id) && e.tag_id == tag.tag_id).Select(e => e.elem_id).ToList();
                var contacts = _contacts.Contacts.Where(n => idConnectedContacts.Contains(n.elem_id)).ToList();
                TagAndElements<Contact> newTag = new TagAndElements<Contact>();
                newTag.tag = tag;
                newTag.elements = contacts;
                allTags.Add(newTag);
            }
            if (allTags.Count > 0)
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
                };
                return new JsonResult(allTags, options) { StatusCode = 200 };
            }
            else
            {
                return new JsonResult("Теги не найдены") { StatusCode = 200 };
            }
        }

        //сортировка тегов для заметок по названию
        [HttpGet("SortTagsForNotesByName")]
        public IActionResult SortTagsForNotesByName()
        {
            var tags = _tags.Tags.Where(e => e.user_id == CurrentUser.UserId && e.tag_type == false);
            tags = tags.OrderBy(t => t.tag_name);
            List<TagAndElements<Note>> allTags = new List<TagAndElements<Note>>();
            foreach (var tag in tags)
            {
                var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == false).Select(e => e.elem_id).ToList();
                var idConnectedNotes = _elementtag.Elements_Tags.Where(e => elemsId.Contains(e.elem_id) && e.tag_id == tag.tag_id).Select(e => e.elem_id).ToList();
                var notes = _notes.Notes.Where(n => idConnectedNotes.Contains(n.elem_id)).ToList();
                TagAndElements<Note> newTag = new TagAndElements<Note>();
                newTag.tag = tag;
                newTag.elements = notes;
                allTags.Add(newTag);
            }
            if (allTags.Count > 0)
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
                };
                return new JsonResult(allTags, options) { StatusCode = 200 };
            }
            else
            {
                return new JsonResult("Теги не найдены") { StatusCode = 200 };
            }
        }

        //сортировка тегов для контактов по названию
        [HttpGet("SortTagsForContactsByName")]
        public IActionResult SortTagsForContactsByName()
        {
            var tags = _tags.Tags.Where(e => e.user_id == CurrentUser.UserId && e.tag_type == true);
            tags = tags.OrderBy(t => t.tag_name);
            List<TagAndElements<Contact>> allTags = new List<TagAndElements<Contact>>();
            foreach (var tag in tags)
            {
                var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == false).Select(e => e.elem_id).ToList();
                var idConnectedContacts = _elementtag.Elements_Tags.Where(e => elemsId.Contains(e.elem_id) && e.tag_id == tag.tag_id).Select(e => e.elem_id).ToList();
                var contacts = _contacts.Contacts.Where(n => idConnectedContacts.Contains(n.elem_id)).ToList();
                TagAndElements<Contact> newTag = new TagAndElements<Contact>();
                newTag.tag = tag;
                newTag.elements = contacts;
                allTags.Add(newTag);
            }
            if (allTags.Count > 0)
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
                };
                return new JsonResult(allTags, options) { StatusCode = 200 };
            }
            else
            {
                return new JsonResult("Теги не найдены") { StatusCode = 200 };
            }
        }
    }
}
