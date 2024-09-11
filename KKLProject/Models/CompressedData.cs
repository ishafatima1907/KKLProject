namespace KKLProject.Models
{
    public class CompressedData
    {
        public int Id { get; set; }
        public byte[] CompressedJson { get; set; } // VARBINARY(MAX)
    }
}
