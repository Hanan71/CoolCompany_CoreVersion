# Cool Company - E-Store Web Application 🛒

A full-stack modern e-commerce web application engineered with **ASP.NET Core (.NET)**, **Entity Framework Core**, and **Bootstrap**. The platform provides full-featured product cataloging, category filtering, responsive browsing, and robust database architecture.

---
### 🏢 Cooperative Training & Mentorship

<p align="left">
  <img width="70" align="right" alt="Jawraa Logo" src="https://github.com/user-attachments/assets/537d8b0a-1a0c-4e9b-8b23-13292f1aaa32" />
  This project was developed during the <b>Cooperative Training Program</b> at <b>Jawraa (شركة جوراء)</b>.
  <br><br>
  • <b>Host Organization:</b> <a href="https://jawraa.com/">Jawraa</a><br>
  • <b>Supervisor & Mentor:</b> <a href="https://www.linkedin.com/in/nahla-mohammed-ahmed/">Eng. Nahla Mohammed Ahmed</a>
</p>
<br clear="right"/>

---
## 🚀 Key Features
- **Responsive UI/UX:** Built with Bootstrap, fully responsive across desktop, tablet, and mobile screens.
- **Product & Category Showcase:** Dynamic browsing for products, featured offers, and shopping categories.
- **Clean Architecture:** Separation of concerns following ASP.NET Core MVC / Web API standards.
- **Database Migrations:** Pre-configured Entity Framework Core migrations for automated schema generation.

---

## 🛠️ Tech Stack
- **Back-End:** ASP.NET Core (.NET)
- **Database & ORM:** Microsoft SQL Server, Entity Framework Core (EF Core)
- **Front-End:** HTML5, CSS3, JavaScript, Bootstrap
- **Development Tools:** Visual Studio / VS Code, Docker Desktop, Azure Data Studio

---

## 💻 Local Setup & Development (macOS Environment)

Because **Microsoft SQL Server** does not run natively on macOS (especially Apple Silicon chips), running this project locally requires **Docker** to host the SQL database engine and **Azure Data Studio** to manage and inspect database instances.

### 1. Prerequisites

| Tool | Status & Reference | Preview |
| :--- | :--- | :---: |
| **Docker Desktop** | [Download & Install](https://www.docker.com/products/docker-desktop/) | <img width="380" alt="Docker Desktop" src="https://github.com/user-attachments/assets/8e2e3e86-f91e-4590-9b8c-9068da1134d4" /> |
| **Azure Data Studio** | [Download & Install](https://learn.microsoft.com/en-us/azure-data-studio/download-azure-data-studio) | <img width="380" alt="Azure Data Studio" src="https://github.com/user-attachments/assets/446ede7a-7721-42f6-b6d0-9a9771782baa" /> |
| **.NET SDK** | [Download & Install](https://dotnet.microsoft.com/download) | <img width="380" alt=".NET SDK" src="https://github.com/user-attachments/assets/8b8f688f-5c38-4d2f-ab54-7e9ec5265d39" /> |


---

### 2. Run Microsoft SQL Server via Docker
Open your **Terminal** and start an optimized SQL Server container instance:

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Password123" \
   -p 1433:1433 --name sql_server \
   -d [mcr.microsoft.com/azure-sql-edge](https://mcr.microsoft.com/azure-sql-edge)
```
Note for MacOS users, Apple Silicon or Intel (M-Series) users: The azure-sql-edge image is lightweight and natively compatible with ARM64 architecture.

3. Connect via Azure Data Studio
Open Azure Data Studio.

Click New Connection and fill in the connection details:

Connection Type: Microsoft SQL Server

Server: localhost,1433

Authentication Type: SQL Login

User name: sa

Password: YourStrong@Password123 (or the password configured in Docker)

Click Connect to establish the session.

4. Configure & Run the .NET Application
In appsettings.json, verify or update your connection string to target your Docker container:
```
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=coolcompanyestoreDB;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;"
}
```

2- Apply database migrations to automatically construct the schema:
dotnet ef database update

3- Launch the application:
dotnet run
}

```
🔒 Security Notice
All production credentials, active passwords, and sensitive keys have been scrubbed and replaced with generic development placeholders prior to committing to version control.
}
```

<a href="https://canva.link/n0iy1dpzt9h0w3u" style="text-decoration: none;">
  <span style="background-color: #1F2428; color: #FFFFFF; padding: 8px 18px; font-weight: bold; border-radius: 6px; border: 2px solid #D4AF37; display: inline-block; font-family: sans-serif; font-size: 13px;">
    🎓 University Training Presentation
  </span>
</a>
