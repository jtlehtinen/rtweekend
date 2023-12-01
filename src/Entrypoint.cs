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

    //var scene = new GradientScene();
    //var scene = new SkyGradientScene();
    //var scene = new NonShadedSphereScene();
    var scene = new NormalShadedSphereScene();
    scene.Render(image);

    PPM.Write("out.ppm", image);
  }
}
