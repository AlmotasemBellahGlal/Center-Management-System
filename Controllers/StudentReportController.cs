using Center_Management.Context;
using Center_Management.Interfaces;
using Center_Management.View_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Center_Management.Controllers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class StudentReportController : Controller
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IAttendenceRepository _attendenceRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly CenterDBContext _context;

        public StudentReportController(
            IStudentRepository studentRepo,
            IAttendenceRepository attendenceRepo,
            IPaymentRepository paymentRepo,
            CenterDBContext context)
        {
            _studentRepo = studentRepo;
            _attendenceRepo = attendenceRepo;
            _paymentRepo = paymentRepo;
            _context = context;
        }

        // GET: StudentReport/Index?id=5
        public async Task<IActionResult> Index(int id)
        {
            // جلب بيانات الطالب الأساسية
            var student = await _studentRepo.GetByIdAsync(id);
            
            if (student == null)
            {
                return NotFound();
            }

            // جلب جميع سجلات StudentGroup للطالب مع المجموعات والسنوات الدراسية
            var studentGroups = await _context.StudentGroups
                .Where(sg => sg.StudentId == id)
                .Include(sg => sg.Group)
                    .ThenInclude(g => g.AcademicYear)
                .ToListAsync();

            // بناء قائمة المجموعات
            var groups = studentGroups.Select(sg => new StudentGroupInfoVM
            {
                GroupName = sg.Group?.Name ?? "غير محدد",
                AcademicYearName = sg.Group?.AcademicYear?.Name ?? "غير محدد",
                EnrollmentDate = sg.EnrollmentDate,
                IsActive = sg.IsActive
            }).ToList();

            // جلب جميع سجلات الحضور للطالب عبر جميع المجموعات
            var attendances = await _context.Attendences
                .Where(a => a.StudentId == id)
                .ToListAsync();

            // حساب ملخص الحضور
            int totalAttendances = attendances.Count;
            int presentCount = attendances.Count(a => a.IsPresent);
            int absentCount = totalAttendances - presentCount;
            int attendanceRate = totalAttendances == 0 
                ? 0 
                : (int)Math.Round((double)presentCount / totalAttendances * 100);

            // جلب جميع المدفوعات للطالب
            var payments = await _paymentRepo.GetStudentPaymentsAsync(id);
            var paymentRecords = payments.Select(p => new PaymentRecordVM
            {
                GroupName = p.Group?.Name ?? "غير محدد",
                Month = p.Month,
                Year = p.Year,
                Amount = p.Amount,
                IsPaid = p.IsPaid,
                PaymentDate = p.PaymentDate
            }).ToList();

            // بناء ViewModel
            var vm = new StudentReportVM
            {
                StudentId = id,
                FullName = student.FullName,
                PhoneNumber = student.PhoneNumber,
                ParentPhoneNumber = student.ParentPhoneNumber,
                Groups = groups,
                TotalAttendanceSessions = totalAttendances,
                TotalPresent = presentCount,
                TotalAbsent = absentCount,
                OverallAttendanceRate = attendanceRate,
                Payments = paymentRecords
            };

            return View(vm);
        }
    }
}
