using PageReplacementDemo.Algorithms.CPUschedulingAlgo;

namespace PageReplacementDemo;

/// <summary>
/// Chứa tất cả các hàm hiển thị menu
/// </summary>
public static class MenuHelpers
{
    public static void ShowMainMenu()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔═════════════════════════════════════════════════════╗");
        Console.WriteLine("║      OPERATING SYSTEM ALGORITHMS SIMULATOR          ║");
        Console.WriteLine("╚═════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Chọn bộ thuật toán cần mô phỏng:");
        Console.WriteLine();
        Console.WriteLine("  [1] CPU Scheduling Algorithms (Định thời CPU)");
        Console.WriteLine("  [2] Banker's Algorithm (Xử lý Deadlock)");
        Console.WriteLine("  [3] Page Replacement Algorithms (Thay thế trang)");
        Console.WriteLine();
        Console.WriteLine("  [0] Thoát");
        Console.WriteLine();
    }

    public static int GetMainMenuChoice()
    {
        while (true)
        {
            Console.Write("Nhập lựa chọn (0-3): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 0 && choice <= 3)
            {
                return choice;
            }
            Console.WriteLine("Lựa chọn không hợp lệ! Vui lòng nhập lại.");
        }
    }

    public static void ShowCPUSchedulingMenu()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔═════════════════════════════════════════════════════╗");
        Console.WriteLine("║         CPU SCHEDULING ALGORITHMS                   ║");
        Console.WriteLine("╚═════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Chọn thuật toán định thời CPU:");
        Console.WriteLine();
        Console.WriteLine("  [1] FCFS (First Come First Served)");
        Console.WriteLine("  [2] SJF (Shortest Job First)");
        Console.WriteLine("  [3] SRTF (Shortest Remaining Time First)");
        Console.WriteLine("  [4] Round Robin");
        Console.WriteLine("  [5] Priority Scheduling");
        Console.WriteLine();
        Console.WriteLine("  [0] Quay lại Menu Chính");
        Console.WriteLine();
    }

    public static int GetCPUSchedulingChoice()
    {
        while (true)
        {
            Console.Write("Nhập lựa chọn (0-5): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 0 && choice <= 5)
            {
                return choice;
            }
            Console.WriteLine("Lựa chọn không hợp lệ! Vui lòng nhập lại.");
        }
    }

    public static void ShowBankerMenu()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔═════════════════════════════════════════════════════╗");
        Console.WriteLine("║         BANKER'S ALGORITHM                          ║");
        Console.WriteLine("╚═════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Chọn lựa chọn:");
        Console.WriteLine();
        Console.WriteLine("  [1] Demo Banker's Algorithm");
        Console.WriteLine();
        Console.WriteLine("  [0] Quay lại Menu Chính");
        Console.WriteLine();
    }

    public static int GetBankerChoice()
    {
        while (true)
        {
            Console.Write("Nhập lựa chọn (0-1): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 0 && choice <= 1)
            {
                return choice;
            }
            Console.WriteLine("Lựa chọn không hợp lệ! Vui lòng nhập lại.");
        }
    }

    public static void ShowPageReplacementMenu()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔═════════════════════════════════════════════════════╗");
        Console.WriteLine("║     PAGE REPLACEMENT ALGORITHMS                     ║");
        Console.WriteLine("╚═════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Chọn thuật toán thay thế trang:");
        Console.WriteLine();
        Console.WriteLine("  [1] FIFO  - First-In, First-Out");
        Console.WriteLine("  [2] LRU   - Least Recently Used");
        Console.WriteLine("  [3] Clock - Second-Chance (Clock)");
        Console.WriteLine("  [4] OPT   - Optimal (MIN)");
        Console.WriteLine();
        Console.WriteLine("  [0] Quay lại Menu Chính");
        Console.WriteLine();
    }

    public static int GetPageReplacementChoice()
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
}
