using FluentAssertions;
using SEEK.Automation.Phantom.Support;
using Xunit;
using NameValueCollection = System.Collections.Specialized.NameValueCollection;

namespace SEEK.Automation.Phantom.Tests.UnitTests
{
    public class NameValueCollectionExtensionTests
    {
        [Fact]
        public void Validate_When_Collection_Equal()
        {
            var collection1 = new NameValueCollection {{"sky", "blue"}, {"car", "black"}};
            var collection2 = new NameValueCollection { { "sky", "blue" }, { "car", "black" } };

            collection1.CollectionEquals(collection2).Should().BeTrue();
        }

        [Fact]
        public void Validate_Filter_Collection()
        {
            var collection = new NameValueCollection { { "oauth_consumer_key", "key" }, { "car", "black" }, { "oauth_timestamp", "timestamp" }, { "oauth_signature", "sig" } };
            var filteredCollection = new NameValueCollection { { "car", "black" } };

            collection.FilterCollection().CollectionEquals(filteredCollection).Should().BeTrue();
        }
    }
}
