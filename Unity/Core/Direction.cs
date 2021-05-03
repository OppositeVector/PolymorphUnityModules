using System;

namespace Polymorph.Unity.Core {

    [Flags]
    public enum Direction { 
        Top = 1,
        TopRight = 5,
        Right = 4,
        BottomRight = 6,
        Bottom = 2, 
        BottomLeft = 10,
        Left = 8, 
        TopLeft = 9,
        Center = 128 
    }

    public static class DirectionUtil {

        public static Direction Spin45(Direction dir, bool counterClockwise = false) {
            if(counterClockwise) {
                switch(dir) {
                    case Direction.Top: return Direction.TopLeft;
                    case Direction.TopLeft: return Direction.Left;
                    case Direction.Left: return Direction.BottomLeft;
                    case Direction.BottomLeft: return Direction.Bottom;
                    case Direction.Bottom: return Direction.BottomRight;
                    case Direction.BottomRight: return Direction.Right;
                    case Direction.Right: return Direction.TopRight;
                    case Direction.TopRight: return Direction.Top;
                    default : return Direction.Center;
                }
            } else {
                switch(dir) {
                    case Direction.Top: return Direction.TopRight;
                    case Direction.TopRight: return Direction.Right;
                    case Direction.Right: return Direction.BottomRight;
                    case Direction.BottomRight: return Direction.Bottom;
                    case Direction.Bottom: return Direction.BottomLeft;
                    case Direction.BottomLeft: return Direction.Left;
                    case Direction.Left: return Direction.TopLeft;
                    case Direction.TopLeft: return Direction.Top;
                    default : return Direction.Center;
                }
            }
        }
        public static Direction Spin90(Direction dir, bool counterClockwise = false) {
            if(counterClockwise) {
                switch(dir) {
                    case Direction.Top: return Direction.Left;
                    case Direction.TopLeft: return Direction.BottomLeft;
                    case Direction.Left: return Direction.Bottom;
                    case Direction.BottomLeft: return Direction.BottomRight;
                    case Direction.Bottom: return Direction.Right;
                    case Direction.BottomRight: return Direction.TopRight;
                    case Direction.Right: return Direction.Top;
                    case Direction.TopRight: return Direction.TopLeft;
                    default : return Direction.Center;
                }
            } else {
                switch(dir) {
                    case Direction.Top: return Direction.Right;
                    case Direction.TopRight: return Direction.BottomRight;
                    case Direction.Right: return Direction.Bottom;
                    case Direction.BottomRight: return Direction.BottomLeft;
                    case Direction.Bottom: return Direction.Left;
                    case Direction.BottomLeft: return Direction.TopLeft;
                    case Direction.Left: return Direction.Top;
                    case Direction.TopLeft: return Direction.TopRight;
                    default : return Direction.Center;
                }
            }
        }
        public static Direction Spin180(Direction dir) {
            switch(dir) {
                case Direction.Top: return Direction.Bottom;
                case Direction.TopRight: return Direction.BottomLeft;
                case Direction.Right: return Direction.Left;
                case Direction.BottomRight: return Direction.TopLeft;
                case Direction.Bottom: return Direction.Top;
                case Direction.BottomLeft: return Direction.TopRight;
                case Direction.Left: return Direction.Right;
                case Direction.TopLeft: return Direction.BottomRight;
                default: return Direction.Center;
            }
        }
    }
}
