using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Machine.Specifications;

namespace rblt.Tests
{
    [Subject("Tools.Convert.EncodeDecodeRandomTimeStamp")]
    public class When_encoding_and_decoding_random_timestamps : TimeStampsTestBase
    {
        Establish context = () =>
        {
            SetupMocks();
        };


        Because of = () => Result = rblt.Tools.Convert.EncodeTimeStamps(RandomTimeStamps);


        It should_not_be_empty_string = () => Result.ShouldNotBeEmpty();
        It should_contain_only_BASE64_characters = () => Result.ShouldMatch("^[a-zA-Z0-9+/]+={0,2}$");
        It should_be_decoded_to_original_input = () => rblt.Tools.Convert.DecodeTimeStamps(Result).ShouldEqual(RandomTimeStamps);
        
        static string Result;
    }
}
