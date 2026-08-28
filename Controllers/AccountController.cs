using Center_Management.Models;
using Center_Management.View_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Center_Management.Context;

namespace Center_Management.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly CenterDBContext _context;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            CenterDBContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Dashboard");

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginVM());
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM vm, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
                return View(vm);

            // Try to find user by phone number (student) or email (teacher)
            var user = await _userManager.FindByNameAsync(vm.PhoneNumber);
            
            // If not found by phone, try email (for backward compatibility with teachers)
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(vm.PhoneNumber);
            }
            
            if (user == null)
            {
                ModelState.AddModelError("", "رقم الهاتف أو البريد الإلكتروني أو كلمة المرور غير صحيحة");
                return View(vm);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, vm.Password, vm.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Student"))
                {
                    // Students go to Dashboard (with welcome banner)
                    return RedirectToAction("Index", "Dashboard");
                }

                // Teachers/Admins go to dashboard
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError("", "رقم الهاتف أو البريد الإلكتروني أو كلمة المرور غير صحيحة");
            return View(vm);
        }

        // GET: Account/Register — Admin only, use Admin/CreateTeacher instead
        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register()
        {
            await LoadStudentsViewBag();
            return View(new RegisterVM());
        }

        // POST: Account/Register — Admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            await LoadStudentsViewBag();

            if (vm.Role == "Student" && !vm.StudentId.HasValue)
                ModelState.AddModelError("StudentId", "يجب اختيار الطالب لدور الطالب");

            if (!ModelState.IsValid)
                return View(vm);

            // For students, use their phone number as username
            string userName = vm.Email;
            string email = vm.Email;

            if (vm.Role == "Student" && vm.StudentId.HasValue)
            {
                var student = await _context.Students.FindAsync(vm.StudentId.Value);
                if (student != null)
                {
                    userName = student.PhoneNumber; // Use phone as username
                    email = $"{student.PhoneNumber}@center.local"; // Generate dummy email
                }
            }

            var user = new AppUser
            {
                FullName = vm.FullName,
                UserName = userName,
                Email = email,
                StudentId = vm.Role == "Student" ? vm.StudentId : null
            };

            var result = await _userManager.CreateAsync(user, vm.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, vm.Role);
                // Admin added this user — don't auto-login, redirect back to Users list
                TempData["SuccessMessage"] = $"تم إنشاء الحساب بنجاح";
                return RedirectToAction("Users", "Admin");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(vm);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // GET: Account/StudentRegister
        [HttpGet]
        public IActionResult StudentRegister()
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("StudentView", "Material");

            return View(new StudentRegisterVM());
        }

        // POST: Account/StudentRegister
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentRegister(StudentRegisterVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // البحث عن الطالب برقم الهاتف
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.PhoneNumber == vm.PhoneNumber);

            if (student == null)
            {
                ModelState.AddModelError("PhoneNumber", "رقم الهاتف غير مسجل في سجلات المركز. يرجى التواصل مع الإدارة.");
                return View(vm);
            }

            // التحقق من عدم وجود حساب مسبق لهذا الطالب
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.StudentId == student.Id);

            if (existingUser != null)
            {
                ModelState.AddModelError("PhoneNumber", "يوجد حساب مسجل بالفعل بهذا الرقم. يرجى تسجيل الدخول.");
                return View(vm);
            }

            // إنشاء حساب جديد للطالب
            var user = new AppUser
            {
                FullName = student.FullName,
                UserName = student.PhoneNumber, // استخدام رقم الهاتف كـ username
                Email = $"{student.PhoneNumber}@center.local", // Email وهمي
                StudentId = student.Id,
                PhoneNumber = student.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, vm.Password);

            if (result.Succeeded)
            {
                // إضافة دور Student
                await _userManager.AddToRoleAsync(user, "Student");
                
                // تسجيل الدخول تلقائياً
                await _signInManager.SignInAsync(user, isPersistent: false);
                
                // الانتقال إلى صفحة المواد
                return RedirectToAction("StudentView", "Material");
            }

            // إذا فشل إنشاء الحساب
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(vm);
        }

        private async Task LoadStudentsViewBag()
        {
            ViewBag.Students = await _context.Students
                .OrderBy(s => s.FullName)
                .Select(s => new { s.Id, s.FullName })
                .ToListAsync();
        }
    }
}
