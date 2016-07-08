using System;
using FluentAssertions;
using SEEK.Automation.Phantom.Support;
using Xunit;

namespace SEEK.Automation.Phantom.Tests.UnitTests
{
    public class HelperTests
    {
        [Fact]
        public void Validate_Between_Function()
        {
            Helper.Between("123", "1", "3").Should().Be("2");
        }

        [Fact]
        public void Validate_TimeStamp_Function()
        {
            var dateTime = new DateTime(2016, 07, 8, 11, 19, 22);
            Helper.GetTimestamp(dateTime).Should().Be("20160708-111922000");
        }
    }
}
