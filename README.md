# 🚗 Car Store API

**Car Store API** is a clean and modular RESTful API for managing a car store system. It allows you to perform CRUD operations on cars and users. The project follows Clean Architecture principles and is built using **ASP.NET Core**.

---

## 📦 Features

- 🔄 Full CRUD operations for cars and users
- 🧱 Clean Architecture (Domain, Application, Infrastructure, API)
- 🌐 RESTful API design
- 🧩 Layered and scalable project structure
- 🔐 JWT authentication support
- 📧 Email confirmation via SMTP

---

## 🛠️ Tech Stack

- **Language:** C#
- **Framework:** ASP.NET Core
- **Architecture:** Clean Architecture
- **Database:** (SQL Server)
- **ORM:** Entity Framework Core
- **Tools:** Swagger (recommended), Visual Studio / VS Code

---

## 📁 Project Structure

```
Car-Store-API/
├── CarStore.API/            # Presentation layer (controllers, startup config)
├── CarStore.Application/    # Business logic and use cases
├── CarStore.Domain/         # Entities and interfaces
├── CarStore.Infrastructure/ # Database and external services
├── CarStore.sln             # Solution file
└── README.md                # Project documentation
```

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/MahmoudSami0/Car-Store-API.git
```

### 2. Open the Project

Open `CarStore.sln` using **Visual Studio 2022** or later.

### 3. Add Configuration File

Create `appsettings.json` inside the `CarStore.API/` project.

### 4. Configure the Database

Update the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "constr": "Your-Connection-String-Here"
}
```

### 5. Configure JWT

Add JWT settings:

```json
"JwtSettings": {
  "SecretKey": "YourSecretKey",
  "Issuer": "YourIssuer",
  "Audience": "YourAudience",
  "DurationInMinutes": 60
}
```

### 6. Configure SMTP for Email

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SmtpUsername": "example@gmail.com",
  "SmtpPassword": "your-app-password",
  "FromName": "App Name",
  "FromAddress": "example@gmail.com",
  "EnableSsl": true,
  "ConfirmationUrl": "https://YourDomain/api/Auth/confirm-email"
}
```

### 7. Configure AI Settings

```json
"AISettings": {
    "ApiKey": "Your Api Key",
    "OpenRouterUrl": "AI Api URL",
    "Model": "Your AI Model"
  }
```

### 8. Apply Migrations

From the CLI:

```bash
cd CarStore.Infrastructure
dotnet ef database update
```

Or from **Package Manager Console** (ensure `CarStore.Infrastructure` is the startup project):

```powershell
Update-Database
```

### 9. Run the Application

```bash
dotnet run --project CarStore.API
```

The API will start on `https://localhost:5001` or `http://localhost:5000`.

---

## 📬 API Endpoints (Examples)

| Method | Endpoint         | Description        |
|--------|------------------|--------------------|
| GET    | `/api/cars`      | Get all cars       |
| GET    | `/api/cars/{id}` | Get a car by ID    |
| POST   | `/api/cars`      | Create a new car   |
| PUT    | `/api/cars/{id}` | Update a car       |
| DELETE | `/api/cars/{id}` | Delete a car       |

---

## 🤝 Contribution

Contributions are welcome! Feel free to fork the repository and submit pull requests.

1. Fork the repository  
2. Create a new branch  
   ```bash
   git checkout -b feature/your-feature
   ```
3. Commit your changes  
4. Push to your branch  
   ```bash
   git push origin feature/your-feature
   ```
5. Open a pull request

---

## 👤 Author

**Mahmoud Sami**  
GitHub: [@MahmoudSami0](https://github.com/MahmoudSami0)  
Email: ms4805727@gmail.com

---
