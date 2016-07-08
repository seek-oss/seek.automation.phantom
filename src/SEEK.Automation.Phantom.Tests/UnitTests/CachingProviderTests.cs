using System.IO;
using FluentAssertions;
using NSubstitute;
using Serilog;
using SEEK.Automation.Phantom.Configuration;
using SEEK.Automation.Phantom.Domain;
using Xunit;

namespace SEEK.Automation.Phantom.Tests.UnitTests
{
    public class CachingProviderTests
    {
        [Fact]
        public void Validate_Load_When_Port_Cache_Found()
        {
            var logger = Substitute.For<ILogger>();
            var broker = Substitute.For<IBroker>();
            var cacheFolder = Keys.TryGetConfigValue(Keys.CacheDir, "Cache");

            if (!Directory.Exists(cacheFolder)) Directory.CreateDirectory(cacheFolder);

            File.WriteAllText(Path.Combine(cacheFolder, "9000.register.cache"), "abc");

            var cachingProvider = new CachingProvider(logger, broker);

            cachingProvider.Load(9000);
            
            broker.Received().Initialise(9000, "register", "abc");
        }

        [Fact]
        public void Validate_Load_When_Loading_All_Ports()
        {
            var logger = Substitute.For<ILogger>();
            var broker = Substitute.For<IBroker>();
            var cacheFolder = Keys.TryGetConfigValue(Keys.CacheDir, "Cache");

            if (!Directory.Exists(cacheFolder)) Directory.CreateDirectory(cacheFolder);

            File.WriteAllText(Path.Combine(cacheFolder, "9000.register.cache"), "abc");

            var cachingProvider = new CachingProvider(logger, broker);

            cachingProvider.Load(0);

            broker.Received().Initialise(9000, "register", "abc");
        }

        [Fact]
        public void Validate_Load_When_Port_Cache_Not_Found()
        {
            var logger = Substitute.For<ILogger>();
            var broker = Substitute.For<IBroker>();
            var cacheFolder = Keys.TryGetConfigValue(Keys.CacheDir, "Cache");

            if (Directory.Exists(cacheFolder))
            {
                Directory.Delete(cacheFolder, true);
            }

            Directory.CreateDirectory(cacheFolder);

            File.WriteAllText(Path.Combine(cacheFolder, "9000.register.cache"), "abc");

            var cachingProvider = new CachingProvider(logger, broker);

            cachingProvider.Load(9001);

            broker.DidNotReceive().Initialise(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void Validate_Load_When_Clearing_Cache()
        {
            var logger = Substitute.For<ILogger>();
            var broker = Substitute.For<IBroker>();
            var cacheFolder = Keys.TryGetConfigValue(Keys.CacheDir, "Cache");

            if (!Directory.Exists(cacheFolder)) Directory.CreateDirectory(cacheFolder);

            File.WriteAllText(Path.Combine(cacheFolder, "test1.tst"), "abc");
            File.WriteAllText(Path.Combine(cacheFolder, "test2.tst"), "abc");

            var cachingProvider = new CachingProvider(logger, broker);

            cachingProvider.ClearCache();

            Directory.GetFiles(cacheFolder).Length.Should().Be(0);
        }
    }
}
