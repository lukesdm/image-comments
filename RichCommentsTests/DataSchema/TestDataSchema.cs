using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichCommentsTests.DataSchema
{
    /// <summary>
    /// Provides a set of static members for conveniently referencing TestData's tables and columns by name.
    /// </summary>
    /// <remarks>
    /// Needs to be manually kept up to date as the actual database schema changes.
    /// Names in CAPS represent metadata, e.g. a table's name.
    /// </remarks>
    static class TestData
    {
        public static class ParserData
        {
            public const string NAME = "ParserData";
            
            public const string LineText = "LineText";
            public const string ShouldBeParsable = "ShouldBeParsable";
            public const string ExpectedElementName = "ExpectedElementName";
            public const string ExpectedXmlPosition = "ExpectedXmlPosition";
            public const string ExpectedParameter1 = "ExpectedParameter1";
            public const string ExpectedParameter2 = "ExpectedParameter2";
            public const string ExpectedParameter3 = "ExpectedParameter3";
            public const string ExpectedException = "ExpectedException";
        }

        public const string CONNECTION_STRING = @"Provider=Microsoft.SqlServerCe.Client.4.0; Data Source=TestData.sdf;";
    }
}
