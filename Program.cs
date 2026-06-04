using PageReplacementDemo.Algorithms.PageReplacementAlgo;
using PageReplacementDemo.Algorithms.CPUschedulingAlgo;
using PageReplacementDemo.Algorithms.DeadlockDetection;
using PageReplacementDemo.Models.PageReplacementAlgo;
using PageReplacementDemo.Program;
using PageReplacementDemo.Models;

namespace PageReplacementDemo
{
    class ApplicationEntry
    {
        static void Main(string[] args)
        {
            // Kiểm tra đang chạy từ IDE/VS Code hay cmd thực sự
            if (SystemHelpers.IsRunningFromIDE())
            {
                SystemHelpers.LaunchInNewConsole();
                return;
            }

            // Đảm bảo console size đủ lớn
            try
            {
                if (OperatingSystem.IsWindows() && (Console.WindowWidth < 90 || Console.WindowHeight < 30))
                {
                    Console.SetWindowSize(120, 35);
                }
            }
            catch { }

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            UIEngine ui = new UIEngine();

            while (true)
            {
                DisplayHelpers.SafeClear();
                MenuHelpers.ShowMainMenu();

                int mainChoice = MenuHelpers.GetMainMenuChoice();

                switch (mainChoice)
                {
                    case 1:
                        HandleCPUScheduling();
                        break;
                    case 2:
                        HandleDeadlockSystem();
                        break;
                    case 3:
                        HandlePageReplacement(ui);
                        break;
                    case 0:
                        return; // Exit
                    default:
                        break;
                }
            }
        }

        // ============ CPU SCHEDULING ============
        static void HandleCPUScheduling()
        {
            while (true)
            {
                DisplayHelpers.SafeClear();
                MenuHelpers.ShowDataSourceMenu();
                int dataSource = MenuHelpers.GetDataSourceChoice();

                if (dataSource == 0) break;
                if (dataSource == 1) HandleCPUSchedulingManualInput();
                else if (dataSource == 2) HandleCPUSchedulingFileInput();
            }
        }

        static void HandleCPUSchedulingManualInput()
        {
            while (true)
            {
                DisplayHelpers.SafeClear();
                MenuHelpers.ShowCPUSchedulingMenu();

                int choice = MenuHelpers.GetCPUSchedulingChoice();
                if (choice == 0) break;

                int numProcesses = InputHelpers.GetProcessCount();
                int quantumTime = 0;
                if (choice == 4) // Round Robin
                {
                    quantumTime = InputHelpers.GetQuantumTime();
                }

                var processes = InputHelpers.GetProcesses(numProcesses, choice == 5);
                string algoName = choice switch
                {
                    1 => "FCFS",
                    2 => "SJF",
                    3 => "SRTF",
                    4 => "Round Robin",
                    5 => "Priority Scheduling",
                    _ => "Unknown"
                };

                if (choice <= 5)
                {
                    // Show display mode menu
                    CPUSchedulingDisplayModes.ShowDisplayModeMenu();
                    int displayMode = CPUSchedulingDisplayModes.GetDisplayModeChoice();

                    if (displayMode == 0) continue;
                    else if (displayMode == 1)
                        CPUSchedulingDisplayModes.RunAlgorithmShowAll(processes, choice, quantumTime, algoName);
                    else if (displayMode == 2)
                        CPUSchedulingDisplayModes.RunAlgorithmStepByStep(processes, choice, quantumTime, algoName);
                }
                else if (choice == 6)
                {
                    // Run all algorithms
                    var allResults = CPUSchedulingHelpers.RunAllAlgorithms(processes, quantumTime);
                    CPUSchedulingHelpers.DisplayComparisonTable(allResults);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                    Console.ResetColor();
                    Console.ReadKey(true);
                }
            }
        }

        static void HandleCPUSchedulingFileInput()
        {
            DisplayHelpers.SafeClear();
            var testCases = FileHelpers.GetAvailableCPUTestCases();

            if (testCases.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Không tìm thấy test case nào!");
                Console.ResetColor();
                Console.ReadKey(true);
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nCác Test Case có sẵn:");
            Console.ResetColor();
            for (int i = 0; i < testCases.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {testCases[i]}");
            }

            Console.Write("\nChọn test case (1-" + testCases.Count + "): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > testCases.Count)
            {
                Console.WriteLine("Lựa chọn không hợp lệ!");
                Console.ReadKey(true);
                return;
            }

            try
            {
                var (processes, quantumTime) = FileHelpers.LoadCPUSchedulingTestCase(testCases[choice - 1]);

                while (true)
                {
                    DisplayHelpers.SafeClear();
                    MenuHelpers.ShowCPUSchedulingMenu();

                    int algoChoice = MenuHelpers.GetCPUSchedulingChoice();
                    if (algoChoice == 0) break;

                    string algoName = algoChoice switch
                    {
                        1 => "FCFS",
                        2 => "SJF",
                        3 => "SRTF",
                        4 => "Round Robin",
                        5 => "Priority Scheduling",
                        _ => "Unknown"
                    };

                    if (algoChoice <= 5)
                    {
                        // Show display mode menu
                        CPUSchedulingDisplayModes.ShowDisplayModeMenu();
                        int displayMode = CPUSchedulingDisplayModes.GetDisplayModeChoice();

                        if (displayMode == 0) continue;
                        else if (displayMode == 1)
                            CPUSchedulingDisplayModes.RunAlgorithmShowAll(processes, algoChoice, quantumTime, algoName);
                        else if (displayMode == 2)
                            CPUSchedulingDisplayModes.RunAlgorithmStepByStep(processes, algoChoice, quantumTime, algoName);
                    }
                    else if (algoChoice == 6)
                    {
                        var allResults = CPUSchedulingHelpers.RunAllAlgorithms(processes, quantumTime);
                        CPUSchedulingHelpers.DisplayComparisonTable(allResults);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ResetColor();
                        Console.ReadKey(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Lỗi: {ex.Message}");
                Console.ResetColor();
                Console.ReadKey(true);
            }
        }

        // ============ DEADLOCK SYSTEM ============
        static DeadlockSystemData? systemData = null;

        static void HandleDeadlockSystem()
        {
            while (true)
            {
                MenuHelpers.ShowDeadlockSystemMenu();
                int choice = MenuHelpers.GetDeadlockSystemChoice();

                if (choice == 0) break;

                try
                {
                    switch (choice)
                    {
                        case 1:
                            HandleDeadlockManualInput();
                            break;
                        case 2:
                            HandleDeadlockFileInput();
                            break;
                        case 3:
                            if (systemData == null)
                                Console.WriteLine("\n[Cảnh báo] Vui lòng nhập dữ liệu trước!");
                            else
                                HandleDeadlockSafetyCheck();
                            break;
                        case 4:
                            if (systemData == null)
                                Console.WriteLine("\n[Cảnh báo] Vui lòng nhập dữ liệu trước!");
                            else
                                HandleDeadlockResourceRequest();
                            break;
                        case 5:
                            if (systemData == null)
                                Console.WriteLine("\n[Cảnh báo] Vui lòng nhập dữ liệu trước!");
                            else
                                HandleDeadlockDetection();
                            break;
                        case 6:
                            if (systemData == null)
                                Console.WriteLine("\n[Cảnh báo] Vui lòng nhập dữ liệu trước!");
                            else
                                HandleDeadlockRecovery();
                            break;
                    }

                    if (choice >= 1 && choice <= 6)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ResetColor();
                        Console.ReadKey(true);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[Lỗi] {ex.Message}");
                    Console.ResetColor();
                    Console.ReadKey(true);
                }
            }
        }

        static void HandleDeadlockManualInput()
        {
            systemData = DeadlockSystemHelpers.InputSystemData();
        }

        static void HandleDeadlockFileInput()
        {
            DisplayHelpers.SafeClear();
            var testCases = FileHelpers.GetAvailableDeadlockTestCases();

            if (testCases.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Không tìm thấy test case nào!");
                Console.ResetColor();
                Console.ReadKey(true);
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nCác Test Case Deadlock có sẵn:");
            Console.ResetColor();
            for (int i = 0; i < testCases.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {testCases[i]}");
            }

            Console.Write("\nChọn test case (1-" + testCases.Count + "): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > testCases.Count)
            {
                Console.WriteLine("Lựa chọn không hợp lệ!");
                Console.ReadKey(true);
                return;
            }

            try
            {
                var (numProc, numRes, total, max, allocation) = FileHelpers.LoadDeadlockTestCase(testCases[choice - 1]);
                systemData = new DeadlockSystemData(numProc, numRes);
                Array.Copy(total, systemData.Total!, numRes);
                systemData.Max = max;
                systemData.Allocation = allocation;
                DeadlockSystemLogic.CalculateInitialState(systemData);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Lỗi: {ex.Message}");
                Console.ResetColor();
                Console.ReadKey(true);
            }
        }

        static void HandleDeadlockSafetyCheck()
        {
            DisplayHelpers.SafeClear();
            if (systemData == null) return;

            DeadlockSystemHelpers.DisplayMatrices(systemData);

            var safeSeq = new List<int>();
            bool isSafe = DeadlockSystemLogic.IsSafeState(systemData, out safeSeq, printTrace: true);

            DeadlockSystemHelpers.DisplaySafetyResult(isSafe, safeSeq);
        }

        static void HandleDeadlockResourceRequest()
        {
            DisplayHelpers.SafeClear();
            if (systemData == null) return;

            Console.Write("\nNhập thứ tự tiến trình xin tài nguyên (1 - " + systemData.NumProcesses + "): ");
            if (!int.TryParse(Console.ReadLine(), out int pid) || pid < 1 || pid > systemData.NumProcesses)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Lỗi] Thứ tự tiến trình không tồn tại!");
                Console.ResetColor();
                return;
            }

            if (systemData.Terminated != null && systemData.Terminated[pid - 1])
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Lỗi] Tiến trình P{pid} đã bị hệ thống hủy! Không thể yêu cầu.");
                Console.ResetColor();
                return;
            }

            var request = DeadlockSystemHelpers.InputResourceRequest(systemData.NumResources, pid - 1);
            DeadlockSystemLogic.RequestResources(systemData, pid - 1, request, out _);
        }

        static void HandleDeadlockDetection()
        {
            DisplayHelpers.SafeClear();
            if (systemData == null) return;

            systemData.RequestMatrix = DeadlockSystemHelpers.InputRequestMatrix(systemData.NumProcesses, systemData.NumResources);
            DeadlockSystemLogic.DetectDeadlock(systemData, out _);
        }

        static void HandleDeadlockRecovery()
        {
            DisplayHelpers.SafeClear();
            if (systemData == null) return;

            DeadlockSystemLogic.RecoverDeadlock(systemData);
        }

        // ============ PAGE REPLACEMENT ============
        static void HandlePageReplacement(UIEngine ui)
        {
            while (true)
            {
                DisplayHelpers.SafeClear();
                MenuHelpers.ShowPageReplacementMenu();

                int choice = MenuHelpers.GetPageReplacementChoice();

                if (choice == 0)
                    break;

                if (choice == 5)
                {
                    HandlePageReplacementComparison(ui);
                    continue;
                }

                if (choice == 6)
                {
                    HandlePageReplacementFileInput(ui);
                    continue;
                }

                int pageCount = InputHelpers.GetPageCount();
                int frameCount = InputHelpers.GetFrameCount();
                int[] referenceString = InputHelpers.GetReferenceString(pageCount);

                IPageReplacement algorithm = CreateAlgorithm(choice);
                string algorithmName = GetAlgorithmName(choice);

                algorithm.Initialize(pageCount, frameCount, referenceString);
                ui.RunStepByStepAccumulative(algorithm, algorithmName, pageCount, frameCount, referenceString);
            }
        }

        /// <summary>
        /// Xử lý tính năng so sánh các thuật toán.
        /// </summary>
        static void HandlePageReplacementComparison(UIEngine ui)
        {
            DisplayHelpers.SafeClear();

            int pageCount = InputHelpers.GetPageCount();
            int frameCount = InputHelpers.GetFrameCount();
            int[] referenceString = InputHelpers.GetReferenceString(pageCount);

            // Tạo 4 thuật toán
            var fifoAlgo = new FIFOAlgorithm();
            var lruAlgo = new LRUAlgorithm();
            var clockAlgo = new ClockAlgorithm();
            var optAlgo = new OPTAlgorithm();

            // Khởi tạo các thuật toán
            fifoAlgo.Initialize(pageCount, frameCount, referenceString);
            lruAlgo.Initialize(pageCount, frameCount, referenceString);
            clockAlgo.Initialize(pageCount, frameCount, referenceString);
            optAlgo.Initialize(pageCount, frameCount, referenceString);

            // Hiển thị so sánh một lượt (không nhấn enter từng bước)
            ui.RunComparisonFromFileNoStep(fifoAlgo, lruAlgo, clockAlgo, optAlgo, pageCount, frameCount, referenceString);
        }

        /// <summary>
        /// Xử lý tính năng đọc dữ liệu từ file.
        /// </summary>
        static void HandlePageReplacementFileInput(UIEngine ui)
        {
            DisplayHelpers.SafeClear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═════════════════════════════════════════════════════╗");
            Console.WriteLine("║      ĐỌC DỮ LIỆU TỪ FILE TEXT                       ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();

            // Hiển thị các file có sẵn
            var testFiles = FileInputHelpers.GetTestDataFiles();
            if (testFiles.Count > 0)
            {
                Console.WriteLine("Các file có sẵn trong thư mục TestData:");
                for (int i = 0; i < testFiles.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {testFiles[i]}");
                }
                Console.WriteLine();
            }

            // Nhập đường dẫn file
            Console.Write("Nhập đường dẫn file (hoặc tên file nếu nó nằm trong TestData): ");
            string? filePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("Đường dẫn file không hợp lệ!");
                Console.ReadKey(true);
                return;
            }

            // Nếu chỉ nhập tên file, tìm trong TestData
            if (!filePath.Contains('\\') && !filePath.Contains('/'))
            {
                var foundPath = FileInputHelpers.FindTestDataFile(filePath);
                if (string.IsNullOrEmpty(foundPath))
                {
                    Console.WriteLine($"Không tìm thấy file: {filePath}");
                    Console.ReadKey(true);
                    return;
                }
                filePath = foundPath;
            }

            try
            {
                var (pageCount, frameCount, referenceString) = FileInputHelpers.ReadPageReplacementDataFromFile(filePath);

                // Hiển thị menu chọn thuật toán
                DisplayHelpers.SafeClear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔═════════════════════════════════════════════════════╗");
                Console.WriteLine("║      CHỌN THUẬT TOÁN                                ║");
                Console.WriteLine("╚═════════════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("  [1] FIFO  - First-In, First-Out");
                Console.WriteLine("  [2] LRU   - Least Recently Used");
                Console.WriteLine("  [3] Clock - Second-Chance (Clock)");
                Console.WriteLine("  [4] OPT   - Optimal (MIN)");
                Console.WriteLine("  [5] So sánh");
                Console.WriteLine("  [0] Quay lại");
                Console.WriteLine();

                while (true)
                {
                    Console.Write("Nhập lựa chọn (0-5): ");
                    string? input = Console.ReadLine();
                    if (int.TryParse(input, out int choice) && choice >= 0 && choice <= 5)
                    {
                        if (choice == 0)
                            return;

                        if (choice >= 1 && choice <= 4)
                        {
                            // Chạy thuật toán đơn với hiển thị accumulative
                            var algorithm = CreateAlgorithm(choice);
                            var algorithmName = GetAlgorithmName(choice);

                            algorithm.Initialize(pageCount, frameCount, referenceString);
                            ui.RunStepByStepAccumulative(algorithm, algorithmName, pageCount, frameCount, referenceString);
                            return;
                        }
                        else if (choice == 5)
                        {
                            // Chạy so sánh từ file
                            var fifoAlgo = new FIFOAlgorithm();
                            var lruAlgo = new LRUAlgorithm();
                            var clockAlgo = new ClockAlgorithm();
                            var optAlgo = new OPTAlgorithm();

                            fifoAlgo.Initialize(pageCount, frameCount, referenceString);
                            lruAlgo.Initialize(pageCount, frameCount, referenceString);
                            clockAlgo.Initialize(pageCount, frameCount, referenceString);
                            optAlgo.Initialize(pageCount, frameCount, referenceString);

                            ui.RunComparisonFromFileNoStep(fifoAlgo, lruAlgo, clockAlgo, optAlgo, pageCount, frameCount, referenceString);
                            return;
                        }
                    }
                    Console.WriteLine("Lựa chọn không hợp lệ! Vui lòng nhập lại.");
                }
            }
            catch (Exception ex)
            {
                DisplayHelpers.SafeClear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Lỗi: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey(true);
            }
        }

        // ============ HELPER METHODS ============
        static IPageReplacement CreateAlgorithm(int choice)
        {
            return choice switch
            {
                1 => new FIFOAlgorithm(),
                2 => new LRUAlgorithm(),
                3 => new ClockAlgorithm(),
                4 => new OPTAlgorithm(),
                _ => throw new ArgumentException("Invalid algorithm choice")
            };
        }

        static string GetAlgorithmName(int choice)
        {
            return choice switch
            {
                1 => "FIFO",
                2 => "LRU",
                3 => "Clock",
                4 => "OPT",
                _ => "Unknown"
            };
        }
    }
}
