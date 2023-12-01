using System;
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
        int r = Math.Clamp((int)(255.99f * image[y, x].X), 0, 255);
        int g = Math.Clamp((int)(255.99f * image[y, x].Y), 0, 255);
        int b = Math.Clamp((int)(255.99f * image[y, x].Z), 0, 255);
        stream.Write($"{r} {g} {b}\n");
      }
    }
  }
}
