// internal
using GeomSharp;

// external
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GeomSharpTests {

  [TestClass]
  public class Triangle2DTests {
    [RepeatedTestMethod(100)]
    public void Contructor() {
      int precision = RandomGenerator.MakeInt(3, 9);

      var random_triangle = RandomGenerator.MakeTriangle2D(decimal_precision: precision);
      var t = random_triangle.Triangle;
      (var p0, var p1, var p2) = (random_triangle.p0, random_triangle.p1, random_triangle.p2);

      if (t is null) {
        return;
      }
      // Console.WriteLine("t = " + t.ToWkt(precision));

      // vectors and points
      Assert.IsTrue(t.P0.AlmostEquals(p0, precision));
      Assert.IsTrue(t.P1.AlmostEquals(p1, precision));
      Assert.IsTrue(t.P2.AlmostEquals(p2, precision));

      Assert.IsTrue(t.U.AlmostEquals((p1 - p0).Normalize(), precision));
      Assert.IsTrue(t.V.AlmostEquals((p2 - p0).Normalize(), precision));

      // center of mass
      Assert.IsTrue(
          t.CenterOfMass().AlmostEquals(new Point2D((t.P0.U + t.P1.U + t.P2.U) / 3.0, (t.P0.V + t.P1.V + t.P2.V) / 3.0),
                                        precision));
    }

    [RepeatedTestMethod(100)]
    public void Containment() {
      int precision = RandomGenerator.MakeInt(3, 9);

      // 2D
      var t = RandomGenerator.MakeTriangle2D(decimal_precision: precision).Triangle;

      // Console.WriteLine("t = " + t.ToWkt(precision));
      if (t is null) {
        return;
      }

      // temporary data
      var cm = t.CenterOfMass();
      var mid01 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      var mid12 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      var mid02 = Point2D.FromVector((t.P0.ToVector() + t.P2.ToVector()) / 2.0);
      // results and test data
      Point2D p;

      p = cm;
      Assert.IsTrue(t.Contains(p, precision),
                    "inner (center of mass), \n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = mid01;
      Assert.IsTrue(t.Contains(p, precision),
                    "border 1" + "\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = mid12;
      Assert.IsTrue(t.Contains(p, precision),
                    "border 2" + "\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = mid02;
      Assert.IsTrue(t.Contains(p, precision),
                    "border 3" + "\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = t.P0;
      Assert.IsTrue(t.Contains(p, precision),
                    "corner 1" + "\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = t.P1;
      Assert.IsTrue(t.Contains(p, precision),
                    "corner 2" + "\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = t.P2;
      Assert.IsTrue(t.Contains(p, precision),
                    "corner 3" + "\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = mid01 + 2 * (mid01 - cm);
      Assert.IsFalse(t.Contains(p, precision),
                     "outer point 1" + "\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = mid12 + 2 * (mid12 - cm);
      Assert.IsFalse(t.Contains(p, precision),
                     "outer point 2" + "\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = mid02 + 2 * (mid02 - cm);
      Assert.IsFalse(t.Contains(p, precision),
                     "outer point 3" + "\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = cm + 2 * (t.P0 - cm);
      Assert.IsFalse(t.Contains(p, precision),
                     "point above" + "\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = cm - 2 * (t.P0 - cm);
      Assert.IsFalse(t.Contains(p, precision),
                     "point below" + "\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void Intersection() {
      int precision = RandomGenerator.MakeInt(3, 9);

      // 2D
      var t1 = RandomGenerator.MakeTriangle2D(decimal_precision: precision).Triangle;

      // Console.WriteLine("t = " + t.ToWkt(precision));
      if (t1 is null) {
        return;
      }

      // temporary data
      var cm = t1.CenterOfMass();
      var mid01 = Point2D.FromVector((t1.P0.ToVector() + t1.P1.ToVector()) / 2.0);
      var mid12 = Point2D.FromVector((t1.P1.ToVector() + t1.P2.ToVector()) / 2.0);
      var mid20 = Point2D.FromVector((t1.P2.ToVector() + t1.P0.ToVector()) / 2.0);
      // results and test data
      Triangle2D t2;
      IntersectionResult res;

      // intersects (two points inside, one point outside) -> triangle intersection
      t2 = Triangle2D.FromPoints(t1.P0, t1.P1, cm + 2 * (mid12 - cm), precision);
      Assert.IsTrue(t1.Intersects(t2, precision),
                    "intersects (two points inside, one point outside)\n\tt1=" + t1.ToWkt(precision) +
                        "\n\tt2=" + t2.ToWkt(precision));
      res = t1.Intersection(t2, precision);
      Assert.IsTrue(res.ValueType == typeof(Triangle2D),
                    "intersects (sharing an edge) (type)\n\tt1=" + t1.ToWkt(precision) +
                        "\n\tt2=" + t2.ToWkt(precision) + "\n\ttype=" + res);

      t2 = Triangle2D.FromPoints(t1.P1, t1.P2, cm + 2 * (mid20 - cm), precision);
      Assert.IsTrue(t1.Intersects(t2, precision),
                    "intersects (two points inside, one point outside)\n\tt1=" + t1.ToWkt(precision) +
                        "\n\tt2=" + t2.ToWkt(precision));
      res = t1.Intersection(t2, precision);
      Assert.IsTrue(res.ValueType == typeof(Triangle2D),
                    "intersects (sharing an edge) (type)\n\tt1=" + t1.ToWkt(precision) +
                        "\n\tt2=" + t2.ToWkt(precision) + "\n\ttype=" + res);

      t2 = Triangle2D.FromPoints(t1.P2, t1.P0, cm + 2 * (mid01 - cm), precision);
      Assert.IsTrue(t1.Intersects(t2, precision),
                    "intersects (two points inside, one point outside)\n\tt1=" + t1.ToWkt(precision) +
                        "\n\tt2=" + t2.ToWkt(precision));
      res = t1.Intersection(t2, precision);
      Assert.IsTrue(res.ValueType == typeof(Triangle2D),
                    "intersects (sharing an edge) (type)\n\tt1=" + t1.ToWkt(precision) +
                        "\n\tt2=" + t2.ToWkt(precision) + "\n\ttype=" + res);

      // intersects (sharing an edge) -> triangle intersection
      t2 = Triangle2D.FromPoints(mid12, mid12 + (t1.P2 - t1.P1), t1.P0 + (t1.P2 - t1.P1), precision);
      Assert.IsTrue(t1.Intersects(t2, precision),
                    "intersects (sharing an edge)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));
      res = t1.Intersection(t2, precision);
      Assert.IsTrue(res.ValueType == typeof(Triangle2D),
                    "intersects (sharing an edge) (type)\n\tt1=" + t1.ToWkt(precision) +
                        "\n\tt2=" + t2.ToWkt(precision) + "\n\ttype=" + res);

      t2 = Triangle2D.FromPoints(mid20, mid20 + (t1.P0 - t1.P2), t1.P1 + (t1.P0 - t1.P2), precision);
      Assert.IsTrue(t1.Intersects(t2, precision),
                    "intersects (sharing an edge)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));
      res = t1.Intersection(t2, precision);
      Assert.IsTrue(res.ValueType == typeof(Triangle2D),
                    "intersects (sharing an edge) (type)\n\tt1=" + t1.ToWkt(precision) +
                        "\n\tt2=" + t2.ToWkt(precision) + "\n\ttype=" + res);

      t2 = Triangle2D.FromPoints(mid01, mid01 + (t1.P1 - t1.P0), t1.P2 + (t1.P1 - t1.P0), precision);
      Assert.IsTrue(t1.Intersects(t2, precision),
                    "intersects (sharing an edge)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));
      res = t1.Intersection(t2, precision);
      Assert.IsTrue(res.ValueType == typeof(Triangle2D),
                    "intersects (sharing an edge) (type)\n\tt1=" + t1.ToWkt(precision) +
                        "\n\tt2=" + t2.ToWkt(precision) + "\n\ttype=" + res);

      // star of David (no vertex contained) -> hexagon intersection
      t2 = Triangle2D.FromPoints(mid20 + 0.5 * (mid20 - mid12),
                                 mid01 + 0.5 * (mid01 - mid12),
                                 mid12 + 0.5 * (mid12 - cm),
                                 precision);
      Assert.IsTrue(t1.Intersects(t2, precision),
                    "star of David\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));
      res = t1.Intersection(t2, precision);
      Assert.IsTrue(
          res.ValueType == typeof(Polygon2D),
          "star of David (type)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision) + "\n\ttype=" + res);
      Assert.IsTrue(
          ((Polygon2D)t1.Intersection(t2, precision).Value).Size == 6,
          "star of David (size)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision) + "\n\ttype=" + res);

      // overlap (mids) -> no intersection
      t2 = Triangle2D.FromPoints(mid01, mid12, mid20, precision);
      Assert.IsFalse(t1.Intersects(t2, precision),
                     "overlap (mids)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      // hourglass (touch) -> no intersection
      t2 = Triangle2D.FromPoints(t1.P0, t1.P0 - (t1.P1 - t1.P0), t1.P0 - (t1.P2 - t1.P0), precision);
      Console.WriteLine("hourglass\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));
      Assert.IsFalse(t1.Intersects(t2, precision),
                     "hourglass (touch)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      // touching mids (touch) -> no intersection
      t2 = Triangle2D.FromPoints(mid01, mid01 - (t1.P2 - t1.P1), mid01 - (t1.P2 - t1.P0), precision);
      Assert.IsFalse(t1.Intersects(t2, precision),
                     "double arrow (touch)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      // outsiders -> no intersection
      t2 = Triangle2D.FromPoints(mid01 - (t1.P2 - t1.P1),
                                 mid01 - 2 * (t1.P2 - t1.P1),
                                 mid01 - 2 * (t1.P2 - t1.P0),
                                 precision);
      Assert.IsFalse(t1.Intersects(t2, precision),
                     "outsiders\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void Overlap() {
      int precision = RandomGenerator.MakeInt(3, 9);

      // 2D
      var t1 = RandomGenerator.MakeTriangle2D(decimal_precision: precision).Triangle;

      // Console.WriteLine("t = " + t.ToWkt(precision));
      if (t1 is null) {
        return;
      }

      // temporary data
      var cm = t1.CenterOfMass();
      var mid01 = Point2D.FromVector((t1.P0.ToVector() + t1.P1.ToVector()) / 2.0);
      var mid12 = Point2D.FromVector((t1.P1.ToVector() + t1.P2.ToVector()) / 2.0);
      var mid20 = Point2D.FromVector((t1.P2.ToVector() + t1.P0.ToVector()) / 2.0);
      // results and test data
      Triangle2D t2;
      Vector2D U;

      // overlap (same vertices)
      t2 = Triangle2D.FromPoints(t1.P0, t1.P1, t1.P2, precision);
      Assert.IsTrue(t1.Overlaps(t2, precision),
                    "overlap (same vertices)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      // overlap (contained)
      t2 = Triangle2D.FromPoints(t1.P0, t1.P1, cm, precision);
      Assert.IsTrue(t1.Overlaps(t2, precision),
                    "overlap (contained)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      t2 = Triangle2D.FromPoints(t1.P1, t1.P2, cm, precision);
      Assert.IsTrue(t1.Overlaps(t2, precision),
                    "overlap (contained)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      t2 = Triangle2D.FromPoints(t1.P2, t1.P0, cm, precision);
      Assert.IsTrue(t1.Overlaps(t2, precision),
                    "overlap (contained)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      t2 = Triangle2D.FromPoints(mid01, mid12, cm, precision);
      Assert.IsTrue(t1.Overlaps(t2, precision),
                    "overlap (contained)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      t2 = Triangle2D.FromPoints(mid12, mid20, cm, precision);
      Assert.IsTrue(t1.Overlaps(t2, precision),
                    "overlap (contained)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      t2 = Triangle2D.FromPoints(mid20, mid01, cm, precision);
      Assert.IsTrue(t1.Overlaps(t2, precision),
                    "overlap (contained)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      // overlap (vertices are mids)
      t2 = Triangle2D.FromPoints(mid01, mid12, mid20, precision);
      Assert.IsTrue(t1.Overlaps(t2, precision),
                    "overlap (vertices are mids)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      // overlap (common edge + mid)
      t2 = Triangle2D.FromPoints(t1.P0, t1.P1, mid12, precision);
      Assert.IsTrue(t1.Overlaps(t2, precision),
                    "overlap (common edge + mid)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      t2 = Triangle2D.FromPoints(t1.P1, t1.P2, mid20, precision);
      Assert.IsTrue(t1.Overlaps(t2, precision),
                    "overlap (common edge + mid)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      t2 = Triangle2D.FromPoints(t1.P2, t1.P0, mid01, precision);
      Assert.IsTrue(t1.Overlaps(t2, precision),
                    "overlap (common edge + mid)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      // intersect (common edge, outside)
      U = mid12 - mid01;
      t2 = Triangle2D.FromPoints(t1.P0, t1.P1, mid12 + U, precision);
      Assert.IsFalse(t1.Overlaps(t2, precision),
                     "intersect (common edge, outside)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      U = mid20 - mid12;
      t2 = Triangle2D.FromPoints(t1.P1, t1.P2, mid20 + U, precision);
      Assert.IsFalse(t1.Overlaps(t2, precision),
                     "intersect (common edge, outside)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      U = mid01 - mid20;
      t2 = Triangle2D.FromPoints(t1.P2, t1.P0, mid01 + U, precision);
      Assert.IsFalse(t1.Overlaps(t2, precision),
                     "intersect (common edge, outside)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void OnPerimeter() {
      int precision = RandomGenerator.MakeInt(3, 9);

      // 2D
      var t = RandomGenerator.MakeTriangle2D(decimal_precision: precision).Triangle;

      // Console.WriteLine("t = " + t.ToWkt(precision));
      if (t is null) {
        return;
      }

      // temporary data
      var cm = t.CenterOfMass();
      var mid01 = Point2D.FromVector((t.P0.ToVector() + t.P1.ToVector()) / 2.0);
      var mid12 = Point2D.FromVector((t.P1.ToVector() + t.P2.ToVector()) / 2.0);
      var mid20 = Point2D.FromVector((t.P2.ToVector() + t.P0.ToVector()) / 2.0);
      // results and test data
      Point2D p;

      // inner point (false)
      p = cm;
      Assert.IsFalse(t.IsOnPerimeter(p, precision),
                     "inner (center of mass)\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      // perimeter points (mid)
      p = mid01;
      Assert.IsTrue(t.IsOnPerimeter(p, precision),
                    "perimeter points (mid)\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = mid12;
      Assert.IsTrue(t.IsOnPerimeter(p, precision),
                    "perimeter points (mid)\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = mid20;
      Assert.IsTrue(t.IsOnPerimeter(p, precision),
                    "perimeter points (mid)\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      // perimeter points (vertices)
      p = t.P0;
      Assert.IsTrue(t.IsOnPerimeter(p, precision),
                    "perimeter points (vertices)\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = t.P1;
      Assert.IsTrue(t.IsOnPerimeter(p, precision),
                    "perimeter points (vertices)\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      p = t.P2;
      Assert.IsTrue(t.IsOnPerimeter(p, precision),
                    "perimeter points (vertices)\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      // outer points
      Vector2D U;

      U = mid01 - cm;
      p = mid01 + U;
      Assert.IsFalse(t.IsOnPerimeter(p, precision),
                     "outer points (mids)\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      U = mid12 - cm;
      p = mid12 + U;
      Assert.IsFalse(t.IsOnPerimeter(p, precision),
                     "outer points (mids)\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));

      U = mid20 - cm;
      p = mid20 + U;
      Assert.IsFalse(t.IsOnPerimeter(p, precision),
                     "outer points (mids)\n\tt=" + t.ToWkt(precision) + "\n\tp=" + p.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void Touch() {
      int precision = RandomGenerator.MakeInt(3, 9);

      // 2D
      var t1 = RandomGenerator.MakeTriangle2D(decimal_precision: precision).Triangle;

      // Console.WriteLine("t = " + t.ToWkt(precision));
      if (t1 is null) {
        return;
      }

      // temporary data
      var cm = t1.CenterOfMass();
      var mid01 = Point2D.FromVector((t1.P0.ToVector() + t1.P1.ToVector()) / 2.0);
      var mid12 = Point2D.FromVector((t1.P1.ToVector() + t1.P2.ToVector()) / 2.0);
      var mid20 = Point2D.FromVector((t1.P2.ToVector() + t1.P0.ToVector()) / 2.0);

      // results and test data
      Point2D p0, p1, p2;
      Vector2D U, V;
      Triangle2D t2;

      // hourglass
      p0 = t1.P0;
      U = -(t1.P1 - t1.P0);
      V = -(t1.P2 - t1.P0);
      p1 = p0 + U;
      p2 = p0 + V;
      t2 = Triangle2D.FromPoints(p0, p1, p2, precision);
      Assert.IsTrue(t1.Touches(t2, precision),
                    "hourglass\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      p0 = t1.P1;
      U = -(t1.P2 - t1.P1);
      V = -(t1.P0 - t1.P1);
      p1 = p0 + U;
      p2 = p0 + V;
      t2 = Triangle2D.FromPoints(p0, p1, p2, precision);
      Assert.IsTrue(t1.Touches(t2, precision),
                    "hourglass\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      p0 = t1.P2;
      U = -(t1.P0 - t1.P2);
      V = -(t1.P1 - t1.P2);
      p1 = p0 + U;
      p2 = p0 + V;
      t2 = Triangle2D.FromPoints(p0, p1, p2, precision);
      Assert.IsTrue(t1.Touches(t2, precision),
                    "hourglass\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      // touching mids
      p0 = mid01;
      U = -(t1.P2 - t1.P1);
      V = -(t1.P2 - t1.P0);
      p1 = p0 + U;
      p2 = p0 + V;
      t2 = Triangle2D.FromPoints(p0, p1, p2, precision);
      Assert.IsTrue(t1.Touches(t2, precision),
                    "double arrow\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      p0 = mid12;
      U = -(t1.P0 - t1.P2);
      V = -(t1.P0 - t1.P1);
      p1 = p0 + U;
      p2 = p0 + V;
      t2 = Triangle2D.FromPoints(p0, p1, p2, precision);
      Assert.IsTrue(t1.Touches(t2, precision),
                    "double arrow\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      p0 = mid20;
      U = -(t1.P1 - t1.P0);
      V = -(t1.P1 - t1.P2);
      p1 = p0 + U;
      p2 = p0 + V;
      t2 = Triangle2D.FromPoints(p0, p1, p2, precision);
      Assert.IsTrue(t1.Touches(t2, precision),
                    "double arrow\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      // external
      U = -(t1.P2 - t1.P1);
      V = -(t1.P2 - t1.P0);
      p0 = mid01 + U;
      p1 = p0 + U;
      p2 = p0 + V;
      t2 = Triangle2D.FromPoints(p0, p1, p2, precision);
      Assert.IsFalse(t1.Touches(t2, precision),
                     "external\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      // intersecting
      p0 = cm;
      U = -(t1.P2 - t1.P1);
      V = -(t1.P2 - t1.P0);
      p1 = p0 + U;
      p2 = p0 + V;
      t2 = Triangle2D.FromPoints(p0, p1, p2, precision);
      Assert.IsFalse(t1.Touches(t2, precision),
                     "intersecting\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));
    }

    [RepeatedTestMethod(100)]
    public void Adjacency() {
      int precision = RandomGenerator.MakeInt(3, 9);

      // 2D
      var t1 = RandomGenerator.MakeTriangle2D(decimal_precision: precision).Triangle;

      // Console.WriteLine("t = " + t.ToWkt(precision));
      if (t1 is null) {
        return;
      }

      // temporary data
      var cm = t1.CenterOfMass();
      var mid01 = Point2D.FromVector((t1.P0.ToVector() + t1.P1.ToVector()) / 2.0);
      var mid12 = Point2D.FromVector((t1.P1.ToVector() + t1.P2.ToVector()) / 2.0);
      var mid20 = Point2D.FromVector((t1.P2.ToVector() + t1.P0.ToVector()) / 2.0);
      // results and test data
      Triangle2D t2;
      IntersectionResult res;
      Point2D p0, p1, p2;

      // adjacent (same side)
      p0 = t1.P0;
      p1 = t1.P1;
      p2 = mid01 + (mid01 - cm);
      t2 = Triangle2D.FromPoints(p0, p1, p2, precision);
      Assert.IsTrue(t1.IsAdjacent(t2, precision),
                    "adjacent (same side)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));
      res = t1.AdjacentSide(t2, precision);
      Assert.IsTrue(
          ((LineSegment2D)res.Value).IsSameSegment(LineSegment2D.FromPoints(p0, p1, precision), precision),
          "adjacent (same side)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision) + "\n\ttype=" + res);
      Assert.IsTrue(
          ((LineSegment2D)res.Value).IsSameSegment(LineSegment2D.FromPoints(p1, p0, precision), precision),
          "adjacent (same side)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision) + "\n\ttype=" + res);

      p0 = t1.P1;
      p1 = t1.P2;
      p2 = mid12 + (mid12 - cm);
      t2 = Triangle2D.FromPoints(p0, p1, p2, precision);
      Assert.IsTrue(t1.IsAdjacent(t2, precision),
                    "adjacent (same side)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));
      res = t1.AdjacentSide(t2, precision);
      Assert.IsTrue(
          ((LineSegment2D)res.Value).IsSameSegment(LineSegment2D.FromPoints(p0, p1, precision), precision),
          "adjacent (same side)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision) + "\n\ttype=" + res);

      p0 = t1.P2;
      p1 = t1.P0;
      p2 = mid20 + (mid20 - cm);
      t2 = Triangle2D.FromPoints(p0, p1, p2, precision);
      Assert.IsTrue(t1.IsAdjacent(t2, precision),
                    "adjacent (same side)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));
      res = t1.AdjacentSide(t2, precision);
      Assert.IsTrue(
          ((LineSegment2D)res.Value).IsSameSegment(LineSegment2D.FromPoints(p0, p1, precision), precision),
          "adjacent (same side)\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision) + "\n\ttype=" + res);

      // internal (one side in common)
      p0 = t1.P0;
      p1 = t1.P1;
      p2 = cm;
      t2 = Triangle2D.FromPoints(p0, p1, p2, precision);
      Assert.IsFalse(t1.IsAdjacent(t2, precision),
                     "internal (one side in common\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      p0 = t1.P1;
      p1 = t1.P2;
      p2 = cm;
      t2 = Triangle2D.FromPoints(p0, p1, p2, precision);
      Assert.IsFalse(t1.IsAdjacent(t2, precision),
                     "internal (one side in common\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));

      p0 = t1.P2;
      p1 = t1.P0;
      p2 = cm;
      t2 = Triangle2D.FromPoints(p0, p1, p2, precision);
      Assert.IsFalse(t1.IsAdjacent(t2, precision),
                     "internal (one side in common\n\tt1=" + t1.ToWkt(precision) + "\n\tt2=" + t2.ToWkt(precision));
    }
  }

}
