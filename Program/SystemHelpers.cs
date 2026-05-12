using System.Diagnostics;
using System.Management;
using DiagProcess = System.Diagnostics.Process;

namespace PageReplacementDemo;

/// <summary>
/// Chứa các hàm system-level như quản lý console, process, etc.
/// </summary>
public static class SystemHelpers
{
    /// <summary>
    /// Kiểm tra đang chạy từ IDE/VS Code hay cmd thực sự
    /// </summary>
    public static bool IsRunningFromIDE()
    {
        if (!OperatingSystem.IsWindows())
        {
            return false; // Chỉ support trên Windows
        }

        try
        {
            // Kiểm tra xem parent process có phải VS Code, Visual Studio hay không
            var parentProcess = GetParentProcess();
            if (parentProcess != null)
            {
                string parentName = parentProcess.ProcessName.ToLower();
                // Nếu parent là IDE, thì mở console mới
                return parentName.Contains("code") || 
                       parentName.Contains("devenv") ||
                       parentName.Contains("visualstudio");
            }
        }
        catch { }

        return false;
    }

    /// <summary>
    /// Lấy parent process của current process
    /// </summary>
    private static DiagProcess? GetParentProcess()
    {
        try
        {
            var currentProcess = DiagProcess.GetCurrentProcess();
            var parentPid = GetParentProcessId(currentProcess.Id);
            if (parentPid > 0)
            {
                return DiagProcess.GetProcessById(parentPid);
            }
        }
        catch { }
        return null;
    }

    /// <summary>
    /// Lấy PID của parent process (Windows-specific)
    /// </summary>
    private static int GetParentProcessId(int processId)
    {
        try
        {
            if (!OperatingSystem.IsWindows())
            {
                return 0; // Không hỗ trợ trên non-Windows
            }

            using (var process = DiagProcess.GetProcessById(processId))
            {
                var query = $"Select ParentProcessId FROM Win32_Process WHERE ProcessId={processId}";
                var searcher = new ManagementObjectSearcher(query);
                var results = searcher.Get();
                foreach (var result in results)
                {
                    return Convert.ToInt32(result["ParentProcessId"]);
                }
            }
        }
        catch { }
        return 0;
    }

    /// <summary>
    /// Tự động mở command prompt riêng để chạy executable
    /// </summary>
    public static void LaunchInNewConsole()
    {
        if (!OperatingSystem.IsWindows())
        {
            return; // Chỉ support trên Windows
        }

        try
        {
            // Lấy đường dẫn tới executable
            string exePath = DiagProcess.GetCurrentProcess().MainModule?.FileName ?? "";
            
            if (exePath.EndsWith(".dll"))
            {
                // Nếu là DLL, dùng dotnet để chạy
                var exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ".";
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/K \"cd /d {exeDir} && dotnet Page-Replacement-Algorithms.dll\"",
                    UseShellExecute = true,
                    CreateNoWindow = false
                };
                DiagProcess.Start(psi);
            }
            else
            {
                // Chạy exe trực tiếp
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/K \"{exePath}\"",
                    UseShellExecute = true,
                    CreateNoWindow = false
                };
                DiagProcess.Start(psi);
            }
        }
        catch
        {
            // Nếu có lỗi, chạy trực tiếp trong terminal hiện tại
        }
    }
}
