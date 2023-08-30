using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personal_project.Data;
using personal_project.Models.Domain;

namespace personal_project.Seed
{
  public static class Seeder
  {
    public static WebApplication Seed(this WebApplication app)
    {
      using (var scope = app.Services.CreateScope())
      {
        using var db = scope.ServiceProvider.GetRequiredService<WebDbContext>();
        try
        {
          db.Database.EnsureCreated();

          if (!db.Users.Any())
          {
            /************
            Users
            ************/
            var demoUser1 = new User
            {
              email = "demo1@example.com",
              provider = "native",
              password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
              isProfileCompleted = true
            };
            db.Users.Add(demoUser1);
            db.SaveChanges();

            db.Profiles.Add(new Profile
            {
              name = "Ray",
              nickname = "Ray",
              gender = "男",
              interest = "滑雪",
              user = demoUser1
            });

            var demoUser2 = new User
            {
              email = "demo2@example.com",
              provider = "native",
              password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
              isProfileCompleted = true
            };
            db.Users.Add(demoUser2);
            db.SaveChanges();

            db.Profiles.Add(new Profile
            {
              name = "Rita",
              nickname = "瑞塔",
              gender = "女",
              interest = "數學",
              user = demoUser2
            });

            var demoUser3 = new User
            {
              email = "demo3@example.com",
              provider = "native",
              password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
              isProfileCompleted = true
            };
            db.Users.Add(demoUser3);
            db.SaveChanges();

            db.Profiles.Add(new Profile
            {
              name = "Zac",
              nickname = "Z",
              gender = "男",
              interest = "瑜珈",
              user = demoUser3
            });

            db.Users.AddRange(
                new User
                {
                  email = "demo4@example.com",
                  provider = "native",
                  password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
                },
                new User
                {
                  email = "demo5@example.com",
                  provider = "native",
                  password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
                },
                new User
                {
                  email = "demo6@example.com",
                  provider = "native",
                  password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
                }
            );

            db.SaveChanges();

            /************
            Teacher1
            ************/
            var user = new Models.Domain.User
            {
              email = "teacher1@example.com",
              provider = "native",
              password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
              role = "teacher",
              isProfileCompleted = true
            };
            db.Users.Add(user);
            db.SaveChanges();

            db.Profiles.Add(new Models.Domain.Profile
            {
              name = "Rick",
              nickname = "Rick",
              gender = "男",
              interest = "滑雪",
              user = user
            });
            db.SaveChanges();

            var snowboardCourse = new Teacher
            {
              courseImage = "https://d3n4wxuzv8xzhg.cloudfront.net/teacher/course/images/20230826232255406.jpeg",
              courseName = "風靡全台的滑雪課程！",
              courseWay = "實體",
              courseLanguage = "中文",
              courseCategory = "滑雪",
              courseLocation = "湯澤",
              courseIntro = "歡迎來到我們精彩的滑雪課程介紹！無論你是初學者還是有一些滑雪經驗，我們的課程都能夠滿足你的需求，讓你在雪地中玩得開心並提升滑雪技巧。",
              courseReminder = "請自行準備雪具及纜車票。",
              userId = user.id
            };
            db.Teachers.Add(snowboardCourse);
            db.SaveChanges();

            db.Courses.AddRange(
                new Course
                {
                  startTime = DateTime.Parse("2024-01-15 09:00"),
                  endTime = DateTime.Parse("2024-01-15 15:00"),
                  price = 10000,
                  teacher = snowboardCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2024-01-16 09:00"),
                  endTime = DateTime.Parse("2024-01-16 15:00"),
                  price = 10000,
                  teacher = snowboardCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2024-01-17 09:00"),
                  endTime = DateTime.Parse("2024-01-17 15:00"),
                  price = 10000,
                  teacher = snowboardCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2024-01-18 09:00"),
                  endTime = DateTime.Parse("2024-01-18 15:00"),
                  price = 10000,
                  teacher = snowboardCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2024-01-15 09:00"),
                  endTime = DateTime.Parse("2024-01-15 15:00"),
                  price = 10000,
                  teacher = snowboardCourse
                }
            );

            /************
            Teacher2
            ************/
            var user2 = new Models.Domain.User
            {
              email = "teacher2@example.com",
              provider = "native",
              password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
              role = "teacher",
              isProfileCompleted = true
            };
            db.Users.Add(user2);
            db.SaveChanges();

            db.Profiles.Add(new Models.Domain.Profile
            {
              name = "Sofia",
              nickname = "大S",
              gender = "F",
              interest = "數學",
              user = user2
            });
            db.SaveChanges();

            var mathCourse = new Teacher
            {
              courseImage = "https://d3n4wxuzv8xzhg.cloudfront.net/teacher/course/images/20230824205727796.jpg",
              courseName = "適合小孩、青少年的數學1對1家教",
              courseWay = "實體",
              courseLanguage = "中文",
              courseCategory = "數學",
              courseLocation = "台中",
              courseIntro = "我的數學課程將啟發孩子的數字思維，深入探索數學的美妙世界，並培養孩子在解決現實問題中的數學應用能力。",
              courseReminder = "請自備文具並保持好奇心。",
              userId = user2.id
            };
            db.Teachers.Add(mathCourse);
            db.SaveChanges();

            db.Courses.AddRange(
                new Course
                {
                  startTime = DateTime.Parse("2023-08-25 18:00"),
                  endTime = DateTime.Parse("2023-08-25 19:00"),
                  price = 700,
                  teacher = mathCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-08-27 18:00"),
                  endTime = DateTime.Parse("2023-08-27 19:00"),
                  price = 700,
                  teacher = mathCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-29 18:00"),
                  endTime = DateTime.Parse("2023-09-29 19:00"),
                  price = 700,
                  teacher = mathCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-30 18:00"),
                  endTime = DateTime.Parse("2023-09-30 19:00"),
                  price = 700,
                  teacher = mathCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-01 18:00"),
                  endTime = DateTime.Parse("2023-09-01 19:00"),
                  price = 700,
                  teacher = mathCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-03 18:00"),
                  endTime = DateTime.Parse("2023-09-03 19:00"),
                  price = 700,
                  teacher = mathCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-05 18:00"),
                  endTime = DateTime.Parse("2023-09-05 19:00"),
                  price = 700,
                  teacher = mathCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-07 18:00"),
                  endTime = DateTime.Parse("2023-09-07 19:00"),
                  price = 700,
                  teacher = mathCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-10 18:00"),
                  endTime = DateTime.Parse("2023-09-10 19:00"),
                  price = 700,
                  teacher = mathCourse
                }
            );

            /************
            Teacher3
            ************/
            var user3 = new Models.Domain.User
            {
              email = "teacher3@example.com",
              provider = "native",
              password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
              role = "teacher",
              isProfileCompleted = true
            };
            db.Users.Add(user3);
            db.SaveChanges();

            db.Profiles.Add(new Models.Domain.Profile
            {
              name = "Annie",
              nickname = "安妮",
              gender = "女",
              interest = "瑜珈",
              user = user3
            });
            db.SaveChanges();

            var yogaCourse = new Teacher
            {
              courseImage = "https://d3n4wxuzv8xzhg.cloudfront.net/teacher/course/images/20230825105304648.jpg",
              courseName = "身心平衡之旅：線上瑜珈課程",
              courseWay = "線上",
              courseLanguage = "中文",
              courseCategory = "瑜珈",
              courseLocation = "台北",
              courseIntro = "歡迎加入我們精心設計的線上瑜珈課程！無論您是瑜珈初學者還是有一定經驗的瑜珈愛好者，我們的課程都將帶領您進入身心平衡的瑜珈之旅。",
              courseReminder = "請自行準備瑜珈墊、保持身心平靜。",
              userId = user3.id
            };
            db.Teachers.Add(yogaCourse);
            db.SaveChanges();

            db.Courses.AddRange(
                new Course
                {
                  startTime = DateTime.Parse("2023-08-30 09:00"),
                  endTime = DateTime.Parse("2023-08-30 10:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-08-30 13:00"),
                  endTime = DateTime.Parse("2023-08-30 14:00"),
                  price = 300,
                  teacher = yogaCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-08-30 16:00"),
                  endTime = DateTime.Parse("2023-08-30 17:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-10 09:00"),
                  endTime = DateTime.Parse("2023-09-10 10:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-10 13:00"),
                  endTime = DateTime.Parse("2023-09-10 14:00"),
                  price = 300,
                  teacher = yogaCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-10 16:00"),
                  endTime = DateTime.Parse("2023-09-10 17:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-05 09:00"),
                  endTime = DateTime.Parse("2023-09-05 10:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-05 13:00"),
                  endTime = DateTime.Parse("2023-09-05 14:00"),
                  price = 300,
                  teacher = yogaCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-05 16:00"),
                  endTime = DateTime.Parse("2023-09-05 17:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-06 09:00"),
                  endTime = DateTime.Parse("2023-09-06 10:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-06 13:00"),
                  endTime = DateTime.Parse("2023-09-06 14:00"),
                  price = 300,
                  teacher = yogaCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-06 16:00"),
                  endTime = DateTime.Parse("2023-09-06 17:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-30 09:00"),
                  endTime = DateTime.Parse("2023-09-30 10:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-30 13:00"),
                  endTime = DateTime.Parse("2023-09-30 14:00"),
                  price = 300,
                  teacher = yogaCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-30 16:00"),
                  endTime = DateTime.Parse("2023-09-30 17:00"),
                  price = 300,
                  teacher = yogaCourse
                }
            );

            db.SaveChanges();
          }
        }
        catch (Exception)
        {
          throw;
        }
        return app;
      }
    }
  }
}