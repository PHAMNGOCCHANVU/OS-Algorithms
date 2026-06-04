using System;
using System.Collections.Generic;
using System.Linq;

namespace PageReplacementDemo.Algorithms.DeadlockDetection;

public static class DeadlockSystemLogic
{
    // --- CÁC HÀM HỖ TRỢ IN MẢNG VÀ LOG MÔ PHỎNG GIỐNG C++ ---
    public static void PrintVector(int[] vec)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("[ ");
        foreach (int x in vec) Console.Write($"{x,2} ");
        Console.Write("]");
        Console.ResetColor();
    }

    public static void PrintMathOp(int[] a, string op, int[] b, int[] res)
    {
        PrintVector(a);
        Console.Write($" {op} ");
        PrintVector(b);
        Console.Write(" = ");
        PrintVector(res);
    }

    // --- 1. TÍNH TOÁN THÔNG SỐ BAN ĐẦU ---
    public static void CalculateInitialState(DeadlockSystemData data)
    {
        int m = data.NumResources;
        int n = data.NumProcesses;
        
        var sumAlloc = new int[m];
        if (data.Available == null) data.Available = new int[m];
        if (data.Need == null) 
        {
            data.Need = new int[n][];
            for(int i=0; i<n; i++) data.Need[i] = new int[m];
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n┌───────────────────────────────────────────────────────┐");
        Console.WriteLine("│      [BƯỚC 1: TÍNH TOÁN CÁC THÔNG SỐ BAN ĐẦU]         │");
        Console.WriteLine("└───────────────────────────────────────────────────────┘");
        Console.ResetColor();

        for (int j = 0; j < m; j++)
        {
            for (int i = 0; i < n; i++) sumAlloc[j] += data.Allocation![i][j];
        }
        Console.Write("1. Tổng tài nguyên đã cấp phát (Sum Allocation):\n   ");
        PrintVector(sumAlloc); Console.WriteLine("\n");

        for (int j = 0; j < m; j++) data.Available[j] = data.Total![j] - sumAlloc[j];
        Console.Write("2. Tài nguyên sẵn có (Available = Total - Sum Allocation):\n   ");
        PrintMathOp(data.Total!, "-", sumAlloc, data.Available); Console.WriteLine("\n");

        Console.WriteLine("3. Ma trận nhu cầu (Need = Max - Allocation):");
        for (int i = 0; i < n; i++)
        {
            Console.Write($"   - P{i + 1}: ");
            for (int j = 0; j < m; j++) data.Need[i][j] = data.Max![i][j] - data.Allocation![i][j];
            PrintMathOp(data.Max![i], "-", data.Allocation![i], data.Need[i]); Console.WriteLine();
        }
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("─────────────────────────────────────────────────────────\n");
        Console.ResetColor();
    }

    // --- 2. KIỂM TRA TRẠNG THÁI AN TOÀN (BANKER'S SAFETY) ---
    public static bool IsSafeState(DeadlockSystemData data, out List<int> safeSeq, bool printTrace = true)
    {
        int n = data.NumProcesses;
        int m = data.NumResources;
        var work = new int[m];
        Array.Copy(data.Available!, work, m);
        
        var finish = new bool[n];
        Array.Copy(data.Terminated!, finish, n);
        
        safeSeq = new List<int>();
        int step = 1;

        if (printTrace)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n┌───────────────────────────────────────────────────────┐");
            Console.WriteLine("│        MÔ PHỎNG THUẬT TOÁN AN TOÀN (SAFETY)           │");
            Console.WriteLine("└───────────────────────────────────────────────────────┘");
            Console.ResetColor();
        }

        int terminatedCount = finish.Count(t => t);
        bool globalFound;
        
        do
        {
            globalFound = false;
            if (printTrace)
            {
                Console.Write($"\n>> [VÒNG DUYỆT {step++}] Work hiện tại = ");
                PrintVector(work); Console.WriteLine();
            }

            for (int i = 0; i < n; i++)
            {
                if (finish[i])
                {
                    if (printTrace) Console.WriteLine($"   + P{i + 1}: Đã hoàn thành   -> Bỏ qua");
                    continue;
                }

                if (printTrace)
                {
                    Console.Write($"   + P{i + 1}: Need "); PrintVector(data.Need![i]);
                    Console.Write(" <= Work ? ");
                }

                bool canProceed = true;
                for (int j = 0; j < m; j++)
                {
                    if (data.Need![i][j] > work[j]) { canProceed = false; break; }
                }

                if (canProceed)
                {
                    if (printTrace)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"-> ĐÚNG (Thỏa mãn!) => Finish[P{i + 1}] = true");
                        Console.ResetColor();
                        Console.WriteLine($"      [Thực thi] P{i + 1} chạy xong & trả tài nguyên:");
                        Console.Write($"      Work_mới = Work_cũ + Alloc(P{i + 1}) = ");

                        var oldWork = new int[m];
                        Array.Copy(work, oldWork, m);
                        for (int j = 0; j < m; j++) work[j] += data.Allocation![i][j];
                        PrintMathOp(oldWork, "+", data.Allocation![i], work); Console.WriteLine();
                        Console.WriteLine($"      => Ghi nhận P{i + 1} vào chuỗi an toàn. (Bắt đầu duyệt lại từ đầu)");
                    }
                    else
                    {
                        for (int j = 0; j < m; j++) work[j] += data.Allocation![i][j];
                    }

                    finish[i] = true;
                    safeSeq.Add(i);
                    globalFound = true;
                    break;
                }
                else
                {
                    if (printTrace)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"-> SAI  (Thiếu tài nguyên -> P{i + 1} phải đợi) => Finish[P{i + 1}] = false");
                        Console.ResetColor();
                    }
                }
            }
        } while (globalFound);

        if (safeSeq.Count == (n - terminatedCount))
        {
            if (printTrace)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n=> KẾT LUẬN: Tất cả tiến trình đều đạt Finish = TRUE.");
                Console.WriteLine("=> HỆ THỐNG AN TOÀN (SAFE STATE).");
                Console.ResetColor();
            }
            return true;
        }
        else
        {
            if (printTrace)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n=> KẾT LUẬN: Duyệt kiệt để nhưng vẫn còn tiến trình chưa Finish.");
                Console.WriteLine("=> HỆ THỐNG KHÔNG AN TOÀN (UNSAFE STATE).");
                Console.Write("=> Các tiến trình bị kẹt: ");
                for (int i = 0; i < n; i++) if (!finish[i]) Console.Write($"P{i + 1} ");
                Console.WriteLine("\n");
                Console.ResetColor();
            }
            return false;
        }
    }

    // --- 3. CẤP PHÁT TÀI NGUYÊN (RESOURCE-REQUEST) ---
    public static bool RequestResources(DeadlockSystemData data, int pid, int[] request, out string message)
    {
        message = "";
        int m = data.NumResources;

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("\n┌───────────────────────────────────────────────────────┐");
        Console.WriteLine($"│    MÔ PHỎNG ALGORITHM: RESOURCE-REQUEST CHO P{pid + 1,-8} │");
        Console.WriteLine("└───────────────────────────────────────────────────────┘");
        Console.ResetColor();

        Console.Write("\x1b[1m[BƯỚC 1]\x1b[0m"); Console.WriteLine($" Kiểm tra Request <= Need của P{pid + 1}");
        Console.Write("         Request: "); PrintVector(request); Console.WriteLine();
        Console.Write("         Need:    "); PrintVector(data.Need![pid]); Console.WriteLine();
        for (int j = 0; j < m; j++)
        {
            if (request[j] > data.Need[pid][j])
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  => KẾT QUẢ: LỖI! Yêu cầu vượt quá Nhu cầu Tối đa (Max).");
                Console.ResetColor();
                return false;
            }
        }
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("  => KẾT QUẢ: Hợp lệ! (Request <= Need)\n");
        Console.ResetColor();

        Console.Write("\x1b[1m[BƯỚC 2]\x1b[0m"); Console.WriteLine(" Kiểm tra Request <= Available hiện tại");
        Console.Write("         Request:   "); PrintVector(request); Console.WriteLine();
        Console.Write("         Available: "); PrintVector(data.Available!); Console.WriteLine();
        for (int j = 0; j < m; j++)
        {
            if (request[j] > data.Available![j])
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("  => KẾT QUẢ: KHÔNG ĐỦ TÀI NGUYÊN! Hệ thống chỉ còn "); PrintVector(data.Available); Console.WriteLine();
                Console.ResetColor();
                return false;
            }
        }
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("  => KẾT QUẢ: Đủ tài nguyên để cấp phát!\n");
        Console.ResetColor();

        Console.Write("\x1b[1m[BƯỚC 3]\x1b[0m"); Console.WriteLine(" GIẢ LẬP CẤP PHÁT tài nguyên để kiểm tra rủi ro:");
        
        var backupAlloc = new int[data.NumProcesses][];
        var backupNeed = new int[data.NumProcesses][];
        var backupAvail = new int[m];

        for (int i = 0; i < data.NumProcesses; i++)
        {
            backupAlloc[i] = new int[m];
            backupNeed[i] = new int[m];
            Array.Copy(data.Allocation![i], backupAlloc[i], m);
            Array.Copy(data.Need[i], backupNeed[i], m);
        }
        Array.Copy(data.Available!, backupAvail, m);

        for (int j = 0; j < m; j++)
        {
            data.Available![j] -= request[j];
            data.Allocation![pid][j] += request[j];
            data.Need[pid][j] -= request[j];
        }

        Console.Write("   - Available Mới = "); PrintMathOp(backupAvail, "-", request, data.Available!); Console.WriteLine();
        Console.Write($"   - Alloc(P{pid + 1})  = "); PrintMathOp(backupAlloc[pid], "+", request, data.Allocation![pid]); Console.WriteLine();
        Console.Write($"   - Need(P{pid + 1})   = "); PrintMathOp(backupNeed[pid], "-", request, data.Need[pid]); Console.WriteLine("\n");

        Console.Write("\x1b[1m[BƯỚC 4]\x1b[0m"); Console.WriteLine(" Chạy Thuật toán Safety trên trạng thái giả lập mới:");
        
        var safeSeq = new List<int>();
        if (IsSafeState(data, out safeSeq, true))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n=> KẾT LUẬN CUỐI CÙNG: Chấp nhận cấp phát thực tế cho P{pid + 1}!");
            Console.ResetColor();
            return true;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n=> KẾT LUẬN CUỐI CÙNG: TỪ CHỐI CẤP PHÁT & PHỤC HỒI trạng thái cũ!");
            Console.WriteLine("   Lý do: Nếu cấp phát sẽ làm hệ thống rơi vào vùng không an toàn.");
            Console.ResetColor();
            
            for (int i = 0; i < data.NumProcesses; i++)
            {
                Array.Copy(backupAlloc[i], data.Allocation![i], m);
                Array.Copy(backupNeed[i], data.Need![i], m);
            }
            Array.Copy(backupAvail, data.Available!, m);
            return false;
        }
    }

    // --- 4. TÌM KIẾM LOGIC DEADLOCK (CHUẨN C++) ---
    public static List<int> GetDeadlockList(DeadlockSystemData data, bool printTrace = false)
    {
        int n = data.NumProcesses;
        int m = data.NumResources;
        var dlList = new List<int>();
        if (data.RequestMatrix == null) return dlList;

        var work = new int[m];
        Array.Copy(data.Available!, work, m);
        
        var finish = new bool[n];
        Array.Copy(data.Terminated!, finish, n);

        if (printTrace)
        {
            Console.Write("\x1b[1m"); Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n>> [BƯỚC 1] Khởi tạo hệ thống:"); Console.ResetColor();
            Console.Write("   Work = Available = "); PrintVector(work); Console.WriteLine();
        }

        for (int i = 0; i < n; i++)
        {
            if (data.Terminated![i]) continue;

            bool hasAlloc = false;
            for (int j = 0; j < m; j++)
            {
                if (data.Allocation![i][j] > 0) hasAlloc = true;
            }

            if (!hasAlloc)
            {
                finish[i] = true;
                if (printTrace) Console.WriteLine($"   - P{i + 1} có Allocation = 0 => Finish[P{i + 1}] = true");
            }
            else
            {
                finish[i] = false;
                if (printTrace) Console.WriteLine($"   - P{i + 1} có Allocation != 0 => Finish[P{i + 1}] = false");
            }
        }

        bool found;
        int loop = 1;
        do
        {
            found = false;
            if (printTrace)
            {
                Console.Write("\x1b[1m"); Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n>> [BƯỚC 2] [VÒNG DUYỆT {loop++}] Tìm i thỏa: Finish[i] == false & Request_i <= Work"); Console.ResetColor();
                Console.Write("   Work hiện tại = "); PrintVector(work); Console.WriteLine();
            }

            for (int i = 0; i < n; i++)
            {
                if (finish[i])
                {
                    if (printTrace) Console.WriteLine($"   + P{i + 1}: Đã có Finish = true -> Bỏ qua");
                    continue;
                }

                if (printTrace)
                {
                    Console.Write($"   + P{i + 1}: Request "); PrintVector(data.RequestMatrix[i]);
                    Console.Write(" <= Work ? ");
                }

                bool canProceed = true;
                for (int j = 0; j < m; j++)
                {
                    if (data.RequestMatrix[i][j] > work[j]) { canProceed = false; break; }
                }

                if (canProceed)
                {
                    if (printTrace)
                    {
                        Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("-> ĐÚNG (Thỏa mãn!)"); Console.ResetColor();
                        Console.Write("\x1b[1m"); Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"      >> [BƯỚC 3] Cập nhật: Work = Work + Allocation_i, Finish[P{i + 1}] = true"); Console.ResetColor();
                        Console.Write("      Work_mới = ");
                        var oldWork = new int[m];
                        Array.Copy(work, oldWork, m);
                        for (int j = 0; j < m; j++) work[j] += data.Allocation![i][j];
                        PrintMathOp(oldWork, "+", data.Allocation![i], work); Console.WriteLine();
                        Console.WriteLine("      => Quay về Bước 2 (duyệt lại từ đầu).");
                    }
                    else
                    {
                        for (int j = 0; j < m; j++) work[j] += data.Allocation![i][j];
                    }

                    finish[i] = true;
                    found = true;
                    break;
                }
                else
                {
                    if (printTrace)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"-> SAI  (Thiếu tài nguyên -> P{i + 1} bị treo) => Finish[P{i + 1}] = false");
                        Console.ResetColor();
                    }
                }
            }
        } while (found);

        if (printTrace)
        {
            Console.Write("\x1b[1m"); Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n>> [BƯỚC 4] TỔNG KẾT VÀ KẾT LUẬN:"); Console.ResetColor();
            Console.Write("   Tập Finish cuối cùng = { ");
            for (int i = 0; i < n; i++)
            {
                if (finish[i]) { Console.ForegroundColor = ConsoleColor.Green; Console.Write("true"); }
                else { Console.ForegroundColor = ConsoleColor.Red; Console.Write("false"); }
                Console.ResetColor();
                if (i < n - 1) Console.Write(", ");
            }
            Console.WriteLine(" }");
        }

        for (int i = 0; i < n; i++)
        {
            if (!finish[i]) dlList.Add(i);
        }
        return dlList;
    }

    // --- 5. PHÁT HIỆN DEADLOCK ---
    public static bool DetectDeadlock(DeadlockSystemData data, out List<int> dlList)
    {
        Console.WriteLine("\n=======================================================");
        dlList = GetDeadlockList(data, true);

        if (dlList.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n=> KẾT LUẬN (Theo Bước 4): Tất cả Finish = true.");
            Console.WriteLine("=> HỆ THỐNG KHÔNG CÓ DEADLOCK (DEADLOCK-FREE).");
            Console.ResetColor();
            return false;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\x1b[1m");
            Console.WriteLine("\n=> KẾT LUẬN (Theo Bước 4): Tồn tại i với Finish[i] = false.");
            Console.WriteLine("=> HỆ THỐNG ĐANG Ở TRẠNG THÁI DEADLOCK!");
            Console.Write("=> Các tiến trình bị deadlocked: ");
            foreach (int p in dlList) Console.Write($"P{p + 1} ");
            Console.WriteLine();
            Console.ResetColor();
            return true;
        }
    }

    // --- 6. PHỤC HỒI DEADLOCK ---
    public static void RecoverDeadlock(DeadlockSystemData data)
    {
        if (data.RequestMatrix == null || data.RequestMatrix.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n[Lỗi] Bạn chưa chạy chức năng 5 (Phát hiện Deadlock) để nạp ma trận Request!");
            Console.ResetColor();
            return;
        }

        var dlList = GetDeadlockList(data, false);

        if (dlList.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n-> Hệ thống hiện tại KHÔNG có Deadlock. Không cần phục hồi!");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("\n┌───────────────────────────────────────────────────────┐");
        Console.WriteLine("│         MÔ PHỎNG QUÁ TRÌNH PHỤC HỒI DEADLOCK          │");
        Console.WriteLine("└───────────────────────────────────────────────────────┘");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("[Cảnh báo] Đang tồn tại Deadlock trên các tiến trình: ");
        foreach (int p in dlList) Console.Write($"P{p + 1} ");
        Console.WriteLine("\n");
        Console.ResetColor();
        
        Console.WriteLine(">> TIÊU CHÍ KHÁCH QUAN: Hủy lần lượt tiến trình đang nắm giữ TỔNG TÀI NGUYÊN nhiều nhất.");

        int step = 1;
        int m = data.NumResources;

        while (dlList.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-------------------------------------------------------");
            Console.ResetColor();
            Console.Write("\x1b[1m"); Console.WriteLine($"[LƯỢT PHỤC HỒI {step++}]"); Console.ResetColor();

            int victim = dlList[0];
            int maxAllocated = -1;

            foreach (int p in dlList)
            {
                int sumAlloc = 0;
                foreach (int x in data.Allocation![p]) sumAlloc += x;
                
                Console.Write($"   - P{p + 1} đang giữ: "); PrintVector(data.Allocation[p]);
                Console.WriteLine($" -> Tổng cộng: {sumAlloc}");

                if (sumAlloc > maxAllocated)
                {
                    maxAllocated = sumAlloc;
                    victim = p;
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n  => QUYẾT ĐỊNH: Hủy (Abort) P{victim + 1} vì chiếm dụng nhiều nhất ({maxAllocated}).");
            Console.ResetColor();

            var oldAvail = new int[m];
            Array.Copy(data.Available!, oldAvail, m);
            var victimAlloc = new int[m];
            Array.Copy(data.Allocation![victim], victimAlloc, m);

            for (int j = 0; j < m; j++)
            {
                data.Available![j] += data.Allocation[victim][j];
                data.Allocation[victim][j] = 0;
                data.Need![victim][j] = 0;
                data.RequestMatrix[victim][j] = 0;
            }
            data.Terminated![victim] = true;

            Console.WriteLine($"   [Thực thi] Giải phóng và thu hồi từ P{victim + 1}:");
            Console.WriteLine($"     - Available_mới = Available_cũ + Alloc(P{victim + 1})");
            Console.Write("     - "); PrintMathOp(oldAvail, "+", victimAlloc, data.Available!); Console.WriteLine();

            dlList = GetDeadlockList(data, false);
            if (dlList.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n   => KẾT QUẢ: Deadlock VẪN CÒN! Các tiến trình kẹt: ");
                foreach (int p in dlList) Console.Write($"P{p + 1} ");
                Console.WriteLine("\n      (Tiếp tục vòng lặp thu hồi...)");
                Console.ResetColor();
            }
        }
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("\x1b[1m");
        Console.WriteLine("\n=> PHỤC HỒI THÀNH CÔNG! Hệ thống đã THOÁT KHỎI DEADLOCK.");
        Console.Write("=> Tài nguyên rảnh rỗi hiện tại (Available): "); PrintVector(data.Available!); Console.WriteLine();
        Console.ResetColor();
    }
}
