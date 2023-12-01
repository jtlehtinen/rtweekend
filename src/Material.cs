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

public record Dielectric(float RefractiveIndex) : IMaterial {
  private static Vector3 Refract(Vector3 incident, Vector3 normal, float refractiveIndexRatio) {
    var cosTheta = Math.Min(Vector3.Dot(-incident, normal), 1.0f);
    var outPerp = refractiveIndexRatio * (incident + cosTheta * normal);
    var outParallel = -MathF.Sqrt(MathF.Abs(1.0f - outPerp.LengthSquared())) * normal;
    return outPerp + outParallel;
  }

  public bool Scatter(Random random, Ray ray, HitRecord rec, out Vector3 attenuation, out Ray scattered) {
    attenuation = Vector3.One;

    // Ratio of the indices of refraction. The refractive index of the medium
    // into which the incident ray is going into is at the bottom of the ratio.
    var refractiveIndexRatio = rec.FrontFaceHit ? 1.0f / RefractiveIndex : RefractiveIndex;

    var unitDirection = Vector3.Normalize(ray.Direction);
    var normal = rec.N;

    var refracted = Refract(unitDirection, normal, refractiveIndexRatio);
    scattered = new Ray(rec.P, refracted);

    return true;
  }
}
