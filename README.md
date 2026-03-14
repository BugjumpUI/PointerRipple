# PointerRipple / 鼠标指针波纹

[English](#english) | [中文](#chinese)

---

## English

### Overview
PointerRipple is a lightweight Windows utility that creates visual ripple effects around your mouse cursor when you click. It provides satisfying visual feedback for mouse clicks with smooth animations.

### Features
- **Visual Click Feedback**: Ripples appear around the cursor on mouse clicks
- **Dual Ripple Types**:
  - Press ripple: Expands while holding the button
  - Release ripples: Fade-out effects when releasing
- **Multi-monitor Support**: Works across all displays
- **System Tray Integration**: Easy access to show/hide and exit options
- **Always on Top**: Overlay stays above all windows
- **Click-Through**: The overlay doesn't interfere with mouse interactions

### How It Works
The application creates a transparent overlay window that covers all monitors. It uses a low-level mouse hook to detect clicks and draws ripple effects using GDI with alpha blending for smooth transparency.

### Requirements
- Windows OS (XP/Vista/7/8/10/11)
- .NET Framework (included with Windows)

### Installation
1. Download the executable
2. Run PointerRipple.exe (no installation required)
3. The app will appear in your system tray

### Usage
- **Left/Right/Middle Click**: Creates ripple effects
- **System Tray Icon**: Right-click to show/hide the overlay or exit the application
- **Automatic Start**: Add to startup folder for auto-run

### Building from Source
Open the solution in Visual Studio and build. The project targets .NET Framework and uses Windows Forms.

---

## Chinese

### 概述
PointerRipple 是一个轻量级的 Windows 工具，能在鼠标点击时创建视觉波纹效果。它为鼠标点击提供流畅的视觉反馈，让操作更加直观有趣。

### 功能特点
- **点击视觉反馈**：鼠标点击时在光标周围显示波纹
- **双重波纹效果**：
  - 按下波纹：按住鼠标时持续扩展
  - 释放波纹：松开鼠标时淡出消散
- **多显示器支持**：可在所有显示器上正常工作
- **系统托盘集成**：轻松访问显示/隐藏和退出选项
- **始终置顶**：覆盖层保持在所有窗口上方
- **点击穿透**：覆盖层不会干扰鼠标操作

### 工作原理
应用程序创建一个覆盖所有显示器的透明窗口。通过底层鼠标钩子检测点击事件，使用 GDI 绘制带有透明混合的波纹效果。

### 系统要求
- Windows 操作系统 (XP/Vista/7/8/10/11)
- .NET Framework (Windows 系统自带)

### 安装方法
1. 下载可执行文件
2. 运行 PointerRipple.exe（无需安装）
3. 应用程序将显示在系统托盘中

### 使用方法
- **左/右/中键点击**：触发波纹效果
- **系统托盘图标**：右键点击可显示/隐藏覆盖层或退出应用
- **开机自启**：添加到启动文件夹可实现开机自动运行

### 源码编译
在 Visual Studio 中打开解决方案并编译。项目基于 .NET Framework，使用 Windows Forms 开发。

---

## License / 许可证
MIT License

## Author / 作者
[Your Name]

## Contributing / 贡献
Feel free to submit issues and pull requests. / 欢迎提交问题和拉取请求。
