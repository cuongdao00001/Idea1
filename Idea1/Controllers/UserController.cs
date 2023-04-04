using Idea1.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Idea1.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        private ApplicationUserManager _userManager;


        public UserController()
        {
        }

        public UserController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().Get<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: User/ListUser
        public ActionResult ListUser()
        {
            List<User> list = new List<User>();
            foreach (var user in UserManager.Users)
                list.Add(new User(user));

            return View(list);
        }        
        
        
        
        public ActionResult Details()
        {
            List<User> list = new List<User>();
            foreach (var user in UserManager.Users)
                list.Add(new User(user));

            return View(list);
        }
        // GET: User/Edit
        // Setting to the Function of Edit in Role
        public async Task<ActionResult> Edit(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            return View(new User(user));
        }
        [HttpPost]
        public async Task<ActionResult> Edit(User model)
        {
            var role = model.ApplicationUser;
            await UserManager.UpdateAsync(role);
            return RedirectToAction("RoleList");
        }
        // GET: User/Delete

        public async Task<ActionResult> Delete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            return View(new User(user));
        }
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            await UserManager.DeleteAsync(user);

            return RedirectToAction("ListUser");
        }
    }
}