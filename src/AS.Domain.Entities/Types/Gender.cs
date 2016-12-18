namespace AS.Domain.Entities
{
    /// <summary>
    /// Represents gender.
    /// Key : ISO/IEC 5218 code of gender
    /// Value : Name of the gender
    /// </summary>
    public class Gender : Pair<int, string>
    {
        public static readonly Gender NotKnown = new Gender(0, "NOTKNOWN");
        public static readonly Gender Male = new Gender(1, "MALE");
        public static readonly Gender Female = new Gender(2, "FEMALE");

        public Gender(int key, string value)
            : base(key, value)
        {
        }
    }
}