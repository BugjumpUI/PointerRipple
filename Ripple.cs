using System;
using System.Drawing;

namespace PointerRipple {
	public class Ripple {
		public Point Position { get; set; }
		public bool IsPress { get; set; }
		public bool IsReleased { get; set; }

		private DateTime pressStartTime;
		private DateTime? releaseStartTime;
		private float releaseAlpha;
		private Config config;

		public Ripple(Point pos, bool isPress, Config cfg) {
			Position = pos;
			IsPress = isPress;
			IsReleased = false;
			pressStartTime = DateTime.Now;
			config = cfg;
		}

		public void Release() {
			if (!IsReleased) {
				IsReleased = true;
				releaseStartTime = DateTime.Now;
				float elapsedPress = (float)(DateTime.Now - pressStartTime).TotalMilliseconds;
				float pressProgress = Math.Min(elapsedPress / config.Duration, 1f);
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
			float pressProgress = Math.Min(elapsedPress / config.Duration, 1f);
			float rev = 1f - pressProgress;
			float curved = 1f - (rev * rev);
			return config.InitialRadius + (curved * (config.FinalRadius - config.InitialRadius));
		}

		public int GetAlpha() {
			float elapsedPress = (float)(DateTime.Now - pressStartTime).TotalMilliseconds;
			float pressProgress = Math.Min(elapsedPress / config.Duration, 1f);
			if (!IsReleased) return (int)GetPressAlpha(pressProgress);
			if (!releaseStartTime.HasValue) return 0;
			float elapsedRelease = (float)(DateTime.Now - releaseStartTime.Value).TotalMilliseconds;
			float releaseProgress = Math.Min(elapsedRelease / config.Duration, 1f);
			float revOut = 1f - releaseProgress;
			float curvedOut = 1f - (revOut * revOut);
			return (int)(releaseAlpha * (1f - curvedOut));
		}

		public bool IsFinished() {
			if (!IsReleased || !releaseStartTime.HasValue) return false;
			float elapsedRelease = (float)(DateTime.Now - releaseStartTime.Value).TotalMilliseconds;
			return elapsedRelease >= config.Duration;
		}

		public Color GetColor() {
			return Color.FromArgb(GetAlpha(), config.RippleColor);
		}
	}
}
