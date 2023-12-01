using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Threading;

namespace RTWeekend;

class TestScene : IScene {
  private readonly bool parallel;

  public TestScene(bool parallel = false) {
    this.parallel = parallel;
  }

  private void TraceRow(Vector3[,] image, World world, Camera camera, Random random, int y) {
    var bounces = 50;
    var sampleCount = 500;
    var sampleContribution = 1.0f / sampleCount;

    var width = image.GetLength(1);
    var height = image.GetLength(0);

    for (int x = 0; x < width; ++x) {
      var color = Vector3.Zero;

      for (int sampleIndex = 0; sampleIndex < sampleCount; ++sampleIndex) {
        var u = (x + random.NextSingle()) / (width - 1);
        var v = (y + random.NextSingle()) / (height - 1);

        var ray = camera.GetRay(random, u, v);
        color += sampleContribution * Color(random, world, ray, bounces);
      }
      image[y, x] = color;
    }
  }

  private static readonly ThreadLocal<Random> ThreadRandom = new(() => new Random());

  private World CreateWorld(Random random) {
    var result = new World();

    result.Add(new Sphere(new Vector3(0.0f, -1000.0f, 0.0f), 1000.0f, new Lambertian(new Vector3(0.5f))));

    for (int a = -11; a < 11; ++a) {
      for (int b = -11; b < 11; ++b) {
        var chooseMaterial = random.NextSingle();
        var center = new Vector3(a + 0.9f * random.NextSingle(), 0.2f, b + 0.9f * random.NextSingle());

        if ((center - new Vector3(4.0f, 0.2f, 0.0f)).Length() > 0.9f) {
          if (chooseMaterial < 0.8f) { // diffuse
            var x = random.NextSingle() * random.NextSingle();
            var y = random.NextSingle() * random.NextSingle();
            var z = random.NextSingle() * random.NextSingle();
            result.Add(new Sphere(center, 0.2f, new Lambertian(new Vector3(x, y, z))));
          } else if (chooseMaterial < 0.95f) { // metal
            var x = random.NextSingle() * 0.5f + 0.5f;
            var y = random.NextSingle() * 0.5f + 0.5f;
            var z = random.NextSingle() * 0.5f + 0.5f;
            var fuzz = random.NextSingle() * 0.5f;
            result.Add(new Sphere(center, 0.2f, new Metal(new Vector3(x, y, z), fuzz)));
          } else { // glass
            result.Add(new Sphere(center, 0.2f, new Dielectric(1.5f)));
          }
        }
      }
    }

    result.Add(new Sphere(new Vector3(0.0f, 1.0f, 0.0f), 1.0f, new Dielectric(1.5f)));
    result.Add(new Sphere(new Vector3(-4.0f, 1.0f, 0.0f), 1.0f, new Lambertian(new Vector3(0.4f, 0.2f, 0.1f))));
    result.Add(new Sphere(new Vector3(4.0f, 1.0f, 0.0f), 1.0f, new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0.0f)));

    return result;
  }

  public void Render(Vector3[,] image) {
    var width = image.GetLength(1);
    var height = image.GetLength(0);

    var aspectRatio = (float)width / height;

    var eye = new Vector3(13.0f, 2.0f, 3.0f);
    var target = new Vector3(0.0f);
    var focusDistance = 10.0f;
    var camera = new Camera(
      eye,
      target,
      new Vector3(0.0f, 1.0f, 0.0f),
      20.0f,
      aspectRatio,
      0.1f,
      focusDistance
    );

    var world = CreateWorld(ThreadRandom.Value!);

    var progress = new Progress();
    if (parallel) {
      var rowsDone = 0;
      Parallel.For(
        0,
        height,
        (row) => {
          TraceRow(image, world, camera, ThreadRandom.Value!, row);
          var result = Interlocked.Increment(ref rowsDone);
          lock (progress) {
            progress.Report(result, height);
          }
        }
      );
    } else {
      for (int y = 0; y < height; ++y) {
        TraceRow(image, world, camera, ThreadRandom.Value!, y);
        progress.Report(y + 1, height);
      }
    }
  }

  private static Vector3 BackgroundColor(Ray ray) {
    var dir = Vector3.Normalize(ray.Direction);
    var t = 0.5f * dir.Y + 0.5f;
    return Vector3.Lerp(new Vector3(1), new Vector3(0.5f, 0.7f, 1.0f), t);
  }

  private Vector3 Color(Random random, World world, Ray ray, int bounces) {
    // If we've exceeded the ray bounce limit, no more light is gathered.
    if (bounces <= 0) return Vector3.Zero;

    var rec = new HitRecord();
    var selfIntersectionBias = 0.001f;
    if (world.Hit(ray, new Range(selfIntersectionBias, float.MaxValue), ref rec)) {
      Vector3 attenuation;
      Ray scattered;
      if (rec.Material.Scatter(random, ray, rec, out attenuation, out scattered)) {
        return attenuation * Color(random, world, scattered, bounces - 1);
      }
      return Vector3.Zero;
    }

    return BackgroundColor(ray);
  }
}
