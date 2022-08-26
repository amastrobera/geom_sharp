﻿// internal
using GeomSharp;

// external
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeomSharpTests {
  /// <summary>
  /// tests for all the MathNet.Spatial.Euclidean functions I created
  /// </summary>
  [TestClass]
  public class IntersectionExtensions2DTests {
    // Line, LineSegment, Ray cross test
    [RepeatedTestMethod(1)]
    public void LineToRay() {
      Assert.IsTrue(false);
    }

    [RepeatedTestMethod(1)]
    public void LineToLineSegment() {
      Assert.IsTrue(false);
    }

    [RepeatedTestMethod(1)]
    public void LineSegmentToRay() {
      Assert.IsTrue(false);
    }

    // Triangle with other basic primitives

    [RepeatedTestMethod(1)]
    public void TriangleToLine() {
      Assert.IsTrue(false);
    }

    [RepeatedTestMethod(100)]
    public void TriangleToRay() {
      Assert.IsTrue(false);
    }

    [RepeatedTestMethod(1)]
    public void TriangleToLineSegment() {
      Assert.IsTrue(false);
    }
  }
}
