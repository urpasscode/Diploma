using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebApplication1.JsonConverter;
using System.Text.Json;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserRepository _context;

        public UserController(UserRepository context)
        {
            _context = context;
        }

        //создание нового аккаунта
        [HttpPost("Signup")]
        public IActionResult Registration([FromForm] UserLoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.user_email == loginModel.user_email))
                {
                    List<string> errors = new List<string> { "Пользователь с таким адресом электронной почты уже существует" };
                    return new JsonResult(errors)
                    { StatusCode = 400 }; // BadRequest
                }
                var newUser = new User
                {
                    user_email = loginModel.user_email,
                    user_password = loginModel.user_password
                };
                _context.Users.Add(newUser);
                _context.SaveChanges();

                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter() }
                };
                return new JsonResult(newUser, options) { StatusCode = 200 };
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

        //выход в существующий аккаунт
        [HttpPost("Login")]
        public JsonResult Login([FromForm] UserLoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.user_email == loginModel.user_email);
                if (user == null)
                {
                    List<string> errors = new List<string> { "Пользователь не найден. Зарегистрируйтесь!" };
                    return new JsonResult(errors)
                    { StatusCode = 404 }; // NotFound
                }
                if (user.user_password != loginModel.user_password)
                {
                    List<string> errors = new List<string> { "Неверный пароль" };
                    return new JsonResult(errors)
                    { StatusCode = 400 }; // BadRequest
                }
                CurrentUser.UserId = user.user_id;
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter() }
                };
                return new JsonResult(user, options) { StatusCode = 200 };
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

        //получение данных пользователя
        [HttpGet("GetUserData")]
        public IActionResult GetUser()
        {
            var user = _context.Users.FirstOrDefault(u => u.user_id == CurrentUser.UserId);
            if (user == null)
            {
                List<string> errors = new List<string> { "Пользователь не найден" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter() }
            };
            return new JsonResult(user,options) { StatusCode = 200 };
        }

        // Изменение данных пользователя
        [HttpPut("ChangeUserData")]
        public IActionResult UpdateUser( [FromForm] UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Find(CurrentUser.UserId);
                if (_context.Users.Any(u => u.user_email == userModel.user_email && u.user_id != CurrentUser.UserId))
                {
                    List<string> errors = new List<string> { "Пользователь с такой почтой уже существует" };
                    return new JsonResult(errors)
                    { StatusCode = 400 }; // BadRequest
                }
                
                user.user_name = userModel.user_name;
                user.user_email = userModel.user_email;
                user.user_password = userModel.user_password;
                user.user_pn = userModel.user_pn;
                if (userModel.user_birth!=null)
                {
                    DateOnly newBirth;
                    if (DateOnly.TryParse(userModel.user_birth, out newBirth))
                    {
                        user.user_birth = newBirth;
                    }
                    else
                    {
                        List<string> errors = new List<string> { "Неверный формат ввода даты рождения" };
                        return new JsonResult(errors)
                        { StatusCode = 400 }; // BadRequest
                    }
                }
                _context.SaveChanges();
                var options = new JsonSerializerOptions
                {
                    Converters = { new DateOnlyConverter() }
                };
                return new JsonResult(user, options) { StatusCode = 200 };
            }
            else
            {
                List<string> errors = new List<string>();
                foreach (var item in ModelState)
                {
                    // если для определенного элемента имеются ошибки
                    if (item.Value.ValidationState == ModelValidationState.Invalid)
                    {
                        // пробегаемся по всем ошибкам
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

        // Удаление пользователя
        [HttpDelete("DeleteAccount")]
        public IActionResult DeleteUser()
        {
            var user = _context.Users.Find(CurrentUser.UserId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                CurrentUser.UserId = -1;
                return new JsonResult("Пользователь успешно удален") { StatusCode = 200 };
            }
            else
            {
                List<string> errors = new List<string> { "Пользователь не найден" };
                return new JsonResult(errors)
                { StatusCode = 404 }; // NotFound
            }
        }
    }
}
   