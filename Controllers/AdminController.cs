using Center_Management.Context;
using Center_Management.Models;
using Center_Management.View_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Center_Management.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly CenterDBContext _context;

        public AdminController(UserManager<AppUser> userManager, CenterDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Admin/Index - Dashboard for Admin
        public async Task<IActionResult> Index()
        {
            // Redirect to Users page for now, or create a dedicated admin dashboard
            return RedirectToAction(nameof(Users));
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users(CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userManager.Users
                    .Include(u => u.Student)
                    .OrderBy(u => u.FullName)
                    .ToListAsync(cancellationToken);

                var userViewModels = new List<UserManagementVM>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userViewModels.Add(new UserManagementVM
                    {
                        Id = user.Id,
                        FullName = user.FullName ?? "غير محدد",
                        UserName = user.UserName ?? "",
                        Email = user.Email ?? "",
                        Roles = string.Join(", ", roles),
                        StudentName = user.Student?.FullName
                    });
                }

                return View(userViewModels);
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                TempData["Error"] = $"حدث خطأ: {ex.Message}";
                return View(new List<UserManagementVM>());
            }
        }

        // GET: Admin/CreateTeacher
        public IActionResult CreateTeacher()
        {
            return View(new CreateTeacherVM());
        }

        // POST: Admin/CreateTeacher
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTeacher(CreateTeacherVM vm, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // التحقق من أن البريد الإلكتروني غير مستخدم
            var existingUser = await _userManager.FindByEmailAsync(vm.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "البريد الإلكتروني مستخدم بالفعل");
                return View(vm);
            }

            // التحقق من أن اسم المستخدم غير مستخدم
            var existingUsername = await _userManager.FindByNameAsync(vm.Email);
            if (existingUsername != null)
            {
                ModelState.AddModelError("Email", "هذا الحساب موجود بالفعل");
                return View(vm);
            }

            // إنشاء المستخدم الجديد
            var user = new AppUser
            {
                FullName = vm.FullName,
                UserName = vm.Email,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                StudentId = null
            };

            var result = await _userManager.CreateAsync(user, vm.Password);

            if (result.Succeeded)
            {
                // إضافة دور المعلم
                await _userManager.AddToRoleAsync(user, "Teacher");
                
                TempData["Success"] = $"تم إضافة المعلم {vm.FullName} بنجاح";
                return RedirectToAction(nameof(Users));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(vm);
        }

        // GET: Admin/DeleteUser?id=xxx
        public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "المستخدم غير موجود";
                return RedirectToAction(nameof(Users));
            }

            var roles = await _userManager.GetRolesAsync(user);
            
            // منع حذف المسؤول
            if (roles.Contains("Admin"))
            {
                TempData["Error"] = "لا يمكن حذف حساب المسؤول!";
                return RedirectToAction(nameof(Users));
            }
            
            var vm = new DeleteUserVM
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                Roles = string.Join(", ", roles)
            };

            return View(vm);
        }

        // POST: Admin/DeleteUserConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "المستخدم غير موجود";
                return RedirectToAction(nameof(Users));
            }

            // التحقق من أن المستخدم ليس Admin (حماية)
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                TempData["Error"] = "لا يمكن حذف حساب الأدمن!";
                return RedirectToAction(nameof(Users));
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = $"تم حذف المستخدم {user.FullName} بنجاح";
            }
            else
            {
                TempData["Error"] = "حدث خطأ أثناء حذف المستخدم";
            }

            return RedirectToAction(nameof(Users));
        }
    }
}
