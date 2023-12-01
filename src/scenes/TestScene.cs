using System;
using System.Numerics;

namespace RTWeekend;

class TestScene : IScene {
  private Random random = new();

  public void Render(Vector3[,] image) {
    var width = image.GetLength(1);
    var height = image.GetLength(0);

    var aspectRatio = (float)width / height;
    var focalLength = 1.0f; // Distance between the eye and the projection plane.

    var camera = new Camera(aspectRatio, focalLength);

    var world = new World();
    world.Add(new Sphere(new Vector3(0.0f, 0.0f, -1.0f), 0.5f));
    world.Add(new Sphere(new Vector3(0.0f, -100.5f, -1.0f), 100.0f));

    var maxBounces = 50;
    var sampleCount = 256;
    var sampleContribution = 1.0f / sampleCount;

    var progress = new Progress();
    for (int y = 0; y < height; ++y) {
      for (int x = 0; x < width; ++x) {
        var color = Vector3.Zero;

        for (int sampleIndex = 0; sampleIndex < sampleCount; ++sampleIndex) {
          var u = (x + random.NextSingle()) / (width - 1);
          var v = (y + random.NextSingle()) / (height - 1);
          var ray = camera.GetRay(u, v);
          color += sampleContribution * Color(world, ray, maxBounces);
        }
        image[y, x] = color;
      }
      progress.Report(y + 1, height);
    }
  }

  private static Vector3 BackgroundColor(Ray ray) {
    var dir = Vector3.Normalize(ray.Direction);
    var t = 0.5f * dir.Y + 0.5f;
    return Vector3.Lerp(new Vector3(1), new Vector3(0.5f, 0.7f, 1.0f), t);
  }

  private Vector3 Color(World world, Ray ray, int bounces) {
    // If we've exceeded the ray bounce limit, no more light is gathered.
    if (bounces <= 0) return Vector3.Zero;

    var rec = new HitRecord();
    var selfIntersectionBias = 0.001f;
    if (world.Hit(ray, new Range(selfIntersectionBias, float.MaxValue), ref rec)) {
      //var target = rec.P + rec.N + random.NextInUnitSphere();
      //var target = rec.P + rec.N + random.NextUnitVector3();
      var target = rec.P + random.NextInHemisphere(rec.N);
      return 0.5f * Color(world, new Ray(rec.P, target - rec.P), bounces - 1);
    }

    return BackgroundColor(ray);
  }
}
