using Polymorph.Primitives;

namespace Polymorph.Shapes {

    public class Cube {

        Vector3 center;
        Vector3 normal;
        Vector3 size;

        public Cube(Vector3 size, Vector3 center, Vector3 up, Vector3 forward) {
            this.center = center;
            this.normal = normal;
            this.size = size;
        }

        public Vector3[] ToSegments() {

            var half = size / 2;

            var p1 = new Vector3(center.x - half.x, center.y - half.y, center.z - half.z);
            var p2 = new Vector3(center.x - half.x, center.y + half.y, center.z - half.z);
            var p3 = new Vector3(center.x - half.x, center.y + half.y, center.z + half.z);
            var p4 = new Vector3(center.x - half.x, center.y - half.y, center.z + half.z);
            var p5 = new Vector3(center.x + half.x, center.y - half.y, center.z - half.z);
            var p6 = new Vector3(center.x + half.x, center.y + half.y, center.z - half.z);
            var p7 = new Vector3(center.x + half.x, center.y + half.y, center.z + half.z);
            var p8 = new Vector3(center.x + half.x, center.y - half.y, center.z + half.z);

            return new Vector3[] { p1, p3, p3, p4, p4, p5, p5, p1, p1, p7, p3, p6, p4, p2, p5, p8, p2, p6, p6, p7, p7, p8, p8, p2 };
        }
    }
}
