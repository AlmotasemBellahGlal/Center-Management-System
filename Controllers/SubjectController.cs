using Center_Management.Context;
using Center_Management.Interfaces;
using Center_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Center_Management.Controllers
{
    public class SubjectController : Controller
    {
       
        private readonly ISubjectRepository repo;

        public SubjectController(ISubjectRepository repo)
        {
           
            this.repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            var subjects = await repo.GetAllAsync();
            return View(subjects);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Create(Subject subject)
        {
            if (ModelState.IsValid)
            {
                repo.Add(subject);
                await repo.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            var subject = await repo.GetByIdAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Subject subject)
        {
            if (id != subject.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                repo.Update(subject);
                await repo.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }
        public async Task<IActionResult> Details(int id)
        {
           var subject = await repo.GetByIdAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var subject =await repo.GetByIdAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id ,Subject subject)
        {
            if (subject!=null)
            {
                repo.Delete(subject);

                await repo.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }



    }
}
