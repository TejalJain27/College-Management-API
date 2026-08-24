# Render Deployment Guide - College Management System

This guide provides step-by-step instructions for deploying the **College Management System (`CollegeManagement.API`)** to **Render** using Docker and Render Managed PostgreSQL.

---

## 📌 Prerequisites

1. A [Render Account](https://render.com/).
2. Git repository pushed to GitHub (e.g., `College-Management-API`).

---

## 🚀 Step 1: Create a PostgreSQL Database on Render

1. Log into your **Render Dashboard** and click **New +** -> **PostgreSQL**.
2. Configure the database details:
   - **Name**: `college-management-db`
   - **Database**: `college_management`
   - **User**: `postgres`
   - **Region**: Choose the region closest to you (e.g., Oregon / Frankfurt / Singapore)
   - **Instance Type**: Free or Starter
3. Click **Create Database**.
4. Once the database status changes to **Available**, scroll down to **Connection Info**.
5. Copy the **Internal Database URL** (e.g., `postgres://postgres:password123@dpg-xxxxxx-a.render.com/college_management`).

---

## 🐳 Step 2: Create a Web Service on Render

1. On the Render Dashboard, click **New +** -> **Web Service**.
2. Connect your GitHub repository containing the project.
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
2. Render will build the Docker container using .NET 8 SDK, restore NuGet packages, compile the project, and start the service.
3. On initial startup, the application executes `context.Database.Migrate()`:
   - EF Core automatically creates the database schema and tables (`Students`, `FacultyMembers`, `Courses`, `Enrollments`, `AttendanceRecords`, `Marks`).
   - `DbInitializer` seeds 10 initial students, 5 faculty members, 8 courses, enrollments, attendance, and mark sheets.

---

## 🌐 Step 5: Verify Deployment URLs

Once deployment completes, open your Render web service URL (e.g., `https://college-management-api.onrender.com`):

* 📊 **Dashboard Portal**: `https://<your-render-app>.onrender.com/`
* 📖 **Swagger OpenAPI Docs**: `https://<your-render-app>.onrender.com/swagger`
* 📡 **API Metrics**: `https://<your-render-app>.onrender.com/api/dashboard`
* 👨‍🎓 **Students API**: `https://<your-render-app>.onrender.com/api/students`

---

## 🛠️ Local Development (Remains Intact)

Local development using `localhost:5000` remains completely functional:

```bash
# Set local PostgreSQL connection string or use default appsettings.json
dotnet restore
dotnet build
dotnet ef database update --project CollegeManagement.API
dotnet run --project CollegeManagement.API
```
App runs locally at `http://localhost:5000/`.
