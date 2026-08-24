# College Management System API & Portal

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue.svg)](https://www.postgresql.org/)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-8.0-purple.svg)](https://docs.microsoft.com/en-us/ef/core/)
[![Swagger](https://img.shields.io/badge/Swagger-OpenAPI%203.0-green.svg)](https://swagger.io/)

A complete, production-quality, beginner-understandable **College Management System** built with **ASP.NET Core Web API (.NET 8)**, **PostgreSQL**, **Entity Framework Core**, **Swagger / OpenAPI**, and an interactive vanilla JS/HTML/CSS Dashboard UI served directly from `wwwroot`.

---

## 📌 Features

* 👨‍🎓 **Student Management**: Full CRUD operations, unique student numbers & email validation, department and semester filtering, and search functionality.
* 📚 **Course Catalog**: Course creation, credit allocation, department assignment, and faculty linkage.
* 👩‍🏫 **Faculty Management**: Employee records, designation tracking, and course load visibility.
* 📝 **Enrollment Management**: Enroll students in active courses with duplicate enrollment prevention (composite unique constraints).
* 📅 **Attendance Records**: Log daily student attendance per course with automated attendance percentage summaries.
* 🏅 **Marks & Grade Evaluation**: Record student marks, enforce range validation (`MarksObtained <= MaximumMarks`), and automatically calculate letter grades (`A+`, `A`, `B`, `C`, `D`, `F`).
* 📊 **Executive Dashboard**: Live summary metrics (Total Students, Courses, Faculty, Enrollments), recent student activity, and department-wise statistics.
* 📖 **Swagger OpenAPI**: Complete interactive API testing interface at `/swagger`.
* ⚡ **Single-Page Dashboard**: Served from root `/` (`wwwroot/index.html`) using clean HTML, CSS variables, dark glassmorphism, and Vanilla JavaScript with no heavy frontend framework required.

---

## 🛠️ Technology Stack

* **Backend Framework**: ASP.NET Core Web API (.NET 8, C#)
* **Database Engine**: PostgreSQL
* **ORM Provider**: Entity Framework Core (`Npgsql.EntityFrameworkCore.PostgreSQL`)
* **Documentation**: Swagger / OpenAPI (`Swashbuckle.AspNetCore`)
* **Frontend UI**: HTML5, Vanilla CSS3 (Custom Design System), Vanilla JavaScript (ES6+)
* **Architecture**: Controller-Service-Repository/DbContext Separation

---

## 📂 Project Architecture & File Structure

```text
CollegeManagement/
│
├── CollegeManagement.API/
│   ├── Controllers/
│   │   ├── StudentsController.cs       # REST endpoints for Students
│   │   ├── CoursesController.cs        # REST endpoints for Courses
│   │   ├── FacultyController.cs        # REST endpoints for Faculty
│   │   ├── EnrollmentsController.cs    # REST endpoints for Enrollments
│   │   ├── AttendanceController.cs     # REST endpoints for Attendance
│   │   ├── MarksController.cs          # REST endpoints for Marks & Grades
│   │   └── DashboardController.cs      # REST endpoints for Dashboard Analytics
│   │
│   ├── Data/
│   │   ├── ApplicationDbContext.cs     # EF Core DbContext with constraints & indexes
│   │   └── DbInitializer.cs            # Automatic seed generator (10 students, 5 faculty, etc.)
│   │
│   ├── Models/
│   │   ├── Student.cs                  # Student entity
│   │   ├── Course.cs                   # Course entity
│   │   ├── Faculty.cs                  # Faculty entity
│   │   ├── Enrollment.cs               # Enrollment composite entity
│   │   ├── Attendance.cs               # Attendance record entity
│   │   └── Mark.cs                     # Academic mark & grade entity
│   │
│   ├── DTOs/
│   │   ├── StudentDto.cs               # Student Data Transfer Objects
│   │   ├── CourseDto.cs                # Course DTOs
│   │   ├── FacultyDto.cs               # Faculty DTOs
│   │   ├── EnrollmentDto.cs            # Enrollment DTOs
│   │   ├── AttendanceDto.cs            # Attendance DTOs & Summaries
│   │   ├── MarkDto.cs                  # Mark DTOs
│   │   └── DashboardDto.cs             # Dashboard Summary DTOs
│   │
│   ├── Services/
│   │   ├── StudentService.cs           # Student business logic & search
│   │   ├── CourseService.cs            # Course management logic
│   │   ├── FacultyService.cs           # Faculty business logic
│   │   ├── EnrollmentService.cs        # Enrollment duplicate checks
│   │   ├── AttendanceService.cs        # Attendance calculation logic
│   │   ├── MarkService.cs              # Mark validation & grade calculation
│   │   └── DashboardService.cs         # Metric aggregation service
│   │
│   ├── wwwroot/
│   │   ├── index.html                  # Dashboard HTML UI layout
│   │   ├── css/
│   │   │   └── style.css               # Modern dark-mode glassmorphism design system
│   │   └── js/
│   │       └── app.js                  # Dynamic API client & UI modal controller
│   │
│   ├── Properties/
│   │   └── launchSettings.json         # Local development environment settings
│   │
│   ├── appsettings.json               # Default configuration & connection strings
│   ├── Program.cs                      # Application pipeline, CORS, Swagger, DI registration
│   └── CollegeManagement.API.csproj    # .NET 8 Project file with NuGet dependencies
│
├── README.md                           # Documentation & Setup Guide
├── .gitignore                          # Visual Studio git ignore configuration
└── CollegeManagement.sln               # Solution file
```

---

## ⚙️ Local Database Setup (PostgreSQL)

1. Install **PostgreSQL** on your system (or run via Docker).
2. Ensure PostgreSQL service is running on port `5432`.
3. Create a database named `college_management` (optional; EF Core will create it automatically if permissions allow):
   ```sql
   CREATE DATABASE college_management;
   ```
4. Verify your credentials in `CollegeManagement.API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=college_management;Username=postgres;Password=YOUR_PASSWORD"
   }
   ```
5. You can also override the connection string without editing code using an environment variable:
   - **Windows Command Prompt**:
     ```cmd
     set ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=college_management;Username=postgres;Password=YOUR_PASSWORD
     ```
   - **PowerShell**:
     ```powershell
     $env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=college_management;Username=postgres;Password=YOUR_PASSWORD"
     ```

---

## 🚀 Commands to Run Locally

1. Clone or open the repository folder:
   ```bash
   cd CollegeManagement
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Apply Entity Framework Core Migrations (or let `DbInitializer` generate tables automatically):
   ```bash
   dotnet ef database update --project CollegeManagement.API
   ```

5. Launch the application:
   ```bash
   dotnet run --project CollegeManagement.API
   ```

6. Open your web browser:
   - **Dashboard Portal**: `http://localhost:5000/`
   - **Swagger OpenAPI Documentation**: `http://localhost:5000/swagger`

---

## 📡 REST API Endpoints Overview

| Component | Method | Endpoint | Description |
|---|---|---|---|
| **Dashboard** | `GET` | `/api/dashboard` | Get summary counts, recent students, & department stats |
| **Students** | `GET` | `/api/students` | Get all students (Supports `?search=rahul&department=CSE&semester=5`) |
| | `GET` | `/api/students/{id}` | Get student by ID |
| | `POST` | `/api/students` | Add new student (Validates unique student number & email) |
| | `PUT` | `/api/students/{id}` | Update student details |
| | `DELETE` | `/api/students/{id}` | Delete student |
| **Courses** | `GET` | `/api/courses` | Get all courses with faculty details |
| | `GET` | `/api/courses/{id}` | Get course by ID |
| | `POST` | `/api/courses` | Create course |
| | `PUT` | `/api/courses/{id}` | Update course |
| | `DELETE` | `/api/courses/{id}` | Delete course |
| **Faculty** | `GET` | `/api/faculty` | Get all faculty members |
| | `GET` | `/api/faculty/{id}` | Get faculty by ID |
| | `POST` | `/api/faculty` | Create faculty member |
| | `PUT` | `/api/faculty/{id}` | Update faculty member |
| | `DELETE` | `/api/faculty/{id}` | Delete faculty member |
| **Enrollments**| `GET` | `/api/enrollments` | List all student course enrollments |
| | `POST` | `/api/enrollments` | Enroll student (Rejects duplicate student+course) |
| | `PUT` | `/api/enrollments/{id}` | Update enrollment status |
| | `DELETE` | `/api/enrollments/{id}` | Remove enrollment |
| **Attendance** | `GET` | `/api/attendance` | Get attendance records (Supports `?studentId=1&courseId=2`) |
| | `GET` | `/api/attendance/summary` | Get attendance % for student in a course |
| | `POST` | `/api/attendance` | Log attendance record for a date |
| | `DELETE` | `/api/attendance/{id}` | Delete attendance record |
| **Marks** | `GET` | `/api/marks` | Get student marks & calculated grades |
| | `POST` | `/api/marks` | Enter student marks (Validates range & auto-calculates grade) |
| | `PUT` | `/api/marks/{id}` | Update marks |
| | `DELETE` | `/api/marks/{id}` | Delete mark record |

---

## 🌐 Deployment Instructions

This application is containerization-ready and cloud-deployment ready (e.g., Azure App Service, AWS Elastic Beanstalk, Render, DigitalOcean, Cloud Run).

1. Set the production environment variables:
   - `ASPNETCORE_ENVIRONMENT` = `Production`
   - `DATABASE_URL` = `Host=your-db-host;Port=5432;Database=college_db;Username=db_user;Password=db_pass;SslMode=Require;`
   - `ASPNETCORE_URLS` = `http://+:8080` (or host provider's port binding)

2. Publish the release bundle:
   ```bash
   dotnet publish CollegeManagement.API/CollegeManagement.API.csproj -c Release -o ./publish
   ```

---

## 🎓 Architecture Summary for Academic Evaluation

> **How to Explain This System During Evaluation:**
>
> 1. **Separation of Concerns**: The application follows a 3-tier architecture:
>    - **Presentation Layer**: ASP.NET Core REST Controllers handling HTTP requests, standard HTTP status codes (`200 OK`, `201 Created`, `400 Bad Request`, `404 Not Found`, `409 Conflict`), and serving the SPA Dashboard UI from `wwwroot`.
>    - **Business Logic Layer**: Services (`StudentService`, `MarkService`, etc.) containing validation logic, grade calculation algorithms, and search filters.
>    - **Data Access Layer**: Entity Framework Core (`ApplicationDbContext`) managing PostgreSQL entities, composite indexes, foreign key safety, and seeding.
> 2. **Database Integrity**: PostgreSQL handles data integrity using foreign keys and unique constraints (e.g. unique student numbers, unique emails, unique course codes, composite unique student-course enrollments). `OnDelete(DeleteBehavior.Restrict)` is configured to prevent accidental cascade deletion of critical academic records.
> 3. **Dependency Injection**: Services are registered in `Program.cs` as Scoped services, keeping controllers lightweight and easily testable.
