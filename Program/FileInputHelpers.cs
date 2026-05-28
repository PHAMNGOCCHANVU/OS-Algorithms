namespace PageReplacementDemo.Program;

/// <summary>
/// Lớp tiện ích để đọc dữ liệu từ file cho thuật toán thay thế trang.
/// </summary>
public static class FileInputHelpers
{
    /// <summary>
    /// Đọc dữ liệu thay thế trang từ file text.
    /// Định dạng file:
    /// Dòng 1: Số lượng trang
    /// Dòng 2: Số lượng frame
    /// Dòng 3: Chuỗi tham chiếu (các số cách nhau bằng dấu cách)
    /// </summary>
    public static (int pageCount, int frameCount, int[] referenceString) ReadPageReplacementDataFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File không tồn tại: {filePath}");
        }

        try
        {
            var lines = File.ReadAllLines(filePath).Select(l => l.Trim()).ToArray();

            if (lines.Length < 3)
            {
                throw new InvalidOperationException("File phải có ít nhất 3 dòng: số trang, số frame, chuỗi tham chiếu");
            }

            // Đọc số trang
            if (!int.TryParse(lines[0], out int pageCount) || pageCount < 1 || pageCount > 100)
            {
                throw new InvalidOperationException("Số trang phải là số nguyên từ 1 đến 100");
            }

            // Đọc số frame
            if (!int.TryParse(lines[1], out int frameCount) || frameCount < 1 || frameCount > pageCount)
            {
                throw new InvalidOperationException($"Số frame phải là số nguyên từ 1 đến {pageCount}");
            }

            // Đọc chuỗi tham chiếu
            var refParts = lines[2].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var referenceString = new int[refParts.Length];

            for (int i = 0; i < refParts.Length; i++)
            {
                if (!int.TryParse(refParts[i], out int page))
                {
                    throw new InvalidOperationException($"Giá trị trang không hợp lệ: {refParts[i]}");
                }

                if (page < 1 || page > pageCount)
                {
                    throw new InvalidOperationException($"Giá trị trang {page} nằm ngoài phạm vi [1, {pageCount}]");
                }

                referenceString[i] = page;
            }

            return (pageCount, frameCount, referenceString);
        }
        catch (Exception ex) when (!(ex is FileNotFoundException || ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"Lỗi khi đọc file: {ex.Message}");
        }
    }

    /// <summary>
    /// Tìm kiếm file trong thư mục TestData mặc định.
    /// </summary>
    public static string FindTestDataFile(string fileName)
    {
        var testDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "TestData");
        var filePath = Path.Combine(testDataDir, fileName);
        
        if (!File.Exists(filePath))
        {
            return string.Empty;
        }

        return filePath;
    }

    /// <summary>
    /// Liệt kê tất cả file .txt trong thư mục TestData.
    /// </summary>
    public static List<string> GetTestDataFiles()
    {
        var testDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "TestData");
        var result = new List<string>();

        if (!Directory.Exists(testDataDir))
        {
            return result;
        }

        var files = Directory.GetFiles(testDataDir, "*.txt");
        result.AddRange(files.Select(f => Path.GetFileName(f)));

        return result;
    }
}
