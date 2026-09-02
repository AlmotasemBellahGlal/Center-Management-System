using Center_Management.Context;
using Center_Management.View_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Center_Management.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly CenterDBContext _context;

        public DashboardController(CenterDBContext context)
        {
            _context = context;
        }

        // GET: Dashboard/Index
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var currentMonth = now.Month;
            var currentYear = now.Year;

            // 1. عدد الطلاب النشطين
            int activeStudentsCount = await _context.StudentGroups
                .Where(sg => sg.IsActive)
                .Select(sg => sg.StudentId)
                .Distinct()
                .CountAsync(cancellationToken);

            // 2. عدد الطلاب غير المدفوعين للشهر الحالي
            // جلب IDs الطلاب المدفوعين
            var paidStudentIds = await _context.Payments
                .Where(p => p.Month == currentMonth && p.Year == currentYear && p.IsPaid)
                .Select(p => p.StudentId)
                .Distinct()
                .ToListAsync(cancellationToken);

            // حساب الطلاب النشطين غير المدفوعين
            int unpaidCount = await _context.StudentGroups
                .Where(sg => sg.IsActive && !paidStudentIds.Contains(sg.StudentId))
                .Select(sg => sg.StudentId)
                .Distinct()
                .CountAsync(cancellationToken);

            // 3. إيراد الشهر الحالي
            decimal monthlyRevenue = await _context.Payments
                .Where(p => p.Month == currentMonth && p.Year == currentYear && p.IsPaid)
                .SumAsync(p => p.Amount, cancellationToken);

            // 4. بيانات آخر 12 شهر للرسم البياني
            var monthlyEnrollments = await GenerateMonthlyEnrollments(cancellationToken);

            // 5. حساب نسبة النمو
            var currentMonthEnrollments = monthlyEnrollments
                .FirstOrDefault(m => m.Year == currentYear && m.Month == currentMonth)?.Count ?? 0;
            
            var previousMonth = now.AddMonths(-1);
            var previousMonthEnrollments = monthlyEnrollments
                .FirstOrDefault(m => m.Year == previousMonth.Year && m.Month == previousMonth.Month)?.Count ?? 0;
            
            string growthRate = CalculateGrowthRate(currentMonthEnrollments, previousMonthEnrollments);

            // بناء ViewModel
            var vm = new DashboardVM
            {
                ActiveStudentsCount = activeStudentsCount,
                UnpaidStudentsCurrentMonth = unpaidCount,
                GrowthRate = growthRate,
                MonthlyEnrollments = monthlyEnrollments,
                MonthlyRevenue = monthlyRevenue
            };

            return View(vm);
        }

        /// <summary>
        /// توليد بيانات التسجيلات الشهرية لآخر 12 شهر
        /// </summary>
        private async Task<List<MonthlyEnrollmentData>> GenerateMonthlyEnrollments(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var monthlyData = new List<MonthlyEnrollmentData>();

            // توليد قائمة 12 شهر من الشهر الحالي إلى 11 شهراً سابقاً
            for (int i = 0; i < 12; i++)
            {
                var targetMonth = now.AddMonths(-i);
                var year = targetMonth.Year;
                var month = targetMonth.Month;

                // حساب عدد التسجيلات الجديدة في هذا الشهر
                var count = await _context.StudentGroups
                    .Where(sg => sg.EnrollmentDate.Year == year && sg.EnrollmentDate.Month == month)
                    .CountAsync(cancellationToken);

                monthlyData.Add(new MonthlyEnrollmentData
                {
                    Year = year,
                    Month = month,
                    Count = count
                });
            }

            // عكس الترتيب ليكون من الأقدم للأحدث (للرسم البياني)
            monthlyData.Reverse();

            return monthlyData;
        }

        /// <summary>
        /// حساب نسبة النمو بين الشهر الحالي والسابق
        /// </summary>
        private string CalculateGrowthRate(int current, int previous)
        {
            if (previous == 0 && current > 0)
            {
                return "جديد";
            }
            else if (previous == 0 && current == 0)
            {
                return "0%";
            }
            else
            {
                double rate = ((current - previous) * 100.0) / previous;
                return $"{rate:+0.#;-0.#;0}%";
            }
        }
    }
}
