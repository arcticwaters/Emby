﻿using MediaBrowser.Common.IO;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Net;
using MediaBrowser.Model.Entities;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediaBrowser.Api.Library
{
    /// <summary>
    /// Class GetDefaultVirtualFolders
    /// </summary>
    [Route("/Library/VirtualFolders", "GET")]
    public class GetVirtualFolders : IReturn<List<VirtualFolderInfo>>
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>The user id.</value>
        public string UserId { get; set; }
    }

    [Route("/Library/VirtualFolders", "POST")]
    public class AddVirtualFolder : IReturnVoid
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the collection.
        /// </summary>
        /// <value>The type of the collection.</value>
        public string CollectionType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [refresh library].
        /// </summary>
        /// <value><c>true</c> if [refresh library]; otherwise, <c>false</c>.</value>
        public bool RefreshLibrary { get; set; }
    }

    [Route("/Library/VirtualFolders", "DELETE")]
    public class RemoveVirtualFolder : IReturnVoid
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [refresh library].
        /// </summary>
        /// <value><c>true</c> if [refresh library]; otherwise, <c>false</c>.</value>
        public bool RefreshLibrary { get; set; }
    }

    [Route("/Library/VirtualFolders/Name", "POST")]
    public class RenameVirtualFolder : IReturnVoid
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string NewName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [refresh library].
        /// </summary>
        /// <value><c>true</c> if [refresh library]; otherwise, <c>false</c>.</value>
        public bool RefreshLibrary { get; set; }
    }

    [Route("/Library/VirtualFolders/Paths", "POST")]
    public class AddMediaPath : IReturnVoid
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [refresh library].
        /// </summary>
        /// <value><c>true</c> if [refresh library]; otherwise, <c>false</c>.</value>
        public bool RefreshLibrary { get; set; }
    }

    [Route("/Library/VirtualFolders/Paths", "DELETE")]
    public class RemoveMediaPath : IReturnVoid
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [refresh library].
        /// </summary>
        /// <value><c>true</c> if [refresh library]; otherwise, <c>false</c>.</value>
        public bool RefreshLibrary { get; set; }
    }

    /// <summary>
    /// Class LibraryStructureService
    /// </summary>
    [Authenticated(Roles = "Admin", AllowBeforeStartupWizard = true)]
    public class LibraryStructureService : BaseApiService
    {
        /// <summary>
        /// The _app paths
        /// </summary>
        private readonly IServerApplicationPaths _appPaths;

        /// <summary>
        /// The _library manager
        /// </summary>
        private readonly ILibraryManager _libraryManager;

        private readonly ILibraryMonitor _libraryMonitor;

        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryStructureService" /> class.
        /// </summary>
        public LibraryStructureService(IServerApplicationPaths appPaths, ILibraryManager libraryManager, ILibraryMonitor libraryMonitor, IFileSystem fileSystem)
        {
            if (appPaths == null)
            {
                throw new ArgumentNullException("appPaths");
            }

            _appPaths = appPaths;
            _libraryManager = libraryManager;
            _libraryMonitor = libraryMonitor;
            _fileSystem = fileSystem;
        }

        /// <summary>
        /// Gets the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>System.Object.</returns>
        public object Get(GetVirtualFolders request)
        {
            var result = _libraryManager.GetVirtualFolders().OrderBy(i => i.Name).ToList();

            return ToOptimizedSerializedResultUsingCache(result);
        }

        /// <summary>
        /// Posts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void Post(AddVirtualFolder request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentNullException("request");
            }

            var name = _fileSystem.GetValidFilename(request.Name);

            var rootFolderPath = _appPaths.DefaultUserViewsPath;

            var virtualFolderPath = Path.Combine(rootFolderPath, name);

            if (Directory.Exists(virtualFolderPath))
            {
                throw new ArgumentException("There is already a media collection with the name " + name + ".");
            }

            _libraryMonitor.Stop();

            try
            {
                Directory.CreateDirectory(virtualFolderPath);

                if (!string.IsNullOrEmpty(request.CollectionType))
                {
                    var path = Path.Combine(virtualFolderPath, request.CollectionType + ".collection");

                    File.Create(path);
                }
            }
            finally
            {
                Task.Run(() =>
                {
                    // No need to start if scanning the library because it will handle it
                    if (request.RefreshLibrary)
                    {
                        _libraryManager.ValidateMediaLibrary(new Progress<double>(), CancellationToken.None);
                    }
                    else
                    {
                        // Need to add a delay here or directory watchers may still pick up the changes
                        var task = Task.Delay(1000);
                        // Have to block here to allow exceptions to bubble
                        Task.WaitAll(task);
                        
                        _libraryMonitor.Start();
                    }
                });
            }
        }

        /// <summary>
        /// Posts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void Post(RenameVirtualFolder request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentNullException("request");
            }

            if (string.IsNullOrWhiteSpace(request.NewName))
            {
                throw new ArgumentNullException("request");
            }

            var rootFolderPath = _appPaths.DefaultUserViewsPath;

            var currentPath = Path.Combine(rootFolderPath, request.Name);
            var newPath = Path.Combine(rootFolderPath, request.NewName);

            if (!Directory.Exists(currentPath))
            {
                throw new DirectoryNotFoundException("The media collection does not exist");
            }

            if (!string.Equals(currentPath, newPath, StringComparison.OrdinalIgnoreCase) && Directory.Exists(newPath))
            {
                throw new ArgumentException("There is already a media collection with the name " + newPath + ".");
            }

            _libraryMonitor.Stop();

            try
            {
                // Only make a two-phase move when changing capitalization
                if (string.Equals(currentPath, newPath, StringComparison.OrdinalIgnoreCase))
                {
                    //Create an unique name
                    var temporaryName = Guid.NewGuid().ToString();
                    var temporaryPath = Path.Combine(rootFolderPath, temporaryName);
                    Directory.Move(currentPath, temporaryPath);
                    currentPath = temporaryPath;
                }

                Directory.Move(currentPath, newPath);
            }
            finally
            {
                Task.Run(() =>
                {
                    // No need to start if scanning the library because it will handle it
                    if (request.RefreshLibrary)
                    {
                        _libraryManager.ValidateMediaLibrary(new Progress<double>(), CancellationToken.None);
                    }
                    else
                    {
                        // Need to add a delay here or directory watchers may still pick up the changes
                        var task = Task.Delay(1000);
                        // Have to block here to allow exceptions to bubble
                        Task.WaitAll(task);

                        _libraryMonitor.Start();
                    }
                });
            }
        }

        /// <summary>
        /// Deletes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void Delete(RemoveVirtualFolder request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentNullException("request");
            }

            var rootFolderPath = _appPaths.DefaultUserViewsPath;

            var path = Path.Combine(rootFolderPath, request.Name);

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("The media folder does not exist");
            }

            _libraryMonitor.Stop();

            try
            {
                _fileSystem.DeleteDirectory(path, true);
            }
            finally
            {
                Task.Run(() =>
                {
                    // No need to start if scanning the library because it will handle it
                    if (request.RefreshLibrary)
                    {
                        _libraryManager.ValidateMediaLibrary(new Progress<double>(), CancellationToken.None);
                    }
                    else
                    {
                        // Need to add a delay here or directory watchers may still pick up the changes
                        var task = Task.Delay(1000);
                        // Have to block here to allow exceptions to bubble
                        Task.WaitAll(task);

                        _libraryMonitor.Start();
                    }
                });
            }
        }

        /// <summary>
        /// Posts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void Post(AddMediaPath request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentNullException("request");
            }

            _libraryMonitor.Stop();

            try
            {
                LibraryHelpers.AddMediaPath(_fileSystem, request.Name, request.Path, _appPaths);
            }
            finally
            {
                Task.Run(() =>
                {
                    // No need to start if scanning the library because it will handle it
                    if (request.RefreshLibrary)
                    {
                        _libraryManager.ValidateMediaLibrary(new Progress<double>(), CancellationToken.None);
                    }
                    else
                    {
                        // Need to add a delay here or directory watchers may still pick up the changes
                        var task = Task.Delay(1000);
                        // Have to block here to allow exceptions to bubble
                        Task.WaitAll(task);

                        _libraryMonitor.Start();
                    }
                });
            }
        }

        /// <summary>
        /// Deletes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void Delete(RemoveMediaPath request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentNullException("request");
            }

            _libraryMonitor.Stop();

            try
            {
                LibraryHelpers.RemoveMediaPath(_fileSystem, request.Name, request.Path, _appPaths);
            }
            finally
            {
                Task.Run(() =>
                {
                    // No need to start if scanning the library because it will handle it
                    if (request.RefreshLibrary)
                    {
                        _libraryManager.ValidateMediaLibrary(new Progress<double>(), CancellationToken.None);
                    }
                    else
                    {
                        // Need to add a delay here or directory watchers may still pick up the changes
                        var task = Task.Delay(1000);
                        // Have to block here to allow exceptions to bubble
                        Task.WaitAll(task);

                        _libraryMonitor.Start();
                    }
                });
            }
        }
    }
}
