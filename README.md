# Eventing

# MeetupX — Event Management Platform

> A full-stack event management web application built with ASP.NET Core 9, Blazor Server, and PostgreSQL. Manage events, handle RSVPs, and issue digital tickets — all in one platform.

---

## 🚀 Features

- **Event Discovery** — Browse upcoming physical and virtual events with search and filter
- **User Registration & Authentication** — JWT-based auth with email confirmation
- **RSVP Management** — Accept, decline, or mark maybe for any event
- **Digital Tickets** — Printable ticket with QR code for every registration
- **Admin Dashboard** — Create, edit, delete events and manage attendees
- **Attendance Tracking** — Mark attendees as present and lock their RSVP
- **Role-based Access** — Admin and General Member roles
- **Responsive UI** — Dark-themed modern interface built with custom CSS

---

## 🛠 Tech Stack

| Layer | Technology |
|---|---|
| Backend API | ASP.NET Core 9 Web API |
| Frontend | Blazor Server (.NET 9) |
| Database | PostgreSQL + Entity Framework Core 9 |
| Auth | ASP.NET Core Identity + JWT Bearer |
| UI Components | Microsoft FluentUI + Custom CSS |
| Email | FluentEmail + SMTP (Gmail) |
| ORM | Npgsql EF Core Provider |
| Hosting | IIS (Windows Server) |

---

## 📁 Project Structure

```
Event-Management/
├── Eventing.ApiService/       # REST API — controllers, entities, migrations, seeders
│   ├── Controllers/
│   │   ├── Account/           # Login, register, email confirmation, password reset
│   │   ├── Event/             # CRUD for events
│   │   └── Attendee/          # RSVP, registration, attendance
│   ├── Data/
│   │   ├── Entities/          # EF Core models
│   │   ├── Migrations/        # Database migrations
│   │   └── Seeders/           # Admin user and roles seeder
│   └── Setup/                 # JWT, Identity, Auth, Email configuration
│
├── Eventing.Web/              # Blazor Server frontend
│   ├── Components/
│   │   ├── Layout/            # MainLayout, DashboardLayout, AdminDashboardLayout
│   │   └── Pages/             # Home page
│   └── Features/
│       ├── Login/             # Sign in page
│       ├── Register/          # Create account page
│       ├── Events/            # Public events listing
│       ├── Dashboard/
│       │   ├── Admin/         # Admin overview, event management, attendees
│       │   └── General/       # User dashboard, browse events
│       ├── Ticket/            # Digital ticket view and print
│       ├── About/             # About page
│       └── Contact/           # Contact page
│
└── Eventing.ServiceDefaults/  # Shared .NET Aspire service defaults
```

---

## ⚙️ Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

### 1. Clone the repository

```bash
git clone https://github.com/yourusername/event-management.git
cd event-management
```

### 2. Configure the API

Edit `Eventing.ApiService/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=eventing_db;Username=postgres;Password=YOUR_PASSWORD"
  },
  "Jwt": {
    "Audience": "https://yourdomain.com",
    "Issuer": "https://api.yourdomain.com",
    "ExpiryInMinutes": 60,
    "SigningKey": "your-secret-signing-key-min-32-chars"
  },
  "Smtp": {
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "MeetupX",
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-gmail-app-password"
  }
}
```

### 3. Configure the Web frontend

Edit `Eventing.Web/appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5000/"
  }
}
```

### 4. Apply database migrations

```bash
cd Eventing.ApiService
dotnet ef database update
```

### 5. Run the projects

```bash
# Terminal 1 — API
cd Eventing.ApiService
dotnet run

# Terminal 2 — Web
cd Eventing.Web
dotnet run
```

The API runs on `http://localhost:5000` and the web app on `http://localhost:5001`.

---

## 🔑 Default Admin Credentials

| Field | Value |
|---|---|
| Email | `admin@meetupx.com` |
| Password | `Admin@12345` |

> Change the password after first login.

---

## 🌐 IIS Deployment

1. Publish both projects:
```powershell
dotnet publish Eventing.ApiService -c Release -o C:\inetpub\wwwroot\EventManagementSystem
dotnet publish Eventing.Web -c Release -o C:\inetpub\wwwroot\Eventoxx
```

2. Create two IIS applications under Default Web Site:
   - `EventManagementSystem` → API
   - `Eventoxx` → Web frontend

3. Set both app pools to **No Managed Code**

4. Grant permissions:
```powershell
icacls "C:\inetpub\wwwroot\EventManagementSystem" /grant "IIS AppPool\YourApiPool:(OI)(CI)F" /T
icacls "C:\inetpub\wwwroot\Eventoxx" /grant "IIS AppPool\YourWebPool:(OI)(CI)F" /T
```

5. Update `Eventing.Web/appsettings.json`:
```json
"ApiSettings": {
  "BaseUrl": "http://yourdomain.com/EventManagementSystem/"
}
```

---

## 📸 Screenshots

| Page | Description |
|---|---|
| Home | Landing page with event calendar |
| Events | Public event listing with search/filter |
| Login | JWT authentication |
| Admin Dashboard | Event and attendee management |
| User Dashboard | My registrations and tickets |
| Digital Ticket | Printable ticket with QR code |

---

## 📄 License

MIT License — free to use and modify.

---

## 👤 Author

Built with ❤️ using .NET 9, Blazor, and PostgreSQL.
