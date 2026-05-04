# ASP.NET Core Salon Management System - Installation & Setup Guide

## Complete Setup Instructions

### Prerequisites
- .NET 8 SDK (or latest)
- SQL Server 2019+ or SQL Server Express with LocalDB
- Visual Studio 2022 (Community, Professional, or Enterprise)
- Git (optional but recommended)

### Step 1: Clone/Extract the Project

```bash
# If using Git
git clone <repository-url>
cd Hairstories

# Or navigate directly to the project folder
cd c:\Users\sribi\Hairstories
```

### Step 2: Restore NuGet Packages

Open PowerShell or Command Prompt in the project directory and run:

```powershell
dotnet restore
```

Or in Visual Studio:
- Tools > NuGet Package Manager > Manage NuGet Packages for Solution
- Click "Restore" button

### Step 3: Configure Database Connection

1. Open `appsettings.json`
2. The default connection string uses LocalDB:
   ```json
   "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HairstoriesDb;Trusted_Connection=true;"
   ```

3. **For SQL Server Express/Full**, modify the connection string:
   ```json
   "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=HairstoriesDb;Trusted_Connection=true;"
   ```

4. **For Azure SQL**, use:
   ```json
   "DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=HairstoriesDb;Persist Security Info=False;User ID=username;Password=password;MultipleActiveResultSets=False;"
   ```

### Step 4: Apply Database Migrations

The application automatically applies migrations on startup, but you can manually apply them:

**Using .NET CLI:**
```powershell
dotnet ef database update
```

**Using Visual Studio Package Manager Console:**
```powershell
Update-Database
```

### Step 5: Run the Application

**Using .NET CLI:**
```powershell
dotnet run
```

**Using Visual Studio:**
1. Press `F5` or click "Start" button
2. Application opens in your default browser at `https://localhost:5001`

### Step 6: Access the Application

1. Navigate to: `https://localhost:5001`
2. You will be redirected to the login page
3. Use one of the demo credentials

## Demo Credentials

### Admin Account
```
Email: admin@salon.com
Password: Admin@123
Permissions: All modules
```

### Receptionist Account
```
Email: receptionist@salon.com
Password: Receptionist@123
Permissions: Appointments, Customers
```

### Stylists (3 available)
```
Email: emma.stylist@salon.com
Password: Stylist@123

Email: olivia.stylist@salon.com
Password: Stylist@123

Email: sophia.stylist@salon.com
Password: Stylist@123

Permissions: View own appointments
```

## Database Schema Overview

The application uses Entity Framework Core with the following main tables:

### AspNetUsers (Identity)
- Stores user authentication information
- Extended with FirstName, LastName, IsActive

### Staff
- Employee information
- Roles: Admin, Receptionist, Stylist
- Linked to AspNetUsers

### Customers
- Customer contact information
- Email and phone for notifications
- Notes for special instructions

### Services
- Service offerings with pricing
- Duration in minutes
- Description

### Appointments
- Links Customer, Service, and Staff
- Booking date/time
- Status: Scheduled, Completed, Cancelled, NoShow
- Payment Status: Pending, Paid, Refunded

### StaffServices (Junction Table)
- Many-to-many relationship between Staff and Services
- Tracks which stylists can perform which services

## Key Features Walkthrough

### Login & Authentication
1. Navigate to home page (redirects to login)
2. Enter demo credentials
3. Password must meet complexity requirements

### Dashboard
1. Shows key metrics: Today's bookings, total customers, revenue, upcoming appointments
2. Quick overview for managers
3. Shows next 5 appointments

### Staff Management (Admin Only)
1. **View Staff**: List all staff members with details
2. **Add Staff**: Create new staff profiles
3. **Edit Staff**: Update contact and role information
4. **Assign Services**: Manage which services each stylist can perform
5. **Delete Staff**: Remove staff (soft delete via IsActive flag)

### Customer Management (Admin/Receptionist)
1. **View Customers**: Browse all customer profiles
2. **Add Customer**: Create new customer records
3. **Edit Customer**: Update customer information
4. **View History**: See appointment history per customer
5. **Delete Customer**: Remove customer records

### Services Management (Admin Only)
1. **View Services**: List all salon services
2. **Add Service**: Create new service with pricing and duration
3. **Edit Service**: Update service details
4. **Delete Service**: Remove services from catalog

### Appointments (All Authenticated Users)
1. **View Appointments**: List view by date
2. **Book Appointment**: Create new appointment
   - Select customer, service, stylist, and time
   - System prevents double-booking
3. **Edit Appointment**: Modify existing appointment
4. **Cancel Appointment**: Mark as cancelled
5. **Mark as Paid**: Update payment status
6. **Calendar View**: Visual calendar of all appointments

## Troubleshooting

### Database Connection Issues

**Error: "Cannot open database 'HairstoriesDb'"**
- Solution 1: Ensure LocalDB is running
  ```powershell
  # Check LocalDB status
  sqllocaldb info
  
  # Start LocalDB (Windows only)
  sqllocaldb start mssqllocaldb
  ```
- Solution 2: Drop and recreate database
  ```powershell
  dotnet ef database drop --force
  dotnet ef database update
  ```

### Port Already in Use

**Error: "Cannot bind to http://*:5000. Address already in use."**
- Solution: Change port in `Properties/launchSettings.json`
  ```json
  "applicationUrl": "https://localhost:5002;http://localhost:5001"
  ```

### Certificate Issues (HTTPS)

**Error: "Invalid certificate" or SSL errors**
- Solution: Trust development certificate
  ```powershell
  dotnet dev-certs https --trust
  ```

### Package/Dependency Issues

**Error: Package version conflicts**
- Solution: Clear NuGet cache and restore
  ```powershell
  dotnet nuget locals all --clear
  dotnet restore
  ```

### Login Issues

**Error: "Invalid login attempt" with correct credentials**
- Verify user exists in database: Check SeededData ran successfully
- Check user role assignments in AspNetUserRoles table
- Ensure account is not locked (too many failed attempts)

## Project Structure

```
Hairstories/
├── Controllers/
│   ├── HomeController.cs          # Dashboard
│   ├── AccountController.cs       # Login/Logout
│   ├── StaffController.cs         # Staff CRUD
│   ├── CustomersController.cs     # Customer CRUD
│   ├── ServicesController.cs      # Service CRUD
│   └── AppointmentsController.cs  # Appointment CRUD
│
├── Models/
│   ├── ApplicationUser.cs         # Extended Identity User
│   ├── Staff.cs                   # Staff entity
│   ├── Customer.cs                # Customer entity
│   ├── Service.cs                 # Service entity
│   ├── Appointment.cs             # Appointment entity
│   └── StaffService.cs            # Junction table
│
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml         # Main layout
│   │   └── Error.cshtml           # Error page
│   ├── Home/
│   │   ├── Index.cshtml           # Dashboard
│   │   └── Privacy.cshtml         # Privacy policy
│   ├── Account/
│   │   ├── Login.cshtml           # Login form
│   │   ├── AccessDenied.cshtml    # Permission denied
│   │   └── Lockout.cshtml         # Account lockout
│   ├── Staff/                     # Staff views
│   ├── Customers/                 # Customer views
│   ├── Services/                  # Service views
│   └── Appointments/              # Appointment views
│
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext
│   ├── RepositoryPattern.cs       # Repository interfaces/implementations
│   └── SeedData.cs                # Initial data seeding
│
├── Migrations/
│   └── [EF Core migrations]
│
├── Properties/
│   └── launchSettings.json        # Application launch configuration
│
├── wwwroot/
│   ├── css/                       # Custom stylesheets
│   └── js/                        # Custom JavaScript
│
├── Program.cs                      # Application entry point
├── appsettings.json               # Application configuration
├── Hairstories.csproj               # Project file
└── README.md                       # Project documentation
```

## Code Best Practices Used

1. **Layered Architecture**: Models, Controllers, Views, Data Access
2. **Repository Pattern**: Abstraction for data access
3. **Dependency Injection**: Constructor-based injection
4. **Entity Framework Code First**: Migrations and DbContext
5. **Validation**: Data annotations and model validation
6. **Error Handling**: Try-catch blocks with logging
7. **Security**: Role-based authorization, password hashing
8. **Responsive UI**: Bootstrap 5 responsive design

## Adding New Features

### Adding a New Entity

1. Create model class in `Models/`
2. Add DbSet to `ApplicationDbContext`
3. Create migration: `dotnet ef migrations add FeatureName`
4. Update database: `dotnet ef database update`
5. Create repository and controller
6. Create CRUD views

### Creating a New Controller

1. Create controller class inheriting from `Controller`
2. Add `[Authorize]` attribute for protection
3. Implement action methods
4. Create corresponding views
5. Add menu link in `_Layout.cshtml`

### Adding New Validation

1. Add data annotations to model properties
2. Use ModelState.IsValid in controller
3. Display validation messages in views using `asp-validation-for`

## Performance Optimization Tips

1. **Database**: Use indexes for frequently queried columns (already configured)
2. **Queries**: Use `Include()` for related data to prevent N+1 queries
3. **Caching**: Implement caching for reference data (Services, Staff)
4. **Pagination**: Implement paging for large lists
5. **Lazy Loading**: Use async/await for async operations

## Security Considerations

1. **HTTPS**: Always use HTTPS in production
2. **Password Policy**: Strong passwords enforced (8+ chars, uppercase, numbers, special chars)
3. **Login Lockout**: Account locked after 5 failed attempts
4. **Input Validation**: All inputs validated server-side
5. **SQL Injection**: Protected via EF Core parameterized queries
6. **XSS Protection**: Razor escapes output by default
7. **CSRF Protection**: AntiForgeryToken on all POST forms

## Deployment to Production

### IIS Deployment
1. Publish application: `dotnet publish -c Release`
2. Create IIS application pool (.NET CLR version: No Managed Code)
3. Create website pointing to published folder
4. Configure connection string for production database
5. Set ASPNETCORE_ENVIRONMENT to Production

### Azure Deployment
1. Create Azure App Service
2. Create Azure SQL Database
3. Publish from Visual Studio: Right-click project > Publish

## Support and Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Bootstrap Documentation](https://getbootstrap.com/docs)
- [jQuery Documentation](https://api.jquery.com)

## License and Credits

This project is provided as-is for educational and development purposes.

---

**Last Updated**: April 2024
**Version**: 1.0.0
**Author**: Senior ASP.NET Core Developer
