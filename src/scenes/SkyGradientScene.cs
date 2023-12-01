using System.Numerics;

namespace RTWeekend;

class SkyGradientScene : IScene {
  public void Render(Vector3[,] image) {
    var imageWidth = image.GetLength(1);
    var imageHeight = image.GetLength(0);

    var aspectRatio = (float)imageWidth / imageHeight;
    var viewportHeight = 2.0f;
    var viewportWidth = aspectRatio * viewportHeight;
    var focalLength = 1.0f; // Distance between the eye and the projection plane.

    var origin = Vector3.Zero;
    var horizontal = new Vector3(viewportWidth, 0, 0);
    var vertical = new Vector3(0, viewportHeight, 0);
    var lowerLeftCorner = origin - 0.5f * horizontal - 0.5f * vertical - new Vector3(0, 0, focalLength);

    var progress = new Progress();
    for (int y = 0; y < imageHeight; ++y) {
      for (int x = 0; x < imageWidth; ++x) {
        float u = (float)x / (imageWidth - 1);
        float v = (float)y / (imageHeight - 1);

        var ray = new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical - origin);
        image[y, x] = Color(ray);
      }
      progress.Report(y + 1, imageHeight);
    }
  }

  private static Vector3 Color(Ray ray) {
    var dir = Vector3.Normalize(ray.Direction);
    var t = 0.5f * dir.Y + 0.5f;
    return Vector3.Lerp(new Vector3(1), new Vector3(0.5f, 0.7f, 1.0f), t);
  }
}
