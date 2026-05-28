using PageReplacementDemo.Algorithms.PageReplacementAlgo;
using PageReplacementDemo.Algorithms.CPUschedulingAlgo;
using PageReplacementDemo.Algorithms.BankerAlgo;
using PageReplacementDemo.Models.PageReplacementAlgo;
using PageReplacementDemo.Program;

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
                        HandleBankerAlgorithm();
                        break;
                    case 3:
                        HandlePageReplacement();
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
                MenuHelpers.ShowCPUSchedulingMenu();

                int choice = MenuHelpers.GetCPUSchedulingChoice();
                if (choice == 0) break;

                int numProcesses = InputHelpers.GetProcessCount();
                int quantumTime = 0;
                if (choice == 4) // Round Robin
                {
                    quantumTime = InputHelpers.GetQuantumTime();
                }

                var processes = InputHelpers.GetProcesses(numProcesses, choice == 5); // choice 5 = Priority
                var result = CPUSchedulingExecutor.ExecuteAlgorithm(processes, choice, quantumTime);

                DisplayHelpers.DisplayCPUSchedulingResult(choice, result);
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nNhấn phím bất kỳ để trở lại menu CPU Scheduling...");
                Console.ResetColor();
                Console.ReadKey(true);
            }
        }

        // ============ BANKER'S ALGORITHM ============
        static void HandleBankerAlgorithm()
        {
            while (true)
            {
                DisplayHelpers.SafeClear();
                MenuHelpers.ShowBankerMenu();

                int choice = MenuHelpers.GetBankerChoice();
                if (choice == 0) break;

                if (choice == 1)
                {
                    int numProcesses = InputHelpers.GetProcessCount();
                    int numResources = InputHelpers.GetResourceCount();

                    var totalResources = InputHelpers.GetTotalResources(numResources);
                    var maxMatrix = InputHelpers.GetMaxMatrix(numProcesses, numResources);
                    var allocationMatrix = InputHelpers.GetAllocationMatrix(numProcesses, numResources, totalResources);

                    var result = BankerExecutor.ExecuteAlgorithm(totalResources, maxMatrix, allocationMatrix);
                    DisplayHelpers.DisplayBankerResult(result);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nNhấn phím bất kỳ để trở lại menu Banker's Algorithm...");
                    Console.ResetColor();
                    Console.ReadKey(true);
                }
            }
        }

        // ============ PAGE REPLACEMENT ============
        static void HandlePageReplacement()
        {
            UIEngine ui2 = new UIEngine();

            while (true)
            {
                DisplayHelpers.SafeClear();
                MenuHelpers.ShowPageReplacementMenu();

                int choice = MenuHelpers.GetPageReplacementChoice();
                
                if (choice == 0)
                    break;
                
                if (choice == 5)
                {
                    HandlePageReplacementComparison(ui2);
                    continue;
                }
                
                if (choice == 6)
                {
                    HandlePageReplacementFileInput(ui2);
                    continue;
                }

                int pageCount = InputHelpers.GetPageCount();
                int frameCount = InputHelpers.GetFrameCount();
                int[] referenceString = InputHelpers.GetReferenceString(pageCount);

                IPageReplacement algorithm = CreateAlgorithm(choice);
                string algorithmName = GetAlgorithmName(choice);

                algorithm.Initialize(pageCount, frameCount, referenceString);
                ui2.RunStepByStepAccumulative(algorithm, algorithmName, pageCount, frameCount, referenceString);
            }
        }

        /// <summary>
        /// Xử lý tính năng so sánh các thuật toán.
        /// </summary>
        static void HandlePageReplacementComparison(UIEngine ui2)
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
            ui2.RunComparisonFromFileNoStep(fifoAlgo, lruAlgo, clockAlgo, optAlgo, pageCount, frameCount, referenceString);
        }

        /// <summary>
        /// Xử lý tính năng đọc dữ liệu từ file.
        /// </summary>
        static void HandlePageReplacementFileInput(UIEngine ui2)
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
                            ui2.RunStepByStepAccumulative(algorithm, algorithmName, pageCount, frameCount, referenceString);
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

                            ui2.RunComparisonFromFileNoStep(fifoAlgo, lruAlgo, clockAlgo, optAlgo, pageCount, frameCount, referenceString);
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
