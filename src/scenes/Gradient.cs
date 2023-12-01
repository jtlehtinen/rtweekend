using System.Numerics;

namespace RTWeekend;

class Gradient : IScene {
  public void Render(Vector3[,] image) {
    var width = image.GetLength(1);
    var height = image.GetLength(0);

    var progress = new Progress();
    for (int y = 0; y < height; ++y) {
      for (int x = 0; x < width; ++x) {
        image[y, x].X = (float)x / (width - 1);
        image[y, x].Y = (float)y / (height - 1);
        image[y, x].Z = 0.25f;
      }
      progress.Report(y + 1, height);
    }
  }
}
