using System;
using System.Collections.Generic;
using System.Numerics;

namespace RTWeekend;

public record struct Range(float Min, float Max) {
  public readonly bool Contains(float x) => Min <= x && x <= Max;
  public readonly bool Surrounds(float x) => Min < x && x < Max;
}

public struct HitRecord {
  public IMaterial Material;
  public Vector3 P;
  public Vector3 N;
  public float T;
  public bool FrontFaceHit;

  public void SetFaceNormal(Ray ray, Vector3 outwardNormal) {
    FrontFaceHit = Vector3.Dot(ray.Direction, outwardNormal) < 0.0f;
    N = FrontFaceHit ? outwardNormal : -outwardNormal;
  }
}

public record Sphere(Vector3 Center, float Radius, IMaterial Material) {
  public bool Hit(Ray ray, Range t, ref HitRecord rec) {
    Vector3 origin = ray.Origin - Center;
    float a = ray.Direction.LengthSquared();
    float halfB = Vector3.Dot(origin, ray.Direction);
    float c = origin.LengthSquared() - Radius * Radius;
    float discriminant = halfB * halfB - a * c;
    if (discriminant < 0.0f) return false;

    float sqrtd = MathF.Sqrt(discriminant);

    // Find the nearest root that lies in the acceptable range.
    float root = (-halfB - sqrtd) / a;
    if (!t.Surrounds(root)) {
      root = (-halfB + sqrtd) / a;
      if (!t.Surrounds(root))
        return false;
    }

    rec.T = root;
    rec.P = ray.At(root);
    var outwardNormal = (rec.P - Center) / Radius;
    rec.SetFaceNormal(ray, outwardNormal);
    rec.Material = Material;

    return true;
  }
}

public class World {
  private readonly List<Sphere> spheres = new();

  public void Add(Sphere sphere)
    => spheres.Add(sphere);

  public void Clear()
    => spheres.Clear();

  public bool Hit(Ray ray, Range t, ref HitRecord outRec) {
    var rec = new HitRecord();
    var hit = false;

    foreach (var sphere in spheres) {
      if (sphere.Hit(ray, t, ref rec)) {
        t.Max = rec.T;
        hit = true;
      }
    }

    if (hit) outRec = rec;

    return hit;
  }
}
