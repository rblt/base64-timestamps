using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Machine.Specifications;

namespace rblt.Tests
{
    [Subject("Tools.Convert.DecodeTimeStamps")]
    public class When_decoding_timestamps_1 : TimeStampsTestBase
    {
        Establish context = () =>
        {
            SetupMocks();
        };


        Because of = () => Result = rblt.Tools.Convert.DecodeTimeStamps(rblt.Tools.Convert.EncodeTimeStamps(TimeStamps1));


        It should_be_equal_with_original_input = () => Result.ShouldEqual(TimeStamps1);

        
        static DateTime[] Result;
    }
}
