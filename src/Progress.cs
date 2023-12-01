using System;

namespace RTWeekend;

public class Progress {
  private static readonly string[] FRAMES = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
  private int frameIndex;
  private long lastTime;

  public Progress() {
    Console.OutputEncoding = System.Text.Encoding.UTF8;
  }

  private bool CheckAdvanceFrame() {
    long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    if (now - lastTime < 100) return false;

    lastTime = now;
    frameIndex = (frameIndex + 1) % FRAMES.Length;
    return true;
  }

  public void Report(int current, int target) {
    if (current == target || CheckAdvanceFrame()) {
      var progress = (current * 100) / target;
      Console.Write($"\r{FRAMES[frameIndex]} {progress,3}%");
    }
  }
}
