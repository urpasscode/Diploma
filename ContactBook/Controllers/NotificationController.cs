using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Entities;
using WebApplication1;
using WebApplication1.Controllers;
using System.Xml.Linq;
using System.Text.Json;
using WebApplication1.JsonConverter;

namespace ContactBook.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationController : Controller
    {
        private readonly NotificationRepository _notifications;
        private readonly EventRepository _events;
        private readonly FrequencyRepository _frequencies;
        private readonly ContactRepository _contacts;
        private readonly ElementRepository _elements;
        private readonly NoteRepository _notes;

        public NotificationController(NoteRepository notes, NotificationRepository notifications, EventRepository events, FrequencyRepository frequencies,
                                        ContactRepository contacts, ElementRepository elements)
        {
            _notes = notes;
            _notifications = notifications;
            _events = events;
            _frequencies = frequencies;
            _contacts = contacts;
            _elements = elements;
        }

        // Получение всех напоминаний пользователя
        [HttpGet("GetNotifications")]
        public IActionResult GetNotifications()
        {
            var notifications = _notifications.Notifications.Where(e => e.user_id == CurrentUser.UserId).ToList();
            var allNotifications = new List<NotificationAndInformation>();
            var events = new EventController(_events);
            var frequencies = new FrequencyController(_frequencies);
            foreach (var notification in notifications)
            {
                var notificationWithInformation = new NotificationAndInformation();
                notificationWithInformation.notification = notification;
                notificationWithInformation.ev = _events.Events.FirstOrDefault(n => n.event_id == notification.ev_id);
                notificationWithInformation.frequency = _frequencies.Frequencies.FirstOrDefault(n => n.freq_id == notification.freq_id);
                allNotifications.Add(notificationWithInformation);
            }

            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
            };
            return new JsonResult(allNotifications, options) { StatusCode = 200 };
        }

        // Получение одного напоминания
        [HttpGet("GetOneNotification/{id}")]
        public IActionResult GetOneNotification(int id)
        {
            var notification = _notifications.Notifications.FirstOrDefault(e => e.user_id == CurrentUser.UserId && e.notif_id == id);
            if (notification != null)
            {
                var events = new EventController(_events);
                var frequencies = new FrequencyController(_frequencies);
                var notificationWithInformation = new NotificationAndInformation();
                notificationWithInformation.notification = notification;
                notificationWithInformation.ev = _events.Events.FirstOrDefault(n => n.event_id == notification.ev_id);
                notificationWithInformation.frequency = _frequencies.Frequencies.FirstOrDefault(n => n.freq_id == notification.freq_id);

                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
                };
                return new JsonResult(notification, options) { StatusCode = 200 };
            }
            else
            {
                List<string> errors = new List<string> { "Уведомление не найдено" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
        }

        // Создание нового напоминания для контакта
        [HttpPost("CreateNotificationForContact")]
        public IActionResult CreateNotificationForContact(int elem_id)
        {
            //получаем сам контакт
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
            var contacts = _contacts.Contacts.Where(n => elemsId.Contains(n.elem_id));
            var contact = contacts.FirstOrDefault(n => n.elem_id == elem_id);
            if (contact == null)
            {
                List<string> errors = new List<string> { "Выберите контакт для создания напоминания" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }

            Notification newNotification = new Notification();
            newNotification.elem_id = elem_id;
            newNotification.user_id = CurrentUser.UserId;
            newNotification.notif_name = "День рождения";
            newNotification.notif_desc = contact.fio + " празднует день рождения. Не забудьте поздравить";
            newNotification.notif_type = true;
            newNotification.freq_id = 6;
            newNotification.ev_id = 1;

            var oldDate = contact.cont_birth;
            var newDate = oldDate.AddYears(DateTime.Today.Year - oldDate.Year + 1);
            newNotification.notif_date = newDate;
            newNotification.notif_time = new TimeOnly(12, 0, 0);
            _notifications.Notifications.Add(newNotification);
            _notifications.SaveChanges();

            var fullNotification = new NotificationAndInformation();
            fullNotification.notification = newNotification;

            var events = new EventController(_events);
            var frequencies = new FrequencyController(_frequencies);

            fullNotification.ev = _events.Events.FirstOrDefault(n => n.event_id == newNotification.ev_id);
            fullNotification.frequency = _frequencies.Frequencies.FirstOrDefault(n => n.freq_id == newNotification.freq_id);

            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
            };

            return new JsonResult(fullNotification, options) { StatusCode = 200 };
        }

        //создание напоминания для заметки
        [HttpPost("CreateNotificationForNote")]
        public IActionResult CreateNotificationForNote([FromForm] NotificationModel notification, int elem_id)
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == false).Select(e => e.elem_id).ToList();
            var notes = _notes.Notes.Where(n => elemsId.Contains(n.elem_id));
            var note = notes.FirstOrDefault(n => n.elem_id == elem_id);
            if (note != null)
            {
                if (ModelState.IsValid)
                {
                    Notification newNotification = new Notification();
                    newNotification.elem_id = elem_id;
                    newNotification.user_id = CurrentUser.UserId;
                    newNotification.notif_name = notification.notif_name;
                    newNotification.notif_desc = notification.notif_desc;
                    newNotification.notif_type = true;
                    newNotification.freq_id = notification.freq_id;
                    newNotification.ev_id = notification.ev_id;
                    if (notification.freq_id == 1)
                    {
                        List<string> answer = new List<string> { "О данном событии не будет напоминаний" };
                        return new JsonResult(answer) { StatusCode = 200 };
                    }
                    else if (notification.freq_id == 2)
                    {
                        int currentYear = newNotification.notif_date.Year;
                        newNotification.notif_date = DateOnly.FromDateTime(DateTime.Now).AddDays(1);
                        newNotification.notif_time = new TimeOnly(12, 0, 0);
                    }
                    else if (notification.freq_id == 3)
                    {
                        newNotification.notif_date = DateOnly.FromDateTime(DateTime.Now).AddDays(7);
                        newNotification.notif_time = new TimeOnly(12, 0, 0);
                    }
                    else if (notification.freq_id == 4)
                    {
                        newNotification.notif_date = DateOnly.FromDateTime(DateTime.Now).AddMonths(1);
                        newNotification.notif_time = new TimeOnly(12, 0, 0);
                    }
                    else if (notification.freq_id == 5)
                    {
                        newNotification.notif_date = DateOnly.FromDateTime(DateTime.Now).AddYears(1);
                        newNotification.notif_time = new TimeOnly(12, 0, 0);
                    }
                    else if (notification.freq_id == 6)
                    {
                        if (notification.notif_date == null)
                        {
                            List<string> errors = new List<string> { "Введите дату уведомления" };
                            return new JsonResult(errors)
                            { StatusCode = 400 };
                        }
                        DateOnly newDate;
                        if (DateOnly.TryParse(notification.notif_date, out newDate))
                        {
                            newNotification.notif_date = newDate;
                        }
                        else
                        {
                            List<string> errors = new List<string> { "Неверный формат ввода даты напоминания" };
                            return new JsonResult(errors)
                            { StatusCode = 400 }; // BadRequest
                        }
                        newNotification.notif_time = new TimeOnly(12, 0, 0);
                        newNotification.notif_time = new TimeOnly(12, 0, 0);
                    }
                    else
                    {
                        List<string> errors = new List<string> { "Выберите переодичность напоминания" };
                        return new JsonResult(errors)
                        { StatusCode = 400 };
                    }
                    _notifications.Notifications.Add(newNotification);
                    _notifications.SaveChanges();

                    var fullNotification = new NotificationAndInformation();
                    fullNotification.notification = newNotification;

                    fullNotification.ev = _events.Events.FirstOrDefault(n => n.event_id == newNotification.ev_id);
                    fullNotification.frequency = _frequencies.Frequencies.FirstOrDefault(n => n.freq_id == newNotification.freq_id);

                    var options = new JsonSerializerOptions
                    {
                        Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
                    };
                    return new JsonResult(fullNotification, options) { StatusCode = 200 };
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
            else
            {
                List<string> errors = new List<string> { "Выберите заметку для создания напоминания" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
        }

        // Изменение напоминания для заметки
        [HttpPut("UpdateNotificationForNote/{id}")]
        public IActionResult UpdateNotificationForNote(int idNote, int id, [FromForm] NotificationModel updateNotification)
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == false).Select(e => e.elem_id).ToList();
            var notes = _notes.Notes.Where(n => elemsId.Contains(n.elem_id));
            var note = notes.FirstOrDefault(n => n.elem_id == idNote);
            if (note != null)
            {
                if (ModelState.IsValid)
                {
                    Notification newNotification = _notifications.Notifications.FirstOrDefault(n => n.notif_id == id);
                    if (newNotification == null)
                    {
                        List<string> errors = new List<string> { "Напоминание не найдено" };
                        return new JsonResult(errors)
                        { StatusCode = 404 }; // NotFound
                    }
                    newNotification.elem_id = idNote;
                    newNotification.notif_name = updateNotification.notif_name;
                    newNotification.notif_desc = updateNotification.notif_desc;
                    newNotification.notif_type = true;
                    newNotification.freq_id = updateNotification.freq_id;
                    newNotification.ev_id = updateNotification.ev_id;

                    if (updateNotification.freq_id == 1)
                    {
                        List<string> answer = new List<string> { "О данном событии не будет напоминаний" };
                        return new JsonResult(answer) { StatusCode = 200 };
                    }
                    else if (updateNotification.freq_id == 2)
                    {
                        int currentYear = newNotification.notif_date.Year;
                        newNotification.notif_date = DateOnly.FromDateTime(DateTime.Now).AddDays(1);
                        newNotification.notif_time = new TimeOnly(12, 0, 0);
                        Console.WriteLine(newNotification.notif_date);
                    }
                    else if (updateNotification.freq_id == 3)
                    {
                        newNotification.notif_date = DateOnly.FromDateTime(DateTime.Now).AddDays(7);
                        newNotification.notif_time = new TimeOnly(12, 0, 0);
                    }
                    else if (updateNotification.freq_id == 4)
                    {
                        newNotification.notif_date = DateOnly.FromDateTime(DateTime.Now).AddMonths(1);
                        newNotification.notif_time = new TimeOnly(12, 0, 0);
                    }
                    else if (updateNotification.freq_id == 5)
                    {
                        newNotification.notif_date = DateOnly.FromDateTime(DateTime.Now).AddYears(1);
                        newNotification.notif_time = new TimeOnly(12, 0, 0);
                    }
                    else if (updateNotification.freq_id == 6)
                    {
                        if (updateNotification.notif_date == null)
                        {
                            List<string> errors = new List<string> { "Введите дату уведомления" };
                            return new JsonResult(errors)
                            { StatusCode = 400 };
                        }
                        DateOnly newDate;
                        if (DateOnly.TryParse(updateNotification.notif_date, out newDate))
                        {
                            newNotification.notif_date = newDate;
                        }
                        else
                        {
                            List<string> errors = new List<string> { "Неверный формат ввода даты напоминания" };
                            return new JsonResult(errors)
                            { StatusCode = 400 }; // BadRequest
                        }
                        newNotification.notif_time = new TimeOnly(12, 0, 0);
                    }
                    else
                    {
                        List<string> errors = new List<string> { "Выберите переодичность напоминания" };
                        return new JsonResult(errors)
                        { StatusCode = 400 };
                    }
                    _notifications.SaveChanges();

                    var fullNotification = new NotificationAndInformation();
                    fullNotification.notification = newNotification;

                    fullNotification.ev = _events.Events.FirstOrDefault(n => n.event_id == newNotification.ev_id);
                    fullNotification.frequency = _frequencies.Frequencies.FirstOrDefault(n => n.freq_id == newNotification.freq_id);

                    var options = new JsonSerializerOptions
                    {
                        Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
                    };

                    return new JsonResult(fullNotification,options) { StatusCode = 200 };
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
            else
            {
                List<string> errors = new List<string> { "Выберите заметку для изменения напоминания" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
        }

        //изменение напоминания для контакта
        [HttpPut("UpdateNotificationForContact/{id}")]
        public IActionResult UpdateNotificationForContact(int idContact, int id)
        {
            var elemsId = _elements.Elements.Where(e => e.user_id == CurrentUser.UserId && e.elem_type == true).Select(e => e.elem_id).ToList();
            var contacts = _contacts.Contacts.Where(n => elemsId.Contains(n.elem_id));
            var contact = contacts.FirstOrDefault(n => n.elem_id == idContact);
            if (contact != null)
            {
                if (ModelState.IsValid)
                {
                    Notification newNotification = _notifications.Notifications.FirstOrDefault(n => n.notif_id == id);
                    if (newNotification == null)
                    {
                        List<string> errors = new List<string> { "Напоминание не найдено" };
                        return new JsonResult(errors)
                        { StatusCode = 404 }; // NotFound
                    }
                    var oldDate = contact.cont_birth;
                    var newDate = oldDate.AddYears(DateTime.Today.Year - oldDate.Year + 1);
                    newNotification.notif_date = newDate;
                    newNotification.notif_time = new TimeOnly(12, 0, 0);

                    _notifications.SaveChanges();

                    var fullNotification = new NotificationAndInformation();
                    fullNotification.notification = newNotification;

                    fullNotification.ev = _events.Events.FirstOrDefault(n => n.event_id == newNotification.ev_id);
                    fullNotification.frequency = _frequencies.Frequencies.FirstOrDefault(n => n.freq_id == newNotification.freq_id);

                    var options = new JsonSerializerOptions
                    {
                        Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
                    };

                    return new JsonResult(fullNotification, options) { StatusCode = 200 };
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
            else
            {
                List<string> errors = new List<string> { "Выберите контакт для изменения напоминания" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
        }

        // Удаление напоминания
        [HttpDelete("DeleteNotification/{id}")]
        public IActionResult DeleteNotification(int id)
        {
            Notification notification = _notifications.Notifications.FirstOrDefault(n => n.notif_id == id);
            if (notification == null)
            {
                List<string> errors = new List<string> { "Уведомление не найдено" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }

            _notifications.Notifications.Remove(notification);
            _notifications.SaveChanges();
            return new JsonResult("Уведомление успешно удалено") { StatusCode = 200 };
        }

        //получение прошедших и текущих напоминаний
        [HttpGet("GetNotificationsBeforeToday")]
        public IActionResult GetNotificationsBeforeToday()
        {
            var notifications = _notifications.Notifications.Where(e => e.user_id == CurrentUser.UserId).ToList();
            var allNotifications = new List<NotificationAndInformation>();
            var events = new EventController(_events);
            var frequencies = new FrequencyController(_frequencies);
            foreach (var notification in notifications)
            {
                if (notification.notif_date <= DateOnly.FromDateTime(DateTime.Now))
                {
                    var notificationWithInformation = new NotificationAndInformation();
                    notificationWithInformation.notification = notification;
                    notificationWithInformation.ev = _events.Events.FirstOrDefault(n => n.event_id == notification.ev_id);
                    notificationWithInformation.frequency = _frequencies.Frequencies.FirstOrDefault(n => n.freq_id == notification.freq_id);
                    allNotifications.Add(notificationWithInformation);
                }
            }
            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter(), new TimeOnlyConverter() }
            };
            return new JsonResult(allNotifications,options) { StatusCode = 200 };
        }
    }
}
