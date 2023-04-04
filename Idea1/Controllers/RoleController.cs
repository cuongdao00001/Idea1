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
    public class RoleController : Controller
    {
        // GET: Role
        private ApplicationRoleManger _rolerManager;
        private ApplicationUserManager _userManager;

        public RoleController()
        {
        }

        public RoleController(ApplicationRoleManger roleManger, ApplicationUserManager userManager)
        {
            RoleManager = roleManger;
            UserManager = userManager;
        }

        public ApplicationRoleManger RoleManager
        {
            get
            {
                return _rolerManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManger>();
            }
            private set
            {
                _rolerManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        /*   public async Task<ActionResult> ManageUserRoles(List<Role> model, string userId)
           {
               var user = await UserManager.FindByIdAsync(userId);
               if (user == null)
               {
                   ViewBag.ErrorMessge = $"USer with Id ={userId} cannot be found";
                   return View("NotFound");
               }
               var roles = await UserManager.GetRolesAsync(user);
               var result = await UserManager.RemoveFromRoleAsync(user, roles);
               if (!result.Succeeded)
               {
                   ModelState.AddModelError("", "cannot remove user existing roles");
                   return View(model);
               }
           }*/
        // // GET: /RoleList
        public ActionResult RoleList()
        {
            List<Role> list = new List<Role>();
            foreach (var role in RoleManager.Roles)
                list.Add(new Role(role));
            return View(list);
        }

        //// GET:  /RoleList/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(Role model)
        {
            var role = new ApplicationRole() { Name = model.RoleName };
            await RoleManager.CreateAsync(role);
            return RedirectToAction("RoleList");
        }


        // Setting to the Function of Edit in Role
        public async Task<ActionResult> Edit(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            return View(new Role(role));
        }
        [HttpPost]
        public async Task<ActionResult> Edit(Role model)
        {
            var role = new ApplicationRole() { Id = model.RoleID, Name = model.RoleName };
            await RoleManager.UpdateAsync(role);
            return RedirectToAction("RoleList");
        }

        public async Task<ActionResult> Details(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            return View(new Role(role));
        }


        // Setting to the Function of Delete in Role

        public async Task<ActionResult> Delete(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            return View(new Role(role));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            await RoleManager.DeleteAsync(role);
            return RedirectToAction("RoleList");
        }
    }
}