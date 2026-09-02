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

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var academicYears = await acadimicYearsRepository.GetAllAsync(
                cancellationToken,
                a => a.Groups
            );
            return View(academicYears);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var academicYear = await acadimicYearsRepository.GetDetailsAsync(id, cancellationToken);

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
        public async Task<IActionResult> Create(AcademicYear academicYear, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                acadimicYearsRepository.Add(academicYear);
                await acadimicYearsRepository.SaveChangesAsync(cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(academicYear);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var academicYear = await acadimicYearsRepository.GetByIdAsync(id, cancellationToken);
            if (academicYear == null)
            {
                return NotFound();
            }
            return View(academicYear);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AcademicYear academicYear, CancellationToken cancellationToken)
        {
            if (id != academicYear.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                acadimicYearsRepository.Update(academicYear);
                await acadimicYearsRepository.SaveChangesAsync(cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(academicYear);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            return View(await acadimicYearsRepository.GetByIdAsync(id, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, AcademicYear academicYear, CancellationToken cancellationToken)
        {
            if (id != academicYear.Id)
            {
                return NotFound();
            }

            acadimicYearsRepository.Delete(academicYear);
            await acadimicYearsRepository.SaveChangesAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
