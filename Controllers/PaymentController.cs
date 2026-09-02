using Center_Management.Context;
using Center_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Center_Management.Controllers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class PaymentController : Controller
    {
        private readonly CenterDBContext _context;

        public PaymentController(CenterDBContext context)
        {
            _context = context;
        }

        // GET: Payment/Groups — group picker page
        public async Task<IActionResult> Groups(CancellationToken cancellationToken)
        {
            var groups = await _context.Groups
                .Include(g => g.AcademicYear)
                     
                .Include(g => g.Schedules)
                .Include(g => g.StudentGroups)
                .OrderBy(g => g.AcademicYear!.Name)
                .ThenBy(g => g.Name)
                .ToListAsync(cancellationToken);

            return View(groups);
        }

        // GET: Payment/Index?groupId=5&month=6&year=2025
        public async Task<IActionResult> Index(int groupId, int? month = null, int? year = null, CancellationToken cancellationToken = default)
        {
            var selectedMonth = month ?? DateTime.Now.Month;
            var selectedYear = year ?? DateTime.Now.Year;

            // جلب المجموعة
            var group = await _context.Groups
                .Include(g => g.AcademicYear)
                     
                .FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);

            if (group == null)
                return NotFound();

            // جلب الطلاب النشطين
            var students = await _context.StudentGroups
                .Where(sg => sg.GroupId == groupId && sg.IsActive)
                .Include(sg => sg.Student)
                .Select(sg => sg.Student!)
                .OrderBy(s => s.FullName)
                .ToListAsync(cancellationToken);

            // جلب المدفوعات الموجودة
            var payments = await _context.Payments
                .Where(p => p.GroupId == groupId && p.Month == selectedMonth && p.Year == selectedYear)
                .ToDictionaryAsync(p => p.StudentId, p => p.IsPaid, cancellationToken);

            var monthlyPrice = group.AcademicYear?.MonthlyPrice ?? 0;

            var vm = new PaymentGroupVM
            {
                GroupId = groupId,
                GroupName = group.Name,
                AcademicYearName = group.AcademicYear?.Name ?? "",
                SubjectName = "",
                Month = selectedMonth,
                Year = selectedYear,
                MonthlyPrice = monthlyPrice,
                Students = students.Select(s => new StudentPaymentItemVM
                {
                    StudentId = s.Id,
                    FullName = s.FullName,
                    IsPaid = payments.ContainsKey(s.Id) && payments[s.Id],
                    Amount = monthlyPrice
                }).ToList()
            };

            return View(vm);
        }

        // POST: Payment/TogglePayment - AJAX
        [HttpPost]
        public async Task<IActionResult> TogglePayment([FromBody] PaymentToggleRequest req, CancellationToken cancellationToken)
        {
            try
            {
                var isActive = await _context.StudentGroups
                    .AnyAsync(sg => sg.StudentId == req.StudentId && sg.GroupId == req.GroupId && sg.IsActive, cancellationToken);

                if (!isActive)
                    return Ok(new { success = false, message = "الطالب غير نشط في هذه المجموعة" });

                var existing = await _context.Payments
                    .FirstOrDefaultAsync(p => p.StudentId == req.StudentId
                                           && p.GroupId == req.GroupId
                                           && p.Month == req.Month
                                           && p.Year == req.Year, cancellationToken);

                if (existing != null)
                {
                    existing.IsPaid = req.MarkPaid;
                    existing.PaymentDate = req.MarkPaid ? DateTime.Now : DateTime.MinValue;
                }
                else
                {
                    _context.Payments.Add(new Payment
                    {
                        StudentId = req.StudentId,
                        GroupId = req.GroupId,
                        Month = req.Month,
                        Year = req.Year,
                        Amount = req.Amount,
                        IsPaid = req.MarkPaid,
                        PaymentDate = req.MarkPaid ? DateTime.Now : DateTime.MinValue
                    });
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Ok(new { success = true });
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                return Ok(new { success = false, message = "حدث خطأ: " + ex.Message });
            }
        }

        // POST: Payment/BulkPayment - AJAX
        [HttpPost]
        public async Task<IActionResult> BulkPayment([FromBody] BulkPaymentRequest req, CancellationToken cancellationToken)
        {
            try
            {
                var activeStudentIds = await _context.StudentGroups
                    .Where(sg => sg.GroupId == req.GroupId && sg.IsActive)
                    .Select(sg => sg.StudentId)
                    .ToListAsync(cancellationToken);

                foreach (var studentId in activeStudentIds)
                {
                    var existing = await _context.Payments
                        .FirstOrDefaultAsync(p => p.StudentId == studentId
                                               && p.GroupId == req.GroupId
                                               && p.Month == req.Month
                                               && p.Year == req.Year, cancellationToken);

                    if (existing != null)
                    {
                        existing.IsPaid = req.MarkPaid;
                        existing.PaymentDate = req.MarkPaid ? DateTime.Now : DateTime.MinValue;
                    }
                    else
                    {
                        _context.Payments.Add(new Payment
                        {
                            StudentId = studentId,
                            GroupId = req.GroupId,
                            Month = req.Month,
                            Year = req.Year,
                            Amount = req.Amount,
                            IsPaid = req.MarkPaid,
                            PaymentDate = req.MarkPaid ? DateTime.Now : DateTime.MinValue
                        });
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Ok(new { success = true });
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                return Ok(new { success = false, message = "حدث خطأ: " + ex.Message });
            }
        }
    }

    // Request models
    public class PaymentToggleRequest
    {
        public int StudentId { get; set; }
        public int GroupId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public bool MarkPaid { get; set; }
    }

    public class BulkPaymentRequest
    {
        public int GroupId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public bool MarkPaid { get; set; }
    }

    // ViewModels
    public class PaymentGroupVM
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = "";
        public string AcademicYearName { get; set; } = "";
        public string SubjectName { get; set; } = "";
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal MonthlyPrice { get; set; }
        public List<StudentPaymentItemVM> Students { get; set; } = new();
    }

    public class StudentPaymentItemVM
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = "";
        public bool IsPaid { get; set; }
        public decimal Amount { get; set; }
    }
}
