namespace PageReplacementDemo.Program;

using PageReplacementDemo.Algorithms.DeadlockDetection;

/// <summary>
/// Display và Input helpers cho Deadlock System
/// </summary>
public static class DeadlockSystemHelpers
{
    // ========== DISPLAY HELPERS ==========
    private static void PrintVector(int[] vec)
    {
        Console.Write("[ ");
        foreach (int x in vec) Console.Write(x + " ");
        Console.Write("]");
    }

    private static void PrintMatrix(int[][] matrix, string label = "")
    {
        if (!string.IsNullOrEmpty(label))
            Console.WriteLine(label);
        foreach (var row in matrix)
        {
            Console.Write("  ");
            PrintVector(row);
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Hiển thị các ma trận hệ thống (Allocation, Max, Need, Available)
    /// </summary>
    public static void DisplayMatrices(DeadlockSystemData data)
    {
        if (data.Allocation == null || data.Max == null || data.Need == null || data.Available == null)
            return;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== THÔNG TIN HỆ THỐNG ===\n");
        Console.ResetColor();

        Console.WriteLine("1. Ma trận Cấp Phát (Allocation):");
        PrintMatrix(data.Allocation);

        Console.WriteLine("\n2. Ma trận Nhu Cầu Tối Đa (Max):");
        PrintMatrix(data.Max);

        Console.WriteLine("\n3. Ma trận Nhu Cầu (Need = Max - Allocation):");
        PrintMatrix(data.Need);

        Console.Write("\n4. Tài Nguyên Sẵn Có (Available): ");
        PrintVector(data.Available);
        Console.WriteLine("\n");
    }

    /// <summary>
    /// Hiển thị kết quả Safety Check
    /// </summary>
    public static void DisplaySafetyResult(bool isSafe, List<int> sequence)
    {
        if (isSafe)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n✓ TRẠNG THÁI AN TOÀN");
            Console.Write("Chuỗi an toàn: < ");
            foreach (int p in sequence)
                Console.Write($"P{p + 1} ");
            Console.WriteLine(">");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n✗ TRẠNG THÁI NGUY HIỂM");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Hiển thị kết quả Resource Request
    /// </summary>
    public static void DisplayResourceRequestResult(bool isApproved, string message)
    {
        if (isApproved)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ " + message);
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠ " + message);
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Hiển thị kết quả Deadlock Detection
    /// </summary>
    public static void DisplayDeadlockResult(bool hasDeadlock, List<int> dlList)
    {
        if (!hasDeadlock)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Hệ thống KHÔNG CÓ DEADLOCK");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("✗ Hệ thống ĐANG CÓ DEADLOCK");
            Console.Write("  Các tiến trình bị kết: ");
            foreach (int p in dlList)
                Console.Write($"P{p + 1} ");
            Console.WriteLine();
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Hiển thị các bước Recovery
    /// </summary>
    public static void DisplayRecoverySteps(List<string> steps, DeadlockSystemData finalData)
    {
        Console.WriteLine("\n--- TÓTEM TẮT QUÁ TRÌNH PHỤC HỒI ---");
        foreach (var step in steps)
            Console.WriteLine(step);

        Console.WriteLine("\n--- TRẠNG THÁI CUỐI CÙNG ---");
        Console.Write("Available: ");
        PrintVector(finalData.Available!);
        Console.WriteLine("\n");
    }

    // ========== INPUT HELPERS ==========
    /// <summary>
    /// Nhập dữ liệu hệ thống từ bàn phím
    /// </summary>
    public static DeadlockSystemData InputSystemData()
    {
        DisplayHelpers.SafeClear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("╔═════════════════════════════════════════════════════╗");
        Console.WriteLine("║   NHẬP DỮ LIỆU HỆ THỐNG TỪ BÀN PHÍM               ║");
        Console.WriteLine("╚═════════════════════════════════════════════════════╝");
        Console.ResetColor();

        Console.Write("\nSố tiến trình (N): ");
        if (!int.TryParse(Console.ReadLine(), out int n) || n <= 0)
        {
            Console.WriteLine("Giá trị không hợp lệ!");
            return new DeadlockSystemData();
        }

        Console.Write("Số loại tài nguyên (M): ");
        if (!int.TryParse(Console.ReadLine(), out int m) || m <= 0)
        {
            Console.WriteLine("Giá trị không hợp lệ!");
            return new DeadlockSystemData();
        }

        var data = new DeadlockSystemData(n, m);

        // Total resources
        Console.Write($"\n1. Tổng tài nguyên ({m} số): ");
        var totalInput = Console.ReadLine()?.Split();
        if (totalInput != null && totalInput.Length == m)
        {
            for (int j = 0; j < m; j++)
                if (int.TryParse(totalInput[j], out int val))
                    data.Total![j] = val;
        }

        // Max matrix
        Console.WriteLine("\n2. Ma trận Nhu Cầu Tối Đa (Max) - Nhập từng hàng:");
        for (int i = 0; i < n; i++)
        {
            Console.Write($"   P{i + 1}: ");
            var input = Console.ReadLine()?.Split();
            if (input != null && input.Length == m)
            {
                for (int j = 0; j < m; j++)
                    if (int.TryParse(input[j], out int val))
                        data.Max![i][j] = val;
            }
        }

        // Allocation matrix
        Console.WriteLine("\n3. Ma trận Cấp Phát (Allocation) - Nhập từng hàng:");
        for (int i = 0; i < n; i++)
        {
            Console.Write($"   P{i + 1}: ");
            var input = Console.ReadLine()?.Split();
            if (input != null && input.Length == m)
            {
                for (int j = 0; j < m; j++)
                    if (int.TryParse(input[j], out int val))
                        data.Allocation![i][j] = val;
            }
        }

        // Calculate initial state
        DeadlockSystemLogic.CalculateInitialState(data);

        return data;
    }

    /// <summary>
    /// Nhập Resource Request cho một tiến trình
    /// </summary>
    public static int[] InputResourceRequest(int m, int processId)
    {
        var request = new int[m];
        Console.Write($"Nhập Request cho P{processId + 1} ({m} số): ");
        var input = Console.ReadLine()?.Split();
        if (input != null && input.Length == m)
        {
            for (int j = 0; j < m; j++)
                if (int.TryParse(input[j], out int val))
                    request[j] = val;
        }
        return request;
    }

    /// <summary>
    /// Nhập Request Matrix cho tất cả tiến trình
    /// </summary>
    public static int[][] InputRequestMatrix(int n, int m)
    {
        var requestMat = new int[n][];
        Console.WriteLine($"\nNhập ma trận Request ({n} tiến trình, {m} tài nguyên):");
        for (int i = 0; i < n; i++)
        {
            Console.Write($"   P{i + 1}: ");
            var input = Console.ReadLine()?.Split();
            requestMat[i] = new int[m];
            if (input != null && input.Length == m)
            {
                for (int j = 0; j < m; j++)
                    if (int.TryParse(input[j], out int val))
                        requestMat[i][j] = val;
            }
        }
        return requestMat;
    }

    /// <summary>
    /// In menu chọn thao tác tiếp theo
    /// </summary>
    public static void PrintContinueMenu()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nNhấn [1] để tiếp tục, [0] để quay lại: ");
        Console.ResetColor();
    }
}
