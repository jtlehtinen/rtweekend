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

  private static float Reflectance(float cosine, float refractiveIndexRatio) {
    // Use Schlick's approximation for reflectance.
    var r0 = ((1.0f - refractiveIndexRatio) / (1.0f + refractiveIndexRatio)).Pow2();
    return r0 + (1.0f - r0) * (1.0f - cosine).Pow5();
  }

  public bool Scatter(Random random, Ray ray, HitRecord rec, out Vector3 attenuation, out Ray scattered) {
    attenuation = Vector3.One;

    // Ratio of the indices of refraction. The refractive index of the medium
    // into which the incident ray is going into is at the bottom of the ratio.
    var refractiveIndexRatio = rec.FrontFaceHit ? 1.0f / RefractiveIndex : RefractiveIndex;

    var unitDirection = Vector3.Normalize(ray.Direction);
    float cosTheta = Math.Min(Vector3.Dot(-unitDirection, rec.N), 1.0f);
    float sinTheta = (float)Math.Sqrt(1.0f - cosTheta * cosTheta);

    bool mustReflect = refractiveIndexRatio * sinTheta > 1.0f;

    if (mustReflect || (Reflectance(cosTheta, refractiveIndexRatio) > random.NextSingle())) {
      var reflected = Vector3.Reflect(unitDirection, rec.N);
      scattered = new Ray(rec.P, reflected);
    } else {
      var refracted = Refract(unitDirection, rec.N, refractiveIndexRatio);
      scattered = new Ray(rec.P, refracted);
    }

    return true;
  }
}
