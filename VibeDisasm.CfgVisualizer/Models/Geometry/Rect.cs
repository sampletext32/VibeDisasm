using System.Numerics;

namespace VibeDisasm.CfgVisualizer.Models.Geometry;

/// <summary>
/// Rectangle structure for hit testing and rendering
/// </summary>
public struct Rect
{
    /// <summary>
    /// X coordinate of the top-left corner
    /// </summary>
    public float X;
    
    /// <summary>
    /// Y coordinate of the top-left corner
    /// </summary>
    public float Y;
    
    /// <summary>
    /// Width of the rectangle
    /// </summary>
    public float Width;
    
    /// <summary>
    /// Height of the rectangle
    /// </summary>
    public float Height;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public Rect(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
    
    /// <summary>
    /// Checks if a point is contained within the rectangle
    /// </summary>
    public bool Contains(Vector2 point)
    {
        return point.X >= X && point.X <= X + Width &&
               point.Y >= Y && point.Y <= Y + Height;
    }
}
