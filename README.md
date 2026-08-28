# 🎓 Center Management System

A comprehensive educational center management platform built with **ASP.NET Core MVC** for managing students, groups, payments, attendance, materials, and exams.

## ✨ Features

### For Teachers/Admins
- 📊 **Dashboard** - Overview with statistics and insights
- 👥 **Student Management** - Add, edit, view student records
- 📚 **Group Management** - Organize students into groups
- 🎓 **Academic Year Management** - Define year levels with pricing
- ✅ **Attendance Tracking** - Mark student attendance per session
- 💰 **Payment Management** - Track monthly payments with AJAX interface
- 📁 **Material Management** - Upload PDFs and videos per academic year
- 📝 **Exam System** - Create MCQ or written exams with auto-grading

### For Students
- 📝 **My Exams** - View and take available exams
- 📊 **View Results** - See exam scores and answers
- 📁 **Materials** - Access materials for their academic year
- 🔒 **One-Time Exams** - Each exam can only be taken once

## 🚀 Quick Start


## 🛠️ Technology Stack

- **Framework:** ASP.NET Core MVC (.NET 10.0)
- **Database:** SQL Server with Entity Framework Core 10.0.9
- **Authentication:** ASP.NET Core Identity
- **Frontend:** Razor Views, Custom CSS, JavaScript
- **Architecture:** MVC with Repository Pattern

## 📋 Requirements

- .NET 10.0 SDK
- SQL Server (LocalDB or Express)
- Visual Studio 2022 or VS Code

## 📦 Installation

1. **Restore packages:**
   ```bash
   dotnet restore
   ```

2. **Update database:**
   ```bash
   dotnet ef database update
   ```

3. **Run application:**
   ```bash
   dotnet run
   ```

## 🗄️ Database Structure

### Core Models
- **Student** - Student information
- **Group** - Class groups
- **AcademicYear** - Year levels with pricing
- **StudentGroup** - Student-Group relationships
- **GroupSchedule** - Class schedules

### Financial Models
- **Payment** - Monthly payment tracking
- **Attendance** - Session attendance records

### Educational Models
- **Material** - Teaching materials (PDFs, Videos)
- **Exam** - Exam definitions
- **ExamQuestion** - Exam questions (MCQ/Written)
- **StudentExamAttempt** - Student exam submissions
- **StudentAnswer** - Individual question answers

### Authentication
- **AppUser** - Identity user with optional Student link

## 👥 User Roles

1. **Admin** - Full system access
2. **Teacher** - Manage students, groups, exams, materials
3. **Student** - View and take exams, access materials

## 🎨 User Interface

- **RTL Support** - Full Arabic language support
- **Responsive Design** - Works on desktop, tablet, mobile
- **Theme Colors:**
  - Teal - Dashboard, Students
  - Green - Payments, Academic Years
  - Orange - Attendance
  - Purple - Exams
  - Blue - Groups

## 📱 Key Pages

### Teacher Pages
- `/Dashboard` - Statistics overview
- `/Student` - Student management
- `/Group` - Group management
- `/AcadimicYear` - Academic year management
- `/Attendance/Groups` - Attendance tracking
- `/Payment/Groups` - Payment management
- `/Material` - Material upload
- `/Exam/GroupPicker` - Exam management

### Student Pages
- `/Exam/MyExams` - Available exams
- `/Material/StudentView` - Academic materials
- `/Exam/Take/{id}` - Take exam
- `/Exam/Result/{attemptId}` - View results

## 🔒 Security Features

- Password-based authentication
- Role-based authorization
- One exam attempt per student (enforced at DB level)
- Secure file uploads for materials
- Form validation and CSRF protection

## 📊 Key Features Detail

### Payment System
- AJAX-based interface with live updates
- Mark individual students as paid/unpaid
- Bulk payment operations
- Monthly payment tracking per group
- Visual statistics (paid count, revenue)

### Exam System
- Two exam types: MCQ (auto-graded) and Written (manual grading)
- Countdown timer during exam
- One attempt per student (unique constraint)
- Automatic scoring for MCQ
- Teacher review interface for written exams
- Date-range based exam availability

### Material System
- Upload PDFs or videos
- Link to external URLs
- Organized by Academic Year
- Students see only materials for their year
- File size limits (10MB PDF, 100MB video)

### Attendance System
- AJAX card-based interface
- Mark present/absent per session
- Date-specific tracking
- Live statistics
- Bulk operations (mark all present/absent)

## 🧪 Testing

Run verification script:
```bash
.\VerifySystem.ps1
```

Tests performed:
- ✅ Build succeeds
- ✅ No malformed references
- ✅ Migration applied
- ✅ Required files deleted




### File Upload Path
Materials are uploaded to:
```
wwwroot/uploads/materials/
```

### Application URL
Default: `http://localhost:5142`

Edit in `Properties/launchSettings.json` to change.

## 📈 Recent Changes (v2.0)



### Student Material Access
- Students can view materials for their Academic Year
- Filtered based on active group enrollment
- Added to student sidebar menu



## 🐛 Known Issues

None currently. System is production-ready.

## 🤝 Contributing

This is a private educational project.

## 📄 License

Private/Educational Use

## 👨‍💻 Developer

Built for ITI Projects



**Version:** 2.0 (Subject-Free Edition)  
**Status:** ✅ Production Ready  
**Build:** SUCCESS  
**Last Updated:** 2026-08-18
