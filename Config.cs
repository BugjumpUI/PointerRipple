using System.Drawing;

namespace PointerRipple {
	public class Config {
		public int FinalRadius { get; set; } = 48;
		public int Duration { get; set; } = 200;
		public Color RippleColor { get; set; } = Color.FromArgb(128, 128, 128);

		public int InitialRadius {
			get { return FinalRadius / 2; }
		}
	}
}
