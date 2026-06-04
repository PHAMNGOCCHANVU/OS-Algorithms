using System;
using System.Linq;
using PageReplacementDemo.Algorithms.DeadlockDetection;

namespace PageReplacementDemo.Program;

public static class DeadlockSystemHelpers
{
    // --- HÀM TƯƠNG ĐƯƠNG VỚI readFromConsole TRONG C++ ---
    public static DeadlockSystemData InputSystemData()
    {
        DisplayHelpers.SafeClear();
        Console.Write("Số tiến trình (N): ");
        int n = int.Parse(Console.ReadLine()!);
        Console.Write("Số loại tài nguyên (M): ");
        int m = int.Parse(Console.ReadLine()!);

        var data = new DeadlockSystemData(n, m);
        data.Total = new int[m];

        Console.Write($"1. Tổng tài nguyên (Total - {m} số): ");
        var totalStr = Console.ReadLine()!.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        for (int j = 0; j < m; j++) data.Total[j] = int.Parse(totalStr[j]);

        Console.WriteLine("2. Ma trận Yêu cầu tối đa (Max):");
        for (int i = 0; i < n; i++)
        {
            Console.Write($"   P{i + 1}: ");
            var maxStr = Console.ReadLine()!.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < m; j++) data.Max![i][j] = int.Parse(maxStr[j]);
        }

        Console.WriteLine("3. Ma trận Đã cấp phát (Allocation):");
        for (int i = 0; i < n; i++)
        {
            Console.Write($"   P{i + 1}: ");
            var allocStr = Console.ReadLine()!.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < m; j++) data.Allocation![i][j] = int.Parse(allocStr[j]);
        }

        DeadlockSystemLogic.CalculateInitialState(data);
        return data;
    }

    // --- CÁC HÀM XỬ LÝ NHẬP INPUT TRUNG GIAN CHO PROGRAM.CS ---
    public static void DisplayMatrices(DeadlockSystemData data)
    {
        // Trống vì IsSafeState đã lo liệu việc hiển thị
    }

    public static void DisplaySafetyResult(bool isSafe, System.Collections.Generic.List<int> safeSeq)
    {
        // Trống vì IsSafeState đã in luôn kết quả chuẩn xác như C++
    }

    public static int[] InputResourceRequest(int numRes, int pid)
    {
        Console.Write($"Nhập vector Request từ P{pid + 1} ({numRes} số): ");
        var parts = Console.ReadLine()!.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Select(int.Parse).ToArray();
    }

    public static void DisplayResourceRequestResult(bool isApproved, string message) { }

    public static int[][] InputRequestMatrix(int numProc, int numRes)
    {
        var reqMat = new int[numProc][];
        
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("\n┌───────────────────────────────────────────────────────┐");
        Console.WriteLine("│                  PHÁT HIỆN DEADLOCK                   │");
        Console.WriteLine("└───────────────────────────────────────────────────────┘");
        Console.ResetColor();

        Console.WriteLine("Nhập ma trận Request hiện tại (Yêu cầu đang bị treo từng tiến trình):");
        for (int i = 0; i < numProc; i++)
        {
            Console.Write($"  - P{i + 1} ({numRes} số): ");
            var parts = Console.ReadLine()!.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != numRes) reqMat[i] = new int[numRes];
            else reqMat[i] = parts.Select(int.Parse).ToArray();
        }
        return reqMat;
    }

    public static void DisplayDeadlockResult(bool hasDeadlock, System.Collections.Generic.List<int> dlList) { }
    
    public static void DisplayRecoverySteps(System.Collections.Generic.List<string> steps, DeadlockSystemData data) { }
}
