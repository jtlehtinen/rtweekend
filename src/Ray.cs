using System.Numerics;

namespace RTWeekend;

public record Ray(Vector3 Origin, Vector3 Direction) {
  public Vector3 At(float t) => Origin + t * Direction;
}
