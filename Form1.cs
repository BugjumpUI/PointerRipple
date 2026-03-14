using System;
using System.Drawing;
using System.Windows.Forms;

namespace PointerRipple
{
    public partial class Form1 : Form
    {
        private Point mouseLocation = Point.Empty;

        public Form1()
        {
            InitializeComponent();

            // 启用双缓冲，防止闪烁
            this.DoubleBuffered = true;

            // 设置窗体属性
            this.Text = "鼠标跟随圆";
            this.Size = new Size(800, 600);
            this.BackColor = Color.White;

            // 鼠标移动事件
            this.MouseMove += Form1_MouseMove;

            // 绘制事件
            this.Paint += Form1_Paint;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            // 更新鼠标位置
            mouseLocation = e.Location;
            // 触发重绘
            this.Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (mouseLocation != Point.Empty)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // 画一个红色圆，半径30像素
                int radius = 30;
                g.FillEllipse(Brushes.Red,
                    mouseLocation.X - radius,
                    mouseLocation.Y - radius,
                    radius * 2, radius * 2);

                // 可选：添加一个外框
                using (Pen pen = new Pen(Color.Blue, 2))
                {
                    g.DrawEllipse(pen,
                        mouseLocation.X - radius,
                        mouseLocation.Y - radius,
                        radius * 2, radius * 2);
                }
            }
        }
    }
}