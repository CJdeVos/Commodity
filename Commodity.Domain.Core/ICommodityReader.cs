using System;

namespace Commodity.Domain.Core
{
    public interface ICommodityReader
    {
        void ReadStartOfObject();
        void ReadEndOfObject();
        string ReadName();
        Type ReadType();
        void ReadNull();
        string ReadString();
    }
}