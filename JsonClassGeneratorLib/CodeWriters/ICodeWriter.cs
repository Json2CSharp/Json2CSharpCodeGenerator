using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public interface ICodeWriter
    {
        string FileExtension { get; }
        string DisplayName { get; }
        IReadOnlyCollection<string> ReservedKeywords { get; }
        string GetTypeName(JsonType type);
        void WriteFileStart(StringBuilder sw);
        void WriteFileEnd(StringBuilder sw);
        void WriteDeserializationComment(StringBuilder sw, bool rootIsArray = false);
        void WriteNamespaceStart(StringBuilder sw, bool root);
        void WriteClassesToFile(StringBuilder sw, IEnumerable<JsonType> types, bool rootIsArray = false);
        void WriteNamespaceEnd(StringBuilder sw, bool root);
        void WriteClassMembers(StringBuilder sw, JsonType type, string prefix);
        void WriteClass(StringBuilder sw, JsonType type);
        bool IsReservedKeyword(string word);
    }

    [Obsolete("Use ICodeWriter using the StringBuilder class instead of TextWriter")]
    public interface ICodeWriterss
    {
        string FileExtension { get; }
        string DisplayName { get; }
        string GetTypeName(JsonType type);
        void WriteClass(TextWriter sw, JsonType type);
        void WriteFileStart(TextWriter sw);
        void WriteFileEnd(TextWriter sw);
        void WriteNamespaceStart(TextWriter sw, bool root);
        void WriteNamespaceEnd(TextWriter sw, bool root);
    }

}
