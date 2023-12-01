using System;
using System.Numerics;

namespace RTWeekend;

public class Camera {
  private Vector3 origin;
  private Vector3 horizontalAxis;
  private Vector3 verticalAxis;
  private Vector3 lowerLeftCorner;
  private Vector3 forward;
  private Vector3 right;
  private Vector3 up;
  private readonly float lensRadius;

  public Camera(Vector3 eye, Vector3 target, Vector3 up, float vfov, float aspectRatio, float aperture, float focusDistance) {
    var vfovRadians = vfov * MathF.PI / 180.0f;
    var viewportHeight = 2.0f * MathF.Tan(vfovRadians / 2.0f);
    var viewportWidth = aspectRatio * viewportHeight;

    this.forward = Vector3.Normalize(target - eye);
    this.right = Vector3.Normalize(Vector3.Cross(forward, up));
    this.up = Vector3.Normalize(Vector3.Cross(right, forward));

    this.origin = eye;
    this.horizontalAxis = focusDistance * viewportWidth * right;
    this.verticalAxis = focusDistance * viewportHeight * up;
    this.lowerLeftCorner = this.origin + focusDistance * forward - 0.5f * horizontalAxis - 0.5f * verticalAxis;

    this.lensRadius = aperture / 2.0f;
  }

  public Ray GetRay(Random random, float u, float v) {
    var rd = lensRadius * random.NextInUnitDisk();
    var offset = rd.X * right + rd.Y * up;

    return new Ray(
      origin + offset,
      lowerLeftCorner + u * horizontalAxis + v * verticalAxis - origin - offset
    );
  }
}
