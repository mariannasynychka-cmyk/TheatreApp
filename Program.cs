using System.Text;
using System.Globalization;

namespace Theatre
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            bool running = true; // єдиний спосіб вийти з циклу меню

            // Сховище вистав: паралельні масиви, по одному на кожне поле запису.
            // Запис номер i — це значення з індексом i в усіх масивах.
            const int Capacity = 100;                      // місткість сховища, не змінюється
            int[] ids = new int[Capacity];                 // код вистави (призначає програма)
            string[] titles = new string[Capacity];        // назва
            int[] genreIds = new int[Capacity];            // код жанру (поки що вводиться числом)
            string[] directors = new string[Capacity];     // режисер
            DateTime[] dates = new DateTime[Capacity];     // дата й час
            int[] durations = new int[Capacity];           // тривалість, хвилин
            double[] prices = new double[Capacity];        // ціна квитка
            int[] sold = new int[Capacity];                // продано квитків
            int count = 0;                                 // скільки місць зайнято
            int nextId = 1;                                // наступний код вистави 

            while (running)
            {
                ShowMenu();
                string? choice = Console.ReadLine(); // string, щоб Enter чи літера не валили програму
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        PrintPerformances(ids, titles, genreIds, directors, dates, durations, prices, sold, count);
                        Pause(); break;
                    case "2":AddPerformance(ids, titles, genreIds, directors, dates, durations, prices, sold,
                                       ref count, ref nextId, Capacity); Pause(); break;
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                    case "8":
                    case "9":
                        Console.WriteLine("Поки не зроблено.");
                        Pause();
                        break;
                    case "10":
                        FillSampleData(ids, titles, genreIds, directors, dates, durations, prices, sold,
                                       ref count, ref nextId, Capacity); Pause(); break;
                    case "11": ShowAbout(); Pause(); break;
                    case "12": ShowRevenueTable(); Pause(); break;
                    case "13": ShowGenreTable(); Pause(); break;
                    case "0":
                        running = false; // а не break: break завершив би лише case
                        Console.WriteLine("До побачення.");
                        break;
                    default:
                        Console.WriteLine("Немає такого пункту. Спробуйте ще раз.");
                        Pause();
                        break;
                }
            }
        }

        static void ShowMenu() // Виводить головне меню
        {
            Console.WriteLine("=== Театр ===");
            Console.WriteLine("1 — показати всі вистави");
            Console.WriteLine("2 — додати виставу");
            Console.WriteLine("3 — знайти за умовою");
            Console.WriteLine("4 — змінити виставу");
            Console.WriteLine("5 — видалити виставу");
            Console.WriteLine("6 — зведена таблиця");
            Console.WriteLine("7 — зберегти у файл");
            Console.WriteLine("8 — завантажити з файлу");
            Console.WriteLine("9 — експорт у CSV");
            Console.WriteLine("10 — заповнити тестовими виставами");
            Console.WriteLine("11 — довідка");
            Console.WriteLine("12 — таблиця виторгу залежно від ціни квитка");
            Console.WriteLine("13 — кількість вистав за жанрами і днями тижня");
            Console.WriteLine("0 — вихід");
            Console.Write("Ваш вибір: ");
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

        // Рахує виторг за ціною квитка й кількістю проданих. Нічого не виводить.
        static double CalculateRevenue(double price, int sold)
        {
            return price * sold;
        }

        // Умовна кількість вистав жанру genre у день day (реальних даних ще немає)
        static int GetPerformanceCount(int genre, int day)
        {
            return (genre * 3 + day * 2) % 5 + 1;
        }

        // Табулювання: виторг залежно від ціни квитка
        static void PrintRevenueTable(double from, double to, double step)
        {
            if (from > to)
            {
                Console.WriteLine("Початок більший за кінець — нема чого виводити.");
                return; // рання відмова: далі не йдемо
            }

            int sold = 120;         // скільки квитків продано (стала для таблиці)
            double discount = 0.10; // знижка 10%

            Console.WriteLine($"Виторг при {sold} проданих квитках");
            Console.WriteLine($"{"Ціна",8}{"Виторг",12}{"Зі знижкою",14}");

            // 1e-9 — запас, щоб остання стрічка не зникла через неточність дробових чисел
            for (double price = from; price <= to + 1e-9; price += step)
            {
                double revenue = CalculateRevenue(price, sold);
                double withDiscount = revenue * (1 - discount);
                Console.WriteLine($"{price,8:F2}{revenue,12:F2}{withDiscount,14:F2}");
            }
        }

        // питає розміри таблиці й віддає їх методу, що її будує
        static void ShowGenreTable()
        {
            const int GenreCount = 5; // Скільки жанрів і днів є в таблиці з двома  входами
            const int DayCount = 7;
            int rows = ReadInt("Скільки жанрів (рядків)", 1, GenreCount);
            int cols = ReadInt("Скільки днів (стовпців)", 1, DayCount);
            Console.WriteLine();
            PrintGenreTable(rows, cols);
        }

        // Таблиця з двома входами: рядки — жанри, стовпці — дні тижня
        static void PrintGenreTable(int rows, int cols)
        {
            string[] genres = { "Драма", "Комедія", "Мюзикл", "Трагедія", "Казка" };
            string[] days = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Нд" };

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
                    int value = GetPerformanceCount(r, c);
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

        // Читає ціле число в межах [min; max]. Питає знову, доки ввід не стане прийнятним.
        static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write($"{prompt} ({min}–{max}): ");
                string? input = Console.ReadLine();

                if (!int.TryParse(input, out int value))
                {
                    Console.WriteLine("Помилка: це не ціле число. Спробуйте ще раз.");
                    continue;
                }
                if (value < min || value > max)
                {
                    Console.WriteLine($"Помилка: число має бути від {min} до {max}. Спробуйте ще раз.");
                    continue;
                }
                return value;
            }
        }

        // Читає дробове число в межах [min; max]. Питає знову, доки ввід не стане прийнятним.
        static double ReadDouble(string prompt, double min, double max)
        {
            while (true)
            {
                Console.Write($"{prompt} ({min}–{max}): ");
                string? input = Console.ReadLine();

                if (!double.TryParse(input, out double value))
                {
                    Console.WriteLine("Помилка: це не число. Спробуйте ще раз.");
                    continue;
                }
                // Перевірка записана через заперечення, щоб відсіяти й NaN
                if (!(value >= min && value <= max))
                {
                    Console.WriteLine($"Помилка: число має бути від {min} до {max}. Спробуйте ще раз.");
                    continue;
                }
                return value;
            }

        }

        // питає межі й крок, далі віддає їх методу табулювання
        static void ShowRevenueTable()
        {
            double from = ReadDouble("Початкова ціна квитка", 1, 1000);
            double to = ReadDouble("Кінцева ціна квитка", 1, 1000);
            double step = ReadDouble("Крок ціни", 0.5, 500);
            Console.WriteLine();
            PrintRevenueTable(from, to, step);
        }
        // Лабораторна 8: розкладання меню на методи

        // Читає текст не коротший за minLength символів (після Trim). Питає знову, доки ввід не стане прийнятним.
        static string ReadText(string prompt, int minLength)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                string text = input == null ? "" : input.Trim();

                if (text.Length < minLength)
                {
                    Console.WriteLine($"Помилка: текст має містити не менше {minLength} символів. Спробуйте ще раз.");
                    continue;
                }
                return text;
            }
        }

        // Читає дату й час у форматі дд.мм.рррр гг:хх. Питає знову, доки ввід не стане прийнятним.
        static DateTime ReadDateTime(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (DateTime.TryParseExact(input, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out DateTime value))
                {
                    return value;
                }
                Console.WriteLine("Помилка: потрібен формат дд.мм.рррр гг:хх, наприклад 25.12.2026 19:00.");
            }
        }

        // Додає виставу в сховище. Усі поля спершу збираються в локальні змінні
        // й лише потім пишуться в масиви. Масиви йдуть без ref (вони посилальні),
        // лічильники count і nextId — з ref, щоб зміна була видна в Main.
        static void AddPerformance(int[] ids, string[] titles, int[] genreIds, string[] directors,
                                   DateTime[] dates, int[] durations, double[] prices, int[] sold,
                                   ref int count, ref int nextId, int capacity)
        {
            // Перевірка на заповненість — до того, як питати поля
            if (count == capacity)
            {
                Console.WriteLine("Сховище заповнене, видаліть щось зайве.");
                return;
            }

            string title = ReadText("Назва: ", 2);
            Console.WriteLine("Жанри: 1 — Драма, 2 — Комедія, 3 — Мюзикл, 4 — Трагедія, 5 — Казка");
            int genreId = ReadInt("Код жанру", 1, 5);
            string director = ReadText("Режисер: ", 2);
            DateTime date = ReadDateTime("Дата й час (дд.мм.рррр гг:хх): ");
            int duration = ReadInt("Тривалість, хв", 1, 600);
            double price = ReadDouble("Ціна квитка", 0, 10000);
            int ticketsSold = ReadInt("Продано квитків", 0, 10000);

            // Усі масиви заповнюються в одному місці, під одним індексом count
            ids[count] = nextId;
            titles[count] = title;
            genreIds[count] = genreId;
            directors[count] = director;
            dates[count] = date;
            durations[count] = duration;
            prices[count] = price;
            sold[count] = ticketsSold;
            count++;  // лічильники збільшуємо після присвоєнь
            nextId++;

            Console.WriteLine($"Додано виставу №{ids[count - 1]}.");
        }

        // Додає значення в кінець масиву. Масив іде без ref
        // лічильник — з ref, щоб зміна була видна в Main.
        static void AddValue(double[] values, ref int count, double newValue, int capacity)
        {
            if (count == capacity)
            {
                Console.WriteLine("Місця більше немає.");
                return;
            }
            values[count] = newValue;
            count++;
        }

        // Виводить перші count елементів таблицею з номерами. З клавіатури нічого не читає.
        static void PrintValues(double[] values, int count)
        {
            if (count == 0)
            {
                Console.WriteLine("Масив порожній.");
                return;
            }
            Console.WriteLine($"{"№",4}{"Ціна",10}");
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"{i + 1,4}{values[i],10:F2}");
            }

        }

        // Видаляє елемент з індексом index, зсуває решту вліво й зменшує лічильник.
        // Неправильний індекс - повідомлення й вихід без змін.
        static void RemoveAt(double[] values, ref int count, int index)
        {
            if (index < 0 || index >= count)
            {
                Console.WriteLine($"Помилка: елемента з номером {index + 1} немає. Нічого не змінено.");
                return;
            }
            for (int i = index; i < count - 1; i++)
            {
                values[i] = values[i + 1];
            }
            count--;
            values[count] = 0; // очищаємо звільнену комірку
        }

        // Повертає через out суму й середнє. Для порожнього набору середнє — 0, а не NaN.
        static void Statistics(double[] values, int count, out double sum, out double average)
        {
            sum = 0;
            for (int i = 0; i < count; i++)
            {
                sum += values[i];
            }
            average = count > 0 ? sum / count : 0;
        }

        // Повертає через out найбільше значення та його індекс.
        // Порожній набір: max = 0, maxIndex = -1 (викликач це перевіряє окремо).
        static void FindMax(double[] values, int count, out double max, out int maxIndex)
        {
            if (count == 0)
            {
                max = 0;
                maxIndex = -1;
                return;
            }
            max = values[0];
            maxIndex = 0;
            for (int i = 1; i < count; i++)
            {
                if (values[i] > max)
                {
                    max = values[i];
                    maxIndex = i;
                }
            }
        }

        // Виводить статистику цін. Порожній набір обробляється окремо.
        static void ShowStatistics(double[] values, int count)
        {
            if (count == 0)
            {
                Console.WriteLine("Масив порожній, статистики немає.");
                return;
            }
            Statistics(values, count, out double sum, out double average);
            FindMax(values, count, out double max, out int maxIndex);
            Console.WriteLine($"Елементів: {count}");
            Console.WriteLine($"Сума: {sum:F2}");
            Console.WriteLine($"Середнє: {average:F2}");
            Console.WriteLine($"Найбільше: {max:F2} (номер {maxIndex + 1})");
        }
        // Спроба змінити int без ref: міняється лише копія
        static void TryChangeInt(int value)
        {
            value = value + 100;
        }

        // Зміна елемента масиву: масив посилальний, тому зміна видна ззовні
        static void ChangeArrayElement(double[] prices)
        {
            prices[0] = prices[0] + 100;
        }

        // Дослід: різниця між значимим і посилальним типом
        static void RunExperiment()
        {
            int price = 50;
            double[] prices = { 50, 70, 90 };

            Console.WriteLine("До виклику:");
            Console.WriteLine($"  int price = {price}");
            Console.WriteLine($"  prices[0] = {prices[0]}");

            TryChangeInt(price);
            ChangeArrayElement(prices);

            Console.WriteLine("Після виклику:");
            Console.WriteLine($"  int price = {price} (метод змінив лише копію)");
            Console.WriteLine($"  prices[0] = {prices[0]} (змінено сам масив)");
        }

        // Виводить перші count вистав таблицею. З клавіатури нічого не читає.
        static void PrintPerformances(int[] ids, string[] titles, int[] genreIds, string[] directors,
                                      DateTime[] dates, int[] durations, double[] prices, int[] sold, int count)
        {
            if (count == 0)
            {
                Console.WriteLine("Записів поки немає.");
                return;
            }

            Console.WriteLine($"{"№",4}{"Назва",-26}{"Жанр",6}  {"Режисер",-18}{"Дата й час",-18}{"Хв",5}{"Ціна",10}{"Продано",9}");
            for (int i = 0; i < count; i++) // до count, а не до ids.Length
            {
                Console.WriteLine($"{ids[i],4}{titles[i],-26}{genreIds[i],6}  {directors[i],-18}{dates[i],-18:dd.MM.yyyy HH:mm}{durations[i],5}{prices[i],10:F2}{sold[i],9}");
            }
            Console.WriteLine($"\nЗаписів: {count}.");
        }

        // Заповнює сховище готовими виставами без запитань, щоб не вводити їх руками.
        static void FillSampleData(int[] ids, string[] titles, int[] genreIds, string[] directors,
                                   DateTime[] dates, int[] durations, double[] prices, int[] sold,
                                   ref int count, ref int nextId, int capacity)
        {
            string[] sampleTitles = { "Лісова пісня", "Мірандоліна", "Ніч перед Різдвом",
                                      "Едіп-цар", "Снігова королева", "Лис Микита" };
            int[] sampleGenres = { 1, 2, 3, 4, 5, 5 };
            string[] sampleDirectors = { "Коваль І.", "Мельник О.", "Шевчук Л.",
                                         "Коваль І.", "Бондар Н.", "Бондар Н." };
            DateTime[] sampleDates =
            {
                new DateTime(2026, 12, 15, 19, 0, 0),
                new DateTime(2026, 12, 16, 18, 30, 0),
                new DateTime(2026, 12, 19, 17, 0, 0),
                new DateTime(2026, 12, 21, 19, 0, 0),
                new DateTime(2026, 12, 26, 12, 0, 0),
                new DateTime(2026, 12, 27, 12, 0, 0)
            };
            int[] sampleDurations = { 130, 120, 150, 110, 90, 75 };
            double[] samplePrices = { 250, 200, 320, 280, 150, 120 };
            int[] sampleSold = { 87, 110, 140, 64, 180, 95 };

            int added = 0;
            for (int i = 0; i < sampleTitles.Length; i++)
            {
                if (count == capacity)
                {
                    Console.WriteLine("Сховище заповнене, решту не додано.");
                    break;
                }
                ids[count] = nextId;
                titles[count] = sampleTitles[i];
                genreIds[count] = sampleGenres[i];
                directors[count] = sampleDirectors[i];
                dates[count] = sampleDates[i];
                durations[count] = sampleDurations[i];
                prices[count] = samplePrices[i];
                sold[count] = sampleSold[i];
                count++;
                nextId++;
                added++;
            }
            Console.WriteLine($"Додано тестових вистав: {added}.");
        }
    }
}
