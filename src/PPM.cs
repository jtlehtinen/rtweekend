using System.IO;
using System.Numerics;
using System.Text;

namespace RTWeekend;

public static class PPM {
  /// <summary>
  /// Write writes the given image to a file in the PPM P3 format.
  /// </summary>
  public static void Write(string filename, Vector3[,] image) {
    // [PPM file format](http://netpbm.sourceforge.net/doc/ppm.html)
    int height = image.GetLength(0);
    int width = image.GetLength(1);

    var encoding = new UTF8Encoding(false);
    using var stream = new StreamWriter(filename, false, encoding);
    stream.Write($"P3\n{width} {height}\n255\n");
    for (int y = height - 1; y >= 0; --y) {
      for (int x = 0; x < width; ++x) {
        int r = (int)(255.99f * image[y, x].X);
        int g = (int)(255.99f * image[y, x].Y);
        int b = (int)(255.99f * image[y, x].Z);
        stream.Write($"{r} {g} {b}\n");
      }
    }
  }
}
