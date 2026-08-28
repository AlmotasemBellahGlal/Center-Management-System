using Center_Management.Context;
using Center_Management.Interfaces;
using Center_Management.Models;
using Center_Management.View_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Center_Management.Controllers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class AcadimicYearController : Controller
    {
        private readonly IAcadimicYearsRepository acadimicYearsRepository;

        public AcadimicYearController(IAcadimicYearsRepository acadimicYearsRepository)
        {
            this.acadimicYearsRepository = acadimicYearsRepository;
        }

        public async Task<IActionResult> Index()
        {
            var academicYears = await acadimicYearsRepository.GetAllAsync(
                a => a.Groups
            );
            return View(academicYears);
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
                SubjectName = "", // No longer needed
                Groups = academicYear.Groups?.ToList() ?? new()
            };

            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AcademicYear academicYear)
        {
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
            var academicYear = await acadimicYearsRepository.GetByIdAsync(id);
            if (academicYear == null)
            {
                return NotFound();
            }
            return View(academicYear);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AcademicYear academicYear)
        {
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
            return View(await acadimicYearsRepository.GetByIdAsync(id));
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
