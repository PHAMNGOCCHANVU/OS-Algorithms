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
        Console.WriteLine("  [2] Deadlock's Algorithm (Xử lý Deadlock - Resource Allocation)");
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
	Console.WriteLine("  [6] So sánh tất cả giải thuật");   
        Console.WriteLine();
        Console.WriteLine("  [0] Quay lại Menu Chính");
        Console.WriteLine();
    }

    public static int GetCPUSchedulingChoice()
    {
        while (true)
        {
            Console.Write("Nhập lựa chọn (0-6): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 0 && choice <= 6)
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
        Console.WriteLine("  [5] So sánh");
        Console.WriteLine("  [6] Đọc dữ liệu từ file text");
        Console.WriteLine();
        Console.WriteLine("  [0] Quay lại Menu Chính");
        Console.WriteLine();
    }

    public static int GetPageReplacementChoice()
    {
        while (true)
        {
            Console.Write("Nhập lựa chọn (0-6): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 0 && choice <= 6)
            {
                return choice;
            }
            Console.WriteLine("Lựa chọn không hợp lệ! Vui lòng nhập lại.");
        }
    }

    public static void ShowCPUSchedulingDataSourceMenu()
    {
        DisplayHelpers.SafeClear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔═════════════════════════════════════════════════════╗");
        Console.WriteLine("║   HỆ THỐNG MỎ PHỎNG ĐỊNH THỜI CPU                  ║");
        Console.WriteLine("╚═════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("  [1] Nhập dữ liệu thủ công");
        Console.WriteLine("  [2] Dùng bộ Test Case mẫu (có sẵn)");
        Console.WriteLine();
        Console.WriteLine("  [0] Quay lại");
        Console.WriteLine();
    }

    public static void ShowDataSourceMenu()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nChọn nguồn dữ liệu:");
        Console.ResetColor();
        Console.WriteLine("  [1] Nhập dữ liệu thủ công");
        Console.WriteLine("  [2] Tải từ file Test Case");
        Console.WriteLine("  [0] Quay lại");
        Console.WriteLine();
    }

    public static int GetDataSourceChoice()
    {
        while (true)
        {
            Console.Write("Nhập lựa chọn (0-2): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 0 && choice <= 2)
            {
                return choice;
            }
            Console.WriteLine("Lựa chọn không hợp lệ! Vui lòng nhập lại.");
        }
    }

    public static void ShowDeadlockSystemMenu()
    {
        DisplayHelpers.SafeClear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔═════════════════════════════════════════════════════╗");
        Console.WriteLine("║   CHUƠNG TRÌNH QUẢN LÝ TÀI NGUYÊN & DEADLOCK       ║");
        Console.WriteLine("╚═════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("  [1] Nhập dữ liệu hệ thống từ bàn phím");
        Console.WriteLine("  [2] Đọc dữ liệu hệ thống từ file .txt");
        Console.WriteLine("  [3] Kiểm tra trạng thái An toàn (Banker's Safety)");
        Console.WriteLine("  [4] Yêu cầu cấp phát tài nguyên (Resource-Request)");
        Console.WriteLine("  [5] Kiểm tra Deadlock hiện tại (Deadlock Detection)");
        Console.WriteLine("  [6] Phục hồi khỏi Deadlock (Deadlock Recovery)");
        Console.WriteLine();
        Console.WriteLine("  [0] Thoát");
        Console.WriteLine();
    }

    public static int GetDeadlockSystemChoice()
    {
        while (true)
        {
            Console.Write("Nhập lựa chọn (0-6): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 0 && choice <= 6)
            {
                return choice;
            }
            Console.WriteLine("Lựa chọn không hợp lệ! Vui lòng nhập lại.");
        }
    }

    public static void ShowDeadlockMenu()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔═════════════════════════════════════════════════════╗");
        Console.WriteLine("║         DEADLOCK DETECTION                          ║");
        Console.WriteLine("╚═════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Chọn lựa chọn:");
        Console.WriteLine();
        Console.WriteLine("  [1] Phát hiện Deadlock (Manual Input)");
        Console.WriteLine("  [2] Tải từ File Test Case");
        Console.WriteLine();
        Console.WriteLine("  [0] Quay lại Menu Chính");
        Console.WriteLine();
    }

    public static int GetDeadlockChoice()
    {
        while (true)
        {
            Console.Write("Nhập lựa chọn (0-2): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 0 && choice <= 2)
            {
                return choice;
            }
            Console.WriteLine("Lựa chọn không hợp lệ! Vui lòng nhập lại.");
        }
    }
}
