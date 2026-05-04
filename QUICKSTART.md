# 🚀 Quick Start Checklist

## Pre-Flight Checklist (5 minutes)

- [ ] .NET 8 SDK installed (`dotnet --version`)
- [ ] SQL Server/LocalDB installed and running
- [ ] Visual Studio 2022 (or VS Code)
- [ ] Project extracted/cloned to `C:\Users\sribi\Hairstories`

## Installation Steps (10 minutes)

### Step 1: Open Terminal/PowerShell
```powershell
cd C:\Users\sribi\Hairstories
```

### Step 2: Restore Dependencies
```powershell
dotnet restore
```
⏱️ Expected: 30-60 seconds

### Step 3: Apply Database Migrations
```powershell
dotnet ef database update
```
⏱️ Expected: 10-20 seconds
- Creates `HairstoriesDb` database
- Creates all tables
- Seeds sample data

### Step 4: Run Application
```powershell
dotnet run
```
⏱️ Expected: 5-10 seconds
- Starts server on `https://localhost:5001`
- Auto-opens browser (usually)

### Step 5: Login
- If browser doesn't open, go to: `https://localhost:5001`
- You'll be redirected to login page
- Use demo credentials (see below)

## Demo Login Credentials

### 👨‍💼 Admin (Full Access)
```
Email:    admin@salon.com
Password: Admin@123
```

### 👩‍💼 Receptionist (Appointments & Customers)
```
Email:    receptionist@salon.com
Password: Receptionist@123
```

### 💇 Stylist (View Own Appointments)
```
Email:    emma.stylist@salon.com
OR        olivia.stylist@salon.com
OR        sophia.stylist@salon.com
Password: Stylist@123
```

## What to Test First

### 1. Login (2 min)
- [ ] Try admin login
- [ ] Try receptionist login
- [ ] Try stylist login

### 2. Dashboard (1 min)
- [ ] Verify metrics display
- [ ] Check upcoming appointments

### 3. Staff Management (3 min)
- [ ] View staff list (Admin)
- [ ] Add new staff
- [ ] Edit staff
- [ ] Assign services to stylist

### 4. Customer Management (3 min)
- [ ] View customers
- [ ] Add new customer
- [ ] Edit customer
- [ ] View customer history

### 5. Services Management (2 min)
- [ ] View services
- [ ] Add new service
- [ ] Edit service

### 6. Appointments (5 min)
- [ ] View appointments by date
- [ ] Book new appointment
- [ ] Check double-booking prevention
- [ ] View calendar
- [ ] Edit appointment
- [ ] Cancel appointment

### 7. Authorization (2 min)
- [ ] Logout as admin
- [ ] Login as receptionist
- [ ] Verify Staff menu is hidden
- [ ] Verify Services menu is hidden
- [ ] Verify Services can see Customers & Appointments

## Troubleshooting Quick Fixes

### ❌ "Cannot open database"
```powershell
# Option 1: Recreate database
dotnet ef database drop --force
dotnet ef database update

# Option 2: Check LocalDB is running
sqllocaldb start mssqllocaldb
```

### ❌ "Port 5000/5001 already in use"
- Edit: `Properties/launchSettings.json`
- Change ports to 5002/5003
- Run: `dotnet run`

### ❌ "Certificate error"
```powershell
dotnet dev-certs https --trust
```

### ❌ "Login fails with correct credentials"
```powershell
# Reseed data
dotnet ef database drop --force
dotnet ef database update
```

## File Structure Quick Reference

```
Hairstories/
├── Controllers/          ← C# Controller classes
├── Views/                ← HTML + Razor templates
├── Models/               ← Database entities
├── Data/                 ← DbContext & Repositories
├── Migrations/           ← Database migrations
├── Properties/           ← Launch settings
├── wwwroot/              ← CSS, JS, static files
├── Program.cs            ← Application startup
├── appsettings.json      ← Config file
└── README.md             ← Documentation
```

## Common Commands Reference

```powershell
# Restore packages
dotnet restore

# Build project
dotnet build

# Run application
dotnet run

# Apply migrations
dotnet ef database update

# Create new migration
dotnet ef migrations add MigrationName

# Drop database (careful!)
dotnet ef database drop

# View database
# Use SQL Server Management Studio (SSMS)
# Or Visual Studio Server Explorer
# Database: HairstoriesDb
```

## Production Checklist

Before deploying to production:

- [ ] Change all demo credentials
- [ ] Update connection string
- [ ] Enable HTTPS with real certificate
- [ ] Set ASPNETCORE_ENVIRONMENT to Production
- [ ] Configure logging appropriately
- [ ] Test all features in staging
- [ ] Set up database backups
- [ ] Create admin user for production
- [ ] Update appsettings.json for production

## Performance Tips

1. **For large datasets**: Add pagination to lists
2. **For slow queries**: Check database indexes
3. **For memory issues**: Implement caching
4. **For UI performance**: Use CDN for Bootstrap/jQuery

## Development Tips

1. **Hot Reload**: Use `dotnet watch run` for auto-restart on changes
2. **Database Access**: Use Visual Studio's "Server Explorer"
3. **Debugging**: Press F5 to run with debugger
4. **Logging**: Check console output for error details
5. **API Testing**: Use "Postman" or "Insomnia"

## Next Steps After Setup

1. **Read Documentation**
   - Start with `README.md`
   - Review `SETUP_GUIDE.md`
   - Check `PROJECT_SUMMARY.md`

2. **Explore Codebase**
   - Open solution in Visual Studio
   - Review controller logic
   - Check view templates
   - Study data models

3. **Customize**
   - Add your salon name/logo
   - Modify colors in `_Layout.cshtml`
   - Update company info
   - Add more services/staff

4. **Extend Features**
   - Add email notifications
   - Implement SMS reminders
   - Add payment processing
   - Create reports/analytics

## Support Resources

- **ASP.NET Core Docs**: https://docs.microsoft.com/aspnet/core
- **Entity Framework**: https://docs.microsoft.com/ef/core
- **Bootstrap**: https://getbootstrap.com/docs
- **Visual Studio Help**: Help > Microsoft Docs

---

## ✅ You're Ready!

Your complete Salon Management System is ready to use. 

**Total setup time: ~15-20 minutes**

Start with the Dashboard and explore all features. Use the demo credentials and sample data to familiarize yourself with the system.

**Questions?** Check the documentation files or review the inline code comments.

**Happy coding!** 🎉
