namespace Inside
{
    public interface IJsonConverter
    {
        string Serilize(object value);
        T Deserilize<T>(string value);
    }
}
