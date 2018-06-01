using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Paho.Models;
using System.Configuration;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersAdminController : ControllerBase
    {
        public UsersAdminController()
        {
        }

        public class AddSelectListItem : SelectListItem
        {
            public int? CountryID { get; set; }
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager) :  base(userManager)
        {  
            RoleManager = roleManager;
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Users/
        public async Task<ActionResult> Index()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var countryId = user.Institution.CountryID ?? 0;
            var Institution_Id = user.InstitutionID ?? 0;
            var Region_id = user.Institution.cod_region_institucional ?? 0;

            var ListUser = await UserManager.Users.OrderBy(s => s.Institution.Country.Code).ThenBy(s => s.Institution.Name).ThenBy(s => s.UserName).ThenBy(s => s.FirstName1).ToListAsync();

            if (user.Institution.AccessLevel == AccessLevel.Regional)
            {
                ListUser = await UserManager.Users.Where(j => j.Institution.cod_region_institucional == Region_id).OrderBy(s => s.Institution.Country.Code).ThenBy(s => s.Institution.Name).ThenBy(s => s.UserName).ThenBy(s => s.FirstName1).ToListAsync();
            } else if ( user.Institution.AccessLevel == AccessLevel.SelfOnly) {
                ListUser = await UserManager.Users.Where(j => j.InstitutionID == Institution_Id).OrderBy(s => s.Institution.Country.Code).ThenBy(s => s.Institution.Name).ThenBy(s => s.UserName).ThenBy(s => s.FirstName1).ToListAsync();
            } else if (user.Institution.AccessLevel == AccessLevel.Country )
            {
                ListUser = await UserManager.Users.Where(j => j.Institution.CountryID == countryId).OrderBy(s => s.Institution.Country.Code).ThenBy(s => s.Institution.Name).ThenBy(s => s.UserName).ThenBy(s => s.FirstName1).ToListAsync();
            }

            setUserRolesListJoin(ListUser);     //#### CAFQ: 180530

            return View(ListUser);
        }

        private void setUserRolesListJoin(List<ApplicationUser> ListUserX)
        {
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("UserRolesListJoin", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                {
                    con.Open();

                    foreach (var item in ListUserX)
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@UserName", SqlDbType.Text).Value = item.UserName;

                        //////con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string cRoles = reader.GetValue(0).ToString();
                                item.UserRolesListJoin = cRoles;
                            }
                        }

                        //command.Parameters.Clear();
                        //////con.Close();
                    }

                    command.Parameters.Clear();
                    con.Close();
                }
            }
        }

        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        //
        // GET: /Users/Create
        public async Task<ActionResult> Create()
        {
            var model = new RegisterViewModel();
            //model.Institutions = from value in db.Institutions
            //                     select new AddSelectListItem
            //                     {
            //                         Text = value.FullName,
            //                         Value = value.ID.ToString(),
            //                         Selected = false,
            //                         CountryID = value.CountryID
            //                     };

            //model.Institutions = model.Institutions.OrderBy(o => o.Text);

            var user_id = UserManager.FindById(User.Identity.GetUserId());

            if (user_id.Institution.AccessLevel == AccessLevel.All)
            {
                model.Institutions = from value in db.Institutions
                                             orderby value.CountryID, value.FullName
                                             select new SelectListItem
                                             {
                                                 Text = value.FullName,
                                                 Value = value.ID.ToString(),
                                                 Selected = false
                                             };
            }
            else if (user_id.Institution.AccessLevel == AccessLevel.Country)
            {
                model.Institutions = from value in db.Institutions
                                             where value.CountryID == user_id.Institution.CountryID
                                             orderby value.CountryID, value.FullName
                                             select new SelectListItem
                                             {
                                                 Text = value.FullName,
                                                 Value = value.ID.ToString(),
                                                 Selected = false
                                             };
            }
            else if (user_id.Institution.AccessLevel == AccessLevel.Regional)
            {
                model.Institutions = from value in db.Institutions
                                             where value.cod_region_institucional == user_id.Institution.cod_region_institucional
                                             orderby value.CountryID, value.FullName
                                             select new SelectListItem
                                             {
                                                 Text = value.FullName,
                                                 Value = value.ID.ToString(),
                                                 Selected = false
                                             };
            }
            else if (user_id.Institution.AccessLevel == AccessLevel.SelfOnly)
            {
                model.Institutions = from value in db.Institutions
                                             where value.ID == user_id.InstitutionID
                                             orderby value.CountryID, value.FullName
                                             select new SelectListItem
                                             {
                                                 Text = value.FullName,
                                                 Value = value.ID.ToString(),
                                                 Selected = false
                                             };
            }

            //Get the list of Roles
            model.RolesList = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");

            return View(model);
        }

        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { 
                    UserName = userViewModel.UserName, 
                    Email = userViewModel.Email,               
                    InstitutionID = userViewModel.InstitutionID,
                    FirstName1 = userViewModel.FirstName1,
                    FirstName2 = userViewModel.FirstName2,
                    LastName1 =   userViewModel.LastName1,
                    LastName2 =  userViewModel.LastName2,
                    Hometown = userViewModel.Hometown,
                    };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                var user_id = UserManager.FindById(User.Identity.GetUserId());

                if (user_id.Institution.AccessLevel == AccessLevel.All)
                {
                    userViewModel.Institutions = from value in db.Institutions
                                                 orderby value.CountryID, value.FullName
                                                 select new SelectListItem
                                                 {
                                                     Text = value.FullName,
                                                     Value = value.ID.ToString(),
                                                     Selected = false
                                                 };
                } else if (user_id.Institution.AccessLevel == AccessLevel.Country)
                {
                    userViewModel.Institutions = from value in db.Institutions
                                                 where value.CountryID == user_id.Institution.CountryID
                                                 orderby value.CountryID, value.FullName
                                                 select new SelectListItem
                                                 {
                                                     Text = value.FullName,
                                                     Value = value.ID.ToString(),
                                                     Selected = false
                                                 };
                } else if (user_id.Institution.AccessLevel == AccessLevel.Regional)
                {
                    userViewModel.Institutions = from value in db.Institutions
                                                 where value.cod_region_institucional == user_id.Institution.cod_region_institucional
                                                 orderby value.CountryID, value.FullName
                                                 select new SelectListItem
                                                 {
                                                     Text = value.FullName,
                                                     Value = value.ID.ToString(),
                                                     Selected = false
                                                 };
                }
                else if (user_id.Institution.AccessLevel == AccessLevel.SelfOnly)
                {
                    userViewModel.Institutions = from value in db.Institutions
                                                 where value.ID == user_id.InstitutionID
                                                 orderby value.CountryID, value.FullName
                                                 select new SelectListItem
                                                 {
                                                     Text = value.FullName,
                                                     Value = value.ID.ToString(),
                                                     Selected = false
                                                 };
                }


                //userViewModel.Institutions =  userViewModel.Institutions.OrderBy(o => o.Text);

                //Get the list of Roles
                userViewModel.RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = selectedRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                });

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View(userViewModel);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View(userViewModel);

                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View(userViewModel);
        }

        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            //var institutions = from value in db.Institutions
            //                   where value.CountryID == user.Institution.CountryID
            //                   orderby value.CountryID, value.FullName
            //                     select new SelectListItem
            //                     {
            //                         Text = value.FullName,
            //                         Value = value.ID.ToString(),
            //                         Selected = false
            //                     };

            var user_id = UserManager.FindById(User.Identity.GetUserId());

            var institutions = from value in db.Institutions
                                             orderby value.CountryID, value.FullName
                                             select new SelectListItem
                                             {
                                                 Text = value.FullName,
                                                 Value = value.ID.ToString(),
                                                 Selected = false
                                             };

             if (user_id.Institution.AccessLevel == AccessLevel.Country)
            {
                institutions = from value in db.Institutions
                                             where value.CountryID == user_id.Institution.CountryID
                                             orderby value.CountryID, value.FullName
                                             select new SelectListItem
                                             {
                                                 Text = value.FullName,
                                                 Value = value.ID.ToString(),
                                                 Selected = false
                                             };
            }
            else if (user_id.Institution.AccessLevel == AccessLevel.Regional)
            {
                institutions = from value in db.Institutions
                                             where value.cod_region_institucional == user_id.Institution.cod_region_institucional
                                             orderby value.CountryID, value.FullName
                                             select new SelectListItem
                                             {
                                                 Text = value.FullName,
                                                 Value = value.ID.ToString(),
                                                 Selected = false
                                             };
            }
            else if (user_id.Institution.AccessLevel == AccessLevel.SelfOnly)
            {
                institutions = from value in db.Institutions
                                             where value.ID == user_id.InstitutionID
                                             orderby value.CountryID, value.FullName
                                             select new SelectListItem
                                             {
                                                 Text = value.FullName,
                                                 Value = value.ID.ToString(),
                                                 Selected = false
                                             };
            }

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                InstitutionID =user.InstitutionID.Value,
                Hometown = user.Hometown,
                FirstName1 = user.FirstName1,
                FirstName2 = user.FirstName2,
                LastName1 = user.LastName1,
                LastName2 = user.LastName2,
                Email = user.Email,           
                Institutions = institutions,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "UserName,Email,Id,InstitutionID,FirstName1,FirstName2,LastName1,LastName2, Hometown")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.UserName;
                user.Email = editUser.Email;
                user.InstitutionID = editUser.InstitutionID;
                user.FirstName1 = editUser.FirstName1;
                user.FirstName2 = editUser.FirstName2;
                user.LastName1 =   editUser.LastName1;
                user.LastName2 =  editUser.LastName2;
                user.Hometown = editUser.Hometown;

                var result = await UserManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                result = UserManager.RemoveFromRolesAsync(user.Id, userRoles.ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }

                result = UserManager.AddToRolesAsync(user.Id, selectedRole.ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Algo falló, por favor comuniquese con el Administrador.");
            return View();
        }

        //
        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
