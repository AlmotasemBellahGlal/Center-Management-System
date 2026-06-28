using Center_Management.Context;
using Center_Management.Interfaces;
using Center_Management.Models;
using Center_Management.View_Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Center_Management.Controllers
{
    public class AcadimicYearController : Controller
    {

        private readonly IAcadimicYearsRepository acadimicYearsRepository;
        private readonly ISubjectRepository subjectRepository;

        public AcadimicYearController(IAcadimicYearsRepository acadimicYearsRepository,ISubjectRepository subjectRepository)
        {
            this.acadimicYearsRepository = acadimicYearsRepository;
            this.subjectRepository = subjectRepository;
        }
        public async Task< IActionResult> Index()
        {
            return View(await acadimicYearsRepository.GetAllAsync(a => a.Subject));
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var academicYear = await acadimicYearsRepository.GetDetailsAsync(id);

            if (academicYear == null)
                return NotFound();

            var vm = new AcademicYearDetailsVM
            {
                Id = academicYear.Id,
                Name = academicYear.Name,
                MonthlyPrice = academicYear.MonthlyPrice,
                SubjectName = academicYear.Subject?.Name ?? "",
                Groups = academicYear.Groups?.ToList() ?? new()
            };

            return View(vm);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.SubjectList=new SelectList(await subjectRepository.GetAllAsync(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AcademicYear academicYear)
        {
            ViewBag.SubjectList = new SelectList(await subjectRepository.GetAllAsync(), "Id", "Name");
            if (ModelState.IsValid)
            {
                acadimicYearsRepository.Add(academicYear);
                await acadimicYearsRepository.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(academicYear);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.SubjectList = new SelectList(await subjectRepository.GetAllAsync(), "Id", "Name");
            var academicYear =await acadimicYearsRepository.GetByIdAsync(id);
            if (academicYear == null)
            {
                return NotFound();
            }
            return View(academicYear);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, AcademicYear academicYear)
        {
            ViewBag.SubjectList = new SelectList(await subjectRepository.GetAllAsync(), "Id", "Name");
            if (id != academicYear.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                acadimicYearsRepository.Update(academicYear);
                await acadimicYearsRepository.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(academicYear);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return View(await acadimicYearsRepository.GetByIdAsync(id,s=>s.Subject));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id, AcademicYear academicYear)
        {
            if (id != academicYear.Id)
            {
                return NotFound();
            }
            
                acadimicYearsRepository.Delete(academicYear);
                await acadimicYearsRepository.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
        }
    }
}
