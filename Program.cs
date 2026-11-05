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
                        ShowGenreTable();
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


                // Таблиця з двома входами: рядки — жанри, стовпці — дні тижня
                static void ShowGenreTable()
                {
                    string[] genres = { "Драма", "Комедія", "Мюзикл", "Трагедія", "Казка" };
                    string[] days = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Нд" };

                    // Межі таблиці задає користувач
                    int rows = ReadSize("Скільки жанрів (рядків)", genres.Length);
                    if (rows < 0) return; // помилка вже показана, нічого не малюємо
                    int cols = ReadSize("Скільки днів (стовпців)", days.Length);
                    if (cols < 0) return;

                    Console.WriteLine();

                    // Шапка друкується до зовнішнього циклу
                    Console.Write(new string(' ', 10)); // зсув на ширину стовпця з назвами
                    for (int c = 0; c < cols; c++)
                    {
                        Console.Write($"{days[c],6}");
                    }
                    Console.WriteLine($"{"Разом",8}");

                    int[] colTotals = new int[cols]; // підсумки по стовпцях
                    int grandTotal = 0;              // загальний підсумок

                    for (int r = 0; r < rows; r++)
                    {
                        int rowTotal = 0; // обнуляється для кожного рядка, тому оголошений тут
                        Console.Write($"{genres[r],-10}");

                        for (int c = 0; c < cols; c++)
                        {
                            // Реальних даних ще немає, тому умовна кількість вистав
                            int value = (r * 3 + c * 2) % 5 + 1;
                            Console.Write($"{value,6}"); // Write, а не WriteLine
                            rowTotal += value;
                            colTotals[c] += value;
                        }

                        grandTotal += rowTotal;
                        Console.WriteLine($"{rowTotal,8}"); // перехід на новий рядок — після внутрішнього циклу
                    }

                    // Рядок підсумків під таблицею
                    Console.Write($"{"Разом",-10}");
                    for (int c = 0; c < cols; c++)
                    {
                        Console.Write($"{colTotals[c],6}");
                    }
                    Console.WriteLine($"{grandTotal,8}");
                }

                // Читає розмір таблиці. Повертає -1, якщо ввід неправильний.
                static int ReadSize(string prompt, int max)
                {
                    Console.Write($"{prompt} (1–{max}): ");
                    string input = Console.ReadLine();

                    if (!int.TryParse(input, out int value))
                    {
                        Console.WriteLine("Помилка: це не число.");
                        return -1;
                    }
                    if (value <= 0)
                    {
                        Console.WriteLine("Помилка: число має бути більше нуля.");
                        return -1;
                    }
                    if (value > max)
                    {
                        Console.WriteLine($"Помилка: максимум {max}.");
                        return -1;
                    }
                    return value;
                }
            }
            
        }
    }
}