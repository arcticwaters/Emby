﻿using System.IO;
using System.Threading;
using MediaBrowser.Common.IO;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.LocalMetadata.Parsers;
using MediaBrowser.Model.Logging;

namespace MediaBrowser.LocalMetadata.Providers
{
    /// <summary>
    /// Class SeriesProviderFromXml
    /// </summary>
    public class SeasonXmlProvider : BaseXmlProvider<Season>, IHasOrder
    {
        private readonly ILogger _logger;

        public SeasonXmlProvider(IFileSystem fileSystem, ILogger logger)
            : base(fileSystem)
        {
            _logger = logger;
        }

        protected override void Fetch(LocalMetadataResult<Season> result, string path, CancellationToken cancellationToken)
        {
            new SeasonXmlParser(_logger).Fetch(result, path, cancellationToken);
        }

        protected override FileSystemInfo GetXmlFile(ItemInfo info, IDirectoryService directoryService)
        {
            return directoryService.GetFile(Path.Combine(info.Path, "season.xml"));
        }

        public int Order
        {
            get
            {
                // After Xbmc
                return 1;
            }
        }
    }
}

