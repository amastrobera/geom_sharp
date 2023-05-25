# geom_sharp

GeomSharp is a 2D (and 3D) geometry library for C#, based on .Net Framework 4.8


Notice: many things are still work in progress. The status is: all tested and developed up to triangles 3D, next I will add functions involving polygons (2D and 3D) and - if all goes well - for polyhedrons. 

## Definition

**Containment**: one shape is completely contained inside another shape (including its borders). 

**Intersection**: one shape crosses another, splitting it into two shapes.
In 2D, two lines crossing each other, or having one point in common without being parallel.
In 3D, the lines (or shapes) do not belong to the same plane, and cross each other. 

**Overlap**: one shape shares a portion of its area with another.
In 2D, two lines are parallel to each other, and have at least one point in common.
In 3D, the two shapes are on the same plane, and one shape's surface lays on top of another - part of it (overlap) or completely (equality or containment).
For triangles and polygons, the Overlap also include cases of adjacency (one+ edge in common) and touch (one point lying over the other shape's border).

**Touch**: one shape merely touches the border of another, in one point. 

**Adjacency**: one shape has a common portion (an edge) with another. 



## Data Structures
- Point
- Set of Points
- Vector
- Line
- Ray
- (Line)Segment
- Set of (Line)Segment
- Triangle
- Polyline
- Polygon
- GeometryCollection
... in 2D or 3D 
- (TODO) Surface
- (TODO) Polyhedron

- Plane
- Rotation, Shift
- (TODO) Transformation of Coordinates


