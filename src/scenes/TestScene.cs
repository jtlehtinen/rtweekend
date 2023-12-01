using System.Numerics;

namespace RTWeekend;

class TestScene : IScene {
  public void Render(Vector3[,] image) {
    var width = image.GetLength(1);
    var height = image.GetLength(0);

    var aspectRatio = (float)width / height;
    var viewportHeight = 2.0f;
    var viewportWidth = aspectRatio * viewportHeight;
    var focalLength = 1.0f; // Distance between the eye and the projection plane.

    var origin = Vector3.Zero;
    var horizontal = new Vector3(viewportWidth, 0, 0);
    var vertical = new Vector3(0, viewportHeight, 0);
    var lowerLeftCorner = origin - 0.5f * horizontal - 0.5f * vertical - new Vector3(0, 0, focalLength);

    var world = new World();
    world.Add(new Sphere(new Vector3(0.0f, 0.0f, -1.0f), 0.5f));
    world.Add(new Sphere(new Vector3(0.0f, -100.5f, -1.0f), 100.0f));

    var progress = new Progress();
    for (int y = 0; y < height; ++y) {
      for (int x = 0; x < width; ++x) {
        var u = (float)x / (width - 1);
        var v = (float)y / (height - 1);

        var ray = new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical - origin);
        image[y, x] = Color(world, ray);
      }
      progress.Report(y + 1, height);
    }
  }

  private static Vector3 BackgroundColor(Ray ray) {
    var dir = Vector3.Normalize(ray.Direction);
    var t = 0.5f * dir.Y + 0.5f;
    return Vector3.Lerp(new Vector3(1), new Vector3(0.5f, 0.7f, 1.0f), t);
  }

  private static Vector3 Color(World world, Ray ray) {
    var rec = new HitRecord();
    if (world.Hit(ray, new Range(0, float.MaxValue), ref rec)) {
      return 0.5f * (rec.N + new Vector3(1.0f));
    }

    return BackgroundColor(ray);
  }
}
