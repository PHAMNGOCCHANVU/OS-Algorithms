// Program/CPUSchedulingHelpers.cs — thay thế toàn bộ file cũ nếu có

using PageReplacementDemo.Algorithms.CPUschedulingAlgo;
using PageReplacementDemo.Models;

namespace PageReplacementDemo.Program
{
    public static class CPUSchedulingHelpers
    {
        private static readonly string[] AlgoNames =
        [
            "FCFS  — First Come First Served",
            "SJF   — Shortest Job First",
            "SRTF  — Shortest Remaining Time First",
            "RR    — Round Robin",
            "PRIOR — Priority Scheduling"
        ];

        // Tạo giải thuật theo choice 1-5
        private static ICPUScheduling CreateAlgorithm(int choice) => choice switch
        {
            1 => new FCFSAlgorithm(),
            2 => new SJFAlgorithm(),
            3 => new SRTFAlgorithm(),
            4 => new RoundRobinAlgorithm(),
            5 => new PriorityAlgorithm(),
            _ => throw new ArgumentException($"Invalid choice: {choice}")
        };

        // Clone — dùng copy constructor, reset output fields về 0
        private static List<Process> Clone(List<Process> src) =>
            src.Select(p => new Process(p.Id, p.ArrivalTime, p.BurstTime, p.Priority)).ToList();

        // Kết quả 1 giải thuật — dùng đúng type từ ICPUScheduling.Execute()
        public record AlgoResult(
            string Name,
            List<(int ProcessId, int EndTime)> Gantt,
            List<Process> Results,
            double AvgWT,
            double AvgTAT,
            double Throughput
        );

        // Chạy tất cả 5 giải thuật
        public static List<AlgoResult> RunAllAlgorithms(
            List<Process> processes, int quantumTime)
        {
            var all = new List<AlgoResult>();
            for (int i = 1; i <= 5; i++)
            {
                var algo = CreateAlgorithm(i);
                algo.Initialize(Clone(processes), quantumTime);
                var (resultProcs, gantt, avgWT, avgTAT, tp) = algo.Execute();
                all.Add(new AlgoResult(AlgoNames[i - 1], gantt, resultProcs, avgWT, avgTAT, tp));
            }
            return all;
        }

        // ═══════════════════════════════════════════════════
        // ENTRY POINT — gọi từ Program.cs choice == 6
        // ═══════════════════════════════════════════════════
        public static void RunAndDisplayCompareAll(
            List<Process> processes, int quantumTime)
        {
            DisplayHelpers.SafeClear();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          SO SÁNH TẤT CẢ GIẢI THUẬT CPU SCHEDULING              ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            var all = RunAllAlgorithms(processes, quantumTime);

            for (int i = 0; i < all.Count; i++)
            {
                var r = all[i];
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n" + new string('═', 70));
                Console.WriteLine($"  [{i + 1}] {r.Name}");
                Console.WriteLine(new string('═', 70));
                Console.ResetColor();

                GanttChart.Display(r.Gantt);
                Console.WriteLine();
                PrintResultTable(r.Results, isPriority: i == 4);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(
                    $"\n  ► Avg WT: {r.AvgWT:F2}  " +
                    $"|  Avg TAT: {r.AvgTAT:F2}  " +
                    $"|  Throughput: {r.Throughput:F4} proc/unit");
                Console.ResetColor();
            }

            DisplayComparisonTable(all);
        }

        // ─────────────────────────────────────────────────
        // Vẽ Gantt chart từ List<(ProcessId, EndTime)>
        // StartTime của block[i] = EndTime của block[i-1] (hoặc 0)
        // ─────────────────────────────────────────────────
        private static void PrintGantt(List<(int ProcessId, int EndTime)> gantt)
        {
            if (gantt.Count == 0)
            {
                Console.WriteLine("  (Không có dữ liệu Gantt)");
                return;
            }

            // Dựng (pid, start, end) từ danh sách (pid, endTime)
            var blocks = new List<(int Pid, int Start, int End)>();
            int cur = 0;
            foreach (var (pid, end) in gantt)
            {
                blocks.Add((pid, cur, end));
                cur = end;
            }

            const int MIN_W = 4, MAX_W = 8;
            var cells = blocks.Select(b =>
            {
                int w = Math.Clamp(b.End - b.Start, MIN_W, MAX_W);
                return (b, w);
            }).ToList();

            Console.ForegroundColor = ConsoleColor.DarkCyan;

            // Top border
            Console.Write("  ");
            foreach (var (_, w) in cells) Console.Write("+" + new string('─', w));
            Console.WriteLine("+");

            // Label row
            Console.Write("  ");
            foreach (var (blk, w) in cells)
            {
                string lbl = blk.Pid < 0 ? "IDLE" : $"P{blk.Pid}";
                if (lbl.Length > w) lbl = lbl[..w];
                int pad = w - lbl.Length;
                Console.Write("|" + new string(' ', pad / 2) + lbl + new string(' ', (pad + 1) / 2));
            }
            Console.WriteLine("|");

            // Bottom border
            Console.Write("  ");
            foreach (var (_, w) in cells) Console.Write("+" + new string('─', w));
            Console.WriteLine("+");

            // Time labels
            Console.Write("  " + blocks[0].Start);
            foreach (var (blk, w) in cells)
            {
                string t = blk.End.ToString();
                Console.Write(new string(' ', Math.Max(1, w + 1 - t.Length)) + t);
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        // ─────────────────────────────────────────────────
        // Bảng kết quả 1 giải thuật
        // ─────────────────────────────────────────────────
        private static void PrintResultTable(List<Process> results, bool isPriority)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (isPriority)
                Console.WriteLine("  {0,-5} {1,-5} {2,-5} {3,-5} {4,-5} {5,-7} {6}",
                    "PID", "AT", "BT", "PR", "CT", "TAT", "WT");
            else
                Console.WriteLine("  {0,-5} {1,-5} {2,-5} {3,-5} {4,-7} {5}",
                    "PID", "AT", "BT", "CT", "TAT", "WT");
            Console.ResetColor();
            Console.WriteLine("  " + new string('─', isPriority ? 44 : 38));

            foreach (var p in results.OrderBy(p => p.Id))
            {
                if (isPriority)
                    Console.WriteLine("  {0,-5} {1,-5} {2,-5} {3,-5} {4,-5} {5,-7} {6}",
                        $"P{p.Id}", p.ArrivalTime, p.BurstTime, p.Priority,
                        p.CompletionTime, p.TurnaroundTime, p.WaitingTime);
                else
                    Console.WriteLine("  {0,-5} {1,-5} {2,-5} {3,-5} {4,-7} {5}",
                        $"P{p.Id}", p.ArrivalTime, p.BurstTime,
                        p.CompletionTime, p.TurnaroundTime, p.WaitingTime);
            }
        }

        // ─────────────────────────────────────────────────
        // Bảng so sánh tổng hợp + detect convoy effect
        // ─────────────────────────────────────────────────
        public static void DisplayComparisonTable(List<AlgoResult> results)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n" + new string('═', 70));
            Console.WriteLine("  BẢNG SO SÁNH TỔNG HỢP");
            Console.WriteLine(new string('═', 70));
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  {0,-38} {1,-10} {2,-10} {3}",
                "Thuật toán", "Avg WT", "Avg TAT", "Throughput");
            Console.ResetColor();
            Console.WriteLine("  " + new string('─', 70));

            double minWT  = results.Min(r => r.AvgWT);
            double minTAT = results.Min(r => r.AvgTAT);
            double maxTP  = results.Max(r => r.Throughput);

            foreach (var r in results)
            {
                Console.Write($"  {r.Name,-38} ");
                Highlight(r.AvgWT.ToString("F2"),      10, Math.Abs(r.AvgWT  - minWT)  < 0.001);
                Highlight(r.AvgTAT.ToString("F2"),     10, Math.Abs(r.AvgTAT - minTAT) < 0.001);
                Highlight(r.Throughput.ToString("F4"), 10, Math.Abs(r.Throughput - maxTP) < 0.0001);
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  (* Xanh lá = Tốt nhất trong tiêu chí đó)");
            Console.ResetColor();

            // Auto-detect convoy effect
            var fcfs = results.FirstOrDefault(r => r.Name.StartsWith("FCFS"));
            var best = results.MinBy(r => r.AvgWT);
            if (fcfs != null && best != null
                && !fcfs.Name.Equals(best.Name)
                && fcfs.AvgWT > best.AvgWT * 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    $"\n  ⚠  Convoy Effect? " +
                    $"FCFS Avg WT ({fcfs.AvgWT:F2}) >> {best.Name.Trim()} ({best.AvgWT:F2})");
                Console.ResetColor();
            }
        }

        private static void Highlight(string val, int width, bool on)
        {
            if (on) Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(val.PadRight(width));
            Console.ResetColor();
        }
    }
}