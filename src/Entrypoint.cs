using System;
using System.Diagnostics;
using System.Numerics;

namespace RTWeekend;

interface IScene {
  void Render(Vector3[,] image);
}

public class Entrypoint {
  /// <summary>
  /// Cheap approximation of gamma correction.
  /// </summary>
  private static void GammaCorrect(Vector3[,] image) {
    int height = image.GetLength(0);
    int width = image.GetLength(1);

    for (int y = 0; y < height; ++y) {
      for (int x = 0; x < width; ++x) {
        image[y, x].X = MathF.Sqrt(image[y, x].X);
        image[y, x].Y = MathF.Sqrt(image[y, x].Y);
        image[y, x].Z = MathF.Sqrt(image[y, x].Z);
      }
    }
  }

  public static void Main() {
    int width = 1200;
    int height = 800;
    var image = new Vector3[height, width];

    //var scene = new GradientScene();
    //var scene = new SkyGradientScene();
    //var scene = new NonShadedSphereScene();
    //var scene = new NormalShadedSphereScene();
    var scene = new TestScene();

    var watch = new Stopwatch();
    watch.Start();

    scene.Render(image);

    watch.Stop();
    Console.WriteLine($"\nrender time: {watch.Elapsed}");

    GammaCorrect(image);

    PPM.Write("out.ppm", image);
  }
}
