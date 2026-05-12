using PageReplacementDemo.Algorithms.CPUschedulingAlgo;
using PageReplacementDemo.Models;

namespace PageReplacementDemo;

/// <summary>
/// Chứa tất cả các hàm hiển thị kết quả
/// </summary>
public static class DisplayHelpers
{
    /// <summary>
    /// Hiển thị kết quả CPU Scheduling với bảng đầy đủ (bao gồm PR column)
    /// </summary>
    public static void DisplayCPUSchedulingResult(int algorithmChoice, 
        (List<Process> Processes, List<(int, int)> GanttChart, double AvgWT, double AvgTAT, double Throughput) result)
    {
        SafeClear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"═══ {CPUSchedulingExecutor.GetAlgorithmName(algorithmChoice)} ═══\n");
        Console.ResetColor();

        // Print table header with PR column
        Console.WriteLine("PID\tAT\tBT\tPR\tCT\tTAT\tWT");
        Console.WriteLine(new string('-', 50));
        
        // Print table rows
        foreach (var p in result.Processes)
        {
            Console.WriteLine($"P{p.Id}\t{p.ArrivalTime}\t{p.BurstTime}\t{p.Priority}\t{p.CompletionTime}\t{p.TurnaroundTime}\t{p.WaitingTime}");
        }

        // Print Gantt chart
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("GANTT CHART:");
        Console.WriteLine(new string('=', 50));
        Console.Write("|");
        foreach (var (pid, endTime) in result.GanttChart)
        {
            Console.Write($" P{pid} |");
        }
        Console.Write("\n0");
        foreach (var (pid, endTime) in result.GanttChart)
        {
            Console.Write($"{endTime,4}");
        }
        Console.WriteLine();

        // Print statistics
        Console.WriteLine(new string('=', 50));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Average Waiting Time (WT): {result.AvgWT:F2}");
        Console.WriteLine($"Average Turnaround Time (TAT): {result.AvgTAT:F2}");
        Console.WriteLine($"Throughput: {result.Throughput:F4} processes/unit time");
        Console.ResetColor();
    }

    /// <summary>
    /// Hiển thị kết quả Banker's Algorithm
    /// </summary>
    public static void DisplayBankerResult((bool IsSafe, List<int> SafeSequence, string Message) result)
    {
        SafeClear();
        Console.ForegroundColor = result.IsSafe ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine(result.Message);
        Console.ResetColor();
    }

    /// <summary>
    /// Xóa console - tương thích với tất cả hệ điều hành
    /// </summary>
    public static void SafeClear()
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
}
