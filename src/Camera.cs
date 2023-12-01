using System.Numerics;

namespace RTWeekend;

public class Camera {
  private Vector3 origin;
  private Vector3 horizontalAxis;
  private Vector3 verticalAxis;
  private Vector3 lowerLeftCorner;

  public Camera(float aspectRatio, float focalLength) {
    var viewportHeight = 2.0f;
    var viewportWidth = aspectRatio * viewportHeight;

    origin = Vector3.Zero;
    horizontalAxis = new Vector3(viewportWidth, 0.0f, 0.0f);
    verticalAxis = new Vector3(0.0f, viewportHeight, 0.0f);
    lowerLeftCorner = origin - 0.5f * horizontalAxis - 0.5f * verticalAxis - new Vector3(0.0f, 0.0f, focalLength);
  }

  public Ray GetRay(float u, float v)
    => new(origin, lowerLeftCorner + u * horizontalAxis + v * verticalAxis - origin);
}
