using System;
using Ascon.Polynom.Api;
using Ascon.Polynom.Login;

namespace PolynomConnectionTest_Net6
{
    internal static class Program
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Тестирование подключения к ПОЛИНОМ:MDM");
                Console.WriteLine("======================================");
                Console.WriteLine();

                // Устанавливаем локаль
                ConsoleTools.SetLocale();

                // Подписываем поиск сборок
               // AsmResolver.DoSubscribe();

                Console.WriteLine("Попытка подключения к ПОЛИНОМ:MDM...");

                // Получение сессии
                GetSession();
            }
            catch (Exception ex)
            {
                ConsoleTools.PrintException(ex);
            }

            ConsoleTools.PressAnyKey();
        }

        private static void GetSession()
        {
            // Создаем уникальный идентификатор клиента
            var clientId = Guid.NewGuid();

            if (LoginManager.TryOpenSession(clientId, SessionOptions.None, ClientType.Client, out var session))
            {
                using (session)
                {
                    Console.WriteLine($"Сессия \"{session.Id}\" успешно получена.");
                    Console.WriteLine($"Подключение успешно установлено!");

                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Не удалось получить сессию");
            }
        }
    }
}
