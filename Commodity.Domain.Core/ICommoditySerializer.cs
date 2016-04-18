namespace Commodity.Domain.Core
{
    public interface ICommoditySerializer<T>
    {
        void Serialize(ICommodityWriter writer, T o);
        T Deserialize(ICommodityReader reader);
    }
}