using Center_Management.Interfaces;
using Center_Management.Models;
using Center_Management.View_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace Center_Management.Controllers
{
    [Authorize]
    public class MaterialController : Controller
    {
        private readonly IMatrialRepository _matrialRepo;
        private readonly IAcadimicYearsRepository _academicYearRepo;
        private readonly IWebHostEnvironment _env;

        public MaterialController(
            IMatrialRepository matrialRepo,
            IAcadimicYearsRepository academicYearRepo,
            IWebHostEnvironment env)
        {
            _matrialRepo = matrialRepo;
            _academicYearRepo = academicYearRepo;
            _env = env;
        }

        // GET: Material/Index
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Index()
        {
            // جلب جميع المواد مع التفاصيل مرتبة حسب AcademicYear
            var materials = await _matrialRepo.GetAllWithDetailsAsync();

            // تجميع المواد حسب AcademicYear فقط (بدون Subject)
            var groupedMaterials = materials
                .GroupBy(m => m.AcademicYear?.Name ?? "غير محدد")
                .Select(ayGroup => new MaterialsByAcademicYearVM
                {
                    AcademicYearName = ayGroup.Key,
                    SubjectGroups = new List<MaterialGroupVM>
                    {
                        new MaterialGroupVM
                        {
                            SubjectName = "", // لم تعد هناك حاجة للمواد الدراسية
                            Materials = ayGroup.ToList()
                        }
                    }
                })
                .ToList();

            return View(groupedMaterials);
        }

        // GET: Material/Create
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateMaterialVM
            {
                AcademicYears = (await _academicYearRepo.GetAllAsync()).ToList()
            };

            return View(vm);
        }

        // POST: Material/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Create(CreateMaterialVM vm)
        {
            // Custom validation for file upload
            if (vm.IsLocalFile)
            {
                if (vm.LocalFile == null)
                {
                    ModelState.AddModelError("LocalFile", "يرجى اختيار ملف للرفع");
                }
                else
                {
                    // Validate file type and size
                    var allowedExtensions = vm.Type == MaterialType.PDF 
                        ? new[] { ".pdf" }
                        : new[] { ".mp4", ".avi", ".mov", ".wmv", ".mkv" };

                    var fileExtension = Path.GetExtension(vm.LocalFile.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("LocalFile", $"نوع الملف غير مدعوم. الأنواع المدعومة: {string.Join(", ", allowedExtensions)}");
                    }

                    // File size validation
                    var maxSize = vm.Type == MaterialType.PDF ? 10 * 1024 * 1024 : 100 * 1024 * 1024; // 10MB for PDF, 100MB for Video
                    if (vm.LocalFile.Length > maxSize)
                    {
                        var maxSizeMB = maxSize / (1024 * 1024);
                        ModelState.AddModelError("LocalFile", $"حجم الملف كبير جداً. الحد الأقصى {maxSizeMB} ميجا");
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(vm.FileUrl))
                {
                    ModelState.AddModelError("FileUrl", "يرجى إدخال رابط الملف");
                }
            }

            if (!ModelState.IsValid)
            {
                return await ReloadCreateViewModel(vm);
            }

            try
            {
                // التحقق من أن النوع هو PDF أو Video
                if (vm.Type != MaterialType.PDF && vm.Type != MaterialType.Video)
                {
                    ModelState.AddModelError("Type", "النوع المحدد غير مدعوم، يرجى اختيار PDF أو Video");
                    return await ReloadCreateViewModel(vm);
                }

                string fileUrl = vm.FileUrl;

                // Handle file upload if local file is selected
                if (vm.IsLocalFile && vm.LocalFile != null)
                {
                    try
                    {
                        // Use WebRootPath (always resolves to the correct wwwroot folder in production)
                        var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "materials");
                        if (!Directory.Exists(uploadsPath))
                        {
                            Directory.CreateDirectory(uploadsPath);
                        }

                        // Generate unique filename
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(vm.LocalFile.FileName)}";
                        var filePath = Path.Combine(uploadsPath, fileName);

                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await vm.LocalFile.CopyToAsync(stream);
                        }

                        // Set the file URL to the uploaded file path
                        fileUrl = $"/uploads/materials/{fileName}";
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("LocalFile", "حدث خطأ أثناء رفع الملف: " + ex.Message);
                        return await ReloadCreateViewModel(vm);
                    }
                }

                // إنشاء المادة التعليمية
                var material = new Matrial
                {
                    Title = vm.Title,
                    Type = vm.Type,
                    FileUrl = fileUrl,
                    AcademicYearId = vm.AcademicYearId
                };

                // حفظ المادة
                _matrialRepo.Add(material);
                await _matrialRepo.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم إضافة المادة التعليمية بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "حدث خطأ أثناء حفظ البيانات: " + ex.Message);
                return await ReloadCreateViewModel(vm);
            }
        }

        // GET: Material/Delete?id=3
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var material = await _matrialRepo.GetByIdWithDetailsAsync(id);
            
            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        // POST: Material/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _matrialRepo.GetByIdAsync(id);
            
            if (material == null)
            {
                return NotFound();
            }

            try
            {
                _matrialRepo.Delete(material);
                await _matrialRepo.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم حذف المادة التعليمية بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء حذف المادة: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Material/StudentView - للطلاب فقط
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentView()
        {
            // الحصول على معرّف الطالب من المستخدم الحالي
            var user = await _matrialRepo.GetCurrentUserWithStudentAsync(User);
            if (user?.StudentId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            int studentId = user.StudentId.Value;

            // جلب السنوات الدراسية للطالب (من مجموعاته النشطة)
            var academicYearIds = await _matrialRepo.GetStudentAcademicYearIdsAsync(studentId);

            if (!academicYearIds.Any())
            {
                ViewBag.Message = "لم يتم العثور على مواد تعليمية";
                return View(new List<MaterialGroupVM>());
            }

            // جلب جميع المواد للسنوات الدراسية الخاصة بالطالب
            var allMaterials = await _matrialRepo.GetAllWithDetailsAsync();
            var materials = allMaterials
                .Where(m => academicYearIds.Contains(m.AcademicYearId))
                .ToList();

            // تجميع المواد حسب السنة الدراسية
            var groupedMaterials = materials
                .GroupBy(m => m.AcademicYear?.Name ?? "غير محدد")
                .Select(ayGroup => new MaterialGroupVM
                {
                    SubjectName = ayGroup.Key, // استخدام اسم السنة الدراسية بدلاً من المادة
                    Materials = ayGroup.ToList()
                })
                .ToList();

            return View(groupedMaterials);
        }

        private async Task<IActionResult> ReloadCreateViewModel(CreateMaterialVM vm)
        {
            vm.AcademicYears = (await _academicYearRepo.GetAllAsync()).ToList();
            return View(vm);
        }
    }
}
