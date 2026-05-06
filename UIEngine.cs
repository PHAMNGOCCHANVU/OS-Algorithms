using PageReplacementDemo.Algorithms;
using PageReplacementDemo.Models;
using System.Reflection;

namespace PageReplacementDemo;

/// <summary>
/// Engine chịu trách nhiệm vẽ giao diện Console và điều khiển step-by-step.
/// </summary>
public class UIEngine
{
    // Kích thước layout
    private const int TableLeft = 2;
    private const int TableTop = 5;
    private const int FrameWidth = 8;
    private const int MenuLeft = 55;
    private const int MenuTop = 5;
    private const int StatusLeft = 2;
    private const int StatusTop = 18;
    private const int InputTop = 22;

    private int _pageCount;
    private int _frameCount;
    private int[] _referenceString = Array.Empty<int>();
    private string _algorithmName = "";

    /// <summary>
    /// Chạy vòng lặp step-by-step cho thuật toán.
    /// </summary>
    public void Run(IPageReplacement algorithm, string algorithmName, int pageCount, int frameCount, int[] referenceString)
    {
        _pageCount = pageCount;
        _frameCount = frameCount;
        _referenceString = referenceString;
        _algorithmName = algorithmName;

        SafeClear();
        DrawStaticUI();

        int stepIndex = 0;
        int totalFaults = 0;

        foreach (var step in algorithm.ExecuteStepByStep())
        {
            stepIndex = step.StepIndex;
            totalFaults = step.TotalPageFaults;
            DrawStep(step);

            // Nếu còn bước tiếp theo, chờ người dùng nhấn Enter
            if (stepIndex < _referenceString.Length - 1)
            {
                DrawPrompt("Nhấn Enter để xem bước tiếp theo... (hoặc 'q' để quay lại menu)");
                var key = Console.ReadKey(true);
                if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                {
                    return;
                }
            }
        }

        // Kết thúc
        DrawSummary(stepIndex + 1, totalFaults);
        DrawPrompt("Hoàn tất! Nhấn phím bất kỳ để quay lại menu...");
        Console.ReadKey(true);
    }

    /// <summary>
    /// Vẽ các phần UI tĩnh (khung, menu, reference string).
    /// </summary>
    private void DrawStaticUI()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    PAGE REPLACEMENT ALGORITHMS SIMULATOR                     ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();

        // Vẽ menu bên phải
        DrawMenu();

        // Vẽ reference string
        DrawReferenceString();

        // Vẽ tiêu đề cột frame
        DrawFrameHeader();
    }

    /// <summary>
    /// Vẽ menu thuật toán ở cột phải.
    /// </summary>
    private void DrawMenu()
    {
        int x = MenuLeft;
        int y = MenuTop;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.SetCursorPosition(x, y++);
        Console.WriteLine("╔════════════════════╗");
        Console.SetCursorPosition(x, y++);
        Console.WriteLine("║   MENU THUẬT TOÁN  ║");
        Console.SetCursorPosition(x, y++);
        Console.WriteLine("╠════════════════════╣");
        Console.SetCursorPosition(x, y++);
        Console.WriteLine("║  [1] FIFO          ║");
        Console.SetCursorPosition(x, y++);
        Console.WriteLine("║  [2] LRU           ║");
        Console.SetCursorPosition(x, y++);
        Console.WriteLine("║  [3] Clock         ║");
        Console.SetCursorPosition(x, y++);
        Console.WriteLine("║  [4] OPT           ║");
        Console.SetCursorPosition(x, y++);
        Console.WriteLine("╚════════════════════╝");
        Console.ResetColor();

        // Hiển thị thông tin process
        y++;
        Console.SetCursorPosition(x, y++);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Đang chạy: {_algorithmName}");
        Console.SetCursorPosition(x, y++);
        Console.WriteLine($"Số trang: {_pageCount}");
        Console.SetCursorPosition(x, y++);
        Console.WriteLine($"Số frame: {_frameCount}");
        Console.ResetColor();
    }

    /// <summary>
    /// Vẽ reference string phía trên bảng frame.
    /// </summary>
    private void DrawReferenceString()
    {
        Console.SetCursorPosition(TableLeft, TableTop - 1);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Reference String: ");
        Console.ForegroundColor = ConsoleColor.Magenta;
        for (int i = 0; i < _referenceString.Length; i++)
        {
            Console.Write($"{_referenceString[i]}  ");
        }
        Console.ResetColor();
    }

    /// <summary>
    /// Vẽ tiêu đề các cột frame.
    /// </summary>
    private void DrawFrameHeader()
    {
        int x = TableLeft;
        int y = TableTop;

        Console.SetCursorPosition(x, y);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("Step".PadRight(6));

        for (int i = 0; i < _referenceString.Length; i++)
        {
            Console.SetCursorPosition(x + 6 + i * FrameWidth, y);
            Console.Write($"  [{i}]  ");
        }
        Console.ResetColor();
    }

    /// <summary>
    /// Vẽ một bước xử lý lên bảng.
    /// </summary>
    private void DrawStep(StepResult step)
    {
        int x = TableLeft;
        int y = TableTop + 1 + step.StepIndex;

        // Vẽ số thứ tự bước
        Console.SetCursorPosition(x, y);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"B{step.StepIndex}".PadRight(6));
        Console.ResetColor();

        // Vẽ các frame
        int colX = x + 6 + step.StepIndex * FrameWidth;
        Console.SetCursorPosition(colX, y);

        if (step.IsPageFault)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Hiển thị trạng thái frames tại bước này
        string frameStr = "";
        for (int i = 0; i < _frameCount; i++)
        {
            if (step.Frames[i] == -1)
            {
                frameStr += " _ ";
            }
            else
            {
                frameStr += $" {step.Frames[i]} ";
            }
        }
        Console.Write(frameStr.PadRight(FrameWidth - 1));
        Console.ResetColor();

        // Vẽ message ở dòng status
        DrawStatusMessage(step);
    }

    /// <summary>
    /// Đếm số frame đang có dữ liệu (khác -1).
    /// </summary>
    private static int GetOccupiedCount(StepResult step)
    {
        int count = 0;
        foreach (var p in step.Frames)
        {
            if (p != -1) count++;
        }
        return count;
    }

    /// <summary>
    /// Lấy chiều rộng console an toàn (mặc định 80 nếu lỗi).
    /// </summary>
    private static int GetWindowWidth()
    {
        try { return Console.WindowWidth; }
        catch { return 80; }
    }

    /// <summary>
    /// Vẽ dòng thông báo trạng thái.
    /// </summary>
    private void DrawStatusMessage(StepResult step)
    {
        int x = StatusLeft;
        int y = StatusTop;

        Console.SetCursorPosition(x, y);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(new string(' ', GetWindowWidth() - x - 1)); // Xóa dòng cũ
        Console.SetCursorPosition(x, y);

        if (step.IsPageFault)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"[Bước {step.StepIndex}] ");
            Console.ResetColor();
            Console.Write(step.Message);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[Bước {step.StepIndex}] ");
            Console.ResetColor();
            Console.Write(step.Message);
        }

        // Hiển thị tổng page fault
        Console.SetCursorPosition(x, y + 1);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"Tổng Page Faults: {step.TotalPageFaults}/{_referenceString.Length}");
        Console.ResetColor();
    }

    /// <summary>
    /// Vẽ dòng nhắc nhở người dùng.
    /// </summary>
    private void DrawPrompt(string message)
    {
        int x = StatusLeft;
        int y = InputTop;

        Console.SetCursorPosition(x, y);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(new string(' ', GetWindowWidth() - x - 1)); // Xóa dòng cũ
        Console.SetCursorPosition(x, y);
        Console.Write(message);
        Console.ResetColor();
    }

    /// <summary>
    /// Xóa màn hình an toàn, tránh lỗi khi không có console.
    /// </summary>
    private static void SafeClear()
    {
        try
        {
            Console.Clear();
        }
        catch (IOException)
        {
            try
            {
                int h = Console.WindowHeight;
                for (int i = 0; i < h; i++)
                    Console.WriteLine();
            }
            catch { }
            try { Console.SetCursorPosition(0, 0); } catch { }
        }
    }

    /// <summary>
    /// Vẽ thông tin tổng kết sau khi hoàn tất.
    /// </summary>
    private void DrawSummary(int totalSteps, int totalFaults)
    {
        try
        {
            int x = StatusLeft;
            int y = StatusTop + 3;

            // Kiểm tra vị trí hợp lệ trước khi set cursor
            if (y < Console.BufferHeight && x < Console.BufferWidth)
            {
                Console.SetCursorPosition(x, y);
            }
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("══════════════════════ KẾT THÚC ══════════════════════");
            Console.WriteLine($"Thuật toán: {_algorithmName}");
            Console.WriteLine($"Tổng số bước: {totalSteps}");
            Console.WriteLine($"Tổng Page Faults: {totalFaults}");
            Console.WriteLine($"Tỉ lệ Page Fault: {(double)totalFaults / totalSteps * 100:F2}%");
            Console.ResetColor();
        }
        catch
        {
            // Nếu có lỗi cursor position, vẫn in ra thông tin
            Console.WriteLine("\n══════════════════════ KẾT THÚC ══════════════════════");
            Console.WriteLine($"Thuật toán: {_algorithmName}");
            Console.WriteLine($"Tổng số bước: {totalSteps}");
            Console.WriteLine($"Tổng Page Faults: {totalFaults}");
        }
    }
}