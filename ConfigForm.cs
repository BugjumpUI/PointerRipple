using System;
using System.Drawing;
using System.Windows.Forms;

namespace PointerRipple {
	public class ConfigForm : Form {
		public Config Config { get; private set; }

		private NumericUpDown radiusInput;
		private NumericUpDown durationInput;
		private Button colorButton;
		private Button okButton;
		private Button cancelButton;

		public ConfigForm(Config config) {
			this.Config = new Config {
				FinalRadius = config.FinalRadius,
				Duration = config.Duration,
				RippleColor = config.RippleColor
			};

			this.Text = "PointerRipple 配置";
			this.Size = new Size(300, 200);
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.StartPosition = FormStartPosition.CenterScreen;

			Label radiusLabel = new Label();
			radiusLabel.Text = "最终半径 (像素):";
			radiusLabel.Location = new Point(10, 15);
			radiusLabel.Size = new Size(100, 20);

			radiusInput = new NumericUpDown();
			radiusInput.Location = new Point(120, 13);
			radiusInput.Size = new Size(60, 20);
			radiusInput.Minimum = 10;
			radiusInput.Maximum = 200;
			radiusInput.Value = Config.FinalRadius;

			Label durationLabel = new Label();
			durationLabel.Text = "动画时长 (ms):";
			durationLabel.Location = new Point(10, 45);
			durationLabel.Size = new Size(100, 20);

			durationInput = new NumericUpDown();
			durationInput.Location = new Point(120, 43);
			durationInput.Size = new Size(60, 20);
			durationInput.Minimum = 50;
			durationInput.Maximum = 1000;
			durationInput.Increment = 10;
			durationInput.Value = Config.Duration;

			Label colorLabel = new Label();
			colorLabel.Text = "颜色:";
			colorLabel.Location = new Point(10, 75);
			colorLabel.Size = new Size(100, 20);

			colorButton = new Button();
			colorButton.Location = new Point(120, 73);
			colorButton.Size = new Size(60, 23);
			colorButton.BackColor = Config.RippleColor;
			colorButton.Click += (s, e) => {
				using (ColorDialog cd = new ColorDialog()) {
					cd.Color = Config.RippleColor;
					if (cd.ShowDialog() == DialogResult.OK) {
						Config.RippleColor = cd.Color;
						colorButton.BackColor = cd.Color;
					}
				}
			};

			okButton = new Button();
			okButton.Text = "确定";
			okButton.Location = new Point(120, 110);
			okButton.Size = new Size(60, 23);
			okButton.DialogResult = DialogResult.OK;
			okButton.Click += (s, e) => {
				Config.FinalRadius = (int)radiusInput.Value;
				Config.Duration = (int)durationInput.Value;
			};

			cancelButton = new Button();
			cancelButton.Text = "取消";
			cancelButton.Location = new Point(190, 110);
			cancelButton.Size = new Size(60, 23);
			cancelButton.DialogResult = DialogResult.Cancel;

			this.Controls.Add(radiusLabel);
			this.Controls.Add(radiusInput);
			this.Controls.Add(durationLabel);
			this.Controls.Add(durationInput);
			this.Controls.Add(colorLabel);
			this.Controls.Add(colorButton);
			this.Controls.Add(okButton);
			this.Controls.Add(cancelButton);
		}
	}
}
