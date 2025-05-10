using System.Numerics;

namespace VibeDisasm.CfgVisualizer.Models.Geometry;

/// <summary>
/// Utility methods for geometric calculations related to graph visualization
/// </summary>
public static class GraphGeometryUtils
{
    /// <summary>
    /// Checks if a point is contained within a rectangle
    /// </summary>
    /// <param name="point">Point to check</param>
    /// <param name="rect">Rectangle to check against</param>
    /// <returns>True if the point is inside the rectangle, false otherwise</returns>
    public static bool IsPointInRect(Vector2 point, Rect rect)
    {
        return rect.Contains(point);
    }
    
    /// <summary>
    /// Creates a rectangle for a node based on its position and size
    /// </summary>
    /// <param name="nodePosition">Node position (center)</param>
    /// <param name="nodeSize">Node size</param>
    /// <returns>Rectangle representing the node's bounds</returns>
    public static Rect CreateNodeRect(Vector2 nodePosition, Vector2 nodeSize)
    {
        return new Rect(
            nodePosition.X - nodeSize.X / 2,
            nodePosition.Y - nodeSize.Y / 2,
            nodeSize.X,
            nodeSize.Y
        );
    }
    
    /// <summary>
    /// Calculates the intersection point of a line with a rectangle
    /// </summary>
    /// <param name="lineStart">Line start point</param>
    /// <param name="lineEnd">Line end point</param>
    /// <param name="rect">Rectangle to intersect with</param>
    /// <returns>Intersection point</returns>
    public static Vector2 GetRectIntersection(Vector2 lineStart, Vector2 lineEnd, Rect rect)
    {
        // Calculate direction vector
        var dir = Vector2.Normalize(lineEnd - lineStart);
        
        // Calculate rectangle center
        var rectCenter = new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        
        // Calculate half-size of rectangle
        var halfSize = new Vector2(rect.Width / 2, rect.Height / 2);
        
        // Calculate intersection with rectangle
        float tx = (dir.X == 0) ? float.MaxValue : 
                   (dir.X > 0) ? (rectCenter.X + halfSize.X - lineStart.X) / dir.X :
                                (rectCenter.X - halfSize.X - lineStart.X) / dir.X;
                                
        float ty = (dir.Y == 0) ? float.MaxValue : 
                   (dir.Y > 0) ? (rectCenter.Y + halfSize.Y - lineStart.Y) / dir.Y :
                                (rectCenter.Y - halfSize.Y - lineStart.Y) / dir.Y;
                                
        float t = Math.Min(Math.Abs(tx), Math.Abs(ty));
        
        return lineStart + dir * t;
    }
}
