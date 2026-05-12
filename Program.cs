using PageReplacementDemo.Algorithms.PageReplacementAlgo;
using PageReplacementDemo.Algorithms.CPUschedulingAlgo;
using PageReplacementDemo.Algorithms.BankerAlgo;

namespace PageReplacementDemo;

class Program
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
        UIEngine ui = new UIEngine();

        while (true)
        {
            DisplayHelpers.SafeClear();
            MenuHelpers.ShowPageReplacementMenu();

            int choice = MenuHelpers.GetPageReplacementChoice();
            if (choice == 0) break;

            int pageCount = InputHelpers.GetPageCount();
            int frameCount = InputHelpers.GetFrameCount();
            int[] referenceString = InputHelpers.GetReferenceString(pageCount);

            IPageReplacement algorithm = CreateAlgorithm(choice);
            string algorithmName = GetAlgorithmName(choice);

            algorithm.Initialize(pageCount, frameCount, referenceString);
            ui.Run(algorithm, algorithmName, pageCount, frameCount, referenceString);
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
