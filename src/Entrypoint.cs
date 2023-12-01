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
    //var scene = new NormalShadedSphereScene();
    var scene = new TestScene();
    scene.Render(image);

    PPM.Write("out.ppm", image);
  }
}

#if false

public class EntryPoint {
  public static void Main() {
    int width = 400;
    int height = 300;
    var image = new Vector3[height, width];

    var aspect = (float)width / (float)height;
    var viewportHeight = 2.0f;
    var viewportWidth = aspect * viewportHeight;
    var focalLength = 1.0f; // Distance between the eye and the projection plane.

    var origin = Vector3.Zero;
    var horizontal = new Vector3(viewportWidth, 0, 0);
    var vertical = new Vector3(0, viewportHeight, 0);
    var lowerLeftCorner = origin - 0.5f * horizontal - 0.5f * vertical - new Vector3(0, 0, focalLength);

    var scene = new HittableList();
    scene.Add(new Sphere(new Vector3(0.0f, 0.0f, -1.0f), 0.5f));
    scene.Add(new Sphere(new Vector3(0.0f, -100.5f, -1.0f), 100.0f));

    var progress = new Progress();
    for (int y = 0; y < height; ++y) {
      for (int x = 0; x < width; ++x) {
        var u = (float)x / (float)(width - 1);
        var v = (float)y / (float)(height - 1);

        var ray = new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical - origin);
        image[y, x] = Color(scene, ray);
      }
      progress.Report(y + 1, height);
    }

    PPM.Write("out.ppm", image);
  }
}
#endif
