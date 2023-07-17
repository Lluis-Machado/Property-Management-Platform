namespace DocumentAnalyzerAPI.DTOs
{
    public class FieldInfo<T>
    {
        public T Value { get; set; }
        public float? Confidence { get; set; }
        public List<BoundingRegionDTO> BoundingRegions { get; set; }
    }
}
