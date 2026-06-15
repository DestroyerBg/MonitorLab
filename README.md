# 🖥️ MonitorLab

ASP.NET Core MVC application for comparing, analyzing and managing computer monitors.

---

## 🔧 Technologies Used

* ASP.NET Core MVC (.NET 10)
* Entity Framework Core
* ASP.NET Identity
* SQL Server
* AutoMapper
* Bootstrap 5
* JavaScript
* NUnit
* GitHub Actions

---

## ✨ Features

### Public Area

* 🔍 Browse monitor catalog
* 🎯 Filter monitors by specifications
* 📊 Compare multiple monitors
* ⭐ Recommendation system
* 📖 Educational monitor information

### Admin Area

* 🔐 Authentication and authorization
* ➕ Add monitors
* ✏️ Edit monitors
* 🗑️ Delete monitors
* 🖼️ Upload monitor images
* 🔌 Manage monitor ports

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/DestroyerBg/MonitorLab.git
cd MonitorLab
```

### 2. Configure appsettings.json

Add your SQL Server connection string and administrator account:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_SQL_SERVER_CONNECTION_STRING"
  },

  "AdminUser": {
    "Email": "admin-email",
    "Username": "admin-user",
    "Password": "admin-pass!"
  }
}
```

The administrator account will be created automatically during the first application startup.

### 3. Run the application

```bash
dotnet run
```

The database, seed data and administrator account are created automatically on startup.

---

## 🧪 Running Tests

Run all tests:

```bash
dotnet test
```

GitHub Actions automatically executes the test suite for pull requests targeting the master branch.

---

## 🏗️ Project Structure

```text
MonitorLab.Web
MonitorLab.Core
MonitorLab.Data
MonitorLab.Tests
```

---

## 📋 Main Entities

* Monitor
* Port
* MonitorPort

---

## 🔒 Authentication

The administration panel is protected with ASP.NET Identity.

Only users assigned to the Admin role can manage monitor data.

---

## 👨‍🎓 Bachelor's Degree Thesis

Technical University of Varna

Topic:

> Information System for Comparative Analysis of Computer Monitors

---

## 📄 License

MIT License
