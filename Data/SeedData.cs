using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Hairstories.Models;

namespace Hairstories.Data
{
    /// <summary>
    /// Database seeding service for initial data population
    /// </summary>
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Create roles
            await CreateRolesAsync(roleManager);

            // Seed users and staff
            await SeedUsersAndStaffAsync(userManager, context);

            // Seed customers
            await SeedCustomersAsync(context);

            // Seed services
            await SeedServicesAsync(context);

            // Seed staff services
            await SeedStaffServicesAsync(context);

            // Seed appointments
            await SeedAppointmentsAsync(context);
        }

        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Admin", "Receptionist", "Stylist" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedUsersAndStaffAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            // Admin user
            var adminUser = await userManager.FindByEmailAsync("admin@salon.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@salon.com",
                    Email = "admin@salon.com",
                    FirstName = "John",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");

                var adminStaff = new Staff
                {
                    FirstName = "John",
                    LastName = "Admin",
                    Email = "admin@salon.com",
                    Phone = "1234567890",
                    Role = StaffRole.Admin,
                    WorkingHours = "9:00 AM - 6:00 PM",
                    IsActive = true,
                    ApplicationUserId = adminUser.Id,
                    CreatedDate = DateTime.Now
                };

                context.Staff.Add(adminStaff);
            }

            // Receptionist user
            var receptionistUser = await userManager.FindByEmailAsync("receptionist@salon.com");
            if (receptionistUser == null)
            {
                receptionistUser = new ApplicationUser
                {
                    UserName = "receptionist@salon.com",
                    Email = "receptionist@salon.com",
                    FirstName = "Sarah",
                    LastName = "Receptionist",
                    EmailConfirmed = true,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                await userManager.CreateAsync(receptionistUser, "Receptionist@123");
                await userManager.AddToRoleAsync(receptionistUser, "Receptionist");

                var receptionistStaff = new Staff
                {
                    FirstName = "Sarah",
                    LastName = "Receptionist",
                    Email = "receptionist@salon.com",
                    Phone = "0987654321",
                    Role = StaffRole.Receptionist,
                    WorkingHours = "9:00 AM - 6:00 PM",
                    IsActive = true,
                    ApplicationUserId = receptionistUser.Id,
                    CreatedDate = DateTime.Now
                };

                context.Staff.Add(receptionistStaff);
            }

            // Stylist users
            var stylists = new[]
            {
                new { Email = "emma.stylist@salon.com", First = "Emma", Last = "Johnson", Phone = "5151234567" },
                new { Email = "olivia.stylist@salon.com", First = "Olivia", Last = "Smith", Phone = "5152345678" },
                new { Email = "sophia.stylist@salon.com", First = "Sophia", Last = "Williams", Phone = "5153456789" }
            };

            foreach (var stylist in stylists)
            {
                var stylistUser = await userManager.FindByEmailAsync(stylist.Email);
                if (stylistUser == null)
                {
                    stylistUser = new ApplicationUser
                    {
                        UserName = stylist.Email,
                        Email = stylist.Email,
                        FirstName = stylist.First,
                        LastName = stylist.Last,
                        EmailConfirmed = true,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };

                    await userManager.CreateAsync(stylistUser, "Stylist@123");
                    await userManager.AddToRoleAsync(stylistUser, "Stylist");

                    var stylistStaff = new Staff
                    {
                        FirstName = stylist.First,
                        LastName = stylist.Last,
                        Email = stylist.Email,
                        Phone = stylist.Phone,
                        Role = StaffRole.Stylist,
                        WorkingHours = "10:00 AM - 7:00 PM",
                        IsActive = true,
                        ApplicationUserId = stylistUser.Id,
                        CreatedDate = DateTime.Now
                    };

                    context.Staff.Add(stylistStaff);
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedCustomersAsync(ApplicationDbContext context)
        {
            if (!context.Customers.Any())
            {
                var customers = new Customer[]
                {
                    new Customer
                    {
                        FirstName = "Alice",
                        LastName = "Brown",
                        Email = "alice@email.com",
                        Phone = "5551234567",
                        Notes = "Prefers short hairs",
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Customer
                    {
                        FirstName = "Bob",
                        LastName = "Davis",
                        Email = "bob@email.com",
                        Phone = "5552345678",
                        Notes = "Allergic to certain shampoos",
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Customer
                    {
                        FirstName = "Carol",
                        LastName = "Miller",
                        Email = "carol@email.com",
                        Phone = "5553456789",
                        Notes = "Regular customer",
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Customer
                    {
                        FirstName = "Diana",
                        LastName = "Wilson",
                        Email = "diana@email.com",
                        Phone = "5554567890",
                        Notes = "Special treatment required",
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    }
                };

                context.Customers.AddRange(customers);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedServicesAsync(ApplicationDbContext context)
        {
            if (!context.Services.Any())
            {
                var services = new Service[]
                {
                    new Service
                    {
                        ServiceName = "Hair Cut",
                        Description = "Professional hair cutting service",
                        DurationMinutes = 30,
                        Price = 35.00m,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Service
                    {
                        ServiceName = "Hair Styling",
                        Description = "Professional hair styling service",
                        DurationMinutes = 45,
                        Price = 50.00m,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Service
                    {
                        ServiceName = "Hair Color",
                        Description = "Hair coloring service",
                        DurationMinutes = 60,
                        Price = 75.00m,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Service
                    {
                        ServiceName = "Facial Treatment",
                        Description = "Relaxing facial treatment",
                        DurationMinutes = 45,
                        Price = 60.00m,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Service
                    {
                        ServiceName = "Manicure",
                        Description = "Professional manicure service",
                        DurationMinutes = 30,
                        Price = 25.00m,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Service
                    {
                        ServiceName = "Pedicure",
                        Description = "Professional pedicure service",
                        DurationMinutes = 30,
                        Price = 30.00m,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    }
                };

                context.Services.AddRange(services);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedStaffServicesAsync(ApplicationDbContext context)
        {
            if (!context.StaffServices.Any())
            {
                var staff = await context.Staff.Where(s => s.Role == StaffRole.Stylist).ToListAsync();
                var services = await context.Services.ToListAsync();

                if (staff.Any() && services.Any())
                {
                    var staffServices = new List<StaffService>();

                    // Assign services to stylists
                    foreach (var s in staff)
                    {
                        // Each stylist can perform Hair Cut, Hair Styling, and Hair Color
                        staffServices.Add(new StaffService
                        {
                            StaffId = s.Id,
                            ServiceId = services.First(svc => svc.ServiceName == "Hair Cut").Id,
                            AssignedDate = DateTime.Now
                        });

                        staffServices.Add(new StaffService
                        {
                            StaffId = s.Id,
                            ServiceId = services.First(svc => svc.ServiceName == "Hair Styling").Id,
                            AssignedDate = DateTime.Now
                        });

                        staffServices.Add(new StaffService
                        {
                            StaffId = s.Id,
                            ServiceId = services.First(svc => svc.ServiceName == "Hair Color").Id,
                            AssignedDate = DateTime.Now
                        });
                    }

                    context.StaffServices.AddRange(staffServices);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedAppointmentsAsync(ApplicationDbContext context)
        {
            if (!context.Appointments.Any())
            {
                var customers = await context.Customers.ToListAsync();
                var staff = await context.Staff.Where(s => s.Role == StaffRole.Stylist).ToListAsync();
                var services = await context.Services.ToListAsync();

                if (customers.Any() && staff.Any() && services.Any())
                {
                    var appointments = new List<Appointment>();

                    // Create sample appointments
                    var baseDate = DateTime.Now.Date.AddDays(1);

                    // Tomorrow's appointments
                    appointments.Add(new Appointment
                    {
                        CustomerId = customers[0].Id,
                        ServiceId = services.First(s => s.ServiceName == "Hair Cut").Id,
                        StaffId = staff[0].Id,
                        AppointmentDateTime = baseDate.AddHours(10),
                        Status = AppointmentStatus.Scheduled,
                        PaymentStatus = PaymentStatus.Pending,
                        Notes = "First time customer",
                        CreatedDate = DateTime.Now
                    });

                    appointments.Add(new Appointment
                    {
                        CustomerId = customers[1].Id,
                        ServiceId = services.First(s => s.ServiceName == "Hair Styling").Id,
                        StaffId = staff[1].Id,
                        AppointmentDateTime = baseDate.AddHours(11),
                        Status = AppointmentStatus.Scheduled,
                        PaymentStatus = PaymentStatus.Pending,
                        Notes = "Regular customer",
                        CreatedDate = DateTime.Now
                    });

                    appointments.Add(new Appointment
                    {
                        CustomerId = customers[2].Id,
                        ServiceId = services.First(s => s.ServiceName == "Hair Color").Id,
                        StaffId = staff[2].Id,
                        AppointmentDateTime = baseDate.AddHours(14),
                        Status = AppointmentStatus.Scheduled,
                        PaymentStatus = PaymentStatus.Pending,
                        Notes = "Color touch up",
                        CreatedDate = DateTime.Now
                    });

                    // Today's completed appointments
                    var today = DateTime.Now.Date;
                    appointments.Add(new Appointment
                    {
                        CustomerId = customers[3].Id,
                        ServiceId = services.First(s => s.ServiceName == "Facial Treatment").Id,
                        StaffId = staff[0].Id,
                        AppointmentDateTime = today.AddHours(15),
                        Status = AppointmentStatus.Completed,
                        PaymentStatus = PaymentStatus.Paid,
                        Notes = "Completed",
                        CreatedDate = DateTime.Now.AddHours(-2)
                    });

                    context.Appointments.AddRange(appointments);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
