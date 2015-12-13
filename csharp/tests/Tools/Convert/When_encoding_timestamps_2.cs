using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Machine.Specifications;

namespace rblt.Tests
{
    [Subject("Tools.Convert.EncodeTimeStamps")]
    public class When_encoding_timestamps_2 : TimeStampsTestBase
    {
        Establish context = () =>
        {
            SetupMocks();
        };


        Because of = () => Result = rblt.Tools.Convert.EncodeTimeStamps(TimeStamps2);


        It should_not_be_empty_string = () => Result.ShouldNotBeEmpty();
        It should_contain_only_BASE64_characters = () => Result.ShouldMatch("^[a-zA-Z0-9+/]+={0,2}$");

        
        static string Result;
    }
}
