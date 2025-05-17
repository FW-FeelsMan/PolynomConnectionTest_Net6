using System;
using System.Globalization;
using System.Text;
using System.Threading;

namespace PolynomConnectionTest_Net6
{
    public static class ConsoleTools
    {
        public const string CultureNameRu = "ru-RU";

        public static void SetLocale()
        {
            Console.OutputEncoding = Encoding.UTF8;
            if (Thread.CurrentThread.CurrentCulture.Name != CultureNameRu)
            {
                var culture = CultureInfo.CreateSpecificCulture(CultureNameRu);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }
        }

        public static void PressAnyKey()
        {
            Console.WriteLine("Для продолжения нажмите любую клавишу...");
            Console.ReadKey(true);
        }

        public static void PrintException(Exception ex)
        {
            var builder = new StringBuilder();
            builder.AppendLine("");
            builder.AppendLine("# Ошибка");
            builder.AppendLine($"# {ex.Message}");
            if (ex.InnerException != null)
            {
                builder.AppendLine($"# {ex.InnerException.Message}");
            }
            builder.AppendLine("#");
            builder.AppendLine("");

            Console.Write(builder);
        }
    }
}
