using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorph {

    /// <summary>
    /// Polymorph Math\n
    /// Standard math functions for internal and external use
    /// </summary>
    public static class PMath {

        /// <summary>
        /// <para>Linear interpolation between from and to using t.</para>
        /// <para>if t is 0 then "from" is returned, if t is 1 then "to" is returned.</para>
        /// <para>any other value of t will return some value between "from" and "to"</para>
        /// </summary>
        /// <returns>Value between from and to, based to t</returns>
        public static float Lerp(float from, float to, float t) {
            return from + ((to - from) * t);
        }

        /// <summary>
        /// <para>Linear interpolation between from and to using t.</para>
        /// <para>if t is 0 then "from" is returned, if t is 1 then "to" is returned.</para>
        /// <para>any other value of t will return some value between "from" and "to"</para>
        /// </summary>
        /// <returns>Value between from and to, based on t</returns>
        public static double Lerp(double from, double to, double t) {
            return from + ((to - from) * t);
        }

        /// <summary>
        /// <para>Inverse linear interpolation, given any value the function will calculate</para>
        /// <para>the t for that value as though it was interpolated.</para>
        /// <para>if "val" is "from" then 0 is retuned, if "val" is "to" then 1 is returned.</para>
        /// <para>any other value will return the ration that "val" sits in between "from" and "to"</para>
        /// </summary>
        /// <returns>Ratio between "from" and "to" where "val" sits</returns>
        public static float InLerp(float from, float to, float val) {
            return (val - from) / (to - from);
        }

        /// <summary>
        /// <para>Inverse linear interpolation, given any value the function will calculate</para>
        /// <para>the t for that value as though it was interpolated.</para>
        /// <para>if "val" is "from" then 0 is retuned, if "val" is "to" then 1 is returned.</para>
        /// <para>any other value will return the ration that "val" sits in between "from" and "to"</para>
        /// </summary>
        /// <returns>Ratio between "from" and "to" where "val" sits</returns>
        public static double InLerp(double from, double to, double val) {
            return (val - from) / (to - from);
        }

        /// <summary>
        /// Transform bases, takes "value" in the source range and transforms it into
        /// destination range
        /// </summary>
        /// <param name="fMin">Source range minimum</param>
        /// <param name="fMax">Source range maximum</param>
        /// <param name="value">Value in source range</param>
        /// <param name="tMin">Destination range minimum</param>
        /// <param name="tMax">Destination range maximum</param>
        /// <returns>Value in destination range</returns>
        public static float Rebase(float fMin, float fMax, float value, float tMin, float tMax) {
            return Lerp(tMin, tMax, InLerp(fMin, fMax, value));
        }

        /// <summary>
        /// Transform bases, takes "value" in the source range and transforms it into
        /// destination range
        /// </summary>
        /// <param name="fMin">Source range minimum</param>
        /// <param name="fMax">Source range maximum</param>
        /// <param name="value">Value in source range</param>
        /// <param name="tMin">Destination range minimum</param>
        /// <param name="tMax">Destination range maximum</param>
        /// <returns>Value in destination range</returns>
        public static double Rebase(double fMin, double fMax, double value, double tMin, double tMax) {
            return Lerp(tMin, tMax, InLerp(fMin, fMax, value));
        }

        /// <summary>
        /// Binds value to the given range
        /// </summary>
        public static float Clamp(float min, float max, float val) {
            if(max < min) {
                return Math.Min(min, Math.Max(max, val));
            } else {
                return Math.Min(max, Math.Max(min, val));
            }
        }

        /// <summary>
        /// Binds value to the given range
        /// </summary>
        public static double Clamp(double min, double max, double val) {
            if(max < min) {
                return Math.Min(min, Math.Max(max, val));
            } else {
                return Math.Min(max, Math.Max(min, val));
            }
        }

        /// <summary>
        /// Binds value to the range 0 - 1
        /// </summary>
        public static float Clamp01(float val) {
            return Clamp(0, 1, val);
        }

        /// <summary>
        /// Binds value to the range 0 - 1
        /// </summary>
        public static double Clamp01(double val) {
            return Clamp(0, 1, val);
        }
    }
}
