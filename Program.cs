using System.Text;

namespace Theatre
{
    internal class Program
    {
        const int GenreCount = 5; // Скільки жанрів і днів є в таблиці з двома  входами
        const int DayCount = 7;
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            bool running = true; // єдиний спосіб вийти з циклу меню

            const int capacity = 100;               // місткість масиву
            double[] values = new double[capacity]; // ціни квитків
            int count = 0;                          // скільки місць зайнято

            while (running)
            {
                ShowMenu();
                string? choice = Console.ReadLine(); // string, щоб Enter чи літера не валили програму
                Console.WriteLine();

                switch (choice)
                {
                    case "1": ShowAbout(); Pause(); break;
                    case "2": ShowRevenueTable(); Pause(); break;
                    case "3": ShowGenreTable(); Pause(); break;
                    case "4":
                        double newValue = ReadDouble("Ціна квитка", 0, 10000);
                        AddValue(values, ref count, newValue, capacity);
                        Console.WriteLine($"Елементів у масиві: {count}");
                        Pause();
                        break;
                    case "5": PrintValues(values, count); Pause(); break;
                    case "6":
                        int number = ReadInt("Номер для видалення", 1, capacity);
                        RemoveAt(values, ref count, number - 1); // номер для людини з 1, індекс з 0
                        Console.WriteLine($"Елементів у масиві: {count}");
                        Pause();
                        break;
                    case "7": ShowStatistics(values, count); Pause(); break;
                    case "8": RunExperiment(); Pause(); break;
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
            Console.WriteLine("1 — довідка");
            Console.WriteLine("2 — таблиця виторгу залежно від ціни квитка");
            Console.WriteLine("3 — кількість вистав за жанрами і днями тижня");
            Console.WriteLine("4 — додати ціну квитка");
            Console.WriteLine("5 — показати ціни квитків");
            Console.WriteLine("6 — видалити ціну за номером");
            Console.WriteLine("7 — показати статистику цін");
            Console.WriteLine("8 — дослід: значиме й посилальне");
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
    }
}
