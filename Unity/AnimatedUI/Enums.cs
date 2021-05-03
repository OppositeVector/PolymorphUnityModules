namespace Polymorph.Unity.AnimatedUI {

    /// <summary>
    /// Used internally by <see cref="AnimatedUIBehaviour"/> to define curve aquasition type
    /// </summary>
    public enum CurveType { 
        /// <summary>
        /// <para>Use the <see cref="AnimatedUIBehaviour.GetStandardCurve(BuiltIn)"/> to generate</para>
        /// <para>A standard curve</para>
        /// </summary>
        BuiltIn,
        /// <summary>
        /// Use the curve directly created in the editor
        /// </summary>
        Curve, 
        /// <summary>
        /// Use a curve from a <see cref="CurveLibrary"/> 
        /// </summary>
        CurveLibrary 
    }

    /// <summary>
    /// <para>Curve type definition used by <see cref="AnimatedUIBehaviour.GetStandardCurve(BuiltIn)"/></para>
    /// <para>to speficy which curve to generate</para>
    /// </summary>
    public enum BuiltIn { 
        /// <summary>
        /// A line which between (0, 0) and (1, 1)
        /// </summary>
        Linear, 
        /// <summary>
        /// A curve resembling a quarter circle starting (decreasing delta towards the end)
        /// </summary>
        Lerp,
        /// <summary>
        /// A curve that lerps to (0.5, 1.20) lerps back to (1, 1)
        /// </summary>
        Overshoot,
        /// <summary>
        /// A curve that lerps to (0.5, 0.8) then lerps to (1, 1)
        /// </summary>
        Undershoot,
        /// <summary>
        /// A sinosoidal motion that starts at (0, 0) does a full step throuh (0.25, 1) and (0.75, -1) then abck to (1, 0)
        /// </summary>
        Sinusoidal, 
        /// <summary>
        /// A linear motion through the points (0, 0), (0.25, 1), (0.75, -1) and (1, 0)
        /// </summary>
        Jagged 
    }
}
