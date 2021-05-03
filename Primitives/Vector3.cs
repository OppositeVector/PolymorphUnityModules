using System;
using System.Collections.Generic;

namespace Polymorph.Primitives {

#pragma warning disable 0660

    public struct Vector3 {

        #region Operators
        public static Vector3 operator +(Vector3 left, Vector3 right) {
            return new Vector3(left.x + right.x, left.y + right.y, left.z + right.z);
        }
        public static Vector3 operator +(Vector3 left, double right) {
            return new Vector3(left.x + right, left.y + right, left.z + right);
        }
        public static Vector3 operator +(double left, Vector3 right) {
            return new Vector3(left + right.x, left + right.y, left + right.z);
        }

        public static Vector3 operator -(Vector3 left, Vector3 right) {
            return new Vector3(left.x - right.x, left.y - right.y, left.z - right.z);
        }
        public static Vector3 operator -(Vector3 left, double right) {
            return new Vector3(left.x - right, left.y - right, left.z - right);
        }
        public static Vector3 operator -(double left, Vector3 right) {
            return new Vector3(left - right.x, left - right.y, left - right.z);
        }
        public static Vector3 operator -(Vector3 operand) {
            return new Vector3(-operand.x, -operand.y, -operand.z);
        }

        public static Vector3 operator *(Vector3 left, Vector3 right) {
            return new Vector3(left.x * right.x, left.y * right.y, left.z * right.z);
        }
        public static Vector3 operator *(Vector3 left, double right) {
            return new Vector3(left.x * right, left.y * right, left.z * right);
        }
        public static Vector3 operator *(double left, Vector3 right) {
            return new Vector3(left * right.x, left * right.y, left * right.z);
        }

        public static Vector3 operator /(Vector3 left, Vector3 right) {
            return new Vector3(left.x / right.x, left.y / right.y, left.z / right.z);
        }
        public static Vector3 operator /(Vector3 left, double right) {
            return new Vector3(left.x / right, left.y / right, left.z / right);
        }
        public static Vector3 operator /(double left, Vector3 right) {
            return new Vector3(left / right.x, left / right.y, left / right.z);
        }

        public static bool operator ==(Vector3 left, Vector3 right) {
            return (left.x == right.x) && (left.y == right.y) && (left.z == right.z);
        }
        public static bool operator !=(Vector3 left, Vector3 right) {
            return (left.x != right.x) && (left.y != right.y) && (left.z != right.z);
        }
        #endregion

        public static Vector3 zero { get; } = new Vector3();
        public static Vector3 one { get; } = new Vector3(1, 1, 1);
        public static Vector3 right { get; } = new Vector3(1, 0, 0);
        public static Vector3 up { get; } = new Vector3(0, 1, 0);
        public static Vector3 forward { get; } = new Vector3(0, 0, 1);

        public double x, y, z;
        public Vector2 xx { get { return new Vector2(x, x); } }
        public Vector2 xy { get { return new Vector2(x, y); } }
        public Vector2 xz { get { return new Vector2(x, z); } }
        public Vector2 yx { get { return new Vector2(y, x); } }
        public Vector2 yy { get { return new Vector2(y, y); } }
        public Vector2 yz { get { return new Vector2(y, z); } }
        public Vector2 zx { get { return new Vector2(z, x); } }
        public Vector2 zy { get { return new Vector2(z, y); } }
        public Vector2 zz { get { return new Vector2(z, z); } }

        public double magnitude { get { return Math.Sqrt((x * x) + (y * y) + (z * z)); } }
        public Vector3 normalized { 
            get {
                var mag = magnitude;
                return new Vector3(x / mag, y / mag, z / mag);
            } 
        }

        public Vector3(double x, double y, double z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
#pragma warning restore 0660
}
