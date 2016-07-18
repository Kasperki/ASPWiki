using System.Collections.Generic;

namespace ASPWiki.Services
{
    public interface IGarbageGenerator<T>
    {
        T Generate();
        List<T> GenerateList(int count);
        void GenerateToDatabase(int count);
    }
}
