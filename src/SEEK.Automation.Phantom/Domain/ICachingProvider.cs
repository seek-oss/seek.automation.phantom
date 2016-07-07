using System.IO;
using Serilog;
using SEEK.Automation.Phantom.Configuration;

// ReSharper disable UseStringInterpolation
namespace SEEK.Automation.Phantom.Domain
{
    public interface ICachingProvider
    {
        void Save(int port, string type, string payload);
        void Load(int port);
        void ClearCache();
    }

    public class CachingProvider : ICachingProvider
    {
        private readonly ILogger _logger;
        private readonly IBroker _broker;
        private readonly string _cacheFolder;

        public CachingProvider(ILogger logger, IBroker broker)
        {
            _logger = logger;
            _broker = broker;

            _cacheFolder = Keys.TryGetConfigValue(Keys.CacheDir, "Cache");
        }

        public void Save(int port, string type, string payload)
        {
            _logger.Information("Caching for port {0}, with type {1} and the specified payload", port, type);
            
            Directory.CreateDirectory(_cacheFolder);

            var fileName = string.Format("{0}.{1}.cache", port, type);
            var fileContents = payload;
            var cacheFilePath = Path.Combine(_cacheFolder, fileName);

            var allRegistrationsOnPort = string.Format("{0}*", port);
            var filesToDelete = Directory.GetFiles(_cacheFolder, allRegistrationsOnPort);
            foreach (var file in filesToDelete)
            {
                File.Delete(file);
            }

            File.WriteAllText(cacheFilePath, fileContents);
        }

        public void Load(int port)
        {
            _logger.Information("Loading from cache, for port {0}", port);

            var files = Directory.GetFiles(_cacheFolder);

            _logger.Information(string.Format("Found {0} simulations in the cache", files.Length));

            foreach (var file in files)
            {
                var fileNameFields = file.Split(".".ToCharArray());

                // ReSharper disable once AssignNullToNotNullAttribute
                var currentPort = int.Parse(Path.GetFileName(path: fileNameFields[0]));
                var type = fileNameFields[1];
                var payload = File.ReadAllText(file);

                if (port != 0)
                {
                    if (currentPort == port)
                    {
                        _broker.Initialise(currentPort, type, payload);
                        break;
                    }
                    continue;
                }

                _broker.Initialise(currentPort, type, payload);
            }
        }

        public void ClearCache()
        {
            var files = Directory.GetFiles(_cacheFolder);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
    }
}
