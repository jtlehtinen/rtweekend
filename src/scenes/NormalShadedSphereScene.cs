using System;
using System.Numerics;

namespace RTWeekend;

class NormalShadedSphereScene : IScene {
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

  private static float HitSphere(Ray ray, Vector3 center, float radius) {
    Vector3 origin = ray.Origin - center;
    float a = Vector3.Dot(ray.Direction, ray.Direction);
    float b = 2.0f * Vector3.Dot(origin, ray.Direction);
    float c = Vector3.Dot(origin, origin) - radius * radius;
    float discriminant = b * b - 4.0f * a * c;
    return discriminant < 0.0f ? -1.0f : (-b - MathF.Sqrt(discriminant)) / (2.0f * a);
  }

  private static Vector3 Color(Ray ray) {
    var center = new Vector3(0.0f, 0.0f, -1.0f);
    float t = HitSphere(ray, center, 0.5f);
    if (t > 0.0f) {
      var N = Vector3.Normalize(ray.At(t) - center);
      return 0.5f * (N + new Vector3(1));
    }

    return BackgroundColor(ray);
  }

  private static Vector3 BackgroundColor(Ray ray) {
    var dir = Vector3.Normalize(ray.Direction);
    var t = 0.5f * dir.Y + 0.5f;
    return Vector3.Lerp(new Vector3(1), new Vector3(0.5f, 0.7f, 1.0f), t);
  }
}
