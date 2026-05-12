using PageReplacementDemo.Models;

namespace PageReplacementDemo;

/// <summary>
/// Chứa tất cả các hàm lấy dữ liệu đầu vào từ người dùng
/// </summary>
public static class InputHelpers
{
    // ============ PAGE REPLACEMENT ============
    public static int GetPageCount()
    {
        while (true)
        {
            Console.Write("Nhập số lượng trang (5-20): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int count) && count >= 5 && count <= 20)
            {
                return count;
            }
            Console.WriteLine("Số trang không hợp lệ! Vui lòng nhập số từ 5 đến 20.");
        }
    }

    public static int GetFrameCount()
    {
        while (true)
        {
            Console.Write("Nhập số lượng Frame (3-10): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int count) && count >= 3 && count <= 10)
            {
                return count;
            }
            Console.WriteLine("Số frame không hợp lệ! Vui lòng nhập số từ 3 đến 10.");
        }
    }

    public static int[] GetReferenceString(int pageCount)
    {
        while (true)
        {
            Console.Write($"Nhập chuỗi tham chiếu (các số từ 1 đến {pageCount}, cách nhau bằng khoảng trắng): ");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Chuỗi không được để trống!");
                continue;
            }

            string[] parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int[] result = new int[parts.Length];
            bool valid = true;

            for (int i = 0; i < parts.Length; i++)
            {
                if (!int.TryParse(parts[i], out result[i]) || result[i] < 1 || result[i] > pageCount)
                {
                    valid = false;
                    break;
                }
            }

            if (valid && result.Length > 0)
            {
                return result;
            }

            Console.WriteLine($"Chuỗi không hợp lệ! Vui lòng nhập các số từ 1 đến {pageCount}, cách nhau bằng khoảng trắng.");
        }
    }

    // ============ CPU SCHEDULING ============
    public static int GetProcessCount()
    {
        while (true)
        {
            Console.Write("Nhập số lượng tiến trình (2-10): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int count) && count >= 2 && count <= 10)
            {
                return count;
            }
            Console.WriteLine("Số tiến trình không hợp lệ! Vui lòng nhập số từ 2 đến 10.");
        }
    }

    public static int GetQuantumTime()
    {
        while (true)
        {
            Console.Write("Nhập Quantum Time (1-10): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int qt) && qt >= 1 && qt <= 10)
            {
                return qt;
            }
            Console.WriteLine("Quantum Time không hợp lệ! Vui lòng nhập số từ 1 đến 10.");
        }
    }

    /// <summary>
    /// Lấy thông tin tiến trình
    /// - Nếu needsPriority = true: Priority bắt buộc (AT BT PR)
    /// - Nếu needsPriority = false: Priority tùy chọn (AT BT hoặc AT BT PR)
    /// </summary>
    public static List<Process> GetProcesses(int numProcesses, bool needsPriority = false)
    {
        var processes = new List<Process>();
        Console.WriteLine();
        
        string prompt = needsPriority 
            ? "nhập AT BT PR (bắt buộc cả 3 giá trị)" 
            : "nhập AT BT hoặc AT BT PR (PR tùy chọn, mặc định 0)";
        
        for (int i = 0; i < numProcesses; i++)
        {
            Console.Write($"Tiến trình {i + 1} ({prompt}, cách nhau bằng khoảng trắng): ");
            string? input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                i--;
                Console.WriteLine("Dữ liệu không được để trống!");
                continue;
            }

            var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            // Validate số lượng input
            if (needsPriority && parts.Length != 3)
            {
                i--;
                Console.WriteLine("Lỗi: Priority Scheduling yêu cầu nhập đủ 3 giá trị (AT BT PR)!");
                continue;
            }

            if (!needsPriority && (parts.Length < 2 || parts.Length > 3))
            {
                i--;
                Console.WriteLine("Lỗi: Nhập AT BT hoặc AT BT PR (2-3 giá trị)!");
                continue;
            }

            if (!int.TryParse(parts[0], out int at))
            {
                i--;
                Console.WriteLine("Lỗi: Arrival Time phải là số nguyên!");
                continue;
            }

            if (!int.TryParse(parts[1], out int bt))
            {
                i--;
                Console.WriteLine("Lỗi: Burst Time phải là số nguyên!");
                continue;
            }

            int priority = 0;
            if (parts.Length >= 3)
            {
                if (!int.TryParse(parts[2], out priority))
                {
                    i--;
                    Console.WriteLine("Lỗi: Priority phải là số nguyên!");
                    continue;
                }
            }

            processes.Add(new Process(i + 1, at, bt, priority));
        }
        return processes;
    }

    // ============ BANKER'S ALGORITHM ============
    public static int GetResourceCount()
    {
        while (true)
        {
            Console.Write("Nhập số loại tài nguyên (1-3): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int count) && count >= 1 && count <= 3)
            {
                return count;
            }
            Console.WriteLine("Số tài nguyên không hợp lệ! Vui lòng nhập số từ 1 đến 3.");
        }
    }

    public static List<int> GetTotalResources(int numResources)
    {
        Console.WriteLine($"\nNhập tổng tài nguyên (mỗi loại cách nhau bằng khoảng trắng):");
        Console.Write("Tài nguyên từng loại: ");
        
        var resources = new List<int>();
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return resources;

        var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            if (int.TryParse(part, out int val))
                resources.Add(val);
        }
        return resources;
    }

    public static List<List<int>> GetMaxMatrix(int numProcesses, int numResources)
    {
        var matrix = new List<List<int>>();
        Console.WriteLine("\nNhập Ma trận MAX (nhu cầu tối đa):");
        
        for (int i = 0; i < numProcesses; i++)
        {
            Console.Write($"P{i + 1}: ");
            var row = new List<int>();
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;

            var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (int.TryParse(part, out int val))
                    row.Add(val);
            }
            matrix.Add(row);
        }
        return matrix;
    }

    public static List<List<int>> GetAllocationMatrix(int numProcesses, int numResources, List<int> totalResources)
    {
        var matrix = new List<List<int>>();
        Console.WriteLine("\nNhập Ma trận ALLOCATION (đã cấp phát):");
        
        for (int i = 0; i < numProcesses; i++)
        {
            Console.Write($"P{i + 1}: ");
            var row = new List<int>();
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;

            var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (int.TryParse(part, out int val))
                    row.Add(val);
            }
            matrix.Add(row);
        }
        return matrix;
    }
}
