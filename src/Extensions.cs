using System;
using System.Numerics;

namespace RTWeekend;

public static class RandomExtensions {
  public static Vector3 NextVector3(this Random random)
    => new (random.NextSingle(), random.NextSingle(), random.NextSingle());

  public static Vector3 NextInUnitSphere(this Random random) {
    for (;;) {
      var v = 2.0f * random.NextVector3() - Vector3.One;
      if (v.LengthSquared() < 1.0f) return v;
    }
  }

  public static Vector3 NextUnitVector3(this Random random) {
    var z = random.NextSingle() * 2.0f - 1.0f;
    var a = random.NextSingle() * 2.0f * MathF.PI;
    var r = MathF.Sqrt(1.0f - z * z);
    var x = MathF.Sin(a);
    var y = MathF.Cos(a);
    return new Vector3(r * x, r * y, z);
  }

  public static Vector3 NextInHemisphere(this Random random, Vector3 normal) {
    var v = random.NextUnitVector3();
    return Vector3.Dot(v, normal) > 0.0f ? v : -v;
  }

  public static Vector3 NextInUnitDisk(this Random random) {
    for (;;) {
      var v = 2.0f * new Vector3(random.NextSingle(), random.NextSingle(), 0) - new Vector3(1, 1, 0);
      if (v.LengthSquared() < 1.0f) return v;
    }
  }
}

public static class Vector3Extensions {
  const float epsilon = 1.0e-8f;

  public static bool IsNearZero(this Vector3 v)
    => MathF.Abs(v.X) < epsilon && MathF.Abs(v.Y) < epsilon && MathF.Abs(v.Z) < epsilon;
}

public static class FloatExtensions {
  public static float Pow2(this float x) => x * x;
  public static float Pow5(this float x) => x * x * x * x * x;
}
