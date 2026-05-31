# OS Algorithms

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
- **Priority Scheduling**
- **Round Robin**
- **Compare All** (So sánh tất cả 5 thuật toán)

### Deadlock Management
- **Banker's Algorithm** (6 tính năng)
  - Manual Input (Nhập dữ liệu thủ công)
  - File Input (Tải từ test case)
  - Safety Check (Kiểm tra trạng thái an toàn)
  - Resource Request (Xử lý xin cấp phát tài nguyên)
  - Deadlock Detection (Phát hiện deadlock)
  - Deadlock Recovery (Phục hồi từ deadlock)

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
│   ├── PageReplacementAlgo/       # Thuật toán thay thế trang
│   │   ├── FIFOAlgorithm.cs
│   │   ├── LRUAlgorithm.cs
│   │   ├── OPTAlgorithm.cs
│   │   ├── ClockAlgorithm.cs
│   │   └── IPageReplacement.cs    # Interface
│   ├── CPUSchedulingAlgo/         # Thuật toán lập lịch CPU
│   │   ├── FCFSAlgorithm.cs
│   │   ├── SJFAlgorithm.cs
│   │   ├── SRTFAlgorithm.cs
│   │   ├── PriorityAlgorithm.cs
│   │   ├── RoundRobinAlgorithm.cs
│   │   └── ICPUScheduling.cs      # Interface
│   └── DeadlockDetection/         # Thuật toán Banker (Deadlock Management)
│       ├── DeadlockSystemData.cs  # Model lưu trữ state
│       ├── DeadlockSystemLogic.cs # 6 tính năng chính
│       └── IPageReplacement.cs    # (legacy)
├── Models/                        # Các model dữ liệu
│   ├── StepResult.cs
│   ├── Process.cs
│   ├── ProcessScheduleStep.cs
│   └── GanttChart.cs
├── Program/                       # Helper functions & Display
│   ├── MenuHelpers.cs             # Menu system
│   ├── InputHelpers.cs            # User input
│   ├── DisplayHelpers.cs          # Display utilities
│   ├── FileHelpers.cs             # File I/O
│   ├── CPUSchedulingDisplayModes.cs # Display modes (Show All / Step-by-Step)
│   └── DeadlockSystemHelpers.cs   # Deadlock display & input
├── TestData/                      # Dữ liệu test
│   ├── CPUScheduling_TestCase*.txt
│   ├── Deadlock_Safe.txt
│   └── Deadlock_Unsafe.txt
├── UIEngine.cs                    # Engine hiển thị giao diện
├── Program.cs                     # Entry point & routing logic
└── Page-Replacement-Algorithms.csproj
```

## Sử dụng

Khi chạy ứng dụng, bạn sẽ nhìn thấy menu chính với 3 tùy chọn:

### Menu Chính
```
[1] CPU Scheduling Algorithms     - Lập lịch CPU
[2] Deadlock's Algorithm          - Quản lý Deadlock (6 tính năng)
[3] Page Replacement Algorithms   - Thay thế trang
[0] Exit                          - Thoát ứng dụng
```

### CPU Scheduling
1. Chọn nguồn dữ liệu: **Manual Input** hoặc **Load from File**
2. Chọn thuật toán: FCFS, SJF, SRTF, Priority, Round Robin, hoặc Compare All
3. Chọn chế độ hiển thị:
   - **[0] Show All** - In toàn bộ kết quả ngay lập tức (không delay)
   - **[1] Step-by-Step** - Hiển thị từng bước với delay 0.8s
4. Nhập thông số (Arrival Time, Burst Time, Priority, Quantum) và xem kết quả

### Deadlock Management (6 Tính Năng)
1. **Manual Input** - Nhập dữ liệu hệ thống: số tiến trình, số tài nguyên, Total, Max matrix, Allocation matrix
2. **File Input** - Tải test case từ `TestData/Deadlock_*.txt`
3. **Safety Check** - Kiểm tra hệ thống ở trạng thái an toàn (Banker's algorithm)
4. **Resource Request** - Xử lý yêu cầu cấp phát tài nguyên (4-bước kiểm tra)
5. **Deadlock Detection** - Phát hiện deadlock bằng cách nhập Request matrix
6. **Deadlock Recovery** - Phục hồi từ deadlock bằng abort process

### Page Replacement
- Chọn thuật toán: FIFO, LRU, Optimal, Clock
- Nhập thông số: Page Count, Frame Count, Reference String
- Xem bảng kết quả với Page Faults và Hit Rate

## Output

### CPU Scheduling
- **Gantt Chart** - Biểu đồ timeline của tiến trình
  - Hiển thị Process ID, thời gian start/end
  - Trong chế độ Step-by-Step: delay 0.8s giữa các bước
- **Results Table** - Bảng kết quả với cột:
  - PID (Process ID)
  - AT (Arrival Time)
  - BT (Burst Time)
  - PR (Priority - nếu Priority scheduling)
  - CT (Completion Time)
  - TAT (Turn Around Time)
  - WT (Waiting Time)

### Deadlock Management
- **Safety Check**:
  - Available resources, Need matrix
  - Safe sequence (nếu an toàn)
  - Trạng thái: SAFE hoặc UNSAFE
  
- **Resource Request**:
  - Kiểm tra 4 điều kiện
  - Kết quả: Approved hoặc Denied (với lý do)
  
- **Deadlock Detection**:
  - Request matrix từ user
  - Danh sách processes bị deadlock
  
- **Deadlock Recovery**:
  - Các bước abort process
  - Tài nguyên được phục hồi sau mỗi bước

### Page Replacement
- **Bảng kết quả**: Hàng là frame, cột là từng bước thực thi
  - Màu **đỏ**: Page Fault xảy ra
  - Màu **xanh**: Không Page Fault
- **Thống kê**: 
  - Tổng Page Faults
  - Hit Rate (%)
  - Fault Rate (%)

## Phát triển

### Thêm thuật toán mới

#### Page Replacement:
1. Tạo class mới trong `Algorithms/PageReplacementAlgo/`
2. Implement interface `IPageReplacement`
3. Cập nhật `CreateAlgorithm()` trong `Program.cs`

#### CPU Scheduling:
1. Tạo class mới trong `Algorithms/CPUSchedulingAlgo/`
2. Implement interface `ICPUScheduling`
3. Cập nhật `CreateAlgorithm()` và menu trong `Program.cs`

#### Deadlock Management:
1. Thêm tính năng mới vào `DeadlockSystemLogic.cs`
2. Cập nhật display helpers trong `DeadlockSystemHelpers.cs`
3. Thêm handler mới trong `Program.cs` dưới `HandleDeadlockSystem()`

### Thêm test case mới
1. Tạo file `TestData/CPUScheduling_TestCaseN.txt` hoặc `TestData/Deadlock_*.txt`
2. Format:
   - **CPU Scheduling**: Dòng 1 = N (số tiến trình); Dòng 2+ = AT BT PR (nếu Priority)
   - **Deadlock**: Dòng 1 = P R; Dòng 2 = Total; Dòng 3..P+2 = Max; Dòng P+3..2P+2 = Allocation (Available tính tự động)

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
- Kiểm tra: `dotnet --version`

### Lỗi khi hiển thị Unicode (ký tự Việt)
- Đảm bảo Terminal/Console hỗ trợ UTF-8
- Trên Windows: Sử dụng PowerShell 7.0+ hoặc Windows Terminal
- Kiểm tra encoding: File `Program.cs` phải là UTF-8

### Build hoặc Run thất bại
```bash
# Clean và restore lại
dotnet clean
dotnet restore
dotnet build
dotnet run
```

### Lỗi: "Cannot find test data files"
- Tạo folder `TestData/` nếu chưa có
- Thêm file test cases: `CPUScheduling_TestCase1.txt`, `Deadlock_Safe.txt`, v.v.
- Tham khảo format trong code comments hoặc README

### Display bị lag hoặc không hiển thị đúng
- Đảm bảo console size ít nhất 120x35 (ứng dụng sẽ cố gắng resize)
- Tăng kích thước font nếu text quá nhỏ
- Nếu Step-by-Step chạy quá nhanh: Tăng `STEP_DELAY_MS` trong `CPUSchedulingDisplayModes.cs`

### Một số thuật toán không hoạt động đúng
- Kiểm tra file `Program.cs` mà đảm bảo `CreateAlgorithm()` mapping đúng
- Xác minh dữ liệu input có hợp lệ (không âm, > 0, v.v.)
- Nếu test case từ file: Kiểm tra format đúng theo yêu cầu


