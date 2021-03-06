﻿using System;
using System.Collections.Generic;

namespace MediaBrowser.Controller.Entities
{
    /// <summary>
    /// This is just a marker interface to denote top level folders
    /// </summary>
    public interface ICollectionFolder
    {
        string CollectionType { get; }
        string Path { get; }
        string Name { get; }
        Guid Id { get; }
        IEnumerable<string> PhysicalLocations { get; }
    }
}
