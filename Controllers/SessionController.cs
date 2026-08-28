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
    public class SessionController : Controller
    {
        private readonly CenterDBContext _db;
        private readonly IGroupRepository _groupRepo;

        public SessionController(CenterDBContext db, IGroupRepository groupRepo)
        {
            _db = db;
            _groupRepo = groupRepo;
        }

        // ─────────────────────────────────────────────────────────────────
        // GET: /Session/Index?groupId=5&date=2026-08-16&month=8&year=2026
        // ─────────────────────────────────────────────────────────────────
        public async Task<IActionResult> Index(int groupId, DateTime? date, int? month, int? year)
        {
            var group = await _db.Groups
                .Include(g => g.AcademicYear)
                     
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null) return NotFound();

            DateTime sessionDate = date?.Date ?? DateTime.Today;
            int selectedMonth    = month ?? DateTime.Now.Month;
            int selectedYear     = year  ?? DateTime.Now.Year;

            // Active students in this group
            var activeStudents = await _db.StudentGroups
                .Where(sg => sg.GroupId == groupId && sg.IsActive)
                .Include(sg => sg.Student)
                .OrderBy(sg => sg.Student.FullName)
                .Select(sg => sg.Student)
                .ToListAsync();

            // Existing attendance records for this group + date
            var attendanceRecords = await _db.Attendences
                .Where(a => a.GroupId == groupId && a.Date.Date == sessionDate.Date)
                .ToListAsync();

            // Existing payment records for this group + month/year
            var paymentRecords = await _db.Payments
                .Where(p => p.GroupId == groupId && p.Month == selectedMonth && p.Year == selectedYear)
                .ToListAsync();

            bool attendanceAlreadySaved = attendanceRecords.Count > 0;

            var vm = new GroupSessionVM
            {
                GroupId              = groupId,
                GroupName            = group.Name,
                AcademicYearName     = group.AcademicYear?.Name ?? "",
                SubjectName          = "",
                MonthlyPrice         = group.AcademicYear?.MonthlyPrice ?? 0,
                SessionDate          = sessionDate,
                Month                = selectedMonth,
                Year                 = selectedYear,
                AttendanceAlreadySaved = attendanceAlreadySaved,
                Students = activeStudents.Select(s =>
                {
                    var att = attendanceRecords.FirstOrDefault(a => a.StudentId == s.Id);
                    var pay = paymentRecords.FirstOrDefault(p => p.StudentId == s.Id);
                    return new StudentSessionItemVM
                    {
                        StudentId    = s.Id,
                        FullName     = s.FullName,
                        IsPresent    = att != null ? att.IsPresent : (bool?)null,
                        AttendanceId = att?.Id,
                        IsPaid       = pay != null,
                        PaymentId    = pay?.Id,
                        MonthlyPrice = group.AcademicYear?.MonthlyPrice ?? 0
                    };
                }).ToList()
            };

            return View(vm);
        }

        // ─────────────────────────────────────────────────────────────────
        // POST AJAX: /Session/ToggleAttendance
        // Body: { studentId, groupId, date, isPresent }
        // ─────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> ToggleAttendance(
            [FromBody] ToggleAttendanceRequest req)
        {
            if (req == null)
                return BadRequest(new { success = false, message = "طلب غير صالح" });

            try
            {
                // Validate student is active in group
                bool isActive = await _db.StudentGroups
                    .AnyAsync(sg => sg.StudentId == req.StudentId
                                 && sg.GroupId   == req.GroupId
                                 && sg.IsActive);

                if (!isActive)
                    return BadRequest(new { success = false, message = "الطالب غير نشط في هذه المجموعة" });

                var existing = await _db.Attendences
                    .FirstOrDefaultAsync(a => a.StudentId == req.StudentId
                                           && a.GroupId   == req.GroupId
                                           && a.Date.Date == req.Date.Date);

                if (existing != null)
                {
                    // Update existing record
                    existing.IsPresent = req.IsPresent;
                }
                else
                {
                    // Create new record
                    _db.Attendences.Add(new Attendence
                    {
                        StudentId = req.StudentId,
                        GroupId   = req.GroupId,
                        Date      = req.Date,
                        IsPresent = req.IsPresent
                    });
                }

                await _db.SaveChangesAsync();

                // Re-fetch to get the Id
                var saved = await _db.Attendences
                    .FirstOrDefaultAsync(a => a.StudentId == req.StudentId
                                           && a.GroupId   == req.GroupId
                                           && a.Date.Date == req.Date.Date);

                return Ok(new
                {
                    success      = true,
                    attendanceId = saved?.Id,
                    isPresent    = req.IsPresent
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // POST AJAX: /Session/TogglePayment
        // Body: { studentId, groupId, month, year, markPaid }
        // ─────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> TogglePayment(
            [FromBody] TogglePaymentRequest req)
        {
            if (req == null)
                return BadRequest(new { success = false, message = "طلب غير صالح" });

            try
            {
                // Validate student is active in group
                bool isActive = await _db.StudentGroups
                    .AnyAsync(sg => sg.StudentId == req.StudentId
                                 && sg.GroupId   == req.GroupId
                                 && sg.IsActive);

                if (!isActive)
                    return BadRequest(new { success = false, message = "الطالب غير نشط في هذه المجموعة" });

                var existing = await _db.Payments
                    .FirstOrDefaultAsync(p => p.StudentId == req.StudentId
                                           && p.GroupId   == req.GroupId
                                           && p.Month     == req.Month
                                           && p.Year      == req.Year);

                if (req.MarkPaid)
                {
                    if (existing == null)
                    {
                        // Fetch price from academic year
                        var group = await _db.Groups
                            .Include(g => g.AcademicYear)
                            .FirstOrDefaultAsync(g => g.Id == req.GroupId);

                        decimal price = group?.AcademicYear?.MonthlyPrice ?? 0;

                        _db.Payments.Add(new Payment
                        {
                            StudentId   = req.StudentId,
                            GroupId     = req.GroupId,
                            Month       = req.Month,
                            Year        = req.Year,
                            Amount      = price,
                            IsPaid      = true,
                            PaymentDate = DateTime.Now
                        });
                    }
                    else
                    {
                        existing.IsPaid      = true;
                        existing.PaymentDate = DateTime.Now;
                    }
                }
                else
                {
                    // Mark as unpaid = remove the payment record
                    if (existing != null)
                        _db.Payments.Remove(existing);
                }

                await _db.SaveChangesAsync();

                int? paymentId = null;
                if (req.MarkPaid)
                {
                    var saved = await _db.Payments
                        .FirstOrDefaultAsync(p => p.StudentId == req.StudentId
                                               && p.GroupId   == req.GroupId
                                               && p.Month     == req.Month
                                               && p.Year      == req.Year);
                    paymentId = saved?.Id;
                }

                return Ok(new
                {
                    success   = true,
                    paymentId = paymentId,
                    isPaid    = req.MarkPaid
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // POST AJAX: /Session/SaveAllAttendance
        // Bulk-save attendance for all students at once
        // ─────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> SaveAllAttendance(
            [FromBody] SaveAllAttendanceRequest req)
        {
            if (req == null || req.Records == null)
                return BadRequest(new { success = false, message = "طلب غير صالح" });

            try
            {
                foreach (var record in req.Records)
                {
                    var existing = await _db.Attendences
                        .FirstOrDefaultAsync(a => a.StudentId == record.StudentId
                                               && a.GroupId   == req.GroupId
                                               && a.Date.Date == req.Date.Date);
                    if (existing != null)
                    {
                        existing.IsPresent = record.IsPresent;
                    }
                    else
                    {
                        _db.Attendences.Add(new Attendence
                        {
                            StudentId = record.StudentId,
                            GroupId   = req.GroupId,
                            Date      = req.Date,
                            IsPresent = record.IsPresent
                        });
                    }
                }

                await _db.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    // ── Request DTOs ─────────────────────────────────────────────────────

    public class ToggleAttendanceRequest
    {
        public int StudentId { get; set; }
        public int GroupId   { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }

    public class TogglePaymentRequest
    {
        public int StudentId { get; set; }
        public int GroupId   { get; set; }
        public int Month     { get; set; }
        public int Year      { get; set; }
        public bool MarkPaid { get; set; }
    }

    public class SaveAllAttendanceRequest
    {
        public int GroupId   { get; set; }
        public DateTime Date { get; set; }
        public List<AttendanceBulkItem> Records { get; set; } = new();
    }

    public class AttendanceBulkItem
    {
        public int StudentId  { get; set; }
        public bool IsPresent { get; set; }
    }
}
