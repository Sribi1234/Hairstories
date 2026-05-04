using Microsoft.EntityFrameworkCore;
using Hairstories.Data;
using Hairstories.Models;
using System.Linq.Expressions;

namespace Hairstories.Data.Repositories
{
    /// <summary>
    /// Generic repository pattern interface
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "",
            int? take = null);
        Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveAsync();
    }

    /// <summary>
    /// Generic repository implementation
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "",
            int? take = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<List<T>> GetListAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "",
            int? take = null)
        {
            var items = await GetAllAsync(filter, orderBy, includeProperties, take);
            return items.ToList();
        }

        public async Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate)
        {
            return await Task.FromResult(_dbSet.Where(predicate).ToList());
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Staff-specific repository
    /// </summary>
    public interface IStaffRepository : IRepository<Staff>
    {
        Task<Staff?> GetStaffByEmailAsync(string email);
        Task<IEnumerable<Staff>> GetActiveStaffAsync();
        Task<IEnumerable<Staff>> GetStylistsAsync();
        Task<Staff?> GetStaffWithServicesAsync(int id);
    }

    public class StaffRepository : Repository<Staff>, IStaffRepository
    {
        private readonly ApplicationDbContext _context;

        public StaffRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Staff?> GetStaffByEmailAsync(string email)
        {
            return await _context.Staff.FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<IEnumerable<Staff>> GetActiveStaffAsync()
        {
            return await _context.Staff.Where(s => s.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Staff>> GetStylistsAsync()
        {
            return await _context.Staff
                .Where(s => s.IsActive && s.Role == StaffRole.Stylist)
                .ToListAsync();
        }

        public async Task<Staff?> GetStaffWithServicesAsync(int id)
        {
            return await _context.Staff
                .Include(s => s.StaffServices)
                .ThenInclude(ss => ss.Service)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }

    /// <summary>
    /// Appointment-specific repository
    /// </summary>
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<bool> IsTimeSlotAvailableAsync(int staffId, DateTime startTime, int durationMinutes, int? excludeAppointmentId = null);
        Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date);
        Task<IEnumerable<Appointment>> GetTodayAppointmentsAsync();
        Task<IEnumerable<Appointment>> GetCustomerAppointmentsAsync(int customerId);
        Task<IEnumerable<Appointment>> GetStaffAppointmentsAsync(int staffId);
        Task<decimal> GetTodayRevenueAsync();
        Task<int> GetTodayBookingCountAsync();
    }

    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int staffId, DateTime startTime, int durationMinutes, int? excludeAppointmentId = null)
        {
            var endTime = startTime.AddMinutes(durationMinutes);

            var conflicts = await _context.Appointments
                .Where(a => a.StaffId == staffId &&
                            a.Status != AppointmentStatus.Cancelled &&
                            a.AppointmentDateTime < endTime &&
                            a.AppointmentEndTime > startTime &&
                            (excludeAppointmentId == null || a.Id != excludeAppointmentId))
                .CountAsync();

            return conflicts == 0;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            return await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .Include(a => a.Staff)
                .Where(a => a.AppointmentDateTime >= startOfDay && a.AppointmentDateTime <= endOfDay)
                .OrderBy(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetTodayAppointmentsAsync()
        {
            return await GetAppointmentsByDateAsync(DateTime.Now);
        }

        public async Task<IEnumerable<Appointment>> GetCustomerAppointmentsAsync(int customerId)
        {
            return await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Staff)
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetStaffAppointmentsAsync(int staffId)
        {
            return await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .Where(a => a.StaffId == staffId && a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        //public async Task<decimal> GetTodayRevenueAsync()
        //{
        //    var today = DateTime.Now.Date;
        //    var endOfDay = today.AddDays(1).AddTicks(-1);

        //    return await _context.Appointments
        //        .Where(a => a.AppointmentDateTime >= today &&
        //                    a.AppointmentDateTime <= endOfDay &&
        //                    a.Status == AppointmentStatus.Completed &&
        //                    a.PaymentStatus == PaymentStatus.Paid)
        //        .SumAsync(a => a.ServicePrice);
        //}
        public async Task<decimal> GetTodayRevenueAsync()
        {
            var today = DateTime.Today;
            var endOfDay = today.AddDays(1).AddTicks(-1);

            return await _context.Appointments
                .Where(a => a.AppointmentDateTime >= today
                            && a.AppointmentDateTime <= endOfDay
                            && a.Status == AppointmentStatus.Completed
                            && a.PaymentStatus == PaymentStatus.Paid)
                .Select(a => a.Service != null ? a.Service.Price : 0m)
                .SumAsync();
        }

        public async Task<int> GetTodayBookingCountAsync()
        {
            var today = DateTime.Now.Date;
            var endOfDay = today.AddDays(1).AddTicks(-1);

            return await _context.Appointments
                .CountAsync(a => a.AppointmentDateTime >= today &&
                                 a.AppointmentDateTime <= endOfDay &&
                                 a.Status != AppointmentStatus.Cancelled);
        }
    }

    /// <summary>
    /// Customer-specific repository
    /// </summary>
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task<IEnumerable<Customer>> GetActiveCustomersAsync();
        Task<Customer?> GetCustomerWithHistoryAsync(int id);
    }

    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
        {
            return await _context.Customers.Where(c => c.IsActive).ToListAsync();
        }

        public async Task<Customer?> GetCustomerWithHistoryAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Appointments)
                .ThenInclude(a => a.Service)
                .Include(c => c.Appointments)
                .ThenInclude(a => a.Staff)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }

    /// <summary>
    /// Service-specific repository
    /// </summary>
    public interface IServiceRepository : IRepository<Service>
    {
        Task<Service?> GetServiceByNameAsync(string name);
        Task<IEnumerable<Service>> GetActiveServicesAsync();
    }

    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Service?> GetServiceByNameAsync(string name)
        {
            return await _context.Services.FirstOrDefaultAsync(s => s.ServiceName == name);
        }

        public async Task<IEnumerable<Service>> GetActiveServicesAsync()
        {
            return await _context.Services.Where(s => s.IsActive).ToListAsync();
        }
    }
}
