# geom_sharp

GeomSharp is a 2D (and 3D) geometry library for C#, based on .Net Framework 4.8

Legend: :heavy_check_mark: done, :arrow_forward: in progress, :x: not done, :test_tube: tested


## Data Structures

| 2D | 3D | Utils | 
| - | - | - | 
| :heavy_check_mark: Point | :heavy_check_mark: Point | :heavy_check_mark: Angle | 
| :heavy_check_mark: Vector | :heavy_check_mark: Vector | :x: Rotation | 
| :heavy_check_mark: Line | :heavy_check_mark: Line | :x: Translation | 
| :heavy_check_mark: Ray | :heavy_check_mark: Ray | :x: Transformation | 
| :heavy_check_mark: LineSegment | :heavy_check_mark: LineSegment |  | 
| :x: Polyline | :x: Polyline |  | 
| :arrow_forward: Triangle | :arrow_forward: Triangle |  | 
| :x: Polygon | :x: Polygon |  | 
| | :heavy_check_mark: Plane |  | 
| | :x: Polyhedron |  | 



## Algorithms

| Category | SubCategory | Classes Involved |
| - | - | - |
| 2D | |
| | Containment | | 
| | | :heavy_check_mark: :test_tube: Line/Point| 
| | | :heavy_check_mark: :test_tube: Ray/Point | 
| | | :heavy_check_mark: :test_tube: Segment/Point | 
| | | :heavy_check_mark: Triangle/Point | 
| | | :x: Polyline/Point | 
| | | :x: Polygon/Point | 
| | Intersection | |
| | | :heavy_check_mark: :test_tube: Line/Line| 
| | | :heavy_check_mark: :test_tube: Line/Ray | 
| | | :heavy_check_mark: :test_tube:Line/Segment | 
| | | :heavy_check_mark: :test_tube: Ray/Ray | 
| | | :heavy_check_mark: :test_tube: Ray/Segment | 
| | | :heavy_check_mark: :test_tube: Segment/Segment | 
| | | :heavy_check_mark: Triangle/Triangle | 
| | | :heavy_check_mark: :test_tube: Triangle/Line | 
| | | :heavy_check_mark: :test_tube: Triangle/Ray | 
| | | :heavy_check_mark: :test_tube: Triangle/Segment | 
| | | :x: Polyline/Line | 
| | | :x: Polyline/Ray | 
| | | :x: Polyline/Segment | 
| | | :x: Polyline/Polyline |
| | | :x: Polyline/Triangle |
| | | :x: Polygon/Line | 
| | | :x: Polygon/Ray | 
| | | :x: Polygon/Segment | 
| | | :x: Polygon/Polyline | 
| | | :x: Polygon/Polygon | 
| | | :x: Polygon/Triangle | 
| | Overlap | |
| | | :heavy_check_mark: :test_tube: Line/Line| 
| | | :heavy_check_mark: :test_tube: Line/Ray | 
| | | :heavy_check_mark: :test_tube: Line/Segment | 
| | | :heavy_check_mark: :test_tube: Ray/Ray | 
| | | :heavy_check_mark: :test_tube: Ray/Segment | 
| | | :heavy_check_mark: :test_tube: Segment/Segment | 
| | | :heavy_check_mark: Triangle/Triangle | 
| | | :heavy_check_mark: Triangle/Line | 
| | | :heavy_check_mark: Triangle/Ray | 
| | | :heavy_check_mark: Triangle/Segment | 
| | | :x: Polyline/Line | 
| | | :x: Polyline/Ray | 
| | | :x: Polyline/Segment | 
| | | :x: Polyline/Polyline |
| | | :x: Polyline/Triangle |
| | | :x: Polygon/Line | 
| | | :x: Polygon/Ray | 
| | | :x: Polygon/Segment | 
| | | :x: Polygon/Polyline | 
| | | :x: Polygon/Polygon | 
| | | :x: Polygon/Triangle | 
| 3D | | |
| | Containment | |
| | | :heavy_check_mark: :test_tube: Line/Point| 
| | | :heavy_check_mark: :test_tube: Ray/Point | 
| | | :heavy_check_mark: :test_tube: Segment/Point | 
| | | :heavy_check_mark: :test_tube: Plane/Point | 
| | | :heavy_check_mark: :test_tube: Plane/Segment | 
| | | :heavy_check_mark: :test_tube: Triangle/Point | 
| | | :x: Polyline/Point | 
| | | :x: Polygon/Point | 
| | Projection | |
| | | :heavy_check_mark: :test_tube: ProjectOnto (Point 3D -> 3D) | 
| | | :heavy_check_mark: :test_tube: VerticalProjectOnto (Point 3D -> 3D) | 
| | | :heavy_check_mark: :test_tube: Evaluate (Point 3D -> 3D along an axis) | 
| | | :heavy_check_mark: :test_tube: ProjectInto (Point 3D -> 2D) | 
| | Intersection | |
| | | :heavy_check_mark: :test_tube: Line/Line| 
| | | :heavy_check_mark: :test_tube: Line/Ray | 
| | | :heavy_check_mark: :test_tube: Line/Segment | 
| | | :heavy_check_mark: :test_tube: Ray/Ray | 
| | | :heavy_check_mark: :test_tube: Ray/Segment | 
| | | :heavy_check_mark: :test_tube: Segment/Segment | 
| | | :heavy_check_mark: :test_tube: Plane/Plane | 
| | | :heavy_check_mark: :test_tube: Plane/Line | 
| | | :heavy_check_mark: :test_tube: Plane/Ray | 
| | | :heavy_check_mark: :test_tube: Plane/Segment | 
| | | :heavy_check_mark: Triangle/Triangle | 
| | | :heavy_check_mark: Triangle/Line | 
| | | :heavy_check_mark: Triangle/Ray | 
| | | :heavy_check_mark: Triangle/Segment | 
| | | :x: Polyline/Line | 
| | | :x: Polyline/Ray | 
| | | :x: Polyline/Segment | 
| | | :x: Polyline/Polyline |
| | | :x: Polyline/Triangle |
| | | :x: Polygon/Line | 
| | | :x: Polygon/Ray | 
| | | :x: Polygon/Segment | 
| | | :x: Polygon/Polyline | 
| | | :x: Polygon/Polygon | 
| | | :x: Polygon/Triangle | 
| | Overlap | |
| | | :heavy_check_mark: :test_tube: Line/Line| 
| | | :heavy_check_mark: :test_tube: Line/Ray | 
| | | :heavy_check_mark: :test_tube: Line/Segment | 
| | | :heavy_check_mark: :test_tube: Ray/Ray | 
| | | :heavy_check_mark: :test_tube: Ray/Segment | 
| | | :heavy_check_mark: :test_tube: Segment/Segment | 
| | | :heavy_check_mark: :test_tube: Plane/Plane | 
| | | :heavy_check_mark: :test_tube: Plane/Line | 
| | | :heavy_check_mark: :test_tube: Plane/Ray | 
| | | :heavy_check_mark: :test_tube: Plane/Segment | 
| | | :heavy_check_mark: Triangle/Triangle | 
| | | :heavy_check_mark: Triangle/Line | 
| | | :heavy_check_mark: Triangle/Ray | 
| | | :heavy_check_mark: Triangle/Segment | 
| | | :x: Polyline/Line | 
| | | :x: Polyline/Ray | 
| | | :x: Polyline/Segment | 
| | | :x: Polyline/Polyline |
| | | :x: Polyline/Triangle |
| | | :x: Polygon/Line | 
| | | :x: Polygon/Ray | 
| | | :x: Polygon/Segment | 
| | | :x: Polygon/Polyline | 
| | | :x: Polygon/Polygon | 
| | | :x: Polygon/Triangle | 




