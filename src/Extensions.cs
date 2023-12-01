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
}
