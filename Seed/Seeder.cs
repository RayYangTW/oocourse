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
              name = "Denny",
              nickname = "Denny",
              gender = "男",
              interest = "游泳",
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

            var demoUser4 = new User
            {
              email = "demo4@example.com",
              provider = "native",
              password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
              isProfileCompleted = false
            };
            db.Users.Add(demoUser4);
            db.SaveChanges();

            db.Profiles.Add(new Profile
            {
              name = "",
              nickname = "",
              gender = "",
              interest = "",
              user = demoUser4
            });

            var demoUser5 = new User
            {
              email = "demo5@example.com",
              provider = "native",
              password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
              isProfileCompleted = false
            };
            db.Users.Add(demoUser5);
            db.SaveChanges();

            db.Profiles.Add(new Profile
            {
              name = "",
              nickname = "",
              gender = "",
              interest = "",
              user = demoUser5
            });

            var demoUser6 = new User
            {
              email = "demo6@example.com",
              provider = "native",
              password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
              isProfileCompleted = false
            };
            db.Users.Add(demoUser6);
            db.SaveChanges();

            db.Profiles.Add(new Profile
            {
              name = "",
              nickname = "",
              gender = "",
              interest = "",
              user = demoUser6
            });

            db.SaveChanges();

            var demoUser7 = new User
            {
              email = "ray1992.tw@gmail.com",
              provider = "native",
              password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
              isProfileCompleted = true
            };
            db.Users.Add(demoUser7);
            db.SaveChanges();

            db.Profiles.Add(new Profile
            {
              name = "Ray",
              nickname = "Ray",
              gender = "男",
              interest = "滑雪",
              user = demoUser7
            });
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
              gender = "女",
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
                  startTime = DateTime.Parse("2023-09-15 18:00"),
                  endTime = DateTime.Parse("2023-09-15 19:00"),
                  price = 700,
                  teacher = mathCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-16 18:00"),
                  endTime = DateTime.Parse("2023-09-16 19:00"),
                  price = 700,
                  teacher = mathCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-17 18:00"),
                  endTime = DateTime.Parse("2023-09-17 19:00"),
                  price = 700,
                  teacher = mathCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-18 18:00"),
                  endTime = DateTime.Parse("2023-09-18 19:00"),
                  price = 700,
                  teacher = mathCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-19 18:00"),
                  endTime = DateTime.Parse("2023-09-19 19:00"),
                  price = 700,
                  teacher = mathCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-20 18:00"),
                  endTime = DateTime.Parse("2023-09-20 19:00"),
                  price = 700,
                  teacher = mathCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-21 18:00"),
                  endTime = DateTime.Parse("2023-09-21 19:00"),
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
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-16 09:00"),
                  endTime = DateTime.Parse("2023-09-16 10:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-16 13:00"),
                  endTime = DateTime.Parse("2023-09-16 14:00"),
                  price = 300,
                  teacher = yogaCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-16 16:00"),
                  endTime = DateTime.Parse("2023-09-16 17:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-17 09:00"),
                  endTime = DateTime.Parse("2023-09-17 10:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-17 13:00"),
                  endTime = DateTime.Parse("2023-09-17 14:00"),
                  price = 300,
                  teacher = yogaCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-17 16:00"),
                  endTime = DateTime.Parse("2023-09-17 17:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-23 09:00"),
                  endTime = DateTime.Parse("2023-09-23 10:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-23 13:00"),
                  endTime = DateTime.Parse("2023-09-23 14:00"),
                  price = 300,
                  teacher = yogaCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-23 16:00"),
                  endTime = DateTime.Parse("2023-09-23 17:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-24 09:00"),
                  endTime = DateTime.Parse("2023-09-24 10:00"),
                  price = 300,
                  teacher = yogaCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-24 13:00"),
                  endTime = DateTime.Parse("2023-09-24 14:00"),
                  price = 300,
                  teacher = yogaCourse
                }, new Course
                {
                  startTime = DateTime.Parse("2023-09-24 16:00"),
                  endTime = DateTime.Parse("2023-09-24 17:00"),
                  price = 300,
                  teacher = yogaCourse
                }
            );

            var user4 = new Models.Domain.User
            {
              email = "teacher4@example.com",
              provider = "native",
              password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
              role = "teacher",
              isProfileCompleted = true
            };
            db.Users.Add(user4);
            db.SaveChanges();

            db.Profiles.Add(new Models.Domain.Profile
            {
              name = "Gina",
              nickname = "Gigi",
              gender = "女",
              interest = "芭蕾",
              user = user4
            });
            db.SaveChanges();

            var balletCourse = new Teacher
            {
              courseImage = "https://d3n4wxuzv8xzhg.cloudfront.net/teacher/course/images/20230904224844401.jpg",
              courseName = "優雅之舞 — 芭蕾舞初級班",
              courseWay = "實體",
              courseLanguage = "中文",
              courseCategory = "芭蕾",
              courseLocation = "星光芭蕾舞學院市中心分校",
              courseIntro = "歡迎來到星光芭蕾舞學院的「優雅之舞」芭蕾舞初級班！這堂課程將帶領你進入芭蕾舞的精彩世界，讓你感受到優雅、力量和藝術的完美結合。不論你是初學者，還是已經有些基礎，都歡迎參加本課程，我們將提供專業的指導，讓你不斷進步。",
              courseReminder = "帶水和毛巾，以確保在課程期間保持身體舒適。",
              userId = user4.id
            };
            db.Teachers.Add(balletCourse);
            db.SaveChanges();

            db.Courses.AddRange(
                new Course
                {
                  startTime = DateTime.Parse("2023-09-15 09:00"),
                  endTime = DateTime.Parse("2023-09-15 11:00"),
                  price = 1000,
                  teacher = balletCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-16 09:00"),
                  endTime = DateTime.Parse("2023-09-16 11:00"),
                  price = 1000,
                  teacher = balletCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-10-17 09:00"),
                  endTime = DateTime.Parse("2023-10-17 11:00"),
                  price = 1000,
                  teacher = balletCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-10-18 09:00"),
                  endTime = DateTime.Parse("2023-10-18 11:00"),
                  price = 1000,
                  teacher = balletCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-10-19 09:00"),
                  endTime = DateTime.Parse("2023-10-19 11:00"),
                  price = 1000,
                  teacher = balletCourse
                }
            );

            var user5 = new Models.Domain.User
            {
              email = "teacher5@example.com",
              provider = "native",
              password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
              role = "teacher",
              isProfileCompleted = true
            };
            db.Users.Add(user5);
            db.SaveChanges();

            db.Profiles.Add(new Models.Domain.Profile
            {
              name = "Angela",
              nickname = "Angel",
              gender = "女",
              interest = "語言",
              user = user5
            });
            db.SaveChanges();

            var kidsEnglishCourse = new Teacher
            {
              courseImage = "https://d3n4wxuzv8xzhg.cloudfront.net/teacher/course/images/20230904225203692.jpg",
              courseName = "童趣美語探險",
              courseWay = "實體",
              courseLanguage = "中文",
              courseCategory = "兒童美語",
              courseLocation = "彩虹小學美語教室",
              courseIntro = "歡迎來到童趣美語探險！這是一個專為兒童度身打造的有趣而互動的美語課程。我們相信學習語言應該是一場冒險，讓孩子們在快樂中探索世界，同時培養語言技能。",
              courseReminder = "帶上筆記本和筆，讓孩子可以記錄他們在課堂上學到的英語詞彙和故事。",
              userId = user5.id
            };
            db.Teachers.Add(kidsEnglishCourse);
            db.SaveChanges();

            db.Courses.AddRange(
                new Course
                {
                  startTime = DateTime.Parse("2023-09-15 09:00"),
                  endTime = DateTime.Parse("2023-09-15 10:00"),
                  price = 800,
                  teacher = kidsEnglishCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-16 09:00"),
                  endTime = DateTime.Parse("2023-09-16 10:00"),
                  price = 800,
                  teacher = kidsEnglishCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-10-17 09:00"),
                  endTime = DateTime.Parse("2023-10-17 10:00"),
                  price = 800,
                  teacher = kidsEnglishCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-10-18 09:00"),
                  endTime = DateTime.Parse("2023-10-18 10:00"),
                  price = 800,
                  teacher = kidsEnglishCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-10-19 09:00"),
                  endTime = DateTime.Parse("2023-10-19 10:00"),
                  price = 800,
                  teacher = kidsEnglishCourse
                }
            );

            var user6 = new Models.Domain.User
            {
              email = "teacher6@example.com",
              provider = "native",
              password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
              role = "teacher",
              isProfileCompleted = true
            };
            db.Users.Add(user6);
            db.SaveChanges();

            db.Profiles.Add(new Models.Domain.Profile
            {
              name = "Shawn",
              nickname = "S哥",
              gender = "男",
              interest = "程式語言",
              user = user6
            });
            db.SaveChanges();

            var codingCourse = new Teacher
            {
              courseImage = "https://d3n4wxuzv8xzhg.cloudfront.net/teacher/course/images/20230912173240660.jpg",
              courseName = "Coding大冒險",
              courseWay = "線上",
              courseLanguage = "中文",
              courseCategory = "程式語言",
              courseLocation = "台北",
              courseIntro = "歡迎來到「Coding大冒險」！這是一個讓你踏上學習編程的奇妙之旅的機會。不論你是初學者，還是已經有一些編程經驗，這個課程都將啟發你的創造力，讓你掌握程式語言的神奇力量。",
              courseReminder = "準備好一顆充滿好奇心的心，我們將一起探索編程的無限可能性！",
              userId = user6.id
            };
            db.Teachers.Add(codingCourse);
            db.SaveChanges();

            db.Courses.AddRange(
                new Course
                {
                  startTime = DateTime.Parse("2023-09-15 09:00"),
                  endTime = DateTime.Parse("2023-09-15 10:00"),
                  price = 800,
                  teacher = codingCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-16 09:00"),
                  endTime = DateTime.Parse("2023-09-16 10:00"),
                  price = 800,
                  teacher = codingCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-10-17 09:00"),
                  endTime = DateTime.Parse("2023-10-17 10:00"),
                  price = 800,
                  teacher = codingCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-10-18 09:00"),
                  endTime = DateTime.Parse("2023-10-18 10:00"),
                  price = 800,
                  teacher = codingCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-10-19 09:00"),
                  endTime = DateTime.Parse("2023-10-19 10:00"),
                  price = 800,
                  teacher = codingCourse
                }
            );

            var user7 = new Models.Domain.User
            {
              email = "teacher7@example.com",
              provider = "native",
              password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
              role = "teacher",
              isProfileCompleted = true
            };
            db.Users.Add(user7);
            db.SaveChanges();

            db.Profiles.Add(new Models.Domain.Profile
            {
              name = "Lucy",
              nickname = "Lulu",
              gender = "女",
              interest = "成人美語",
              user = user7
            });
            db.SaveChanges();

            var adultEnglishCourse = new Teacher
            {
              courseImage = "https://d3n4wxuzv8xzhg.cloudfront.net/teacher/course/images/20230912173854090.jpg",
              courseName = "成人美語精英班",
              courseWay = "線上",
              courseLanguage = "中文",
              courseCategory = "成人美語",
              courseLocation = "台北",
              courseIntro = "歡迎參加我們的成人美語精英班！這是一個專為成年學習者打造的高效美語課程，無論你是為了職業需要、旅遊還是個人成長，都能在這裡找到最佳的語言學習體驗。",
              courseReminder = "請攜帶筆記本、電子設備或筆記工具，以便參與課堂互動和練習。",
              userId = user7.id
            };
            db.Teachers.Add(adultEnglishCourse);
            db.SaveChanges();

            db.Courses.AddRange(
                new Course
                {
                  startTime = DateTime.Parse("2023-09-15 09:00"),
                  endTime = DateTime.Parse("2023-09-15 10:00"),
                  price = 800,
                  teacher = adultEnglishCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-09-16 09:00"),
                  endTime = DateTime.Parse("2023-09-16 10:00"),
                  price = 800,
                  teacher = adultEnglishCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-10-17 09:00"),
                  endTime = DateTime.Parse("2023-10-17 10:00"),
                  price = 800,
                  teacher = adultEnglishCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-10-18 09:00"),
                  endTime = DateTime.Parse("2023-10-18 10:00"),
                  price = 800,
                  teacher = adultEnglishCourse
                },
                new Course
                {
                  startTime = DateTime.Parse("2023-10-19 09:00"),
                  endTime = DateTime.Parse("2023-10-19 10:00"),
                  price = 800,
                  teacher = adultEnglishCourse
                }
            );
            /************
            Admin
            ************/
            var demoAdmin = new User
            {
              email = "admin1@example.com",
              provider = "native",
              password = "$2a$11$a6XTHe4Z576lMuBU4zZNIO7sriKmfTV/.2n906WNe/g9FOf3CG91m",
              isProfileCompleted = true,
              role = "admin"
            };
            db.Users.Add(demoAdmin);
            db.SaveChanges();

            db.Profiles.Add(new Profile
            {
              name = "Admin",
              nickname = "Ad",
              gender = "男",
              interest = "管理",
              user = demoAdmin
            });

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