using System;
using System.Numerics;

namespace RTWeekend;

public interface IMaterial {
  bool Scatter(Random random, Ray ray, HitRecord rec, out Vector3 attenuation, out Ray scattered);
}

public record Lambertian(Vector3 Albedo) : IMaterial {
  public bool Scatter(Random random, Ray ray, HitRecord rec, out Vector3 attenuation, out Ray scattered) {
    //var scatterDirection = rec.N + random.NextInUnitSphere();
    //var scatterDirection = rec.N + random.NextUnitVector3();
    var scatterDirection = random.NextInHemisphere(rec.N);

    // Catch degenerate scatter direction
    if (scatterDirection.IsNearZero()) scatterDirection = rec.N;

    scattered = new Ray(rec.P, scatterDirection);
    attenuation = Albedo;
    return true;
  }
}

public record Metal(Vector3 Albedo, float Fuzz = 0.0f) : IMaterial {
  public bool Scatter(Random random, Ray ray, HitRecord rec, out Vector3 attenuation, out Ray scattered) {
    var reflected = Vector3.Reflect(Vector3.Normalize(ray.Direction), rec.N);
    scattered = new Ray(rec.P, reflected + Fuzz * random.NextInUnitSphere());
    attenuation = Albedo;
    return Vector3.Dot(scattered.Direction, rec.N) > 0.0f;
  }
}
