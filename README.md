<table>
  <tr>
    <td valign="middle">
      <h1 style="margin: 0;">Cool Company - E-Store Web Application 🛒</h1>
    </td>
    <td valign="middle">
      <img width="160" alt="logo" src="https://github.com/user-attachments/assets/c0dfcaa6-04f3-4411-afe2-f382b9596e0d" />
    </td>
  </tr>
</table>

A full-stack modern e-commerce web application engineered with **ASP.NET Core (.NET)**, **Entity Framework Core**, and **Bootstrap**. The platform provides full-featured product cataloging, category filtering, responsive browsing, and robust database architecture.

---
### 🏢 Cooperative Training & Mentorship

<p align="left">
  <a href="https://www.jawraa.com/" target="_blank">
    <img width="70" align="right" alt="Jawraa Logo" src="https://github.com/user-attachments/assets/537d8b0a-1a0c-4e9b-8b23-13292f1aaa32" />
  </a>
  This project was developed during the <b>Cooperative Training Program</b> at <b>Jawraa (شركة جوراء)</b>.
  <br><br>
  • <b>Host Organization:</b> <a href="https://www.jawraa.com/">Jawraa</a><br>
  • <b>Supervisor & Mentor:</b> <a href="https://www.linkedin.com/in/nahla-mohammed-ahmed/">Eng. Nahla Mohammed Ahmed</a>
</p>
<br clear="right"/>

---

<table>
  <tr>
    <td align="center" width="50%">
      <b>Store Overview</b><br><br>
      <img src="https://github.com/user-attachments/assets/fbc8c595-48ca-484f-90b2-2aa4ac7fb72b" alt="Store Overview" />
    </td>
    <td align="center" width="50%">
      <b>Products Catalog</b><br><br>
      <img src="https://github.com/user-attachments/assets/afea7be5-33a3-44f3-82d3-34a6e7fd17cb" alt="Products Catalog" />
    </td>
  </tr>
  <tr>
    <td align="center" width="50%">
      <b>Admin Panel</b><br><br>
      <img src="https://github.com/user-attachments/assets/92930857-5145-4e6d-8846-2fc445b12026" alt="Admin Panel" />
    </td>
    <td align="center" width="50%">
      <b>Tablet View</b><br><br>
      <img src="https://github.com/user-attachments/assets/abcbb4ed-b897-4d84-bb81-85ac8cee4558" alt="Tablet View" />
    </td>
  </tr>
  <tr>
    <td align="center" colspan="2">
      <b>Mobile Responsive View</b><br><br>
      <img width="85%" src="https://github.com/user-attachments/assets/77650b81-e2d0-48a4-87bc-21323628c93a" alt="Mobile Responsive View" />
    </td>
  </tr>
</table>

---

## 🚀 Key Features
- **Fully Responsive Across All Devices:** Engineered using Bootstrap to deliver an optimal, fluid browsing experience across desktop monitors, tablets, and mobile smartphones.
- **Role-Based Access Control (RBAC):** Multi-tier authorization system providing distinct portals and permissions for Super Admins, Store Managers (catalog/table/content editors), and Customers (shoppers).
- **Product & Category Showcase:** Dynamic browsing for products, featured offers, and shopping categories with real-time inventory views.
- **Clean Architecture:** Separation of concerns following ASP.NET Core MVC standards to ensure maintainability and modular design.
- **Database Migrations:** Pre-configured Entity Framework Core migrations for automated schema generation and data seeding.
  
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

> 🚀 **You are viewing the latest, most up-to-date version** of this project. Looking for the earlier baseline? Explore the [Legacy ASP.NET MVC Repository](https://github.com/Hanan71/CoolCompanyMVC/tree/main)

---

[![University Training Presentation](https://img.shields.io/badge/University%20Presentation-Canva%20Slides-D4AF37?style=for-the-badge&logo=canva&logoColor=white&labelColor=0056B3)](https://canva.link/n0iy1dpzt9h0w3u) 👈 Click to View
