namespace Bookline.Domain.ValueObjects
{
    public class Token
    {
        public static string Generate(int tokenCandidate)
        {
            return $"BL{tokenCandidate.ToString().PadLeft(3, '0')}";
        }
    }
}