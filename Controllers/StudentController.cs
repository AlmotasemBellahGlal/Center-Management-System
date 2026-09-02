using Center_Management.Interfaces;
using Center_Management.Models;
using Center_Management.View_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Center_Management.Controllers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class StudentController : Controller
    {
        private readonly IStudentRepository studentRepository;
        private readonly IGroupRepository groupRepository;

        public StudentController(
            IStudentRepository studentRepository,
            IGroupRepository groupRepository)
        {
            this.studentRepository = studentRepository;
            this.groupRepository = groupRepository;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var students = await studentRepository.GetAllWithGroupsAsync(cancellationToken);

            // Separate students with active groups and those without
            var studentsWithActiveGroup = students.Where(s => s.StudentGroups.Any(g => g.IsActive)).ToList();
            var studentsWithoutActiveGroup = students.Where(s => !s.StudentGroups.Any(g => g.IsActive)).ToList();

            // Group students with active groups by Academic Year, then by Group name
            var groupedStudents = studentsWithActiveGroup
                .GroupBy(s => s.StudentGroups.FirstOrDefault(g => g.IsActive)?.Group?.AcademicYear?.Name ?? "بدون سنة دراسية")
                .OrderBy(g => g.Key)
                .Select(yearGroup => new
                {
                    AcademicYearName = yearGroup.Key,
                    Groups = yearGroup
                        .GroupBy(s => s.StudentGroups.FirstOrDefault(g => g.IsActive)?.Group?.Name ?? "بدون مجموعة")
                        .OrderBy(g => g.Key)
                        .Select(groupGroup => new
                        {
                            GroupName = groupGroup.Key,
                            GroupId   = groupGroup.First()
                                            .StudentGroups
                                            .FirstOrDefault(sg => sg.IsActive)?.GroupId ?? 0,
                            Students = groupGroup.OrderBy(s => s.FullName).ToList()
                        })
                        .ToList()
                })
                .ToList();

            // Pass data to view via ViewData
            ViewData["GroupedStudents"] = groupedStudents;
            ViewData["StudentsWithoutActiveGroup"] = studentsWithoutActiveGroup;

            return View();
        }
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var student = await studentRepository.GetDetailsAsync(id, cancellationToken);

            if (student == null)
                return NotFound();

            return View(student);
        }
        private async Task LoadGroups(CreateStudentVM vm, CancellationToken cancellationToken)
        {
            var groups = await groupRepository
                .GetGroupsWithAcademicYearAndSubjectAsync(cancellationToken);

            vm.AcademicYears = groups
                .GroupBy(g => g.AcademicYearId)
                .Select(g => new AcademicYearGroupsVM
                {
                    AcademicYearId = g.Key,
                    AcademicYearName = g.First().AcademicYear!.Name,
                   

                    Groups = g.Select(x => new GroupSelectionVM
                    {
                        GroupId = x.Id,
                        GroupName = x.Name,

                    }).ToList()

                }).ToList();
        }
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            CreateStudentVM vm = new();

            await LoadGroups(vm, cancellationToken);

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStudentVM vm, CancellationToken cancellationToken)
        {
            // التحقق من اختيار مجموعة
            if (vm.SelectedGroupId <= 0)
            {
                ModelState.AddModelError("SelectedGroupId", "يجب اختيار مجموعة دراسية");
            }

            if (!ModelState.IsValid)
            {
                await LoadGroups(vm, cancellationToken);
                return View(vm);
            }

            // إنشاء الطالب الجديد
            var student = new Student
            {
                FullName = vm.FullName,
                PhoneNumber = vm.PhoneNumber,
                ParentPhoneNumber = vm.ParentPhoneNumber
            };

            // إضافة الطالب للمجموعة المختارة
            student.StudentGroups.Add(new StudentGroup
            {
                GroupId = vm.SelectedGroupId,
                IsActive = true,
                EnrollmentDate = DateOnly.FromDateTime(DateTime.Now)
            });

            studentRepository.Add(student);
            await studentRepository.SaveChangesAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var student = await studentRepository.GetForEditAsync(id, cancellationToken);

            if (student == null)
                return NotFound();

            CreateStudentVM vm = new()
            {
                Id = student.Id,
                FullName = student.FullName,
                PhoneNumber = student.PhoneNumber,
                ParentPhoneNumber = student.ParentPhoneNumber
            };

            await LoadGroups(vm, cancellationToken);

            // تعيين المجموعة الحالية للطالب
            var currentGroup = student.StudentGroups.FirstOrDefault(sg => sg.IsActive);
            if (currentGroup != null)
            {
                vm.SelectedGroupId = currentGroup.GroupId;
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateStudentVM vm, CancellationToken cancellationToken)
        {
            // التحقق من اختيار مجموعة
            if (vm.SelectedGroupId <= 0)
            {
                ModelState.AddModelError("SelectedGroupId", "يجب اختيار مجموعة دراسية");
            }

            if (!ModelState.IsValid)
            {
                await LoadGroups(vm, cancellationToken);
                return View(vm);
            }

            var student = await studentRepository.GetForEditAsync(vm.Id, cancellationToken);
            if (student == null)
                return NotFound();

            // تحديث البيانات الأساسية
            student.FullName = vm.FullName;
            student.PhoneNumber = vm.PhoneNumber;
            student.ParentPhoneNumber = vm.ParentPhoneNumber;

            // إلغاء تفعيل جميع المجموعات القديمة
            foreach (var sg in student.StudentGroups)
            {
                sg.IsActive = false;
            }

            // التحقق إذا كانت المجموعة الجديدة موجودة مسبقاً
            var existingGroup = student.StudentGroups
                .FirstOrDefault(sg => sg.GroupId == vm.SelectedGroupId);

            if (existingGroup != null)
            {
                // إعادة تفعيل المجموعة الموجودة
                existingGroup.IsActive = true;
            }
            else
            {
                // إضافة مجموعة جديدة
                student.StudentGroups.Add(new StudentGroup
                {
                    GroupId = vm.SelectedGroupId,
                    IsActive = true,
                    EnrollmentDate = DateOnly.FromDateTime(DateTime.Now)
                });
            }

            studentRepository.Update(student);
            await studentRepository.SaveChangesAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }
    }
}
