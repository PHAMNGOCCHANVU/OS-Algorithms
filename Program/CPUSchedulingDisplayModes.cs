namespace PageReplacementDemo.Program;

using PageReplacementDemo.Algorithms.CPUschedulingAlgo;
using PageReplacementDemo.Models;

/// <summary>
/// Xử lý 2 chế độ hiển thị cho CPU Scheduling:
/// [0] Show All - in toàn bộ kết quả ngay (không delay)
/// [1] Step by Step - delay 0.8s giữa các bước
/// </summary>
public static class CPUSchedulingDisplayModes
{
    private const int STEP_DELAY_MS = 800; // 0.8s delay giữa các bước

    /// <summary>
    /// Hiển thị menu chế độ hiển thị
    /// </summary>
    public static void ShowDisplayModeMenu()
    {
        DisplayHelpers.SafeClear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n╔═════════════════════════════════════════════════════╗");
        Console.WriteLine("║      CHỌN CHẾ ĐỘ HIỂN THỊ                          ║");
        Console.WriteLine("╚═════════════════════════════════════════════════════╝");
        Console.ResetColor();

        Console.WriteLine();
        Console.WriteLine("  [1] In toàn bộ kết quả (không delay)");
        Console.WriteLine("  [2] Chạy tự động (Delay 0.8s mỗi bước)");
        Console.WriteLine("  [0] Quay lại");
        Console.WriteLine();
    }

    /// <summary>
    /// Lấy lựa chọn chế độ hiển thị
    /// </summary>
    public static int GetDisplayModeChoice()
    {
        while (true)
        {
            Console.Write("Nhập lựa chọn (0-2): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice <= 2)
                return choice;
            Console.WriteLine("Lựa chọn không hợp lệ! Vui lòng nhập lại.");
        }
    }

    /// <summary>
    /// Chạy thuật toán và in toàn bộ kết quả (không delay)
    /// </summary>
    public static void RunAlgorithmShowAll(
        List<Process> processes,
        int algorithmChoice,
        int quantum,
        string algorithmName)
    {
        DisplayHelpers.SafeClear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n╔═════════════════════════════════════════════════════╗");
        Console.WriteLine($"║      THUẬT TOÁN: {algorithmName,-35}║");
        Console.WriteLine($"╚═════════════════════════════════════════════════════╝");
        Console.ResetColor();

        // Clone processes để không sửa dữ liệu gốc
        var processCopy = processes.Select(p => new Process(p)).ToList();

        List<Process> results;
        List<(int, int)> ganttChart;
        double avgWT, avgTAT, throughput;

        // Chạy thuật toán tương ứng
        switch (algorithmChoice)
        {
            case 1: // FCFS
                (results, ganttChart, avgWT, avgTAT, throughput) = CPUSchedulingLogic.SolveFCFS(processCopy);
                break;
            case 2: // SJF
                (results, ganttChart, avgWT, avgTAT, throughput) = CPUSchedulingLogic.SolveSJF(processCopy);
                break;
            case 3: // SRTF
                (results, ganttChart, avgWT, avgTAT, throughput) = CPUSchedulingLogic.SolveSRTF(processCopy);
                break;
            case 4: // Round Robin
                (results, ganttChart, avgWT, avgTAT, throughput) = CPUSchedulingLogic.SolveRoundRobin(processCopy, quantum);
                break;
            case 5: // Priority
                (results, ganttChart, avgWT, avgTAT, throughput) = CPUSchedulingLogic.SolvePriority(processCopy);
                break;
            default:
                Console.WriteLine("Thuật toán không hợp lệ!");
                return;
        }

        // Hiển thị Gantt Chart
        Console.WriteLine("\n--- GANTT CHART ---");
        GanttChart.Display(ganttChart);

        // Hiển thị bảng kết quả
        DisplayCPUSchedulingResults(results, avgWT, avgTAT, throughput);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
        Console.ResetColor();
        Console.ReadKey(true);
    }

    /// <summary>
    /// Chạy thuật toán step-by-step với delay 0.8s giữa các bước
    /// </summary>
    public static void RunAlgorithmStepByStep(
        List<Process> processes,
        int algorithmChoice,
        int quantum,
        string algorithmName)
    {
        DisplayHelpers.SafeClear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n╔═════════════════════════════════════════════════════╗");
        Console.WriteLine($"║      THUẬT TOÁN: {algorithmName,-35}║");
        Console.WriteLine($"║      (Chế độ Step-by-Step - Delay 0.8s)              ║");
        Console.WriteLine($"╚═════════════════════════════════════════════════════╝");
        Console.ResetColor();

        var processCopy = processes.Select(p => new Process(p)).ToList();

        List<Process> results;
        List<(int, int)> ganttChart;
        double avgWT, avgTAT, throughput;

        // Chạy thuật toán
        switch (algorithmChoice)
        {
            case 1: // FCFS
                (results, ganttChart, avgWT, avgTAT, throughput) = CPUSchedulingLogic.SolveFCFS(processCopy);
                break;
            case 2: // SJF
                (results, ganttChart, avgWT, avgTAT, throughput) = CPUSchedulingLogic.SolveSJF(processCopy);
                break;
            case 3: // SRTF
                (results, ganttChart, avgWT, avgTAT, throughput) = CPUSchedulingLogic.SolveSRTF(processCopy);
                break;
            case 4: // Round Robin
                (results, ganttChart, avgWT, avgTAT, throughput) = CPUSchedulingLogic.SolveRoundRobin(processCopy, quantum);
                break;
            case 5: // Priority
                (results, ganttChart, avgWT, avgTAT, throughput) = CPUSchedulingLogic.SolvePriority(processCopy);
                break;
            default:
                Console.WriteLine("Thuật toán không hợp lệ!");
                return;
        }

        // Hiển thị step-by-step
        Console.WriteLine("\n--- GANTT CHART (STEP-BY-STEP) ---");
        DisplayGanttChartStepByStep(ganttChart);

        Console.WriteLine("\n--- BẢNG KẾT QUẢ (STEP-BY-STEP) ---");
        DisplayResultsTableStepByStep(results);

        // Hiển thị metrics cuối cùng
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n--- METRICS CUỐI CÙNG ---");
        Console.ResetColor();
        Console.WriteLine($"Thời gian chờ trung bình (Avg WT): {avgWT:F2}");
        Console.WriteLine($"Thời gian lưu hệ thống trung bình (Avg TAT): {avgTAT:F2}");
        Console.WriteLine($"Thông lượng (Throughput): {throughput:F4} tiến trình/đơn vị thời gian");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
        Console.ResetColor();
        Console.ReadKey(true);
    }

    /// <summary>
    /// Hiển thị Gantt Chart từng bước
    /// </summary>
    private static void DisplayGanttChartStepByStep(List<(int, int)> ganttChart)
{
    var partial = new List<(int, int)>();
    foreach (var step in ganttChart)
    {
        partial.Add(step);
        Thread.Sleep(STEP_DELAY_MS);
        Console.Clear();
        Console.WriteLine("--- GANTT CHART (STEP-BY-STEP) ---");
        GanttChart.Display(partial);   // gọi đúng method như ShowAll
    }
}

    /// <summary>
    /// Hiển thị bảng kết quả từng tiến trình
    /// </summary>
    private static void DisplayResultsTableStepByStep(List<Process> results)
    {
        Console.WriteLine("\n┌─────┬─────┬─────┬─────┬─────┬─────┬─────┐");
        Console.WriteLine("│ PID │  AT │  BT │  PR │  CT │ TAT │  WT │");
        Console.WriteLine("├─────┼─────┼─────┼─────┼─────┼─────┼─────┤");

        foreach (var p in results)
        {
            Console.WriteLine($"│ P{p.Id,-2}  │ {p.ArrivalTime,3:D} │ {p.BurstTime,3:D} │ {p.Priority,3:D} │ {p.CompletionTime,3:D} │ {p.TurnaroundTime,3:D} │ {p.WaitingTime,3:D} │");
            System.Threading.Thread.Sleep(STEP_DELAY_MS);
        }

        Console.WriteLine("└─────┴─────┴─────┴─────┴─────┴─────┴─────┘");
    }

    /// <summary>
    /// Hiển thị bảng kết quả CPU Scheduling
    /// </summary>
    private static void DisplayCPUSchedulingResults(List<Process> results, double avgWT, double avgTAT, double throughput)
    {
        Console.WriteLine("\n--- BẢNG KẾT QUẢ ---");
        Console.WriteLine("┌─────┬─────┬─────┬─────┬─────┬─────┬─────┐");
        Console.WriteLine("│ PID │  AT │  BT │  PR │  CT │ TAT │  WT │");
        Console.WriteLine("├─────┼─────┼─────┼─────┼─────┼─────┼─────┤");

        foreach (var p in results)
        {
            Console.WriteLine($"│ P{p.Id,-2}  │ {p.ArrivalTime,3:D} │ {p.BurstTime,3:D} │ {p.Priority,3:D} │ {p.CompletionTime,3:D} │ {p.TurnaroundTime,3:D} │ {p.WaitingTime,3:D} │");
        }

        Console.WriteLine("└─────┴─────┴─────┴─────┴─────┴─────┴─────┘");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n--- METRICS ---");
        Console.ResetColor();
        Console.WriteLine($"Thời gian chờ trung bình (Avg WT): {avgWT:F2}");
        Console.WriteLine($"Thời gian lưu hệ thống trung bình (Avg TAT): {avgTAT:F2}");
        Console.WriteLine($"Thông lượng (Throughput): {throughput:F4} tiến trình/đơn vị thời gian");
    }
}
