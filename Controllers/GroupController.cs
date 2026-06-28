using Center_Management.Context;
using Center_Management.Interfaces;
using Center_Management.Models;
using Center_Management.Repositories;
using Center_Management.View_Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Center_Management.Controllers
{
    public class GroupController : Controller
    {
        private readonly IGroupRepository grouprepo;
        private readonly IAcadimicYearsRepository acadimicYearsRepository;

        public GroupController(IGroupRepository grouprepo, IAcadimicYearsRepository acadimicYearsRepository)
        {
            this.grouprepo = grouprepo;
            this.acadimicYearsRepository = acadimicYearsRepository;
        }
        // GET: GroupController
        public async Task<ActionResult> Index()
        {
            return View(await grouprepo.GetAllAsync(g => g.AcademicYear, g => g.Schedules));
        }

        // GET: GroupController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var group = await grouprepo.GetByIdAsync(
                id,
                g => g.AcademicYear,
                g => g.Schedules
            );

            if (group == null)
                return NotFound();

            return View(group);
        }

        // GET: GroupController/Create
        [HttpGet]
        public IActionResult Create(int academicYearId)
        {
            var vm = new CreateGroupVM
            {
                AcademicYearId = academicYearId
            };

            vm.Schedules.Add(new GroupScheduleVM());

            return View(vm);
        }
        // POST: GroupController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateGroupVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            Models.Group group = new()
            {
                Name = vm.Name,
                AcademicYearId = vm.AcademicYearId,
                Schedules = vm.Schedules.Select(s => new GroupSchedule
                {
                    Day = s.Day,
                    StartTime = s.StartTime
                }).ToList()
            };
            var duplicatedDays = vm.Schedules
    .GroupBy(s => s.Day)
    .Where(g => g.Count() > 1)
    .Select(g => g.Key)
    .ToList();

            if (duplicatedDays.Any())
            {
                ModelState.AddModelError(
                    "",
                    "لا يمكن اختيار نفس اليوم أكثر من مرة."
                );

                return View(vm);
            }
            grouprepo.Add(group);

            await grouprepo.SaveChangesAsync();

            return RedirectToAction(
                "Details",
                "AcadimicYear",
                new { id = vm.AcademicYearId });
        }

        // GET: GroupController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var group = await grouprepo.GetByIdAsync(id, g => g.Schedules);

            if (group == null)
                return NotFound();

            var vm = new CreateGroupVM

            {
                Id = group.Id,
                Name = group.Name,
                AcademicYearId = group.AcademicYearId,

                Schedules = group.Schedules.Select(s => new GroupScheduleVM
                {
                    Id = s.Id,
                    Day = s.Day,
                    StartTime = s.StartTime
                }).ToList()
            };

           

            return View(vm);

        }

        // POST: GroupController/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(CreateGroupVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);
            var duplicatedDays = vm.Schedules
    .GroupBy(s => s.Day)
    .Where(g => g.Count() > 1)
    .Select(g => g.Key)
    .ToList();

            if (duplicatedDays.Any())
            {
                ModelState.AddModelError(
                    "",
                    "لا يمكن اختيار نفس اليوم أكثر من مرة."
                );

                return View(vm);
            }
            await grouprepo.UpdateGroupAsync(vm);

            return RedirectToAction(
                "Details",
                "AcadimicYear",
                new { id = vm.AcademicYearId });
        }

        // GET: GroupController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var group = await grouprepo.GetByIdAsync(
                id,
                g => g.AcademicYear,
                g => g.Schedules
            );

            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        // POST: GroupController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await grouprepo.GetByIdAsync(id);

            if (group == null)
            {
                return NotFound();
            }

            int academicYearId = group.AcademicYearId;

            grouprepo.Delete(group);

            await grouprepo.SaveChangesAsync();

            return RedirectToAction(
                "Details",
                "AcadimicYear",
                new { id = academicYearId });
        }
    }
    }
