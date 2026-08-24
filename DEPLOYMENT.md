# Render Deployment Guide - College Management System

This guide provides step-by-step instructions for deploying the **College Management System (`CollegeManagement.API`)** to **Render** using Docker and Render Managed PostgreSQL.

---

## 🐞 Fix Summary: Resolving Container Exit Status 139 (SIGSEGV)

### What Caused Exit Code 139?
Exit code 139 on Linux indicates a **Segmentation Fault (`SIGSEGV`)**. In .NET 8 Docker containers on cloud Linux platforms (like Render), `WebApplication.CreateBuilder(args)` crashed due to:
1. **Linux `inotify` File System Watcher Segfaults**: Default host configuration uses `reloadOnChange: true` on `appsettings.json`. On container overlay file systems, `System.IO.FileSystemWatcher`'s native `inotify` calls triggered memory violations (`SIGSEGV`).
2. **Native Globalization/ICU Segfaults**: Native glibc culture/locale initialization in minimal runtime containers caused segfaults during host initialization.

### Fix Applied
1. **Disabled `reloadOnChange` & Configured `WebApplicationOptions`** (`Program.cs`):
   Set `ContentRootPath = AppContext.BaseDirectory` and registered configuration files with `reloadOnChange: false`.
2. **Enabled Invariant Globalization** (`CollegeManagement.API.csproj` & `Dockerfile`):
   Set `<InvariantGlobalization>true</InvariantGlobalization>` and `ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true`.
3. **Set Polling File Watcher** (`Dockerfile`):
   Added `ENV DOTNET_USE_POLLING_FILE_WATCHER=true`.

---

## 📌 Prerequisites

1. A [Render Account](https://render.com/).
2. Git repository pushed to GitHub containing this updated code.

---

## 🚀 Step 1: Create a PostgreSQL Database on Render

1. Log into your **Render Dashboard** and click **New +** -> **PostgreSQL**.
2. Configure the database details:
   - **Name**: `college-management-db`
   - **Database**: `college_management`
   - **User**: `postgres`
   - **Region**: Choose the region closest to you
   - **Instance Type**: Free or Starter
3. Click **Create Database**.
4. Once the database status changes to **Available**, scroll down to **Connection Info**.
5. Copy the **Internal Database URL** (e.g., `postgres://postgres:password123@dpg-xxxxxx-a.render.com/college_management`).

---

## 🐳 Step 2: Create a Web Service on Render

1. On the Render Dashboard, click **New +** -> **Web Service**.
2. Connect your GitHub repository.
3. Configure the web service settings:
   - **Name**: `college-management-api`
   - **Region**: Select the **same region** as your PostgreSQL database.
   - **Branch**: `main`
   - **Language / Runtime**: **Docker**
   - **Docker Command / Context**: `.` (Repository Root)
   - **Dockerfile Path**: `./Dockerfile`
   - **Instance Type**: Free or Starter

---

## 🔑 Step 3: Configure Environment Variables

Scroll to the **Environment Variables** section and add the following keys:

| Environment Key | Value / Source | Description |
|---|---|---|
| `DATABASE_URL` | `postgres://postgres:password123@dpg-xxxxxx-a.render.com/college_management` | Paste the **Internal Database URL** from Step 1 |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Sets ASP.NET Core production runtime mode |
| `EnableSwagger` | `true` | Enables Swagger OpenAPI documentation in production |

> **Note**: Render automatically injects the `PORT` environment variable (typically `10000`), which the ASP.NET Core Web API listens on automatically.

---

## ⚡ Step 4: Deploy and Verify

1. Click **Create Web Service**.
2. Render will build the Docker container using .NET 8 SDK, restore NuGet packages, compile the project, and start the service cleanly without exit 139 errors.
3. On initial startup, `context.Database.Migrate()` executes automatically:
   - EF Core creates database tables (`Students`, `FacultyMembers`, `Courses`, `Enrollments`, `AttendanceRecords`, `Marks`).
   - `DbInitializer` seeds 10 initial students, 5 faculty members, 8 courses, enrollments, attendance, and mark sheets.

---

## 🌐 Step 5: Verify Deployment URLs

Open your Render web service URL (e.g., `https://college-management-api.onrender.com`):

* 📊 **Dashboard Portal**: `https://<your-render-app>.onrender.com/`
* 📖 **Swagger OpenAPI Docs**: `https://<your-render-app>.onrender.com/swagger`
* 📡 **API Metrics**: `https://<your-render-app>.onrender.com/api/dashboard`
* 👨‍🎓 **Students API**: `https://<your-render-app>.onrender.com/api/students`

---

## 🛠️ Local Development

Local development remains 100% operational:

```bash
dotnet restore
dotnet build
dotnet ef database update --project CollegeManagement.API
dotnet run --project CollegeManagement.API
```
App runs locally at `http://localhost:5000/`.
