using System;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            try
            {
                DataClassesDataContext data = new DataClassesDataContext();

                //---1.	Список людей, которые прошли тесты.-----------------------------------------
                dataGridView1.DataSource = data.TestWorks
                    .Where(x => x.result >= 6)
                    .OrderByDescending(x => x.result)
                    .Join(data.Users,         // второй набор
                            u => u.userID,    // свойство-селектор объекта из первого набора TestWorks
                            c => c.id,        // свойство-селектор объекта из второго набора Users
                            (u, c) => new     // результат
                            {
                                ID = u.Users.id,
                                Студент = u.Users.name,
                                Результат = u.result,
                                Город = c.Cities.name,
                                Email = u.Users.email,
                                Возраст = u.Users.age,
                                Университет = c.Unvercities.name,
                            });

                //---2.	Список тех, кто прошли тесты успешно и уложились во время.--------------------
                dataGridView2.DataSource = data.TestWorks
                    .Where(x => x.result >= 6 && x.leadTime <= 25)
                    .OrderByDescending(x => x.result)
                    .Join(data.Users,         // второй набор
                            u => u.userID,    // свойство-селектор объекта из первого набора TestWorks
                            c => c.id,        // свойство-селектор объекта из второго набора Users
                            (u, c) => new     // результат
                            {
                                ID = u.Users.id,
                                Студент = u.Users.name,
                                Результат = u.result,
                                Время = u.leadTime,
                                Тест = u.Tests.name,
                                Город = c.Cities.name,
                                Email = u.Users.email,
                                Возраст = u.Users.age,
                                Университет = c.Unvercities.name,
                            });

                //---3.	Список людей, которые прошли тесты успешно и не уложились во время------------
                dataGridView3.DataSource = data.TestWorks
                .Where(x => x.result >= 6 && x.leadTime > 25)
                .OrderByDescending(x => x.result)
                .Join(data.Users,         // второй набор
                u => u.userID,            // свойство-селектор объекта из первого набора TestWorks
                c => c.id,                // свойство-селектор объекта из второго набора Users
                (u, c) => new             // результат
                {
                    ID = u.Users.id,
                    Студент = u.Users.name,
                    Результат = u.result,
                    Время = u.leadTime,
                    Тест = u.Tests.name,
                    Город = c.Cities.name,
                    Email = u.Users.email,
                    Возраст = u.Users.age,
                    Университет = c.Unvercities.name,
                });

                //---4.	Список студентов по городам. (Из Донецка: 8 студентов, из Киева: 5)
                dataGridView4.DataSource =
                    from cities in data.Cities
                    join users in data.Users on cities.id equals users.cityID into usersByCity
                    let count = usersByCity.Count()
                    orderby count descending
                    select new
                    {
                        Город = cities.name,
                        Всего = count
                    };

                /*---через Linq методы------------------------------------*/
                //dataGridView4.DataSource =
                //    data.Cities
                //        .GroupJoin(data.Users, c => c.id, u => u.cityID,
                //            (c, u) => new {c, userByCity = u})
                //        .Select(tp => new {tp, numCount = tp.userByCity.Count()})
                //        .OrderByDescending(tp2 => tp2.numCount)
                //        .Select(tp2 => new {Город = tp2.tp.c.name, Количество = tp2.numCount});


                //---5.	Список успешных студентов по городам.--------------------------------

                dataGridView5.DataSource =
                    from works in data.TestWorks
                    where works.result >= 6
                    join users in data.Users on works.userID equals users.id
                    group works.Users.cityID by works.Users.Cities.name into citiesAndUsers
                    let count = citiesAndUsers.Count()
                    orderby count descending
                    select new
                    {
                        Город = citiesAndUsers.Key,
                        Всего = count,
                    };

                //---6.	Результат для каждого студента - его баллы, время, баллы в процентах для каждой категории.

                dataGridView6.DataSource =
                    from a in data.TestWorks
                    where a.result >= 0
                    orderby a.result descending
                    let koef = 100 / (a.Tests.passMark + 4) //получаем коэфициент в процентах
                    select new
                    {
                        Студент = a.Users.name,
                        Баллы = a.result,
                        Проценты = (a.result) * koef,
                        Время = a.leadTime,
                        Категория = a.Tests.name
                    };
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message);
            }

        }
    }
}
