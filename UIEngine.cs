using PageReplacementDemo.Algorithms.PageReplacementAlgo;
using PageReplacementDemo.Models;

namespace PageReplacementDemo;

/// <summary>
/// Engine chịu trách nhiệm vẽ giao diện Console hiển thị kết quả dạng bảng.
/// Bảng có hàng là frame, cột là tiến trình (reference string index).
/// </summary>
public class UIEngine
{
    private int _pageCount;
    private int _frameCount;
    private int[] _referenceString = Array.Empty<int>();
    private string _algorithmName = "";

    /// <summary>
    /// Chạy thuật toán và hiển thị kết quả dạng bảng.
    /// </summary>
    public void Run(IPageReplacement algorithm, string algorithmName, int pageCount, int frameCount, int[] referenceString)
    {
        _pageCount = pageCount;
        _frameCount = frameCount;
        _referenceString = referenceString;
        _algorithmName = algorithmName;

        SafeClear();

        // Thực thi tất cả các bước
        var steps = algorithm.ExecuteStepByStep().ToList();

        // Hiển thị tiêu đề
        DrawHeader();

        // Hiển thị bảng kết quả
        DrawResultsTable(steps);

        // Hiển thị thống kê
        CalculateAndDrawMetrics(steps);

        // Chờ người dùng nhấn phím
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nNhấn phím bất kỳ để quay lại menu...");
        Console.ResetColor();
        Console.ReadKey(true);
    }

    /// <summary>
    /// Hiển thị tiêu đề thông tin thuật toán.
    /// </summary>
    private void DrawHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    PAGE REPLACEMENT ALGORITHMS RESULTS                         ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\nThuật toán: {_algorithmName}");
        Console.WriteLine($"Số trang: {_pageCount} | Số frame: {_frameCount} | Độ dài chuỗi tham chiếu: {_referenceString.Length}");
        Console.ResetColor();

        Console.WriteLine();
    }

    /// <summary>
    /// Hiển thị bảng kết quả: hàng là frame, cột là bước thực thi.
    /// </summary>
    private void DrawResultsTable(List<StepResult> steps)
    {
        // Xác định độ rộng của mỗi cột (2 ký tự + 1 khoảng trắng)
        const int colWidth = 3;
        
        // Hiển thị dòng tiêu đề (reference string index)
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Frame".PadRight(8));
        
        for (int i = 0; i < _referenceString.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"[{i}]".PadRight(colWidth));
        }
        Console.ResetColor();
        Console.WriteLine();

        // Hiển thị dòng giá trị reference string
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("Ref".PadRight(8));
        for (int i = 0; i < _referenceString.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{_referenceString[i]}".PadRight(colWidth));
        }
        Console.ResetColor();
        Console.WriteLine();

        // Dòng separator
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(new string('─', 8));
        for (int i = 0; i < _referenceString.Length; i++)
        {
            Console.Write(new string('─', colWidth));
        }
        Console.ResetColor();
        Console.WriteLine();

        // Hiển thị từng frame
        for (int frameIdx = 0; frameIdx < _frameCount; frameIdx++)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"F{frameIdx}".PadRight(8));
            Console.ResetColor();

            for (int stepIdx = 0; stepIdx < steps.Count; stepIdx++)
            {
                var step = steps[stepIdx];
                int pageInFrame = step.Frames[frameIdx];

                if (pageInFrame == -1)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("_".PadRight(colWidth));
                }
                else
                {
                    // Tô màu đỏ nếu page fault, xanh nếu không
                    if (step.IsPageFault)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    Console.Write($"{pageInFrame}".PadRight(colWidth));
                }
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Lấy màu cho một page dựa trên page number.
    /// </summary>
    private ConsoleColor GetPageColor(int pageNumber)
    {
        return pageNumber switch
        {
            0 => ConsoleColor.Blue,
            1 => ConsoleColor.Red,
            2 => ConsoleColor.Yellow,
            3 => ConsoleColor.Cyan,
            4 => ConsoleColor.Green,
            5 => ConsoleColor.Magenta,
            6 => ConsoleColor.White,
            7 => ConsoleColor.Gray,
            _ => ConsoleColor.DarkYellow
        };
    }

    /// <summary>
    /// Tính toán và hiển thị các chỉ số hiệu suất.
    /// </summary>
    private void CalculateAndDrawMetrics(List<StepResult> steps)
    {
        if (steps.Count == 0)
            return;

        // Đếm page faults
        int totalPageFaults = 0;
        foreach (var step in steps)
        {
            if (step.IsPageFault)
                totalPageFaults++;
        }

        // Tính hit rate
        int hits = steps.Count - totalPageFaults;
        double hitRate = (double)hits / steps.Count * 100;
        double faultRate = (double)totalPageFaults / steps.Count * 100;

        // Tính memory utilization (trung bình số frame được sử dụng)
        double totalFramesUsed = 0;
        foreach (var step in steps)
        {
            int occupiedFrames = 0;
            for (int i = 0; i < _frameCount; i++)
            {
                if (step.Frames[i] != -1)
                    occupiedFrames++;
            }
            totalFramesUsed += occupiedFrames;
        }
        double avgMemoryUtilization = (totalFramesUsed / steps.Count) / _frameCount * 100;

        // Hiển thị thống kê
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Page Faults: {totalPageFaults}/{steps.Count} | Hit Rate: {hitRate:F2}% | Fault Rate: {faultRate:F2}%");
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
}
