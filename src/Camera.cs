using System;
using System.Numerics;

namespace RTWeekend;

public class Camera {
  private Vector3 origin;
  private Vector3 horizontalAxis;
  private Vector3 verticalAxis;
  private Vector3 lowerLeftCorner;

  public Camera(Vector3 eye, Vector3 target, Vector3 up, float vfov, float aspectRatio) {
    var vfovRadians = vfov * MathF.PI / 180.0f;
    var viewportHeight = 2.0f * MathF.Tan(vfovRadians / 2.0f);
    var viewportWidth = aspectRatio * viewportHeight;

    var forward = Vector3.Normalize(target - eye);
    var right = Vector3.Normalize(Vector3.Cross(forward, up));
    up = Vector3.Normalize(Vector3.Cross(right, forward));

    origin = eye;
    horizontalAxis = viewportWidth * right;
    verticalAxis = viewportHeight * up;
    lowerLeftCorner = origin + forward - 0.5f * horizontalAxis - 0.5f * verticalAxis;
  }

  public Ray GetRay(float u, float v)
    => new(origin, lowerLeftCorner + u * horizontalAxis + v * verticalAxis - origin);
}
