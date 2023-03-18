using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using Easel.Audio;
using Easel.Content.Builder;
using Easel.Content.Localization;
using Easel.Core;
using Easel.Data;
using Easel.Formats;
using Easel.Graphics;
using Easel.GUI;

namespace Easel.Content;

public class ContentManager
{
    private string _defaultName;

    private Dictionary<string, ContentDefinition> _definitions;
    private Dictionary<Type, IContentProcessor> _processors;

    private string _location;

    public ContentManager()
    {
        _definitions = new Dictionary<string, ContentDefinition>();
        _location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        _processors = new Dictionary<Type, IContentProcessor>();
        AddContentProcessor(typeof(Texture2D), new TextureProcessor());
        AddContentProcessor(typeof(Sound), new SoundProcessor());
        AddContentProcessor(typeof(Model), new ModelProcessor());
        AddContentProcessor(typeof(Font), new FontProcessor());
        AddContentProcessor(typeof(Bitmap), new BitmapProcessor());
    }

    /// <summary>
    /// Add content with a content definition, either from a file, or manually generated.
    /// </summary>
    /// <param name="definition">The definition to initialize with.</param>
    public void AddContent(ContentDefinition definition)
    {
        if (_definitions.Count == 0)
            _defaultName = definition.Name;
        _definitions.Add(definition.Name, definition);
    }

    /*/// <summary>
    /// Add content to the content manager. This internally calls <see cref="AddContent(Easel.Content.Builder.ContentDefinition)"/>,
    /// and generates a content definition automatically.
    /// </summary>
    public void AddContent()
    {
        
    }*/
    
    public T Load<T>(string path)
    {
        if (!TryLoad(path, out T item))
            Logger.Fatal($"No content file with name \"{path}\" could be found.");

        return item;
    }

    public bool TryLoad<T>(string path, [NotNullWhen(true)] out T item)
    {
        item = default;
        
        Logger.Debug("Loading content item \"" + path + "\"...");
        if (!_processors.TryGetValue(typeof(T), out IContentProcessor processor))
            Logger.Fatal($"A content processor for type {typeof(T)} could not be found.");

        if (!_definitions[_defaultName].ContentTypes.TryGetValue(path, out IContentType type))
            return false;
        
        item = (T) processor!.Load(Path.Combine(_location, _defaultName), type);
        return true;
    }

    public string[] GetAllFiles(string path, string searchPattern = "*", bool recursive = true)
    {
        return Directory.GetFiles(Path.Combine(_location, _defaultName, path), searchPattern,
            recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
    }

    public void AddContentProcessor(Type type, IContentProcessor processor)
    {
        _processors.Add(type, processor);
    }
}