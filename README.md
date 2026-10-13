# ApexSenseBridge 1.1.0

**Tay cầm Flydigi APEX. Tính năng gốc của PlayStation 5 DualSense. Trên Windows.**

ApexSenseBridge mang toàn bộ tính năng điều khiển và phản hồi xúc giác gốc của **PlayStation 5 DualSense** lên các dòng tay cầm Flydigi **APEX 4, APEX 5 và APEX 6 Pro** thông qua cơ chế giả lập tay cầm DualSense ảo:
- **APEX 4 & APEX 5:** Chuyển đổi Adaptive Triggers (cò phản hồi lực) thành lực cản FORCEADAPT và chuyển đổi âm thanh rung haptics thành rung báng cầm.
- **APEX 6 Pro:** Sử dụng hệ thống 4 mô-tơ voice-coil độc quyền để tái hiện âm thanh rung haptic gốc và hiệu ứng cò lực.

Bản phát hành **1.1.0** mang đến giao diện lưới poster 8 cột hiện đại, kệ game chơi gần đây, lưu theme bền vững, khởi động mở cửa sổ trực tiếp và hoàn thiện 100% ảnh bìa chính thức cho 220 tựa game.

[Tải về bản mới nhất (v1.1.0)](https://github.com/Huyenbk97/ApexSenseBridge/releases/latest) · [Khắc phục sự cố](TROUBLESHOOTING.md) · [Lịch sử cập nhật](CHANGELOG.md)

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%20%7C%2011%20x64-lightgrey.svg)](#yêu-cầu-hệ-thống)
[![Release](https://img.shields.io/github/v/release/Huyenbk97/ApexSenseBridge?color=brightgreen)](https://github.com/Huyenbk97/ApexSenseBridge/releases/latest)
[![Authenticode](https://img.shields.io/badge/code%20signing-Authenticode-blueviolet.svg)](#bảo-mật-chữ-ký-số--tự-động-khôi-phục)

<img width="500" alt="Giao diện ApexSenseBridge" src="https://github.com/user-attachments/assets/c47eba4c-e718-4890-9947-4afcca206aa8" />

---

## Mục lục

- [Điểm nổi bật trên phiên bản 1.1.0](#điểm-nổi-bật-trên-phiên-bản-110)
- [Các tay cầm được hỗ trợ](#các-tay-cầm-được-hỗ-trợ)
- [Khả năng tương thích trò chơi](#khả-năng-tương-thích-trò-chơi)
- [Nguyên lý hoạt động](#nguyên-lý-hoạt-động)
- [Hướng dẫn cài đặt & Bắt đầu](#hướng-dẫn-cài-đặt--bắt-đầu)
- [Các chế độ sử dụng](#các-chế-độ-sử-dụng)
- [Cài đặt ứng dụng & Khôi phục kết nối](#cài-đặt-ứng-dụng--khôi-phục-kết-nối)
- [Phím tắt thao tác Touchpad](#phím-tắt-thao-tác-touchpad)
- [Bảo mật & Tự động phục hồi](#bảo-mật-chữ-ký-số--tự-động-khôi-phục)
- [Lệnh chuẩn đoán (CLI)](#lệnh-chuẩn-đoán-cli)
- [Biên dịch từ mã nguồn](#biên-dịch-từ-mã-nguồn)
- [Giấy phép & Bản quyền](#giấy-phép--ghi-nhận-bản-quyền)

---

## Điểm nổi bật trên phiên bản 1.1.0

- **🎛️ Chế độ xem lưới 8 cột (8-Column Grid View):** Bổ sung tùy chọn xem danh sách game dạng poster lưới 8 cột tối ưu cho màn hình rộng và TV, duyệt game trực quan và sinh động.
- **⏱️ Kệ trò chơi đã chơi gần đây (Recently Played Shelf):** Tự động theo dõi lịch sử khởi chạy và ghim tối đa 8 game vừa chơi lên đầu màn hình cùng thời gian tương đối (ví dụ: *vừa xong*, *2 giờ trước*).
- **🎨 Lưu Theme bảng điều khiển cố định:** Hệ thống theme Console (PlayStation, Xbox, Nintendo Switch, Steam Deck và Màu nhấn tùy chỉnh) được lưu bền vững vào cài đặt, không còn bị reset về mặc định khi mở lại app.
- **🖼️ 100% Ảnh bìa chính thức chuẩn nét:** Tích hợp đầy đủ ảnh bìa dọc chuẩn PlayStation Store và Cloudflare Steam CDN cho toàn bộ 220+ tựa game có hỗ trợ DualSense.
- **🖱️ Không bị nhảy danh sách khi chọn game:** Sửa lỗi giật/nhảy vị trí cuộn khi người dùng click chuột vào thẻ game ở cả hai chế độ Danh sách và Lưới.
- **🚀 Mở ứng dụng là hiện giao diện ngay:** Nhấp đúp vào `ApexSenseBridgeTray.exe` sẽ lập tức mở cửa sổ Dashboard lên màn hình chính mà không bắt buộc người dùng phải tìm bấm vào icon dưới khay hệ thống (System Tray). Tham số chạy nền (`--minimized`) được bảo toàn cho tính năng khởi động cùng Windows.
- **🎮 Điều hướng tay cầm chuẩn Console:** Hỗ trợ đầy đủ điều hướng bằng tay cầm (D-pad nhảy 8 ô khi ở chế độ lưới, nút A/B/X/Y, cò và cần gạt) kèm các phím tắt bàn phím (mũi tên, Enter, Esc, Ctrl+Tab, Ctrl+F).
- **🌍 9 ngôn ngữ tích hợp:** Tiếng Việt, Tiếng Anh, Pháp, Tây Ban Nha, Trung giản thể, Nga, Hàn, Nhật và Bồ Đào Nha, tự động nhận diện ngôn ngữ Windows và đổi tức thì trong app.
- **🕹️ Hiệu chỉnh riêng biệt theo từng dòng tay cầm:** Hồ sơ hiệu chỉnh độc lập cho APEX 4, APEX 5 và APEX 6 Pro, tự động nhận diện phần cứng thực tế trước khi áp dụng.
- **🔊 Khuếch đại lực rung báng cầm (0–200%):** Tăng cường lực rung truyền thống lên tới 200% trên APEX 4 và APEX 5 mà không làm ảnh hưởng đến độ trễ hay lực cản của cò.
- **🎯 Rung Voice-Coil gốc trên APEX 6 Pro:** Hỗ trợ đầy đủ APEX 6 Pro và biến thể Phantom Blade Zero `0x98` với xung nhịp đồng bộ 1 kHz và hiệu ứng Bow `0x22` gốc.
- **🧭 Giải mã Gyroscope 16-bit trên APEX 4:** Đọc trực tiếp con quay hồi chuyển phần cứng với độ nhạy tùy chỉnh 25–400% cùng thanh trượt bù trừ trục Yaw độc lập.
- **🛡️ Bộ giải cứu kết nối & Dọn dẹp tiến trình rác:** Tự động phát hiện khi tay cầm bị ngắt kết nối/ngủ và hỗ trợ khôi phục tức thì mà không cần tắt game.

---

## Các tay cầm được hỗ trợ

| Tay cầm | Phương thức kết nối | Tình trạng kiểm thử phần cứng | Tính năng đặc biệt |
|---|---|---|---|
| **Flydigi APEX 6 Pro** | Đầu thu 2.4 GHz / Cáp USB | ✅ Đã kiểm thử thực tế (Bản thường & PBZ `0x98`) | 4 mô-tơ voice-coil, rung haptic DualSense PCM gốc & gain khuếch đại, đồng bộ xung nhịp cò, hiệu ứng Bow `0x22` |
| **Flydigi APEX 5** | Đầu thu 2.4 GHz / Cáp USB | ✅ Hoạt động hoàn hảo trong game | Lực cản cò FORCEADAPT đầy đủ, Audio Haptics, khuếch đại rung báng cầm 0–200%, chuyển đổi nhanh Profile phần cứng 1–4 |
| **Flydigi APEX 4** | Đầu thu 2.4 GHz / Cáp USB (Chế độ DInput) | ✅ Hoạt động hoàn hảo trong game | Con quay hồi chuyển Gyro 16-bit gốc (độ nhạy 25–400% & bù Yaw), Lực cản cò FORCEADAPT, khuếch đại rung 0–200%, tự nhận dạng interface 32/64 byte |

---

## Khả năng tương thích trò chơi

### ✅ Đã kiểm thử thực tế trên phần cứng

- **Call of Duty: Modern Warfare:** Khấc cò động theo từng loại súng (tường chặn súng bán tự động, độ giật súng liên thanh, nhả chốt súng ngắm) và rung haptic âm thanh (bước chân, trượt, cháy nổ).
- **Marvel's Spider-Man 2:** Lực căng khi đu tơ, tiếng click khi bắn tơ, vuốt cử chỉ camera và thao tác ứng dụng FNSM.
- **Grand Theft Auto V Enhanced:** Lực cản chân ga, nhịp giật chống bó cứng phanh ABS, rung địa hình và vòng tua máy.
- **Death Stranding 2:** Lực cản khi mang vác hàng hóa / địa hình dốc, thao tác đầy đủ nút DualSense và rung haptic chân thực.
- **Ghost of Tsushima Director's Cut:** Lực va chạm vũ khí, cử chỉ vuốt gió dẫn đường (Giữ D-pad Phải + gạt cần Analog) và rung xúc giác chi tiết.
- **Marvel's Spider-Man: Miles Morales:** Lực giật điện Venom, độ căng tơ nhện và cử chỉ ứng dụng FNSM.
- **Warframe:** Cử chỉ vuốt thi triển chiêu thức tức thì và lực cản vũ khí theo thời gian thực.

### 📚 Hơn 220 tựa game tự động nhận diện DualSense

Thư viện game được đồng bộ tự động từ cơ sở dữ liệu PCGamingWiki. Ứng dụng khay hệ thống (Tray App) và tiện ích mở rộng Playnite sẽ tự động kích hoạt cầu nối khi bạn mở bất kỳ tựa game nào trong danh sách.

---

## Nguyên lý hoạt động

```text
Game thông thường / Không hỗ trợ DualSense:
Tay cầm APEX ──► XInput gốc của Windows ──► Game (ApexSenseBridge hoàn toàn ngủ yên)

Game hỗ trợ DualSense:
Tay cầm APEX ──► ApexSenseBridge (C++20 Engine) ──► DualSense ảo ──► Game
                      └── HidHide ẩn tay cầm vật lý tránh bị trùng lặp nút
                          (Macro chuột/phím của Flydigi Space Station vẫn hoạt động)
```

- **Không can thiệp vào bộ nhớ game (Zero In-Game Injection):** Không dùng injection, không chèn DLL, không chỉnh sửa file game. Mọi xử lý đều thông qua tầng driver HID chuẩn của Windows.
- **Độ trễ cực thấp (< 2 ms):** Tốc độ chuyển tiếp báo cáo đo được từ 0.67 đến 1.54 ms ở tần số ~800–900 Hz trên APEX 5.
- **Bảo vệ chống loạn nút:** Nếu driver không thể ẩn tay cầm vật lý an toàn, phiên làm việc sẽ dừng lại ngay lập tức để tránh tình trạng game nhận cả 2 tay cầm cùng lúc.

---

## Hướng dẫn cài đặt & Bắt đầu

> [!TIP]
> Nếu gặp bất kỳ vấn đề nào trong quá trình cài đặt hay sử dụng, vui lòng tham khảo [Hướng dẫn xử lý lỗi (TROUBLESHOOTING.md)](TROUBLESHOOTING.md).

### Yêu cầu hệ thống

- Windows 10 hoặc Windows 11 **64-bit (x64)**.
- Tay cầm APEX được kết nối qua **Cáp USB hoặc đầu thu USB 2.4 GHz** (Với APEX 4, vui lòng gạt sang chế độ **DInput**).
- Quyền Quản trị viên (Administrator) để cài đặt driver USBip và HidHide trong lần đầu tiên.
- Đối với game chạy qua Steam: **Hãy tắt Steam Input trong thuộc tính của game đó** để tín hiệu DualSense gốc truyền thẳng tới tay cầm ảo.

### Cách 1: Sử dụng Bộ cài đặt Offline (Khuyên dùng)

1. Tải về file **`ApexSenseBridge-Setup.exe`** từ mục [Bản phát hành mới nhất (Releases)](https://github.com/Huyenbk97/ApexSenseBridge/releases/latest).
2. Chạy file cài đặt (cấp quyền Administrator khi được hỏi).
   - Bộ cài sẽ tự động thiết lập Engine, Tray app vào thư mục `%ProgramFiles%\ApexSenseBridge`.
   - Cài đặt driver chứng thực `usbip-win2 0.9.8.0` và `HidHide 1.5.230`.
   - Tự động tích hợp tiện ích mở rộng vào Playnite nếu bạn đã cài Playnite.
3. **Khởi động lại máy tính (Restart PC)** nếu được yêu cầu để nạp driver USBip.

### Cách 2: Sử dụng Bản Portable (Không cần cài đặt)

1. Tải về file **`ApexSenseBridge-Portable.zip`**.
2. Giải nén file zip vào bất kỳ thư mục nào trên máy tính của bạn.
3. Nhấp chuột phải vào `Install-Drivers.cmd` chọn **Run as administrator** một lần duy nhất để cài driver, sau đó khởi động lại máy tính.
4. Chạy `ApexSenseBridgeTray.exe` để bắt đầu sử dụng.

---

### Trải nghiệm phiên chơi đầu tiên

1. Mở **`ApexSenseBridgeTray.exe`**. Giao diện Dashboard sẽ hiện ra ngay lập tức.
2. Kết nối tay cầm APEX của bạn và chờ trong giây lát để tên dòng tay cầm được xác thực hiển thị.
3. Mở một tựa game được hỗ trợ. Ứng dụng sẽ tự động kích hoạt cầu nối DualSense ảo; khi tắt game, cầu nối sẽ tự động đóng lại.
4. Nếu game chỉ nhận diện tay cầm lúc mới khởi động: Hãy bật công tắc **Kích hoạt thủ công (Manual Bridge)** trên giao diện, chờ dòng chữ báo **Bridge active**, rồi hẵng mở game. Chơi xong thì tắt công tắc này đi.

---

## Các chế độ sử dụng

### 1. Ứng dụng khay hệ thống độc lập (Steam, Epic Games, EA, Game Pass, v.v.)

- Chạy **`ApexSenseBridgeTray.exe`** (có thể bật tùy chọn khởi động cùng Windows).
- Ứng dụng chạy gọn nhẹ trong khay hệ thống với giao diện đậm chất máy console.
- **Tự động nhận diện:** Tự động mở và đóng cầu nối DualSense cho hơn 220 tựa game.
- **Tự học file thực thi (.exe):** Tự động phát hiện các file thực thi của game đã qua mod, đổi tên hoặc chạy qua launcher trung gian sau 30 giây chơi ổn định.
- **Tùy chỉnh riêng cho từng game:** Chỉ định cấu hình lực cò, gán khe cắm phần cứng APEX 5 (1–4) theo từng tựa game cụ thể.

### 2. Tích hợp với Playnite

- Hoạt động liền mạch ở cả chế độ Desktop và Fullscreen (Big Picture) của Playnite.
- Khi khởi động game từ Playnite, tiện ích sẽ tự động ẩn tay cầm gốc và kích hoạt DualSense ảo.
- Nhấp chuột phải vào game trong Playnite > chọn **ApexSenseBridge** để điều chỉnh độ nhạy cò, cử chỉ touchpad, khe cấu hình hoặc độ khuếch đại rung.

---

## Cài đặt ứng dụng & Khôi phục kết nối

### Điều khiển cảm giác tay cầm theo từng Model

Trong mục Cài đặt (Settings), bạn có thể tùy chỉnh:
- **Lực cản của cò (Trigger Strength):** 0–100%.
- **Độ mạnh rung báng cầm (Grip Vibration):** 0–200% trên APEX 4/5 (0–100% trên APEX 6 Pro).
- **Ngưỡng rung âm thanh (Audio-Haptic Threshold):** 0–95%.
- **Độ nhạy con quay hồi chuyển (APEX 4 Gyro):** 25–400% kèm thanh trượt bù lệch trục ngang (Yaw).
- Các tùy chọn này được lưu độc lập cho từng dòng tay cầm (APEX 4, 5, 6 riêng biệt) và chỉ áp dụng khi phần cứng tương ứng được kết nối.

### Tự động khôi phục khi mất kết nối (Disconnection Recovery)

Nếu tay cầm bị hết pin, ngắt dây hoặc rơi vào trạng thái ngủ trong lúc đang chơi game:
- Ứng dụng sẽ tạm dừng kích hoạt để bảo vệ game và hiển thị thông báo khôi phục trên trang chủ.
- Khi bạn cắm lại tay cầm, chỉ cần bấm **Khôi phục cầu nối (Resume bridge)**. Game sẽ tiếp tục nhận tín hiệu bình thường mà bạn không cần phải tắt game đi mở lại.

---

## Phím tắt thao tác Touchpad

Đối với các game đòi hỏi thao tác vuốt hoặc nhấn bàn cảm ứng (Touchpad) của DualSense, ApexSenseBridge chuyển đổi qua các tổ hợp phím sau:

| Tựa game | Thao tác trên tay cầm | Tín hiệu Touchpad DualSense ảo |
|---|---|---|
| **Spider-Man 2** | Giữ nút `View` / Giữ `D-pad Lên` | Vuốt sang Trái (App FNSM) / Vuốt Lên/Xuống (Mở Máy ảnh) |
| **Miles Morales** | Giữ nút `View` | Vuốt sang Trái (Mở App FNSM) |
| **Ghost of Tsushima** | Giữ `D-pad Phải` + Gạt cần Analog Phải | Vuốt hướng Gió dẫn đường theo chiều gạt cần |
| **Warframe** | Giữ `RB` + Bấm `A / B / X / Y` | Vuốt dùng Chiêu thức 1, 2, 3, 4 (Lên / Xuống / Trái / Phải) |
| **Death Stranding 2** | Bấm hoặc giữ `View/Back` / Tổ hợp `LB + Menu` | Bấm/Giữ bên Phải (Thả tim, Giao tiếp) / Bấm bên Trái (Chế độ chụp ảnh) |
| **Game DualSense chung** | Bấm nút `View` | Nhấn Touchpad thông thường (Click) |

> [!NOTE]
> Bốn nút lưng M1–M4 không nằm trong gói dữ liệu chuẩn của DualSense. Bạn có thể dùng phần mềm Flydigi Space Station để gán M1–M4 thành các nút tay cầm tiêu chuẩn hoặc phím bàn phím tùy ý.

---

## Bảo mật, Chữ ký số & Tự động phục hồi

- **Chữ ký số Authenticode:** Toàn bộ file thực thi chính thức đều được ký số xác thực nguồn gốc an toàn.
- **Cơ chế Watchdog bảo vệ:** Nếu game bị crash đột ngột hoặc máy tính bị mất điện, hệ thống sẽ tự động mở lại khả năng nhìn thấy của tay cầm gốc thông qua registry RunOnce.
- **Khôi phục thủ công khi cần:** Nếu tay cầm bị ẩn trong hệ thống và bạn muốn hiện lại ngay lập tức, hãy chạy lệnh sau trong PowerShell/CMD:
  ```powershell
  ApexSenseBridge.exe restore-controller-visibility
  ```

---

## Lệnh chuẩn đoán (CLI)

Bạn có thể chạy các lệnh này qua Command Prompt hoặc PowerShell để kiểm tra trạng thái phần cứng:

```text
ApexSenseBridge.exe list                              # Liệt kê các tay cầm đang kết nối
ApexSenseBridge.exe diagnose [--all-hid] [--json]     # Chuẩn đoán xung đột phần mềm (HidHide, DSX, v.v.)
ApexSenseBridge.exe identify [vị_trí]                 # Nhận dạng chính xác model tay cầm
ApexSenseBridge.exe input-status [vị_trí]             # Xem dữ liệu nút bấm và cần gạt theo thời gian thực
ApexSenseBridge.exe test-rt [vị_trí]                  # Kiểm tra lực cản cò phải (RT)
ApexSenseBridge.exe test-trigger [vị_trí]             # Kiểm tra cò thích ứng (LT, RT hoặc cả hai)
ApexSenseBridge.exe test-rumble [vị_trí]              # Kiểm tra mô-tơ rung báng cầm
ApexSenseBridge.exe restore-controller-visibility     # Hiện lại tay cầm gốc trong Windows
```

---

## Biên dịch từ mã nguồn

### Yêu cầu môi trường

- Windows 10 / 11 64-bit
- Visual Studio 2022 / 2026 Build Tools (Desktop C++ & Windows SDK)
- MSBuild và .NET Framework 4.6.2 Developer Pack
- CMake 3.25 trở lên
- Inno Setup 6 (để đóng gói bộ cài đặt)

```powershell
# 1. Biên dịch native engine và chạy bài kiểm thử
.\scripts\build-windows.ps1
ctest --test-dir .\build-win -C Release --output-on-failure

# 2. Biên dịch Tray App và kiểm tra tính nhất quán phiên bản
.\scripts\build-tray-app.ps1
.\scripts\verify-version-consistency.ps1

# 3. Đóng gói bộ cài đặt và bản portable
.\scripts\build-installer.ps1
```

---

## Giấy phép & Ghi nhận bản quyền

- Phát hành theo Giấy phép **MIT License**. Xem chi tiết tại [LICENSE](LICENSE).
- Sử dụng các thành phần mã nguồn mở được vá tối ưu từ VIIPER, usbip-win2 và HidHide. Xem chi tiết tại [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).
