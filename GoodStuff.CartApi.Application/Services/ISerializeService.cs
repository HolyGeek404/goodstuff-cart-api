namespace GoodStuff.CartApi.Infrastructure.Services;

public interface ISerializeService
{
    string Serialize(object obj);
    T? Deserialize<T>(string json);
}