using System;
using System.Collections.Generic;

namespace Polymorph.Primitives {

    public struct Vector2 {

        #region Operators
        public static Vector2 operator +(Vector2 left, Vector2 right) {
            return new Vector2(left.x + right.x, left.y + right.y);
        }
        public static Vector2 operator +(Vector2 left, double right) {
            return new Vector2(left.x + right, left.y + right);
        }
        public static Vector2 operator +(double left, Vector2 right) {
            return new Vector2(left + right.x, left + right.y);
        }

        public static Vector2 operator -(Vector2 left, Vector2 right) {
            return new Vector2(left.x - right.x, left.y - right.y);
        }
        public static Vector2 operator -(Vector2 left, double right) {
            return new Vector2(left.x - right, left.y - right);
        }
        public static Vector2 operator -(double left, Vector2 right) {
            return new Vector2(left - right.x, left - right.y);
        }
        public static Vector2 operator -(Vector2 operand) {
            return new Vector2(-operand.x, -operand.y);
        }

        public static Vector2 operator *(Vector2 left, Vector2 right) {
            return new Vector2(left.x * right.x, left.y * right.y);
        }
        public static Vector2 operator *(Vector2 left, double right) {
            return new Vector2(left.x * right, left.y * right);
        }
        public static Vector2 operator *(double left, Vector2 right) {
            return new Vector2(left * right.x, left * right.y);
        }

        public static Vector2 operator /(Vector2 left, Vector2 right) {
            return new Vector2(left.x / right.x, left.y / right.y);
        }
        public static Vector2 operator /(Vector2 left, double right) {
            return new Vector2(left.x / right, left.y / right);
        }
        public static Vector2 operator /(double left, Vector2 right) {
            return new Vector2(left / right.x, left / right.y);
        }

        public static bool operator ==(Vector2 left, Vector2 right) {
            return (left.x == right.x) && (left.y == right.y);
        }
        public static bool operator !=(Vector2 left, Vector2 right) {
            return (left.x != right.x) && (left.y != right.y);
        }
        #endregion
        static Vector2 _zero = new Vector2();
        public static Vector2 zero { get { return _zero; } }
        static Vector2 _one = new Vector2(1, 1);
        public static Vector2 one { get { return _one; } }
        static Vector2 _right = new Vector2(1, 0);
        public static Vector2 right { get { return _right; } }
        static Vector2 _up = new Vector2(0, 1);
        public static Vector2 up { get { return _up; } }

        public double x, y;
        public Vector2 xx { get { return new Vector2(x, x); } }
        public Vector2 yx { get { return new Vector2(y, x); } }
        public Vector2 yy { get { return new Vector2(y, y); } }

        public Vector2(double x, double y) {
            this.x = x;
            this.y = y;
        }
    }
}
