using System.Collections.Generic;

namespace ObjectInitializerGenerator
{
    public interface ICodeWriter
    {
        string Write(List<ObjectModel> objectModels);
    }
}