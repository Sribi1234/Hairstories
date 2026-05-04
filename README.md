## Salon Management System

This is a complete ASP.NET Core MVC web application for managing salon operations.

### Features
- Staff Management (Admin, Receptionist, Stylist roles)
- Customer Management with appointment history
- Services Management with pricing and duration
- Appointment/Booking System with double-booking prevention
- Dashboard with key metrics
- Calendar view for scheduling
- Role-based access control

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or Full)
- Visual Studio 2022 (or VS Code with C# extension)

### Getting Started

1. **Clone or navigate to the project folder**
   ```
   cd Hairstories
   ```

2. **Restore dependencies**
   ```
   dotnet restore
   ```

3. **Update the connection string** (if needed)
   - Open `appsettings.json`
   - Modify `DefaultConnection` if you're not using LocalDB

4. **Apply migrations and seed data**
   ```
   dotnet ef database update
   ```
   
   This will create the database and populate it with sample data automatically on first run.

5. **Run the application**
   ```
   dotnet run
   ```

6. **Access the application**
   - Open your browser and navigate to `https://localhost:5001`
   - Login with demo credentials (see login page)

### Demo Credentials

**Admin:**
- Email: admin@salon.com
- Password: Admin@123

**Receptionist:**
- Email: receptionist@salon.com
- Password: Receptionist@123

**Stylist:**
- Email: emma.stylist@salon.com
- Password: Stylist@123

### Project Structure

```
Hairstories/
├── Controllers/           # Application controllers
├── Models/               # Data models and entities
├── Views/                # Razor views for UI
├── Data/                 # DbContext and repositories
├── wwwroot/              # Static files (CSS, JS, images)
├── Program.cs            # Application entry point
└── appsettings.json      # Configuration
```

### Technologies Used
- ASP.NET Core 8.0 MVC
- Entity Framework Core
- SQL Server
- Bootstrap 5
- jQuery
- Razor Pages

### Database Schema

The application includes the following main entities:
- **ApplicationUser**: Extended Identity user with additional fields
- **Staff**: Salon employees with roles and assigned services
- **Customer**: Salon customers with contact information
- **Service**: Salon services with pricing and duration
- **Appointment**: Bookings linking customers, services, and staff
- **StaffService**: Junction table for many-to-many relationship

### Key Features Explained

1. **Authentication & Authorization**
   - Login/Logout functionality
   - Role-based access control (Admin, Receptionist, Stylist)
   - Password validation and account lockout

2. **Double Booking Prevention**
   - Checks for time conflicts before confirming appointments
   - Prevents the same stylist from having overlapping appointments

3. **Dashboard**
   - Shows today's bookings, total customers, revenue, and upcoming appointments
   - Quick overview of business metrics

4. **Calendar View**
   - Visual representation of appointments by month
   - Shows appointment count per day
   - Navigation between months

5. **Appointment History**
   - Customers' appointment history visible in customer details
   - Payment status tracking
   - Service history

### Future Enhancements

- Email notifications for appointments
- SMS reminders
- Online payment integration
- Stylist ratings and reviews
- Service package deals
- Mobile app version
- Advance reporting and analytics

### Troubleshooting

**Connection String Issues:**
If you encounter database connection problems, ensure:
1. SQL Server or LocalDB is running
2. Connection string is correct in `appsettings.json`
3. Run `dotnet ef database update` again if migrations failed

**Port Issues:**
If port 5001 is already in use:
1. Open `Properties/launchSettings.json`
2. Change the port number in the applicationUrl

**Entity Framework Issues:**
If database creation fails:
```
dotnet ef database drop --force
dotnet ef database update
```

### Support

For issues or questions, please check the code comments and documentation within each file.

### License

This project is created for educational purposes.
