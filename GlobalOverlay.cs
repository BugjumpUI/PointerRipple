using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PointerRipple {
	public partial class GlobalOverlay : Form {
		[DllImport("user32.dll")]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll")]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		private static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, int crKey, ref BLENDFUNCTION pblend, uint dwFlags);

		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

		[DllImport("gdi32.dll")]
		private static extern bool DeleteDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

		[DllImport("user32.dll")]
		private static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("user32.dll")]
		private static extern int GetSystemMetrics(int nIndex);

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("user32.dll")]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll")]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll")]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

		private const int WS_EX_LAYERED = 0x80000;
		private const int WS_EX_TRANSPARENT = 0x20;
		private const int WS_EX_TOOLWINDOW = 0x80;
		private const int GWL_EXSTYLE = -20;
		private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
		private const uint SWP_SHOWWINDOW = 0x0040;
		private const uint SWP_NOMOVE = 0x0002;
		private const uint SWP_NOSIZE = 0x0001;

		private const int AC_SRC_OVER = 0x00;
		private const int AC_SRC_ALPHA = 0x01;
		private const int ULW_ALPHA = 0x02;

		private const int WH_MOUSE_LL = 14;
		private const int WM_MOUSEMOVE = 0x0200;
		private const int WM_LBUTTONDOWN = 0x0201;
		private const int WM_LBUTTONUP = 0x0202;
		private const int WM_RBUTTONDOWN = 0x0204;
		private const int WM_RBUTTONUP = 0x0205;
		private const int WM_MBUTTONDOWN = 0x0207;
		private const int WM_MBUTTONUP = 0x0208;

		private const int SM_XVIRTUALSCREEN = 76;
		private const int SM_YVIRTUALSCREEN = 77;
		private const int SM_CXVIRTUALSCREEN = 78;
		private const int SM_CYVIRTUALSCREEN = 79;

		[StructLayout(LayoutKind.Sequential)]
		private struct BLENDFUNCTION {
			public byte BlendOp;
			public byte BlendFlags;
			public byte SourceConstantAlpha;
			public byte AlphaFormat;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct POINT {
			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct MSLLHOOKSTRUCT {
			public POINT pt;
			public uint mouseData;
			public uint flags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		private Point currentMousePos = Point.Empty;
		private LowLevelMouseProc mouseProc;
		private IntPtr mouseHookId = IntPtr.Zero;

		private Ripple pressRipple;
		private List<Ripple> releaseRipples = new List<Ripple>();
		private System.Windows.Forms.Timer animationTimer;

		private Bitmap backBuffer;
		private Graphics backGraphics;

		private NotifyIcon trayIcon;
		private ContextMenuStrip trayMenu;

		private class Ripple {
			public Point Position { get; set; }
			public bool IsPress { get; set; }
			public bool IsReleased { get; set; }

			private DateTime pressStartTime;
			private DateTime? releaseStartTime;
			private float releaseAlpha;

			private const float DURATION = 200f;

			public Ripple(Point pos, bool isPress) {
				Position = pos;
				IsPress = isPress;
				IsReleased = false;
				pressStartTime = DateTime.Now;
			}

			public void Release() {
				if (!IsReleased) {
					IsReleased = true;
					releaseStartTime = DateTime.Now;
					float elapsedPress = (float)(DateTime.Now - pressStartTime).TotalMilliseconds;
					float pressProgress = Math.Min(elapsedPress / DURATION, 1f);
					releaseAlpha = GetPressAlpha(pressProgress);
				}
			}

			private float GetPressAlpha(float progress) {
				float rev = 1f - progress;
				float curved = 1f - (rev * rev);
				return curved * 127f;
			}

			public float GetRadius() {
				float elapsedPress = (float)(DateTime.Now - pressStartTime).TotalMilliseconds;
				float pressProgress = Math.Min(elapsedPress / DURATION, 1f);
				float rev = 1f - pressProgress;
				float curved = 1f - (rev * rev);
				return 24f + (curved * 24f);
			}

			public int GetAlpha() {
				float elapsedPress = (float)(DateTime.Now - pressStartTime).TotalMilliseconds;
				float pressProgress = Math.Min(elapsedPress / DURATION, 1f);
				if (!IsReleased) return (int)GetPressAlpha(pressProgress);
				if (!releaseStartTime.HasValue) return 0;
				float elapsedRelease = (float)(DateTime.Now - releaseStartTime.Value).TotalMilliseconds;
				float releaseProgress = Math.Min(elapsedRelease / DURATION, 1f);
				float revOut = 1f - releaseProgress;
				float curvedOut = 1f - (revOut * revOut);
				return (int)(releaseAlpha * (1f - curvedOut));
			}

			public bool IsFinished() {
				if (!IsReleased || !releaseStartTime.HasValue) return false;
				float elapsedRelease = (float)(DateTime.Now - releaseStartTime.Value).TotalMilliseconds;
				return elapsedRelease >= DURATION;
			}
		}

		public GlobalOverlay() {
			int virtualWidth = GetSystemMetrics(SM_CXVIRTUALSCREEN);
			int virtualHeight = GetSystemMetrics(SM_CYVIRTUALSCREEN);
			int virtualLeft = GetSystemMetrics(SM_XVIRTUALSCREEN);
			int virtualTop = GetSystemMetrics(SM_YVIRTUALSCREEN);

			this.FormBorderStyle = FormBorderStyle.None;
			this.TopMost = true;
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.Manual;
			this.Bounds = new Rectangle(virtualLeft, virtualTop, virtualWidth, virtualHeight);

			int initialStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
			SetWindowLong(this.Handle, GWL_EXSTYLE, initialStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
			SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

			backBuffer = new Bitmap(virtualWidth, virtualHeight, PixelFormat.Format32bppArgb);
			backGraphics = Graphics.FromImage(backBuffer);
			backGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			mouseProc = HookCallback;
			using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
			using (var curModule = curProcess.MainModule) {
				mouseHookId = SetWindowsHookEx(WH_MOUSE_LL, mouseProc, GetModuleHandle(curModule.ModuleName), 0);
			}

			animationTimer = new System.Windows.Forms.Timer();
			animationTimer.Interval = 16;
			animationTimer.Tick += (s, e) => {
				if (pressRipple != null) pressRipple.Position = currentMousePos;
				for (int i = releaseRipples.Count - 1; i >= 0; i--) {
					if (releaseRipples[i].IsFinished()) releaseRipples.RemoveAt(i);
				}
				UpdateLayeredWindow();
			};
			animationTimer.Start();

			trayIcon = new NotifyIcon();
			trayIcon.Text = "PointerRipple";
			trayIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
			trayMenu = new ContextMenuStrip();
			ToolStripMenuItem showHideItem = new ToolStripMenuItem("显示/隐藏");
			showHideItem.Click += (s, e) => {
				this.Visible = !this.Visible;
				if (this.Visible) {
					SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
				}
			};
			trayMenu.Items.Add(showHideItem);
			trayMenu.Items.Add(new ToolStripSeparator());
			ToolStripMenuItem exitItem = new ToolStripMenuItem("退出应用");
			exitItem.Click += (s, e) => {
				trayIcon.Visible = false;
				Application.Exit();
			};
			trayMenu.Items.Add(exitItem);
			trayIcon.ContextMenuStrip = trayMenu;
			trayIcon.Visible = true;
		}

		private void UpdateLayeredWindow() {
			backGraphics.Clear(Color.FromArgb(0, 0, 0, 0));
			foreach (var r in releaseRipples) {
				float radius = r.GetRadius();
				int alpha = r.GetAlpha();
				int x = (int)(r.Position.X - radius);
				int y = (int)(r.Position.Y - radius);
				int size = (int)(radius * 2);
				using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, 128, 128, 128))) {
					backGraphics.FillEllipse(brush, x, y, size, size);
				}
			}
			if (pressRipple != null) {
				float radius = pressRipple.GetRadius();
				int alpha = pressRipple.GetAlpha();
				int x = (int)(pressRipple.Position.X - radius);
				int y = (int)(pressRipple.Position.Y - radius);
				int size = (int)(radius * 2);
				using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, 128, 128, 128))) {
					backGraphics.FillEllipse(brush, x, y, size, size);
				}
			}
			IntPtr screenDC = GetDC(IntPtr.Zero);
			IntPtr memDC = CreateCompatibleDC(screenDC);
			IntPtr hBitmap = backBuffer.GetHbitmap(Color.FromArgb(0));
			IntPtr oldBitmap = SelectObject(memDC, hBitmap);
			Point topPos = new Point(this.Left, this.Top);
			Size formSize = this.Size;
			Point srcPos = new Point(0, 0);
			BLENDFUNCTION blend = new BLENDFUNCTION();
			blend.BlendOp = AC_SRC_OVER;
			blend.BlendFlags = 0;
			blend.SourceConstantAlpha = 255;
			blend.AlphaFormat = AC_SRC_ALPHA;
			UpdateLayeredWindow(this.Handle, screenDC, ref topPos, ref formSize, memDC, ref srcPos, 0, ref blend, ULW_ALPHA);
			SelectObject(memDC, oldBitmap);
			DeleteObject(hBitmap);
			DeleteDC(memDC);
			ReleaseDC(IntPtr.Zero, screenDC);
		}

		private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
			if (nCode >= 0) {
				MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
				Point pos = new Point(hookStruct.pt.x, hookStruct.pt.y);
				int msg = wParam.ToInt32();
				switch (msg) {
					case WM_MOUSEMOVE:
						currentMousePos = pos;
						break;
					case WM_LBUTTONDOWN:
					case WM_RBUTTONDOWN:
					case WM_MBUTTONDOWN:
						if (this.IsHandleCreated) {
							this.BeginInvoke(new Action(() => {
								if (pressRipple != null) {
									pressRipple.Release();
									releaseRipples.Add(pressRipple);
								}
								pressRipple = new Ripple(pos, true);
							}));
						}
						break;
					case WM_LBUTTONUP:
					case WM_RBUTTONUP:
					case WM_MBUTTONUP:
						if (this.IsHandleCreated) {
							this.BeginInvoke(new Action(() => {
								if (pressRipple != null) {
									pressRipple.Release();
									releaseRipples.Add(pressRipple);
									pressRipple = null;
								}
							}));
						}
						break;
				}
			}
			return CallNextHookEx(mouseHookId, nCode, wParam, lParam);
		}

		protected override void OnFormClosing(FormClosingEventArgs e) {
			if (mouseHookId != IntPtr.Zero) UnhookWindowsHookEx(mouseHookId);
			animationTimer?.Stop();
			animationTimer?.Dispose();
			backGraphics?.Dispose();
			backBuffer?.Dispose();
			trayIcon.Visible = false;
			trayIcon.Dispose();
			base.OnFormClosing(e);
		}
	}
}
