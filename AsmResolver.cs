
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PolynomConnectionTest_Net6
{
    internal static class AsmResolver
    {
    }
}
        /*
        /// <summary>
        /// Наименование системной переменной PATH.
        /// </summary>
        private const string VarSystemPath = "PATH";

        /// <summary>
        /// Наименование переменной установки продуктов ASCON. 
        /// </summary>
        public static string VarAsconInstallPath = "ASCON_INSTALL_PATH";

        /// <summary>
        /// Наименование переменной установки ПОЛИНОМ.
        /// </summary>
        public static string VarPolynomInstallPath = "POLYNOM_INSTALL_PATH";

        /// <summary>
        /// Имя переменной локального ПОЛИНОМа.
        /// </summary>
        public static string VarPolynomLibrary = "POLYNOM_LIBRARY";

        /// <summary>
        /// Наименование списка путей для поиска.
        /// </summary>
        private const string PolynomAssemblyResolvePaths = "PolynomAssemblyResolvePaths";

        /// <summary>
        /// Возвращает имя компании.
        /// </summary>
        public static string CompanyName => "ASCON";

        /// <summary>
        /// Возвращает имя проекта.
        /// </summary>
        public static string ProjectName => "Polynom";

        /// <summary>
        /// Имя папки содержащей файлы конфигурации.
        /// </summary>
        public static readonly string ConfigurationDirectoryName = "Configurations";

        /// <summary>
        /// Работа в Windows
        /// </summary>
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Исполняемая среда 64 бит 
        /// </summary>
        public static bool Is64Bit => Environment.Is64BitProcess;

        static AsmResolver()
        {
            // Добавить путь к InstallCommonPath в PATH.
            var installCommonPath = InstallCommonPath;
            var systemPath = Environment.GetEnvironmentVariable(VarSystemPath) ?? string.Empty;
            if (!string.IsNullOrEmpty(installCommonPath) && !systemPath.Contains(installCommonPath))
            {
                Environment.SetEnvironmentVariable(
                    VarSystemPath, $@"{systemPath};{installCommonPath}",
                    EnvironmentVariableTarget.Process
                );
            }

            // Сохранить список для других запросов.
            AppDomain.CurrentDomain.SetData(PolynomAssemblyResolvePaths, GetAssemblyResolvePaths());
            DoSubscribe();
        }

        /// <summary>
        /// Подписывает на AssemblyResolve.
        /// </summary>
        internal static void DoSubscribe()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
        }

        /// <summary>
        /// Возвращает значение переменной окружения.
        /// </summary>
        /// <param name="variableName">Имя переменной.</param>
        /// <returns>Значение переменной.</returns>
        public static string GetVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine) ??
                   Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User) ??
                   Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Process) ??
                   string.Empty;
        }

        /// <summary>
        /// Путь к папке установки продуктов ASCON.
        /// </summary>
        public static string AsconInstallPath => GetVariable(VarAsconInstallPath);

        /// <summary>
        /// Возвращает путь до общей папки.
        /// </summary>
        public static string AsconCommonPath
        {
            get
            {
                var path =
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        CompanyName
                    );

                if (IsWindows)
                {
                    path = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        CompanyName
                    );
                }

                return EnsureDirectory(path);
            }
        }

        /// <summary>
        /// Возвращает путь к каталогу расположения исполняемой сборки.
        /// </summary>
        public static string WorkingDirectoryPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                                                     Environment.CurrentDirectory;

        /// <summary>
        ///  Возвращает путь до папки установки ПОЛИНОМ:MDM.
        /// </summary>
        public static string InstallPath
        {
            get
            {
                try
                {
                    var installPath = GetVariable(VarPolynomInstallPath);
                    if (!string.IsNullOrEmpty(installPath))
                    {
                        if (Directory.Exists(installPath))
                            return installPath;
                    }

                    installPath = AsconInstallPath;
                    if (!string.IsNullOrEmpty(installPath))
                    {
                        installPath = Path.Combine(installPath, ProjectName);
                        if (Directory.Exists(installPath))
                            return installPath;
                    }

                    return Directory.GetParent(WorkingDirectoryPath)?.Parent?.FullName ?? WorkingDirectoryPath;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return string.Empty;
                }
            }
        }

        /// <summary> Возвращает путь до папки содержащей все модули ПОЛИНОМ:MDM. </summary>
        public static readonly string BinPath = Path.Combine(InstallPath, "Bin");

        /// <summary> Возвращает путь до папки содержащей клиентские модули ПОЛИНОМ:MDM. </summary>
        public static readonly string ExamplePath = Path.Combine(BinPath, "Example");

        /// <summary> Возвращает путь до папки содержащей модули сервера ПОЛИНОМ:MDM. </summary>
        public static readonly string ServerPath = Path.Combine(BinPath, "Server");

        public static string EnsurePathTo(string path, string? subPath = null)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            
            try
            {
                string fullPath;
                try
                {
                    fullPath = InstallPath;
                    if (string.IsNullOrEmpty(fullPath))
                        fullPath = WorkingDirectoryPath;
                }
                catch (Exception e)
                {
                    fullPath = WorkingDirectoryPath;
                    Console.WriteLine(e);
                }
                
                fullPath = string.IsNullOrEmpty(subPath)
                    ? Path.Combine(fullPath, path)
                    : Path.Combine(fullPath, path, subPath);
                
                return EnsureDirectory(fullPath);
            }
            catch (Exception)
            {
                return WorkingDirectoryPath;
            }
        }

        /// <summary>
        /// Возвращает путь к указанной папке. Если папка отсутствует то она будет создана.
        /// </summary>
        /// <param name="path">Путь к папке.</param>
        /// <param name="silent">Признак что нужно умолчать про ошибку.</param>
        /// <returns></returns>
        public static string EnsureDirectory(string path, bool silent = true)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                if (silent)
                    Console.WriteLine(e);
                else
                    throw;
            }

            return path;
        }

        /// <summary>
        /// Возвращает путь к указанной папке. Если папка отсутствует то она будет создана.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string? GetExistsPathOrNull(string path)
        {
            return Directory.Exists(path) ? path : null;
        }

        /// <summary>
        /// Путь к папке содержащей файлы конфигурации.
        /// </summary>
        public static string GetConfigurationsPathOrDefault()
        {
            var path = Path.Combine(InstallPath, ConfigurationDirectoryName);

            return EnsureDirectory(path);
        }

        /// <summary>
        /// Возвращает значение AsconInstallPath или вычисленное значение.
        /// </summary>
        public static string GetAsconInstallPathOrDefault()
        {
            var path = AsconInstallPath;

            if (string.IsNullOrEmpty(path))
                path = Directory.GetParent(WorkingDirectoryPath)?.Parent?.Parent?.FullName ?? WorkingDirectoryPath;

            return path;
        }

        /// <summary>
        /// Признак локального ПОЛИНОМ:MDM.
        /// </summary>
        public static bool PolynomIsLibrary
        {
            get
            {
                var isLibrary = GetVariable(VarPolynomLibrary);
                if (string.IsNullOrEmpty(isLibrary))
                    isLibrary = GetVariable("ASCON_POLYNOM_LIBRARY");

                return !string.IsNullOrEmpty(isLibrary) && isLibrary == "1";
            }
        }

        /// <summary>
        /// Возвращает путь общих компонентов ASCON.
        /// </summary>
        private static string InstallCommonPath => PolynomIsLibrary
            ? ExamplePath
            : Path.Combine(GetAsconInstallPathOrDefault(), "Commons");

        /// <summary>
        /// Возвращает путь хранения общих данных.
        /// </summary>
        public static string PolynomCommonPath
        {
            get
            {
                var path = Path.Combine(AsconCommonPath, ProjectName);
                return EnsureDirectory(IsWindows ? path : path.ToLower());
            }
        }

        /// <summary> Возвращает список базовых путей для поиска библиотек. </summary>
        /// <returns> Список путей, по которым следует искать требуемые библиотеки. </returns>
        private static List<string> GetAssemblyResolvePaths()
        {
            var asconInstallPath = GetAsconInstallPathOrDefault();
            var paths = new HashSet<string>
            {
                WorkingDirectoryPath,
                ExamplePath,
                ServerPath,
                InstallCommonPath,
                Path.Combine(InstallCommonPath, "net48"),
                Is64Bit ? Path.Combine(ExamplePath, @"x64") : Path.Combine(ExamplePath, @"x86"),
                Is64Bit ? Path.Combine(InstallCommonPath, @"x64") : Path.Combine(InstallCommonPath, @"x86"),
                Path.Combine(asconInstallPath, @"Measurement")
            };

            return paths.ToList();
        }

        /// <summary>
        /// Вызывается при загрузке сборки.
        /// </summary>
        /// <param name="sender">Объект, посылающий событие.</param>
        /// <param name="args">Аргументы события.</param>
        /// <returns></returns>
        private static Assembly? OnResolveAssembly(object? sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name).Name;

            if (string.IsNullOrEmpty(assemblyName))
                return null;

            var pureAssemblyName = assemblyName.Replace(".dll", "");

            var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(t => string.Equals(t.GetName().Name, pureAssemblyName, StringComparison.InvariantCultureIgnoreCase));

            if (loadedAssembly != null)
            {
                return loadedAssembly;
            }

            assemblyName = pureAssemblyName + ".dll";

            if (AppDomain.CurrentDomain.GetData(PolynomAssemblyResolvePaths) is not List<string> paths)
                paths = GetAssemblyResolvePaths();

            return paths.Select(path => Path.Combine(path, assemblyName))
                .Select(TryLoadAssembly)
                .FirstOrDefault(assembly => assembly != null);
        }

        private static Assembly? TryLoadAssembly(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            try
            {
                if (File.Exists(path))
                {
                    return Assembly.LoadFrom(path);;
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
*/