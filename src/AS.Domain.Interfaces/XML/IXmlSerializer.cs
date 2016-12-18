namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Interface for xml serializer.
    /// </summary>
    public interface IXmlSerializer
    {
        string SerializeToXML<T>(T value);

        string SerializeToXML(object value);

        T DeserializeFromXML<T>(string xmlValue);
    }
}