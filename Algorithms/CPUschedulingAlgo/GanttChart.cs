using PageReplacementDemo.Models;

namespace PageReplacementDemo.Algorithms.CPUschedulingAlgo;

/// <summary>
/// Gantt Chart display for CPU scheduling
/// </summary>
public static class GanttChart
{
    /// <summary>
    /// Display Gantt chart as timeline
    /// </summary>
    public static void Display(List<(int ProcessId, int EndTime)> ganttChart)
    {
        if (ganttChart.Count == 0)
            return;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n--- QUA TRINH THUC THI (GANTT CHART) ---");
        Console.ResetColor();

        int prevTime = 0;
        foreach (var (processId, endTime) in ganttChart)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"[ {prevTime} -> {endTime} ] : ");
            Console.ResetColor();

            if (processId == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("CPU Nhan roi (Idle)");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Tien trinh P{processId} chay");
            }

            prevTime = endTime;
        }
        Console.ResetColor();

        // Display horizontal bar chart
        Console.WriteLine();
        Console.Write("Timeline: |");
        foreach (var (processId, _) in ganttChart)
        {
            if (processId == 0)
                Console.Write(" IDLE |");
            else
                Console.Write($" P{processId} |");
        }
        Console.WriteLine();

        Console.Write("Time:     0");
        foreach (var (_, endTime) in ganttChart)
        {
            Console.Write($"{endTime,5}");
        }
        Console.WriteLine("\n");
    }
}
