using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Text.Json;
using System.Xml.Linq;
using WebApplication1.Entities;
using WebApplication1.JsonConverter;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NoteController : Controller
    {
        private readonly NoteRepository _notes;
        private readonly ElementRepository _elements;

        public NoteController(NoteRepository notes, ElementRepository elements)
        {
            _notes = notes;
            _elements = elements;
        }

        // Получение всех заметок пользователя
        [HttpGet("GetNotes")]
        public IActionResult GetNotes()
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == false).Select(e => e.elem_id).ToList();
            var notes = _notes.Notes.Where(n => elemsId.Contains(n.elem_id));
            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter() }
            };
            return new JsonResult(notes,options) { StatusCode = 200 };
        }

        // Получение заметки по ID (с проверкой принадлежности пользователю)
        [HttpGet("GetNote/{id}")]
        public IActionResult GetNote(int id)
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == false).Select(e => e.elem_id).ToList();
            var notes = _notes.Notes.Where(n => elemsId.Contains(n.elem_id));
            var note = notes.FirstOrDefault(n => n.elem_id == id);
            if (note == null)
            {
                List<string> errors = new List<string> { "Заметка не найдена" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }

            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter() }
            };
            return new JsonResult(note, options) { StatusCode = 200 };
        }

        // Создание новой заметки
        [HttpPost("CreateNote")]
        public IActionResult CreateNote([FromForm] NoteCreateModel newNote)
        {
            if (ModelState.IsValid)
            {
                Element newElem = new Element();
                newElem.user_id = CurrentUser.UserId;
                newElem.elem_type = false;
                _elements.Elements.Add(newElem);
                _elements.SaveChanges();
                int newElemId = newElem.elem_id;

                var note = new Note();
                note.elem_id = newElemId;
                note.note_name = newNote.note_name;
                note.note_create = DateOnly.FromDateTime(DateTime.Now);
                note.description = newNote.description;
                _notes.Notes.Add(note);
                _notes.SaveChanges();

                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter() }
                };
                return new JsonResult(note,options) { StatusCode = 200 };
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

        // Изменение заметки (с проверкой принадлежности пользователю)
        [HttpPut("UpdateNote/{id}")]
        public IActionResult UpdateNote(int id, [FromForm] NoteCreateModel newNote)
        {
            if (ModelState.IsValid)
            {
                var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == false).Select(e => e.elem_id).ToList();
                var notes = _notes.Notes.Where(n => elemsId.Contains(n.elem_id));
                var note = notes.FirstOrDefault(n => n.elem_id == id);
                if (note == null)
                {
                    List<string> errors = new List<string> { "Заметка не найдена" };
                    return new JsonResult(errors)
                    { StatusCode = 404 }; // NotFound
                }
                note.note_name = newNote.note_name;
                note.note_create = DateOnly.FromDateTime(DateTime.Now);
                note.description = newNote.description;
                _notes.SaveChanges();

                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter() }
                };
                return new JsonResult(note, options) { StatusCode = 200 };
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

        // Удаление заметки (с проверкой принадлежности пользователю)
        [HttpDelete("DeleteNote/{id}")]
        public IActionResult DeleteNote(int id)
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == false).Select(e => e.elem_id).ToList();
            var notes = _notes.Notes.Where(n => elemsId.Contains(n.elem_id));
            var note = notes.FirstOrDefault(n => n.elem_id == id);
            if (note == null)
            {
                List<string> errors = new List<string> { "Заметка не найдена" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            _notes.Notes.Remove(note);
            _notes.SaveChanges();
            return new JsonResult("Заметка успешно удалена") { StatusCode = 200 };
        }

        [HttpPost("FindNoteByName")]
        public IActionResult FindNotesByName([Required] string request)
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == false).Select(e => e.elem_id).ToList();
            var notes = _notes.Notes.Where(n => elemsId.Contains(n.elem_id)).Where(c => c.note_name.Contains(request)).ToList();
            if (notes.Count != 0)
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter() }
                };
                return new JsonResult(notes,options) { StatusCode = 200 };
            }
            return new JsonResult("Заметки не найдены") { StatusCode = 200 };
        }

        [HttpGet("SortNotesByName")]
        public IActionResult SortNotesByName()
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == false).Select(e => e.elem_id).ToList();
            var notes = _notes.Notes.Where(n => elemsId.Contains(n.elem_id)).OrderBy(c => c.note_name).ToList();
            if (notes.Count != 0)
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter() }
                };
                return new JsonResult(notes, options) { StatusCode = 200 };
            }
            return new JsonResult("Заметки не найдены") { StatusCode = 200 };
        }

        //месяц считывается числом
        [HttpPost("FilterByDate")]
        public IActionResult FilterByDate([FromForm][Required] int month, [FromForm][Required] int year)
        {
            if (month <= 12 && month > 0)
            {
                if (year >= 2020 && year <= 2024)
                {
                    var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == false).Select(e => e.elem_id).ToList();
                    var notes = _notes.Notes.Where(n => elemsId.Contains(n.elem_id)).Where(c => c.note_create.Month.Equals(month) && c.note_create.Year.Equals(year)).ToList();
                    if (notes.Count != 0)
                    {
                        var options = new JsonSerializerOptions
                        {
                            Converters = { new DateOnlyConverter() }
                        };
                        return new JsonResult(notes, options) { StatusCode = 200 };
                    }
                }
                else {
                    List<string> errors = new List<string> { "Некорректно введен год" };
                    return new JsonResult(errors)
                    { StatusCode = 400 }; // BadRequest
                }
            }
            else {
                List<string> errors = new List<string> { "Некорректно введен месяц" };
                return new JsonResult(errors)
                { StatusCode = 400 }; // BadRequest
            }
            return new JsonResult("Заметки не найдены") { StatusCode = 200 };
        }
    }
}
