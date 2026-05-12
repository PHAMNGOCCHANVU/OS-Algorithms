# Page Replacement Algorithms

Ứng dụng Console thực thi và trực quan hóa các thuật toán thay thế trang (Page Replacement), lập lịch CPU (CPU Scheduling), và thuật toán Banker trong hệ điều hành.

## Các thuật toán được hỗ trợ

### Page Replacement Algorithms
- **FIFO** (First In First Out)
- **LRU** (Least Recently Used)
- **Optimal** (Bélády's Algorithm)
- **Clock Algorithm**

### CPU Scheduling Algorithms
- **FCFS** (First Come First Served)
- **SJF** (Shortest Job First)
- **SRTF** (Shortest Remaining Time First)
- **Priority Algorithm**
- **Round Robin**

### Deadlock Avoidance
- **Banker's Algorithm**

## Yêu cầu hệ thống

- **.NET Runtime 10.0** trở lên
- **Windows/Linux/macOS** (tùy thuộc vào .NET support)
- **Terminal/Console** hỗ trợ Unicode

## Cài đặt cho Developer

### Bước 1: Clone hoặc tải về project
```bash
cd Page-Replacement-Algorithms
```

### Bước 2: Cài đặt .NET (nếu chưa có)
Nếu chưa cài đặt .NET 10.0, tải từ: https://dotnet.microsoft.com/download

Kiểm tra phiên bản:
```bash
dotnet --version
```

### Bước 3: Restore dependencies
```bash
dotnet restore
```

### Bước 4: Build project
```bash
dotnet build
```

### Bước 5: Chạy ứng dụng
```bash
dotnet run
```

## Cấu trúc Project

```
Page-Replacement-Algorithms/
├── Algorithms/
│   ├── PageReplacementAlgo/      # Thuật toán thay thế trang
│   │   ├── FIFOAlgorithm.cs
│   │   ├── LRUAlgorithm.cs
│   │   ├── OPTAlgorithm.cs
│   │   ├── ClockAlgorithm.cs
│   │   └── IPageReplacement.cs   # Interface
│   ├── CPUschedulingAlgo/         # Thuật toán lập lịch CPU
│   │   ├── FCFSAlgorithm.cs
│   │   ├── SJFAlgorithm.cs
│   │   ├── SRTFAlgorithm.cs
│   │   ├── PriorityAlgorithm.cs
│   │   ├── RoundRobinAlgorithm.cs
│   │   └── ICPUScheduling.cs      # Interface
│   └── BankerAlgo/                # Thuật toán Banker
│       ├── BankerAlgorithm.cs
│       ├── BankerExecutor.cs
│       └── IBankerAlgorithm.cs    # Interface
├── Models/                        # Các model dữ liệu
│   ├── StepResult.cs
│   ├── Process.cs
│   ├── ProcessScheduleStep.cs
│   └── BankerStep.cs
├── Program/                       # Helper functions
│   ├── MenuHelpers.cs
│   ├── InputHelpers.cs
│   ├── DisplayHelpers.cs
│   └── SystemHelpers.cs
├── UIEngine.cs                    # Engine hiển thị giao diện
├── Program.cs                     # Entry point
└── Page-Replacement-Algorithms.csproj
```

## Sử dụng

Khi chạy ứng dụng, bạn sẽ nhìn thấy menu chính với các tùy chọn:

1. **Page Replacement Algorithms** - Chạy các thuật toán thay thế trang
2. **CPU Scheduling Algorithms** - Chạy các thuật toán lập lịch CPU
3. **Banker's Algorithm** - Chạy thuật toán Banker
4. **Exit** - Thoát ứng dụng

Chọn thuật toán mong muốn và nhập các thông số:
- Số trang (Page Count)
- Số frame (Frame Count)
- Chuỗi tham chiếu (Reference String)

## Output

Ứng dụng sẽ hiển thị:
- **Bảng kết quả**: Hàng là frame, cột là từng bước thực thi
  - Màu **đỏ**: Page Fault xảy ra
  - Màu **xanh**: Không Page Fault
- **Thống kê**: Page Faults, Hit Rate, Fault Rate

## Phát triển

### Thêm thuật toán mới

#### Page Replacement:
1. Tạo class mới trong `Algorithms/PageReplacementAlgo/`
2. Implement interface `IPageReplacement`
3. Thêm vào menu trong `Program.cs`

#### CPU Scheduling:
1. Tạo class mới trong `Algorithms/CPUschedulingAlgo/`
2. Implement interface `ICPUScheduling`
3. Thêm vào menu trong `Program.cs`

### Chạy tests
```bash
dotnet test
```

### Publish
```bash
dotnet publish -c Release
```

## Troubleshooting

### Lỗi: "The .NET runtime or SDK couldn't be found"
- Cài đặt .NET 10.0 từ https://dotnet.microsoft.com/download

### Lỗi khi hiển thị Unicode
- Đảm bảo Terminal/Console hỗ trợ UTF-8
- Trên Windows, cập nhật Terminal hoặc sử dụng PowerShell 7.0+

### Build thất bại
```bash
dotnet clean
dotnet restore
dotnet build
```

## Thông tin liên hệ

Nếu có bất kỳ vấn đề hoặc đề xuất, vui lòng tạo Issue hoặc Pull Request.

## License

Dự án này được phát hành theo giấy phép MIT.
