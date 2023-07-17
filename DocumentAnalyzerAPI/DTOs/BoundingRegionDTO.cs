using System.Drawing;

namespace DocumentAnalyzerAPI.DTOs
{
    public class BoundingRegionDTO
    {
        public int PageNumber { get; set; }
        public List<PointF> BoundingPolygon { get; set; }
    }
}
