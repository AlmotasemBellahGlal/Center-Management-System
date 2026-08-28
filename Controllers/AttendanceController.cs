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
    public class AttendanceController : Controller
    {
        private readonly IAttendenceRepository _attendenceRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly CenterDBContext _context;

        public AttendanceController(
            IAttendenceRepository attendenceRepo,
            IGroupRepository groupRepo,
            IStudentRepository studentRepo,
            CenterDBContext context)
        {
            _attendenceRepo = attendenceRepo;
            _groupRepo = groupRepo;
            _studentRepo = studentRepo;
            _context = context;
        }

        // GET: Attendance/Groups — group picker page
        public async Task<IActionResult> Groups()
        {
            var groups = await _context.Groups
                .Include(g => g.AcademicYear)
                .Include(g => g.Schedules)
                .Include(g => g.StudentGroups)
                .OrderBy(g => g.AcademicYear!.Name)
                .ThenBy(g => g.Name)
                .ToListAsync();

            return View(groups);
        }

        // GET: Attendance/Create?groupId=5&date=2024-01-15
        public async Task<IActionResult> Create(int groupId, DateTime? date)
        {
            var selectedDate = date?.Date ?? DateTime.Today;

            // جلب المجموعة مع البيانات المطلوبة
            var group = await _context.Groups
                .Include(g => g.Schedules)
                .Include(g => g.AcademicYear)
                     
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
                return NotFound();

            // جلب الطلاب النشطين في المجموعة مرتبين بالاسم
            var activeStudents = await _context.StudentGroups
                .Where(sg => sg.GroupId == groupId && sg.IsActive)
                .Include(sg => sg.Student)
                .Select(sg => sg.Student!)
                .OrderBy(s => s.FullName)
                .ToListAsync();

            // جلب سجلات الحضور الموجودة لهذا التاريخ
            var existingAttendance = await _context.Attendences
                .Where(a => a.GroupId == groupId && a.Date.Date == selectedDate.Date)
                .ToDictionaryAsync(a => a.StudentId, a => a.IsPresent);

            if (!activeStudents.Any())
            {
                ViewBag.Message = "لا يوجد طلاب نشطون في هذه المجموعة";
                return View(new AttendanceFormVM
                {
                    GroupId = groupId,
                    GroupName = group.Name,
                    AcademicYearName = group.AcademicYear?.Name ?? "",
                    SubjectName = "",
                    Date = selectedDate
                });
            }

            var vm = new AttendanceFormVM
            {
                GroupId = groupId,
                GroupName = group.Name,
                AcademicYearName = group.AcademicYear?.Name ?? "",
                SubjectName = "",
                Date = selectedDate,
                Schedule = group.Schedules.Select(s => new GroupScheduleVM
                {
                    Id = s.Id,
                    Day = s.Day,
                    StartTime = s.StartTime
                }).ToList(),
                Students = activeStudents.Select(s => new StudentAttendanceItemVM
                {
                    StudentId = s.Id,
                    FullName = s.FullName,
                    IsPresent = existingAttendance.ContainsKey(s.Id)
                        ? existingAttendance[s.Id]
                        : (bool?)null
                }).ToList()
            };

            return View(vm);
        }

        // POST: Attendance/ToggleAttendance (AJAX)
        [HttpPost]
        public async Task<IActionResult> ToggleAttendance([FromBody] AttendanceToggleRequest req)
        {
            try
            {
                // التحقق من أن الطالب نشط في المجموعة
                var isActive = await _context.StudentGroups
                    .AnyAsync(sg => sg.StudentId == req.StudentId && sg.GroupId == req.GroupId && sg.IsActive);

                if (!isActive)
                    return Ok(new { success = false, message = "الطالب غير نشط في هذه المجموعة" });

                var targetDate = req.Date.Date;

                // البحث عن سجل موجود
                var existing = await _context.Attendences
                    .FirstOrDefaultAsync(a => a.StudentId == req.StudentId
                                           && a.GroupId == req.GroupId
                                           && a.Date.Date == targetDate);

                if (existing != null)
                {
                    // تحديث السجل الموجود
                    existing.IsPresent = req.IsPresent;
                }
                else
                {
                    // إنشاء سجل جديد
                    _context.Attendences.Add(new Attendence
                    {
                        StudentId = req.StudentId,
                        GroupId = req.GroupId,
                        Date = targetDate,
                        IsPresent = req.IsPresent
                    });
                }

                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "حدث خطأ: " + ex.Message });
            }
        }

        // POST: Attendance/BulkAttendance (AJAX)
        [HttpPost]
        public async Task<IActionResult> BulkAttendance([FromBody] BulkAttendanceRequest req)
        {
            try
            {
                var targetDate = req.Date.Date;

                // جلب جميع الطلاب النشطين في المجموعة
                var activeStudentIds = await _context.StudentGroups
                    .Where(sg => sg.GroupId == req.GroupId && sg.IsActive)
                    .Select(sg => sg.StudentId)
                    .ToListAsync();

                foreach (var studentId in activeStudentIds)
                {
                    var existing = await _context.Attendences
                        .FirstOrDefaultAsync(a => a.StudentId == studentId
                                               && a.GroupId == req.GroupId
                                               && a.Date.Date == targetDate);
                    if (existing != null)
                    {
                        existing.IsPresent = req.IsPresent;
                    }
                    else
                    {
                        _context.Attendences.Add(new Attendence
                        {
                            StudentId = studentId,
                            GroupId = req.GroupId,
                            Date = targetDate,
                            IsPresent = req.IsPresent
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "حدث خطأ: " + ex.Message });
            }
        }

        // GET: Attendance/Report?groupId=5
        public async Task<IActionResult> Report(int groupId)
        {
            var group = await _groupRepo.GetByIdAsync(groupId);
            if (group == null)
                return NotFound();

            var attendances = await _attendenceRepo.GetGroupAttendanceAsync(groupId);

            var vm = new AttendanceReportVM
            {
                GroupId = groupId,
                GroupName = group.Name,
                Records = attendances.Select(a => new AttendanceRecordVM
                {
                    StudentFullName = a.Student?.FullName ?? "",
                    Date = a.Date,
                    IsPresent = a.IsPresent
                }).ToList()
            };

            return View(vm);
        }

        // GET: Attendance/StudentReport?studentId=3&groupId=5
        public async Task<IActionResult> StudentReport(int studentId, int groupId)
        {
            var group = await _groupRepo.GetByIdAsync(groupId);
            if (group == null)
                return NotFound();

            var student = await _studentRepo.GetByIdAsync(studentId);
            if (student == null)
                return NotFound();

            var studentAttendances = await _attendenceRepo.GetStudentAttendanceInGroupAsync(studentId, groupId);
            var studentAttendancesList = studentAttendances.ToList();

            var totalDistinctDates = await _context.Attendences
                .Where(a => a.GroupId == groupId)
                .Select(a => a.Date.Date)
                .Distinct()
                .CountAsync();

            var presentCount = studentAttendancesList.Count(a => a.IsPresent);
            var absentCount = studentAttendancesList.Count - presentCount;

            int attendanceRate = totalDistinctDates == 0
                ? 0
                : (int)Math.Round((double)presentCount / totalDistinctDates * 100);

            var vm = new StudentAttendanceReportVM
            {
                StudentId = studentId,
                StudentFullName = student.FullName,
                GroupId = groupId,
                GroupName = group.Name,
                TotalSessions = totalDistinctDates,
                PresentCount = presentCount,
                AbsentCount = absentCount,
                AttendanceRate = attendanceRate,
                Records = studentAttendancesList.Select(a => new AttendanceRecordVM
                {
                    StudentFullName = student.FullName,
                    Date = a.Date,
                    IsPresent = a.IsPresent
                }).ToList()
            };

            return View(vm);
        }
    }

    // Request models for AJAX
    public class AttendanceToggleRequest
    {
        public int StudentId { get; set; }
        public int GroupId { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }

    public class BulkAttendanceRequest
    {
        public int GroupId { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }
}
