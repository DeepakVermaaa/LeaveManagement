using LeaveManagement.Data;
using LeaveManagement.Models;
using LeaveManagement.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MySqlConnection"),
    new MySqlServerVersion(new Version(8, 0, 36))));

// Register the IUserRepository,ILeaveRepository, and ILeaveLimitRepository with the DI container
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
builder.Services.AddScoped<ILeaveLimitRepository, LeaveLimitRepository>();


var app = builder.Build();

// Seed the admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    try
    {
        var userRepository = services.GetRequiredService<IUserRepository>();
        await SeedAdminUser(userRepository, builder.Configuration);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Welcome}/{action=Welcome}");

app.MapControllerRoute(
    name: "EmployeeDashboard",
    pattern: "UserRegistration/EmployeeDashboard",
    defaults: new { controller = "UserRegistration", action = "EmployeeDashboard" }
);

app.Run();

async Task SeedAdminUser(IUserRepository userRepository, IConfiguration configuration)
{
    var adminUser = configuration.GetSection("AdminUser").Get<User>();
    if (await userRepository.GetByUsernameAsync(adminUser.Username) == null)
    {
        var user = new User
        {
            Username = adminUser.Username,
            Password = adminUser.Password,
            Role = adminUser.Role
        };
        await userRepository.AddAsync(user);
    }
}
