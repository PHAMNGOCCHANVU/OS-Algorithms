using PageReplacementDemo.Algorithms;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Management;

namespace PageReplacementDemo;

class Program
{
    static void Main(string[] args)
    {
        // Kiểm tra đang chạy từ IDE/VS Code hay cmd thực sự
        if (IsRunningFromIDE())
        {
            LaunchInNewConsole();
            return;
        }

        // Đảm bảo console size đủ lớn
        try
        {
            if (OperatingSystem.IsWindows() && (Console.WindowWidth < 90 || Console.WindowHeight < 30))
            {
                Console.SetWindowSize(120, 35);
            }
        }
        catch { }

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        UIEngine ui = new UIEngine();

        while (true)
        {
            SafeClear();
            ShowMainMenu();

            int choice = GetMenuChoice();
            if (choice == 0) break;

            int pageCount = GetPageCount();
            int frameCount = GetFrameCount();
            int[] referenceString = GetReferenceString(pageCount);

            IPageReplacement algorithm = CreateAlgorithm(choice);
            string algorithmName = GetAlgorithmName(choice);

            algorithm.Initialize(pageCount, frameCount, referenceString);
            ui.Run(algorithm, algorithmName, pageCount, frameCount, referenceString);
        }
    }

    static void ShowMainMenu()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════════════╗");
        Console.WriteLine("║        PAGE REPLACEMENT ALGORITHMS SIMULATOR    ║");
        Console.WriteLine("╚══════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Chọn thuật toán thay thế trang:");
        Console.WriteLine();
        Console.WriteLine("  [1] FIFO  - First-In, First-Out");
        Console.WriteLine("  [2] LRU   - Least Recently Used");
        Console.WriteLine("  [3] Clock - Second-Chance (Clock)");
        Console.WriteLine("  [4] OPT   - Optimal (MIN)");
        Console.WriteLine();
        Console.WriteLine("  [0] Thoát");
        Console.WriteLine();
    }

    static int GetMenuChoice()
    {
        while (true)
        {
            Console.Write("Nhập lựa chọn (0-4): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 0 && choice <= 4)
            {
                return choice;
            }
            Console.WriteLine("Lựa chọn không hợp lệ! Vui lòng nhập lại.");
        }
    }

    static int GetPageCount()
    {
        while (true)
        {
            Console.Write("Nhập số lượng trang (5-20): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int count) && count >= 5 && count <= 20)
            {
                return count;
            }
            Console.WriteLine("Số trang không hợp lệ! Vui lòng nhập số từ 5 đến 20.");
        }
    }

    static int GetFrameCount()
    {
        while (true)
        {
            Console.Write("Nhập số lượng Frame (3-10): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int count) && count >= 3 && count <= 10)
            {
                return count;
            }
            Console.WriteLine("Số frame không hợp lệ! Vui lòng nhập số từ 3 đến 10.");
        }
    }

    static int[] GetReferenceString(int pageCount)
    {
        while (true)
        {
            Console.Write($"Nhập chuỗi tham chiếu (các số từ 1 đến {pageCount}, cách nhau bằng khoảng trắng): ");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Chuỗi không được để trống!");
                continue;
            }

            string[] parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int[] result = new int[parts.Length];
            bool valid = true;

            for (int i = 0; i < parts.Length; i++)
            {
                if (!int.TryParse(parts[i], out result[i]) || result[i] < 1 || result[i] > pageCount)
                {
                    valid = false;
                    break;
                }
            }

            if (valid && result.Length > 0)
            {
                return result;
            }

            Console.WriteLine($"Chuỗi không hợp lệ! Vui lòng nhập các số từ 1 đến {pageCount}, cách nhau bằng khoảng trắng.");
        }
    }

    static IPageReplacement CreateAlgorithm(int choice)
    {
        return choice switch
        {
            1 => new FIFOAlgorithm(),
            2 => new LRUAlgorithm(),
            3 => new ClockAlgorithm(),
            4 => new OPTAlgorithm(),
            _ => throw new ArgumentException("Invalid algorithm choice")
        };
    }

    static string GetAlgorithmName(int choice)
    {
        return choice switch
        {
            1 => "FIFO",
            2 => "LRU",
            3 => "Clock",
            4 => "OPT",
            _ => "Unknown"
        };
    }

    static void SafeClear()
    {
        try
        {
            Console.Clear();
        }
        catch (IOException)
        {
            // Fallback: in nhiều dòng trống nếu Console.Clear() không hỗ trợ
            try
            {
                int h = Console.WindowHeight;
                for (int i = 0; i < h; i++)
                    Console.WriteLine();
            }
            catch
            {
                // Hoàn toàn không có console - bỏ qua
            }
            try { Console.SetCursorPosition(0, 0); } catch { }
        }
    }

    /// <summary>
    /// Kiểm tra đang chạy từ IDE/VS Code hay cmd thực sự
    /// </summary>
    static bool IsRunningFromIDE()
    {
        if (!OperatingSystem.IsWindows())
        {
            return false; // Chỉ support trên Windows
        }

        try
        {
            // Kiểm tra xem parent process có phải VS Code, Visual Studio hay không
            var parentProcess = GetParentProcess();
            if (parentProcess != null)
            {
                string parentName = parentProcess.ProcessName.ToLower();
                // Nếu parent là IDE, thì mở console mới
                return parentName.Contains("code") || 
                       parentName.Contains("devenv") ||
                       parentName.Contains("visualstudio");
            }
        }
        catch { }

        return false;
    }

    /// <summary>
    /// Lấy parent process của current process
    /// </summary>
    static Process? GetParentProcess()
    {
        try
        {
            var currentProcess = Process.GetCurrentProcess();
            var parentPid = GetParentProcessId(currentProcess.Id);
            if (parentPid > 0)
            {
                return Process.GetProcessById(parentPid);
            }
        }
        catch { }
        return null;
    }

    /// <summary>
    /// Lấy PID của parent process (Windows-specific)
    /// </summary>
    static int GetParentProcessId(int processId)
    {
        try
        {
            if (!OperatingSystem.IsWindows())
            {
                return 0; // Không hỗ trợ trên non-Windows
            }

            using (var process = Process.GetProcessById(processId))
            {
                var query = $"Select ParentProcessId FROM Win32_Process WHERE ProcessId={processId}";
                var searcher = new System.Management.ManagementObjectSearcher(query);
                var results = searcher.Get();
                foreach (var result in results)
                {
                    return Convert.ToInt32(result["ParentProcessId"]);
                }
            }
        }
        catch { }
        return 0;
    }

    /// <summary>
    /// Tự động mở command prompt riêng để chạy executable
    /// </summary>
    static void LaunchInNewConsole()
    {
        if (!OperatingSystem.IsWindows())
        {
            return; // Chỉ support trên Windows
        }

        try
        {
            // Lấy đường dẫn tới executable
            string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? "";
            
            if (exePath.EndsWith(".dll"))
            {
                // Nếu là DLL, dùng dotnet để chạy
                var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/K \"cd /d {exeDir} && dotnet Page-Replacement-Algorithms.dll\"",
                    UseShellExecute = true,
                    CreateNoWindow = false
                };
                Process.Start(psi);
            }
            else
            {
                // Chạy exe trực tiếp
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/K \"{exePath}\"",
                    UseShellExecute = true,
                    CreateNoWindow = false
                };
                Process.Start(psi);
            }
        }
        catch
        {
            // Nếu có lỗi, chạy trực tiếp trong terminal hiện tại
        }
    }
}
