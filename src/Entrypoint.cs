using System.Numerics;

namespace RTWeekend;

interface IScene {
  void Render(Vector3[,] image);
}

public class Entrypoint {
  public static void Main() {
    int width = 256;
    int height = 256;
    var image = new Vector3[height, width];

    var scene = new Gradient();
    scene.Render(image);

    PPM.Write("out.ppm", image);
  }
}
