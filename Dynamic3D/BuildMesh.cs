using System;
using System.Collections.Generic;
using Polymorph.Primitives;

namespace Polymorph.Dynamic3D {

    public static class BuildMesh {

        static MeshData BuildMeshS(Vector3[] points, Vector3[] normals, int complexity, double width) {

            var retVal = new MeshData();
            var pointSets = new List<Vector3[]>();

            for(int i = 0; i < points.Length; ++i) {
                var arr = new Vector3[complexity];
                // float d = ((float) i / intervals) * spline.length;
                var point = points[i]; // spline.GetPoint(d);
                var dir = normals[i]; // spline.GetVelocity(d);
                for(int j = 0; j < arr.Length; ++j) {
                    arr[j] = GetCircularPoint(point, dir, width, (j / (double) complexity) * Math.PI);
                }
                pointSets.Add(arr);
            }

            var verticies = new List<Vector3>();
            var triangles = new List<int>();

            int set1Start = 0;
            int set2Start = 0;
            verticies.AddRange(pointSets[0]);

            for(int i = 1; i < pointSets.Count; ++i) {
                var set = pointSets[i];
                set1Start = set2Start;
                set2Start = verticies.Count;
                verticies.AddRange(set);
                for(int j = 0; j < set.Length - 1; ++j) {

                    triangles.Add(set1Start + j);
                    triangles.Add(set1Start + j + 1);
                    triangles.Add(set2Start + j);

                    triangles.Add(set1Start + j + 1);
                    triangles.Add(set2Start + j + 1);
                    triangles.Add(set2Start + j);
                }

                triangles.Add(set1Start + set.Length - 1);
                triangles.Add(set1Start);
                triangles.Add(set2Start + set.Length - 1);

                triangles.Add(set1Start);
                triangles.Add(set2Start);
                triangles.Add(set2Start + set.Length - 1);
            }
            retVal.vertices = verticies.ToArray();
            retVal.triangles = triangles.ToArray();
            // retVal.RecalculateNormals();

            return retVal;
        }

        static Vector3 GetCircularPoint(Vector3 point, Vector3 normal, double width, double angle) {
            //normal = normal.normalized;
            //var x = Math.Cos(angle);
            //var y = Math.Sin(angle);
            //var v = new Vector3(x, y, 0);
            ////var tangent0 = Vector3.Cross(normal, Vector3.right);
            ////if(Vector3.Dot(tangent0, tangent0) < 0.001) {
            ////    tangent0 = Vector3.Cross(normal, Vector3.up);
            ////}
            ////tangent0.Normalize();
            ////// Find another vector in the plane
            ////var tangent1 = Vector3.Cross(normal, tangent0).normalized;
            ////var lastRow = Vector4.zero;
            ////lastRow.w = 1;
            ////var mat = new Matrix4x4(tangent0, tangent1, normal, lastRow);
            ////v = mat.MultiplyVector(v);
            //// v = VMath.ProjectPointOnPlane(normal, Vector3.zero, v);
            //// v = Matrix4x4.LookAt(point, point + normal, up).MultiplyVector(v);
            //v = Matrix4x4.Rotate(Quaternion.LookRotation(normal, Vector3.up)).MultiplyVector(v);
            //// var z = ((-normal.x * x) - (normal.y * y)) / normal.z;
            //return (v.normalized * width) + point;
            return Vector3.zero;
        }
    }
}
