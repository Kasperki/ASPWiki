using System.Collections.Generic;

namespace ASPWiki.Services.Generators
{
    public interface IGarbageGenerator<T>
    {
        T Generate();
        List<T> GenerateList(int count);
    }
}
