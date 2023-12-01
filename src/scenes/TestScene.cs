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
    var groundMaterial = new Lambertian(new Vector3(0.8f, 0.8f, 0.0f));
    var centerMaterial = new Lambertian(new Vector3(0.1f, 0.2f, 0.5f));
    var leftMaterial = new Dielectric(1.5f);
    var rightMaterial = new Metal(new Vector3(0.8f, 0.6f, 0.2f));

    world.Add(new Sphere(new Vector3(0.0f, -100.5f, -1.0f), 100.0f, groundMaterial));
    world.Add(new Sphere(new Vector3(0.0f, 0.0f, -1.0f), 0.5f, centerMaterial));
    world.Add(new Sphere(new Vector3(-1.0f, 0.0f, -1.0f), 0.5f, leftMaterial));
    world.Add(new Sphere(new Vector3(-1.0f, 0.0f, -1.0f), -0.4f, leftMaterial));
    world.Add(new Sphere(new Vector3(1.0f, 0.0f, -1.0f), 0.5f, rightMaterial));

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
      Vector3 attenuation;
      Ray scattered;
      if (rec.Material.Scatter(random, ray, rec, out attenuation, out scattered)) {
        return attenuation * Color(world, scattered, bounces - 1);
      }
      return Vector3.Zero;
    }

    return BackgroundColor(ray);
  }
}
