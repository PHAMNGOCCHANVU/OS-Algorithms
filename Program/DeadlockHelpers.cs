using PageReplacementDemo.Algorithms.DeadlockDetection;
using PageReplacementDemo.Models;

namespace PageReplacementDemo.Program;

/// <summary>
/// Deadlock Detection specific helpers for display
/// </summary>
public static class DeadlockHelpers
{
    /// <summary>
    /// Display deadlock detection results
    /// </summary>
    public static void DisplayDeadlockResults(DeadlockResult result)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n╔════════════════════ DEADLOCK DETECTION RESULT ════════════════════╗");
        Console.ResetColor();

        // Status
        Console.ForegroundColor = result.IsSafe ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"Status: {(result.IsSafe ? "SAFE STATE" : "UNSAFE STATE")}");
        Console.ResetColor();

        // Safe sequence
        if (result.IsSafe)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Safe Sequence: ");
            Console.ResetColor();
            for (int i = 0; i < result.SafeSequence.Count; i++)
            {
                Console.Write($"P{result.SafeSequence[i]}");
                if (i < result.SafeSequence.Count - 1)
                    Console.Write(" -> ");
            }
            Console.WriteLine();
        }

        Console.WriteLine();

        // Display matrices in table format
        DisplayMatrices(result);
    }

    /// <summary>
    /// Display allocation, need, and available matrices in table format
    /// </summary>
    private static void DisplayMatrices(DeadlockResult result)
    {
        int p = result.NumProcesses;
        int r = result.NumResources;

        // Allocation Matrix
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("ALLOCATION MATRIX:");
        Console.ResetColor();
        DisplayMatrix(result.Allocation, "Process");

        Console.WriteLine();

        // Need Matrix
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("NEED MATRIX:");
        Console.ResetColor();
        DisplayMatrix(result.Need, "Process");

        Console.WriteLine();

        // Available Resources
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("AVAILABLE: ");
        Console.ResetColor();
        for (int j = 0; j < result.Available.Length; j++)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"R{j}");
            Console.ResetColor();
            Console.Write($"={result.Available[j]}  ");
        }
        Console.WriteLine("\n");
    }

    /// <summary>
    /// Display a matrix in table format
    /// </summary>
    private static void DisplayMatrix(int[][] matrix, string rowLabel)
    {
        if (matrix.Length == 0 || matrix[0].Length == 0)
            return;

        // Header
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write(rowLabel.PadRight(10));
        for (int j = 0; j < matrix[0].Length; j++)
        {
            Console.Write($"R{j}".PadRight(4));
        }
        Console.ResetColor();
        Console.WriteLine();

        // Data
        for (int i = 0; i < matrix.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"P{i}".PadRight(10));
            Console.ResetColor();

            for (int j = 0; j < matrix[i].Length; j++)
            {
                Console.Write($"{matrix[i][j]}".PadRight(4));
            }
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Run deadlock detection and return result
    /// </summary>
    public static DeadlockResult RunDeadlockDetection(int[][] max, int[][] allocation, int[] available)
    {
        var detector = new DeadlockDetector(max, allocation, available);
        var (isSafe, safeSequence) = detector.Detect();

        var result = new DeadlockResult
        {
            IsSafe = isSafe,
            SafeSequence = safeSequence,
            NumProcesses = detector.NumProcesses,
            NumResources = detector.NumResources,
            Max = max,
            Allocation = allocation,
            Need = detector.GetNeedMatrix(),
            Available = available
        };

        return result;
    }
}
