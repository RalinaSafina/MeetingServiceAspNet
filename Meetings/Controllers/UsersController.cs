using Meetings.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Meetings.Controllers
{
    /// <summary>
    /// Контроллер для работы со страницей для работы с пользователями.
    /// </summary>
    /// <remarks>
    /// Содержит методы добавления, удаления, редактирования и отображения всех пользователей.
    /// </remarks>
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private ApplicationUserManager userManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        /// <summary>
        /// Отображает на странице весь список зарегистрированных пользователей.
        /// </summary>
        public ActionResult Index()
        {
            return View(userManager.Users.ToList());
        }

        /// <summary>
        /// Get метод для отображения представления для добавления нового пользователя.
        /// </summary>
        /// <returns></returns>
        public ActionResult AddUser()
        {
            return View();
        }

        /// <summary>
        /// Обрабатывает Post-запрос и добавляет нового пользователя в базу данных.
        /// </summary>
        /// <param name="model">Заполненная модель, возвращенная из представления.</param>
        [HttpPost]
        public async Task<ActionResult> AddUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Name = model.Name, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user.Id, "user");
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }
        
        /// <summary>
        /// Метод для удаления пользователя с заданным идентификатором id.
        /// </summary>
        public async Task<ActionResult> DeleteUser(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                IList<string> roles = userManager.GetRoles(user.Id);
                if(!roles.Contains("admin"))
                {
                    await userManager.DeleteAsync(user);
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                View("Error", new string[] { "Пользователь не найден" });
            }
            return View("../Shared/Error");
        }

        /// <summary>
        /// Метод для редактирования записи о пользователе в базе данных с заданным идентификатором.
        /// </summary>
        /// <param name="id">Идентификатор пользователя из базы данных.</param>
        /// <returns></returns>
        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        /// <summary>
        /// Post-метод для внесения и сохранения изменений данных о пользователе.
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="name">Имя пользователя</param>
        /// <param name="email">Электронный адрес пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        [HttpPost]
        public async Task<ActionResult> Edit(string id, string name, string email, string password)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);

            if (ModelState.IsValid)
            {
                user.Email = email;
                user.UserName = email;
                user.Name = name;
                user.PasswordHash = userManager.PasswordHasher.HashPassword(password);
                await userManager.UpdateAsync(user);
                return RedirectToAction("Index");
            }
            return View(user);
        }
    }
}