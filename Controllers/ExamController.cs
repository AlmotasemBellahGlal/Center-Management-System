using Center_Management.Context;
using Center_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Center_Management.Controllers
{
    [Authorize]
    public class ExamController : Controller
    {
        private readonly CenterDBContext _context;
        private readonly IWebHostEnvironment _env;

        public ExamController(CenterDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Exam/GroupPicker - للمعلم لاختيار المجموعة
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> GroupPicker(CancellationToken cancellationToken)
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

        // GET: Exam/Index?groupId=5 - قائمة الاختبارات للمجموعة
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Index(int groupId, CancellationToken cancellationToken)
        {
            var group = await _context.Groups
                .Include(g => g.AcademicYear)
                .FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);

            if (group == null)
                return NotFound();

            var exams = await _context.Exams
                .Where(e => e.GroupId == groupId)
                .Include(e => e.Questions)
                .Include(e => e.Attempts)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync(cancellationToken);

            ViewBag.Group = group;
            return View(exams);
        }

        // GET: Exam/Create?groupId=5
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Create(int groupId, CancellationToken cancellationToken)
        {
            var group = await _context.Groups
                .Include(g => g.AcademicYear)
                .FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);

            if (group == null)
                return NotFound();

            ViewBag.Group = group;
            return View();
        }

        // POST: Exam/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Create(int groupId, Exam exam, CancellationToken cancellationToken)
        {
            exam.GroupId = groupId;

            // إزالة navigation properties من التحقق
            ModelState.Remove(nameof(exam.Group));
            ModelState.Remove(nameof(exam.Questions));
            ModelState.Remove(nameof(exam.Attempts));

            if (!ModelState.IsValid)
            {
                var group = await _context.Groups
                    .Include(g => g.AcademicYear)
                    .FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);
                ViewBag.Group = group;
                return View(exam);
            }

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync(cancellationToken);

            TempData["Success"] = "تم إنشاء الاختبار بنجاح";
            return RedirectToAction(nameof(Details), new { id = exam.Id });
        }

        // GET: Exam/Details/5
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var exam = await _context.Exams
                .Include(e => e.Group)
                    .ThenInclude(g => g!.AcademicYear)
                .Include(e => e.Questions.OrderBy(q => q.Order))
                .Include(e => e.Attempts)
                    .ThenInclude(a => a.Student)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (exam == null)
                return NotFound();

            return View(exam);
        }

        // GET: Exam/AddQuestion?examId=5
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> AddQuestion(int examId, CancellationToken cancellationToken)
        {
            var exam = await _context.Exams
                .Include(e => e.Group)
                    .ThenInclude(g => g!.AcademicYear)
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId, cancellationToken);

            if (exam == null)
                return NotFound();

            ViewBag.Exam = exam;
            ViewBag.NextOrder = exam.Questions.Any() ? exam.Questions.Max(q => q.Order) + 1 : 1;

            return View(new ExamQuestion { ExamId = examId });
        }

        // POST: Exam/AddQuestion
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> AddQuestion(ExamQuestion question, IFormFile? QuestionImage, CancellationToken cancellationToken)
        {
            // إزالة navigation properties من التحقق
            ModelState.Remove(nameof(question.Exam));
            ModelState.Remove(nameof(question.StudentAnswers));
            ModelState.Remove(nameof(question.ImagePath));

            if (!ModelState.IsValid)
            {
                var examForView = await _context.Exams
                    .Include(e => e.Group)
                        .ThenInclude(g => g!.AcademicYear)
                    .Include(e => e.Questions)
                    .FirstOrDefaultAsync(e => e.Id == question.ExamId, cancellationToken);
                ViewBag.Exam = examForView;
                ViewBag.NextOrder = examForView!.Questions.Any() ? examForView.Questions.Max(q => q.Order) + 1 : 1;
                return View(question);
            }

            // رفع الصورة إن وُجدت
            if (QuestionImage != null && QuestionImage.Length > 0)
            {
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                if (!allowedTypes.Contains(QuestionImage.ContentType))
                {
                    ModelState.AddModelError("QuestionImage", "يُسمح فقط بصور JPG, PNG, GIF, WEBP");
                    var examForView = await _context.Exams
                        .Include(e => e.Group).ThenInclude(g => g!.AcademicYear)
                        .Include(e => e.Questions)
                        .FirstOrDefaultAsync(e => e.Id == question.ExamId, cancellationToken);
                    ViewBag.Exam = examForView;
                    ViewBag.NextOrder = examForView!.Questions.Any() ? examForView.Questions.Max(q => q.Order) + 1 : 1;
                    return View(question);
                }

                var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "questions");
                Directory.CreateDirectory(uploadDir);

                var ext = Path.GetExtension(QuestionImage.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await QuestionImage.CopyToAsync(stream, cancellationToken);

                question.ImagePath = $"/uploads/questions/{fileName}";
            }

            _context.ExamQuestions.Add(question);
            await _context.SaveChangesAsync(cancellationToken);

            TempData["Success"] = "تم إضافة السؤال بنجاح";
            return RedirectToAction(nameof(Details), new { id = question.ExamId });
        }

        // GET: Exam/Delete/5
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var exam = await _context.Exams
                .Include(e => e.Group)
                    .ThenInclude(g => g!.AcademicYear)
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (exam == null)
                return NotFound();

            return View(exam);
        }

        // POST: Exam/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            var exam = await _context.Exams.FindAsync(new object[] { id }, cancellationToken);
            if (exam == null)
                return NotFound();

            var groupId = exam.GroupId;
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync(cancellationToken);

            TempData["Success"] = "تم حذف الاختبار بنجاح";
            return RedirectToAction(nameof(Index), new { groupId });
        }

        // GET: Exam/Edit/5 - تعديل معلومات الامتحان (الوقت والتاريخ)
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var exam = await _context.Exams
                .Include(e => e.Group)
                    .ThenInclude(g => g!.AcademicYear)
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (exam == null)
                return NotFound();

            return View(exam);
        }

        // POST: Exam/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Edit(int id, Exam updatedExam, CancellationToken cancellationToken)
        {
            if (id != updatedExam.Id)
                return NotFound();

            var exam = await _context.Exams.FindAsync(new object[] { id }, cancellationToken);
            if (exam == null)
                return NotFound();

            // إزالة navigation properties من التحقق
            ModelState.Remove(nameof(updatedExam.Group));
            ModelState.Remove(nameof(updatedExam.Questions));
            ModelState.Remove(nameof(updatedExam.Attempts));

            if (!ModelState.IsValid)
            {
                var examForView = await _context.Exams
                    .Include(e => e.Group)
                        .ThenInclude(g => g!.AcademicYear)
                    .Include(e => e.Questions)
                    .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
                return View(examForView);
            }

            // تحديث البيانات
            exam.Title = updatedExam.Title;
            exam.Description = updatedExam.Description;
            exam.Type = updatedExam.Type;
            exam.StartDate = updatedExam.StartDate;
            exam.EndDate = updatedExam.EndDate;
            exam.DurationMinutes = updatedExam.DurationMinutes;

            await _context.SaveChangesAsync(cancellationToken);

            TempData["Success"] = "تم تحديث الاختبار بنجاح";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Exam/ResetStudent - إعادة الامتحان لطالب واحد
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> ResetStudent(int examId, int studentId, CancellationToken cancellationToken)
        {
            var attempt = await _context.StudentExamAttempts
                .Include(a => a.Answers)
                .FirstOrDefaultAsync(a => a.ExamId == examId && a.StudentId == studentId, cancellationToken);

            if (attempt == null)
            {
                TempData["Error"] = "لا توجد محاولة لهذا الطالب";
                return RedirectToAction(nameof(Details), new { id = examId });
            }

            // حذف الإجابات أولاً
            _context.StudentAnswers.RemoveRange(attempt.Answers);
            // ثم حذف المحاولة
            _context.StudentExamAttempts.Remove(attempt);
            await _context.SaveChangesAsync(cancellationToken);

            TempData["Success"] = "تم إعادة تعيين الاختبار للطالب بنجاح";
            return RedirectToAction(nameof(Details), new { id = examId });
        }

        // POST: Exam/ResetAll - إعادة الامتحان لجميع الطلبة
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> ResetAll(int examId, CancellationToken cancellationToken)
        {
            var attempts = await _context.StudentExamAttempts
                .Include(a => a.Answers)
                .Where(a => a.ExamId == examId)
                .ToListAsync(cancellationToken);

            if (!attempts.Any())
            {
                TempData["Info"] = "لا توجد محاولات لحذفها";
                return RedirectToAction(nameof(Details), new { id = examId });
            }

            // حذف جميع الإجابات
            foreach (var attempt in attempts)
            {
                _context.StudentAnswers.RemoveRange(attempt.Answers);
            }
            
            // حذف جميع المحاولات
            _context.StudentExamAttempts.RemoveRange(attempts);
            await _context.SaveChangesAsync(cancellationToken);

            TempData["Success"] = $"تم إعادة تعيين الاختبار لجميع الطلبة ({attempts.Count} محاولة)";
            return RedirectToAction(nameof(Details), new { id = examId });
        }

        // ═══════════════════════════════════════════════════════
        // Student Actions
        // ═══════════════════════════════════════════════════════

        // GET: Exam/MyExams - الاختبارات المتاحة للطالب
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyExams(CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name, cancellationToken);
            if (user?.StudentId == null)
            {
                ViewBag.Error = "لا يوجد حساب طالب مرتبط بهذا المستخدم";
                return View(new List<Exam>());
            }

            var studentId = user.StudentId.Value;

            // Auto-submit any incomplete attempts that are past deadline
            await AutoSubmitExpiredAttempts(studentId, cancellationToken);

            // الحصول على مجموعات الطالب النشطة
            var studentGroupIds = await _context.StudentGroups
                .Where(sg => sg.StudentId == studentId && sg.IsActive)
                .Select(sg => sg.GroupId)
                .ToListAsync(cancellationToken);

            // الحصول على الاختبارات المتاحة (نافذة الفتح)
            var now = DateTime.Now;
            var exams = await _context.Exams
                .Where(e => studentGroupIds.Contains(e.GroupId)
                         && e.StartDate <= now
                         && e.EndDate >= now)
                .Include(e => e.Group)
                    .ThenInclude(g => g!.AcademicYear)
                .Include(e => e.Questions)
                .Include(e => e.Attempts.Where(a => a.StudentId == studentId))
                .OrderBy(e => e.StartDate)
                .ToListAsync(cancellationToken);

            return View(exams);
        }

        /// <summary>
        /// Auto-submit any attempts that passed their deadline without being submitted
        /// </summary>
        private async Task AutoSubmitExpiredAttempts(int studentId, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            
            // Find all incomplete attempts for this student
            var incompleteAttempts = await _context.StudentExamAttempts
                .Include(a => a.Exam)
                    .ThenInclude(e => e!.Questions)
                .Include(a => a.Answers)
                .Where(a => a.StudentId == studentId && !a.IsSubmitted)
                .ToListAsync(cancellationToken);

            foreach (var attempt in incompleteAttempts)
            {
                if (attempt.Exam == null) continue;

                // Calculate deadline
                DateTime deadline;
                if (attempt.Exam.DurationMinutes.HasValue)
                {
                    var personalDeadline = attempt.StartedAt.AddMinutes(attempt.Exam.DurationMinutes.Value);
                    deadline = personalDeadline < attempt.Exam.EndDate ? personalDeadline : attempt.Exam.EndDate;
                }
                else
                {
                    deadline = attempt.Exam.EndDate;
                }

                // If past deadline, auto-submit
                if (now > deadline)
                {
                    attempt.SubmittedAt = deadline; // Use deadline as submit time
                    attempt.IsSubmitted = true;

                    // Calculate score for MCQ
                    if (attempt.Exam.Type == ExamType.MCQ)
                    {
                        var score = await _context.StudentAnswers
                            .Where(a => a.AttemptId == attempt.Id && a.IsCorrect == true)
                            .Include(a => a.Question)
                            .SumAsync(a => a.Question!.Points, cancellationToken);

                        attempt.Score = score;
                    }
                }
            }

            if (incompleteAttempts.Any(a => a.IsSubmitted))
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        // GET: Exam/Take/5 - بدء الاختبار
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Take(int id, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name, cancellationToken);
            if (user?.StudentId == null)
            {
                TempData["Error"] = "لا يوجد حساب طالب مرتبط بهذا المستخدم";
                return RedirectToAction(nameof(MyExams));
            }

            var studentId = user.StudentId.Value;

            var exam = await _context.Exams
                .Include(e => e.Group)
                    .ThenInclude(g => g!.AcademicYear)
                .Include(e => e.Questions.OrderBy(q => q.Order))
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (exam == null)
                return NotFound();

            // التحقق من أن الطالب في المجموعة
            var isInGroup = await _context.StudentGroups
                .AnyAsync(sg => sg.StudentId == studentId && sg.GroupId == exam.GroupId && sg.IsActive, cancellationToken);

            if (!isInGroup)
            {
                TempData["Error"] = "غير مسموح لك بدخول هذا الاختبار";
                return RedirectToAction(nameof(MyExams));
            }

            // التحقق من صلاحية نافذة الاختبار
            var now = DateTime.Now;
            if (now < exam.StartDate || now > exam.EndDate)
            {
                TempData["Error"] = "الاختبار غير متاح حالياً";
                return RedirectToAction(nameof(MyExams));
            }

            // التحقق من عدم وجود محاولة سابقة
            var existingAttempt = await _context.StudentExamAttempts
                .FirstOrDefaultAsync(a => a.StudentId == studentId && a.ExamId == id, cancellationToken);

            if (existingAttempt != null)
            {
                TempData["Error"] = "لقد قمت بإجراء هذا الاختبار من قبل";
                return RedirectToAction(nameof(Result), new { id });
            }

            // إنشاء محاولة جديدة
            var startedAt = DateTime.Now;
            var attempt = new StudentExamAttempt
            {
                StudentId = studentId,
                ExamId = id,
                StartedAt = startedAt,
                IsSubmitted = false
            };

            _context.StudentExamAttempts.Add(attempt);
            await _context.SaveChangesAsync(cancellationToken);

            // حساب deadline الطالب = الأقل بين (وقت البدء + المدة) و (وقت نهاية الامتحان)
            DateTime deadlineTime;
            if (exam.DurationMinutes.HasValue)
                deadlineTime = startedAt.AddMinutes(exam.DurationMinutes.Value) < exam.EndDate
                    ? startedAt.AddMinutes(exam.DurationMinutes.Value)
                    : exam.EndDate;
            else
                deadlineTime = exam.EndDate;

            ViewBag.Attempt = attempt;
            ViewBag.DeadlineTime = deadlineTime;
            ViewBag.DurationMinutes = exam.DurationMinutes;

            return View(exam);
        }

        // POST: Exam/Submit - إرسال الإجابات
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Submit(int attemptId, Dictionary<int, string> answers, CancellationToken cancellationToken)
        {
            var attempt = await _context.StudentExamAttempts
                .Include(a => a.Exam)
                    .ThenInclude(e => e!.Questions)
                .FirstOrDefaultAsync(a => a.Id == attemptId, cancellationToken);

            if (attempt == null)
                return NotFound();

            // التحقق من أن الطالب لم يتجاوز وقته الشخصي (server-side)
            if (attempt.Exam!.DurationMinutes.HasValue)
            {
                var studentDeadline = attempt.StartedAt.AddMinutes(attempt.Exam.DurationMinutes.Value);
                var hardDeadline = studentDeadline < attempt.Exam.EndDate ? studentDeadline : attempt.Exam.EndDate;
                // نسمح بهامش 30 ثانية
                if (DateTime.Now > hardDeadline.AddSeconds(30))
                {
                    // نسليم تلقائياً بما لديه
                }
            }

            // حفظ الإجابات
            foreach (var answer in answers)
            {
                var questionId = answer.Key;
                var answerText = answer.Value;

                var question = attempt.Exam!.Questions.FirstOrDefault(q => q.Id == questionId);
                if (question == null) continue;

                bool? isCorrect = null;
                if (attempt.Exam.Type == ExamType.MCQ && question.CorrectAnswer != null)
                {
                    isCorrect = answerText?.Trim().ToUpper() == question.CorrectAnswer.Trim().ToUpper();
                }

                _context.StudentAnswers.Add(new StudentAnswer
                {
                    AttemptId = attemptId,
                    QuestionId = questionId,
                    AnswerText = answerText,
                    IsCorrect = isCorrect
                });
            }

            // تحديث المحاولة
            attempt.SubmittedAt = DateTime.Now;
            attempt.IsSubmitted = true;

            // حساب الدرجة للـ MCQ
            if (attempt.Exam!.Type == ExamType.MCQ)
            {
                await _context.SaveChangesAsync(cancellationToken);

                var score = await _context.StudentAnswers
                    .Where(a => a.AttemptId == attemptId && a.IsCorrect == true)
                    .Include(a => a.Question)
                    .SumAsync(a => a.Question!.Points, cancellationToken);

                attempt.Score = score;
            }

            await _context.SaveChangesAsync(cancellationToken);

            TempData["Success"] = "تم إرسال الاختبار بنجاح";
            return RedirectToAction(nameof(Result), new { id = attempt.ExamId });
        }

        // GET: Exam/Result/5 - عرض نتيجة الاختبار
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Result(int id, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name, cancellationToken);
            if (user?.StudentId == null)
            {
                TempData["Error"] = "لا يوجد حساب طالب مرتبط بهذا المستخدم";
                return RedirectToAction(nameof(MyExams));
            }

            var studentId = user.StudentId.Value;

            var attempt = await _context.StudentExamAttempts
                .Include(a => a.Exam)
                    .ThenInclude(e => e!.Questions.OrderBy(q => q.Order))
                .Include(a => a.Answers)
                    .ThenInclude(a => a.Question)
                .FirstOrDefaultAsync(a => a.StudentId == studentId && a.ExamId == id, cancellationToken);

            if (attempt == null)
            {
                TempData["Error"] = "لم تقم بإجراء هذا الاختبار";
                return RedirectToAction(nameof(MyExams));
            }

            return View(attempt);
        }

        // GET: Exam/AddMultipleQuestions?examId=5
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> AddMultipleQuestions(int examId, CancellationToken cancellationToken)
        {
            var exam = await _context.Exams
                .Include(e => e.Group)
                    .ThenInclude(g => g!.AcademicYear)
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId, cancellationToken);

            if (exam == null)
                return NotFound();

            ViewBag.Exam = exam;
            ViewBag.NextOrder = exam.Questions.Any() ? exam.Questions.Max(q => q.Order) + 1 : 1;

            return View(new Center_Management.View_Models.AddMultipleQuestionsVM 
            { 
                ExamId = examId,
                QuestionCount = 5
            });
        }

        // POST: Exam/AddMultipleQuestions - Step 1: Generate Form
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> GenerateQuestionsForm(Center_Management.View_Models.AddMultipleQuestionsVM vm, CancellationToken cancellationToken)
        {
            var exam = await _context.Exams
                .Include(e => e.Group)
                    .ThenInclude(g => g!.AcademicYear)
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == vm.ExamId, cancellationToken);

            if (exam == null)
                return NotFound();

            ViewBag.Exam = exam;
            var startOrder = exam.Questions.Any() ? exam.Questions.Max(q => q.Order) + 1 : 1;

            vm.Questions = new List<Center_Management.View_Models.QuestionItemVM>();
            for (int i = 0; i < vm.QuestionCount; i++)
            {
                vm.Questions.Add(new Center_Management.View_Models.QuestionItemVM
                {
                    Order = startOrder + i,
                    Points = 1
                });
            }

            return View("EnterMultipleQuestions", vm);
        }

        // POST: Exam/SaveMultipleQuestions - Step 2: Save All Questions
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> SaveMultipleQuestions(Center_Management.View_Models.AddMultipleQuestionsVM vm, CancellationToken cancellationToken)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == vm.ExamId, cancellationToken);

            if (exam == null)
                return NotFound();

            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "questions");
            Directory.CreateDirectory(uploadDir);

            int savedCount = 0;
            foreach (var questionVM in vm.Questions)
            {
                var question = new ExamQuestion
                {
                    ExamId = vm.ExamId,
                    QuestionText = questionVM.Text,
                    Order = questionVM.Order,
                    OptionA = questionVM.OptionA,
                    OptionB = questionVM.OptionB,
                    OptionC = questionVM.OptionC,
                    OptionD = questionVM.OptionD,
                    CorrectAnswer = questionVM.CorrectAnswer,
                    Points = (int)questionVM.Points
                };

                // رفع الصورة إن وُجدت
                if (questionVM.QuestionImage != null && questionVM.QuestionImage.Length > 0)
                {
                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                    if (allowedTypes.Contains(questionVM.QuestionImage.ContentType))
                    {
                        var ext = Path.GetExtension(questionVM.QuestionImage.FileName);
                        var fileName = $"{Guid.NewGuid()}{ext}";
                        var filePath = Path.Combine(uploadDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                            await questionVM.QuestionImage.CopyToAsync(stream, cancellationToken);

                        question.ImagePath = $"/uploads/questions/{fileName}";
                    }
                }

                _context.ExamQuestions.Add(question);
                savedCount++;
            }

            await _context.SaveChangesAsync(cancellationToken);

            TempData["Success"] = $"تم إضافة {savedCount} سؤال بنجاح";
            return RedirectToAction(nameof(Details), new { id = vm.ExamId });
        }
    }
}
