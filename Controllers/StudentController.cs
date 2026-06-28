using Center_Management.Interfaces;
using Center_Management.Models;
using Center_Management.View_Models;
using Microsoft.AspNetCore.Mvc;

namespace Center_Management.Controllers
{
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
        public async Task<IActionResult> Index()
        {
            var students = await studentRepository.GetAllWithGroupsAsync();

            return View(students);
        }
        public async Task<IActionResult> Details(int id)
        {
            var student = await studentRepository.GetDetailsAsync(id);

            if (student == null)
                return NotFound();

            return View(student);
        }
        private async Task LoadGroups(CreateStudentVM vm)
        {
            var groups = await groupRepository
                .GetGroupsWithAcademicYearAndSubjectAsync();

            vm.AcademicYears = groups
                .GroupBy(g => g.AcademicYearId)
                .Select(g => new AcademicYearGroupsVM
                {
                    AcademicYearId = g.Key,
                    AcademicYearName = g.First().AcademicYear!.Name,
                    SubjectName = g.First().AcademicYear!.Subject!.Name,

                    Groups = g.Select(x => new GroupSelectionVM
                    {
                        GroupId = x.Id,
                        GroupName = x.Name,

                       
                    }).ToList()

                }).ToList();
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateStudentVM vm = new();

            await LoadGroups(vm);

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStudentVM vm)
        {
            foreach (var item in ModelState)
            {
                foreach (var error in item.Value.Errors)
                {
                    Console.WriteLine($"{item.Key} ---> {error.ErrorMessage}");
                }
            }
            if (!ModelState.IsValid)
            {
                await LoadGroups(vm);
                return View(vm);
            }

            bool result = await studentRepository.RegisterStudentAsync(vm);

            if (!result)
            {
                ModelState.AddModelError("", "لا يمكن تسجيل الطالب في أكثر من مجموعة لنفس السنة الدراسية.");

                await LoadGroups(vm);

                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await studentRepository.GetForEditAsync(id);

            if (student == null)
                return NotFound();

            CreateStudentVM vm = new()
            {
                Id = student.Id,
                FullName = student.FullName,
                PhoneNumber = student.PhoneNumber,
                ParentPhoneNumber = student.ParentPhoneNumber
            };

            await LoadGroups(vm);

            foreach (var sg in student.StudentGroups)
            {
                var academicYear = vm.AcademicYears
                    .FirstOrDefault(a => a.AcademicYearId == sg.Group.AcademicYearId);

                if (academicYear != null)
                {
                    academicYear.SelectedGroupId = sg.GroupId;
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateStudentVM vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadGroups(vm);
                return View(vm);
            }

            bool result = await studentRepository.UpdateStudentAsync(vm);

            if (!result)
            {
                ModelState.AddModelError("", "لا يمكن تسجيل الطالب في أكثر من مجموعة لنفس المادة والسنة الدراسية.");

                await LoadGroups(vm);

                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
