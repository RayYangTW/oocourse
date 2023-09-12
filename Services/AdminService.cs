using AutoMapper;
using personal_project.Data;
using personal_project.Helpers;
using personal_project.Models.Domain;
using personal_project.Models.FormModels;
using personal_project.Models.ResultModels;

namespace personal_project.Services
{
  public class AdminService : IAdminService
  {
    private readonly ILogger<AdminService> _logger;
    private readonly WebDbContext _db;
    private readonly IMapper _mapper;
    private readonly GetUserDataFromJWTHelper _jwtHelper;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;

    public AdminService(ILogger<AdminService> logger, WebDbContext db, IMapper mapper, GetUserDataFromJWTHelper jwtHelper, IConfiguration config, IEmailService emailService)
    {
      _logger = logger;
      _db = db;
      _mapper = mapper;
      _jwtHelper = jwtHelper;
      _config = config;
      _emailService = emailService;
    }
    public async Task<object> GetAllUnapprovedTeacherApplicationsAsync()
    {
      try
      {
        var applications = await _db.TeacherApplications
            .Where(data => data.isApproved == false && data.status == "unapproved")
            .OrderBy(data => data.createdTime)
            .Select(data => new
            {
              id = data.id,
              createdTime = data.createdTime.ToString("yyyy/MM/dd"),
              description = data.description
            })
            .ToListAsync();
        return applications;
      }
      catch (Exception ex)
      {
        return new { message = "Error retrieving teacher applications.", ex.Message };
      }
    }

    public async Task<object> GetTeacherApplicationByIdAsync(long id)
    {
      try
      {
        var application = await _db.TeacherApplications
            .Where(data => data.id == id)
            .Include(data => data.certifications)
            .SingleOrDefaultAsync();

        return application;
      }
      catch (Exception ex)
      {
        return new { message = "Error retrieving teacher application.", ex.Message };
      }
    }

    public async Task<object> ApproveTeacherApplicationAsync(long id)
    {
      var application = await _db.TeacherApplications
                      .Where(data => data.id == id)
                      .FirstOrDefaultAsync();
      if (application is not null)
      {
        application.isApproved = true;
        application.status = "approved";
      }

      var userId = application.userId;
      var user = await _db.Users
                  .Where(data => data.id == userId)
                  .FirstOrDefaultAsync();
      if (user is not null && user.role is not "teacher")
        user.role = "teacher";

      try
      {
        await _db.SaveChangesAsync();
        await _emailService.SendApproveApplicationMailAsync(id);
        return application;
      }
      catch (Exception ex)
      {
        return new { message = "Error retrieving teacher application.", ex.Message };
      }
    }

    public async Task<object> DenyTeacherApplicationAsync(long id)
    {
      var application = await _db.TeacherApplications
                      .Where(data => data.id == id)
                      .FirstOrDefaultAsync();
      if (application is not null)
      {
        application.status = "denied";
      }

      try
      {
        await _db.SaveChangesAsync();
        await _emailService.SendDenyApplicationMailAsync(id);
        return application;
      }
      catch (Exception ex)
      {
        return new { message = "Error retrieving teacher application.", ex.Message };
      }
    }

    public async Task<AdminResult> GetPlatformDataAsync()
    {
      var userData = await _db.Users
                            .Where(data => data.role == "user")
                            .CountAsync();

      var teacherData = await _db.Users
                              .Where(data => data.role == "teacher")
                              .CountAsync();

      var offlineCourseData = await _db.Courses
                                .Where(data => data.teacher.courseWay == "實體" || data.teacher.courseWay == "線下")
                                .CountAsync();

      var onlineCourseData = await _db.Courses
                                .Where(data => data.teacher.courseWay == "線上")
                                .CountAsync();

      var courseAmountData = await _db.Courses
                                      .CountAsync();

      var courseIsBookedData = await _db.Courses
                                        .Where(data => data.isBooked == true)
                                        .CountAsync();

      return new AdminResult
      {
        statusCode = 200,
        data = new
        {
          userData = userData,
          teacherData = teacherData,
          offlineCourseData = offlineCourseData,
          onlineCourseData = onlineCourseData,
          courseAmountData = courseAmountData,
          courseIsBookedData = courseIsBookedData
        }
      };
    }

    public async Task<AdminResult> GetTransactionDataAsync(string start, string end)
    {
      if (!DateTime.TryParse(start, out DateTime startDate) || !DateTime.TryParse(end, out DateTime endDate))
      {
        return new AdminResult
        {
          statusCode = 400,
          message = "Invalid date format."
        };
      }

      var turnoverData = await _db.Courses
                                  .Where(data => data.isBooked == true)
                                  .Where(data => data.startTime >= startDate && data.endTime <= endDate.AddDays(1))
                                  .Where(data => data.bookings.Any(booking => booking.status == "paid"))
                                  .SumAsync(data => data.price);

      var transactionData = await _db.Courses
                                  .Where(data => data.isBooked == true)
                                  .Where(data => data.startTime >= startDate && data.endTime <= endDate.AddDays(1))
                                  .Where(data => data.bookings.Any(booking => booking.status == "paid"))
                                  .CountAsync();

      var revenueData = Math.Round((decimal)(turnoverData) * 0.05M);

      return new AdminResult
      {
        statusCode = 200,
        data = new
        {
          turnoverData = turnoverData,
          transactionData = transactionData,
          revenueData = revenueData
        }
      };
    }

    public async Task<AdminResult> GetCourseDataAsync(string start, string end)
    {
      if (!DateTime.TryParse(start, out DateTime startDate) || !DateTime.TryParse(end, out DateTime endDate))
      {
        return new AdminResult
        {
          statusCode = 400,
          message = "Invalid date format."
        };
      }

      var courseOfferingData = await _db.Courses
                                  .Where(data => data.startTime >= startDate && data.endTime <= endDate.AddDays(1))
                                  .CountAsync();

      var courseFinishedData = await _db.Courses
                                  .Where(data => data.isBooked == true)
                                  .Where(data => data.startTime >= startDate && data.endTime <= endDate.AddDays(1))
                                  .Where(data => data.bookings.Any(booking => booking.status == "paid"))
                                  .CountAsync();


      decimal achievementRate = 0;
      if (courseOfferingData != 0)
      {
        achievementRate = Math.Round(((decimal)courseFinishedData / courseOfferingData), 2);
      }

      return new AdminResult
      {
        statusCode = 200,
        data = new
        {
          courseOfferingData = courseOfferingData,
          courseFinishedData = courseFinishedData,
          achievementRate = achievementRate
        }
      };
    }

    public async Task<List<CourseCategory>> GetCourseCategoriesAsync()
    {
      var categoryData = await _db.CourseCategories
        .OrderBy(data => data.category)
        .ToListAsync();

      return categoryData;
    }

    public async Task<AdminResult> AddCourseCategory(CourseCategoryFormModel category)
    {
      var categoryDataExists = await _db.CourseCategories
                                      .Where(data => data.category == category.category)
                                      .AnyAsync();
      if (categoryDataExists is true)
        return new AdminResult
        {
          statusCode = 409,
          message = "Category record repeat."
        };

      var newCategory = new CourseCategory
      {
        category = category.category
      };

      try
      {
        await _db.CourseCategories.AddAsync(newCategory);
        await _db.SaveChangesAsync();
        return new AdminResult
        {
          statusCode = 200,
          message = "Category record created."
        };
      }
      catch (Exception ex)
      {
        return new AdminResult
        {
          statusCode = 400,
          message = ex.Message
        };
      }
    }
  }
}