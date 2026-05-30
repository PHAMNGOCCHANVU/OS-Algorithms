namespace PageReplacementDemo.Algorithms.DeadlockDetection;

/// <summary>
/// Implements 6 deadlock management features from C++ deadlock.cpp
/// </summary>
public static class DeadlockSystemLogic
{
    // ========== HELPER METHODS ==========
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

    // ========== 1. CALCULATE INITIAL STATE ==========
    /// <summary>
    /// Tính toán các thông số ban đầu: Available, Need
    /// </summary>
    public static void CalculateInitialState(DeadlockSystemData data)
    {
        if (data.Total == null || data.Allocation == null || data.Max == null)
            throw new ArgumentNullException("Data không được khởi tạo");

        int n = data.NumProcesses;
        int m = data.NumResources;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("  [BƯỚC 1: TÍNH TOÁN CÁC THÔNG SỐ BAN ĐẦU]");
        Console.WriteLine("=======================================================");
        Console.ResetColor();

        // 1. Tính Sum Allocation
        var sumAlloc = new int[m];
        for (int j = 0; j < m; j++)
        {
            for (int i = 0; i < n; i++)
                sumAlloc[j] += data.Allocation[i][j];
        }
        Console.Write("1. Tổng tài nguyên đã cấp phát (Sum Allocation): ");
        PrintVector(sumAlloc);
        Console.WriteLine("\n");

        // 2. Tính Available = Total - Sum Allocation
        if (data.Available == null)
            data.Available = new int[m];

        for (int j = 0; j < m; j++)
            data.Available[j] = data.Total[j] - sumAlloc[j];

        Console.Write("2. Tài nguyên sẵn có (Available = Total - Sum Allocation): ");
        PrintVector(data.Total); Console.Write(" - "); PrintVector(sumAlloc); Console.Write(" = "); PrintVector(data.Available);
        Console.WriteLine("\n");

        // 3. Tính Need = Max - Allocation
        Console.WriteLine("3. Ma trận nhu cầu (Need = Max - Allocation):");
        for (int i = 0; i < n; i++)
        {
            Console.Write($"   - P{i + 1}: ");
            for (int j = 0; j < m; j++)
                data.Need![i][j] = data.Max[i][j] - data.Allocation[i][j];
            PrintVector(data.Max[i]); Console.Write(" - "); PrintVector(data.Allocation[i]); Console.Write(" = "); PrintVector(data.Need[i]);
            Console.WriteLine();
        }
        Console.WriteLine("-------------------------------------------------------\n");
    }

    // ========== 2. SAFETY ALGORITHM (BANKER'S) ==========
    /// <summary>
    /// Kiểm tra trạng thái an toàn (Safety Algorithm)
    /// </summary>
    public static bool IsSafeState(DeadlockSystemData data, out List<int> safeSeq, bool printTrace = true)
    {
        safeSeq = new List<int>();

        if (data.Available == null || data.Allocation == null || data.Need == null || data.Terminated == null)
            throw new ArgumentNullException("Data không được khởi tạo");

        int n = data.NumProcesses;
        int m = data.NumResources;

        var work = new int[m];
        Array.Copy(data.Available, work, m);

        var finish = new bool[n];
        Array.Copy(data.Terminated, finish, n);

        if (printTrace)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n--- CHẠY THUẬT TOÁN SAFETY ---");
            Console.ResetColor();
            Console.Write("Khởi tạo: Work = Available = ");
            PrintVector(work);
            Console.WriteLine("\n");
        }

        int terminatedCount = finish.Count(t => t);
        int step = 1;

        while (safeSeq.Count < (n - terminatedCount))
        {
            bool found = false;

            if (printTrace)
                Console.WriteLine($">> VÒNG LẶP {step++}:");

            for (int i = 0; i < n; i++)
            {
                if (finish[i]) continue;

                if (printTrace)
                {
                    Console.Write($"   Kiểm tra P{i + 1}: Need ");
                    PrintVector(data.Need[i]);
                }

                // Check if Need[i] <= Work
                bool canProceed = true;
                for (int j = 0; j < m; j++)
                {
                    if (data.Need[i][j] > work[j])
                    {
                        canProceed = false;
                        break;
                    }
                }

                if (canProceed)
                {
                    if (printTrace)
                    {
                        Console.Write(" <= Work ");
                        PrintVector(work);
                        Console.WriteLine(" -> THỎA MÃN!");
                    }

                    // Update Work = Work + Allocation[i]
                    var oldWork = new int[m];
                    Array.Copy(work, oldWork, m);

                    for (int j = 0; j < m; j++)
                        work[j] += data.Allocation[i][j];

                    if (printTrace)
                    {
                        Console.Write($"      Cập nhật Work = Work + Alloc(P{i + 1}) = ");
                        PrintVector(oldWork); Console.Write(" + "); PrintVector(data.Allocation[i]); Console.Write(" = "); PrintVector(work);
                        Console.WriteLine();
                        Console.WriteLine($"      Finish[P{i + 1}] = TRUE\n");
                    }

                    finish[i] = true;
                    safeSeq.Add(i);
                    found = true;
                    break;
                }
                else
                {
                    if (printTrace)
                    {
                        Console.Write(" > Work ");
                        PrintVector(work);
                        Console.WriteLine(" -> BỎ QUA, đợi!\n");
                    }
                }
            }

            if (!found) break;
        }

        if (safeSeq.Count == (n - terminatedCount))
        {
            if (printTrace)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("=> KẾT LUẬN: Hệ thống AN TOÀN.");
                Console.ResetColor();
            }
            return true;
        }
        else
        {
            if (printTrace)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("=> KẾT LUẬN: Hệ thống NGUY HIỂM (Unsafe).");
                Console.ResetColor();
            }
            return false;
        }
    }

    // ========== 3. RESOURCE REQUEST ==========
    /// <summary>
    /// Kiểm tra và cấp phát tài nguyên cho tiến trình
    /// </summary>
    public static bool RequestResources(DeadlockSystemData data, int pid, int[] request, out string message)
    {
        message = "";

        if (data.Available == null || data.Allocation == null || data.Need == null || data.Terminated == null)
            throw new ArgumentNullException("Data không được khởi tạo");

        if (pid < 0 || pid >= data.NumProcesses)
        {
            message = $"[Lỗi] Tiến trình P{pid + 1} không tồn tại!";
            return false;
        }

        if (data.Terminated[pid])
        {
            message = $"[Lỗi] Tiến trình P{pid + 1} đã bị hệ thống hủy! Không thể yêu cầu.";
            return false;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n--- CHẠY THUẬT TOÁN RESOURCE-REQUEST CHO P{pid + 1} ---");
        Console.ResetColor();

        int m = data.NumResources;

        // Bước 1: Check request <= need
        for (int j = 0; j < m; j++)
        {
            if (request[j] > data.Need[pid][j])
            {
                message = "Bước 1: [TỪ CHỐI] Tiến trình xin vượt quá Max đã khai báo.";
                Console.WriteLine(message);
                return false;
            }
        }

        // Bước 2: Check request <= available
        for (int j = 0; j < m; j++)
        {
            if (request[j] > data.Available[j])
            {
                message = "Bước 2: [CHỜ ĐỢI] Hệ thống không đủ tài nguyên hiện tại.";
                Console.WriteLine(message);
                return false;
            }
        }

        Console.WriteLine("Bước 3: Giả lập cấp phát tài nguyên...");

        // Backup state
        var backupAlloc = new int[data.NumProcesses][];
        var backupNeed = new int[data.NumProcesses][];
        var backupAvail = new int[m];

        for (int i = 0; i < data.NumProcesses; i++)
        {
            backupAlloc[i] = new int[m];
            backupNeed[i] = new int[m];
            Array.Copy(data.Allocation[i], backupAlloc[i], m);
            Array.Copy(data.Need[i], backupNeed[i], m);
        }
        Array.Copy(data.Available, backupAvail, m);

        // Cập nhật trạng thái giả lập
        for (int j = 0; j < m; j++)
        {
            data.Available[j] -= request[j];
            data.Allocation[pid][j] += request[j];
            data.Need[pid][j] -= request[j];
        }

        Console.WriteLine("Bước 4: Kiểm tra tính an toàn sau giả lập");

        var safeSeq = new List<int>();
        if (IsSafeState(data, out safeSeq, printTrace: false))
        {
            message = "\n=> KẾT LUẬN: Yêu cầu được chấp nhận!";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
            return true;
        }
        else
        {
            message = "\n=> KẾT LUẬN: Cấp phát gây ra Unsafe. TỪ CHỐI VÀ PHỤC HỒI!";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();

            // Restore backup
            for (int i = 0; i < data.NumProcesses; i++)
            {
                Array.Copy(backupAlloc[i], data.Allocation[i], m);
                Array.Copy(backupNeed[i], data.Need[i], m);
            }
            Array.Copy(backupAvail, data.Available, m);

            return false;
        }
    }

    // ========== 4. GET DEADLOCK LIST ==========
    /// <summary>
    /// Lấy danh sách các tiến trình bị deadlock
    /// </summary>
    private static List<int> GetDeadlockList(DeadlockSystemData data)
    {
        if (data.Available == null || data.Allocation == null || data.RequestMatrix == null || data.Terminated == null)
            throw new ArgumentNullException("Data không được khởi tạo");

        int n = data.NumProcesses;
        int m = data.NumResources;

        var work = new int[m];
        Array.Copy(data.Available, work, m);

        var finish = new bool[n];
        Array.Copy(data.Terminated, finish, n);

        var dlList = new List<int>();

        // Mark processes with no allocation
        for (int i = 0; i < n; i++)
        {
            if (finish[i]) continue;
            bool hasAlloc = false;
            for (int j = 0; j < m; j++)
                if (data.Allocation[i][j] > 0)
                {
                    hasAlloc = true;
                    break;
                }
            if (!hasAlloc) finish[i] = true;
        }

        // Banker's algorithm
        bool found;
        do
        {
            found = false;
            for (int i = 0; i < n; i++)
            {
                if (!finish[i])
                {
                    bool canProceed = true;
                    for (int j = 0; j < m; j++)
                    {
                        if (data.RequestMatrix[i][j] > work[j])
                        {
                            canProceed = false;
                            break;
                        }
                    }

                    if (canProceed)
                    {
                        for (int j = 0; j < m; j++)
                            work[j] += data.Allocation[i][j];
                        finish[i] = true;
                        found = true;
                    }
                }
            }
        } while (found);

        // Collect unfinished processes
        for (int i = 0; i < n; i++)
        {
            if (!finish[i])
                dlList.Add(i);
        }

        return dlList;
    }

    // ========== 5. DETECT DEADLOCK ==========
    /// <summary>
    /// Phát hiện deadlock trong hệ thống
    /// </summary>
    public static bool DetectDeadlock(DeadlockSystemData data, int[][] requestMatrix, out List<int> dlList)
    {
        dlList = new List<int>();

        if (data.Available == null || data.Allocation == null || data.Terminated == null)
            throw new ArgumentNullException("Data không được khởi tạo");

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n--- THUẬT TOÁN PHÁT HIỆN DEADLOCK ---");
        Console.ResetColor();

        // Set request matrix
        data.RequestMatrix = requestMatrix;

        dlList = GetDeadlockList(data);

        if (dlList.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n=> KẾT LUẬN: Tất cả đều Finish. Hệ thống KHÔNG CÓ DEADLOCK.");
            Console.ResetColor();
            return false;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n=> KẾT LUẬN: Hệ thống ĐANG BỊ DEADLOCK!");
            Console.Write("=> Các tiến trình gây kết: ");
            foreach (int p in dlList)
                Console.Write($"P{p + 1} ");
            Console.WriteLine();
            Console.ResetColor();
            return true;
        }
    }

    // ========== 6. RECOVER DEADLOCK ==========
    /// <summary>
    /// Phục hồi deadlock bằng cách abort tiến trình (theo tiêu chí tài nguyên sử dụng)
    /// </summary>
    public static List<string> RecoverDeadlock(DeadlockSystemData data, out List<int> abortedProcesses)
    {
        abortedProcesses = new List<int>();
        var recoverySteps = new List<string>();

        if (data.Available == null || data.Allocation == null || data.RequestMatrix == null || data.Terminated == null)
            throw new ArgumentNullException("Data không được khởi tạo");

        if (data.RequestMatrix == null || data.RequestMatrix.Length == 0)
        {
            string msg = "\n[Lỗi] Bạn chưa chạy chức năng 5 (Phát hiện Deadlock) để cập nhật Request!";
            recoverySteps.Add(msg);
            Console.WriteLine(msg);
            return recoverySteps;
        }

        var dlList = GetDeadlockList(data);

        if (dlList.Count == 0)
        {
            string msg = "\n-> Hệ thống hiện KHÔNG có Deadlock. Không cần phục hồi!";
            recoverySteps.Add(msg);
            Console.WriteLine(msg);
            return recoverySteps;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("       THUẬT TOÁN PHỤC HỒI DEADLOCK (ABORT PROCESS)");
        Console.WriteLine("=======================================================");
        Console.ResetColor();
        Console.Write("[Cảnh báo] Hệ thống đang bị Deadlock bởi các tiến trình: ");
        foreach (int p in dlList)
            Console.Write($"P{p + 1} ");
        Console.WriteLine("\n");

        Console.WriteLine(">> THỰC HIỆN: Chấm dứt lần lượt từng tiến trình (theo tiêu chí 'Tài nguyên đã sử dụng')...\n");

        int step = 1;
        int m = data.NumResources;

        while (dlList.Count > 0)
        {
            // Chọn tiến trình có tổng tài nguyên ghi nhận nhiều nhất
            int victim = dlList[0];
            int maxAllocated = -1;

            foreach (int p in dlList)
            {
                int sumAlloc = 0;
                for (int j = 0; j < m; j++)
                    sumAlloc += data.Allocation[p][j];

                if (sumAlloc > maxAllocated)
                {
                    maxAllocated = sumAlloc;
                    victim = p;
                }
            }

            string stepMsg = $"   [Bước {step++}] Chọn nạn nhân P{victim + 1} (Đang giữ tổng cộng {maxAllocated} tài nguyên).";
            recoverySteps.Add(stepMsg);
            Console.WriteLine(stepMsg);

            var oldAvail = new int[m];
            Array.Copy(data.Available, oldAvail, m);

            var victimAlloc = new int[m];
            Array.Copy(data.Allocation[victim], victimAlloc, m);

            Console.Write("      - Thu hồi Alloc(P" + (victim + 1) + ") = ");
            PrintVector(victimAlloc);
            Console.WriteLine();

            // Release resources
            for (int j = 0; j < m; j++)
            {
                data.Available[j] += data.Allocation[victim][j];
                data.Allocation[victim][j] = 0;
                data.Need![victim][j] = 0;
                if (data.RequestMatrix != null)
                    data.RequestMatrix[victim][j] = 0;
            }
            data.Terminated[victim] = true;
            abortedProcesses.Add(victim);

            Console.Write("      - Available Mới = Available Cũ + Thu Hồi = ");
            PrintVector(oldAvail);
            Console.Write(" + ");
            PrintVector(victimAlloc);
            Console.Write(" = ");
            PrintVector(data.Available);
            Console.WriteLine();

            // Re-detect deadlock
            dlList = GetDeadlockList(data);
            if (dlList.Count > 0)
            {
                string dlMsg = "      -> Deadlock VẪN CÒN (Các tiến trình kết: ";
                foreach (int p in dlList)
                    dlMsg += $"P{p + 1} ";
                dlMsg += "). Tiếp tục tìm nạn nhân...";
                recoverySteps.Add(dlMsg);
                Console.WriteLine(dlMsg + "\n");
            }
        }

        string finalMsg = "\n=> [THÀNH CÔNG] Hệ thống đã phá vỡ vòng lặp và thoát khỏi Deadlock!";
        recoverySteps.Add(finalMsg);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(finalMsg);
        Console.Write("   - Tài nguyên rảnh rỗi hiện tại (Available): ");
        PrintVector(data.Available);
        Console.WriteLine();
        Console.ResetColor();

        return recoverySteps;
    }
}
