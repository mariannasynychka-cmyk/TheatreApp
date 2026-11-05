using System.Text;

namespace Theatre
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            bool running = true; // єдиний спосіб вийти з циклу меню

            while (running)
            {
                Console.WriteLine("=== Театр ===");
                Console.WriteLine("1 — довідка");
                Console.WriteLine("2 — таблиця виторгу залежно від ціни квитка");
                Console.WriteLine("3 — кількість вистав за жанрами і днями тижня");
                Console.WriteLine("0 — вихід");
                Console.Write("Ваш вибір: ");
                string choice = Console.ReadLine(); // string, щоб Enter чи літера не валили програму
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ShowAbout();
                        Pause();
                        break;
                    case "2":
                        ShowRevenueTable();
                        break;
                    case "3":
                        Console.WriteLine("Поки не зроблено.");
                        break;
                    case "0":
                        running = false; // а не break: break завершив би лише case
                        Console.WriteLine("До побачення.");
                        break;
                    default:
                        Console.WriteLine("Немає такого пункту. Спробуйте ще раз.");
                        Pause();
                        break;
                }
                // Пауза, щоб результат не зник відразу після дії
                static void Pause()
                {
                    Console.WriteLine("\nНатисніть будь-яку клавішу...");
                    Console.ReadKey(true);
                    Console.Clear();
                }

                // Довідка: тема, сутність, поля з типами
                static void ShowAbout()
                {
                    Console.WriteLine("Тема: Театр.");
                    Console.WriteLine("Головна сутність: вистава. Полів: 8.");
                    Console.WriteLine("Поля:");
                    Console.WriteLine("  int      код");
                    Console.WriteLine("  string   назва");
                    Console.WriteLine("  int      код жанру (з довідника)");
                    Console.WriteLine("  string   режисер");
                    Console.WriteLine("  DateTime дата й час");
                    Console.WriteLine("  int      тривалість");
                    Console.WriteLine("  double   ціна квитка");
                    Console.WriteLine("  int      продано квитків");
                    Console.WriteLine("Підсумкова величина: найкасовіші вистави сезону.");
                }
                // Табулювання: виторг залежно від ціни квитка
                static void ShowRevenueTable()
                {
                    double from = 150;      // початок проміжку
                    double to = 250;        // кінець проміжку
                    double step = 12.5;     // дробовий крок
                    int sold = 120;         // скільки квитків продано (стала для таблиці)
                    double discount = 0.10; // знижка 10%

                    Console.WriteLine($"Виторг при {sold} проданих квитках");
                    Console.WriteLine($"{"Ціна",8}{"Виторг",12}{"Зі знижкою",14}");

                    // 1e-9 — запас, щоб остання стрічка не зникла через неточність дробових чисел
                    for (double price = from; price <= to + 1e-9; price += step)
                    {
                        double revenue = price * sold;
                        double withDiscount = revenue * (1 - discount);
                        Console.WriteLine($"{price,8:F2}{revenue,12:F2}{withDiscount,14:F2}");
                    }
                }
            }
        }
    }
}