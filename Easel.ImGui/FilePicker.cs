using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ImGuiNET;

namespace Easel.Imgui;

public class FilePicker
{
    private string _currentPath;

    private int _selectedFile;
    private FileSystemEntry[] _files;

    public readonly FilePickerType Type;
    public bool ShowHidden;

    public FilePicker(FilePickerType type, string initialPath = null)
    {
        Type = type;

        _currentPath = initialPath ?? Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        _files = Array.Empty<FileSystemEntry>();
        FetchFiles();

        ShowHidden = false;
    }
    
    public void Update()
    {
        if (ImGui.Begin("File Picker"))
        {
            if (ImGui.InputText("Path", ref _currentPath, 2000, ImGuiInputTextFlags.EnterReturnsTrue))
                FetchFiles();

            if (ImGui.BeginListBox("Stuff"))
            {
                int i = 0;
                foreach (FileSystemEntry file in _files)
                {
                    if (ImGui.Selectable(file.Name, _selectedFile == i))
                    {
                        if (file.IsDirectory)
                        {
                            _currentPath = file.FullPath;
                            FetchFiles();
                        }
                        else
                            _selectedFile = i;
                    }

                    i++;
                }
            }

            ImGui.End();
        }
    }

    private bool FetchFiles()
    {
        if (!Directory.Exists(_currentPath))
            return false;
        
        List<FileSystemEntry> fileEntries = new List<FileSystemEntry>();

        DirectoryInfo di = new DirectoryInfo(_currentPath);
        DirectoryInfo[] directories = di.GetDirectories();
        Array.Sort(directories, (info, directoryInfo) => string.Compare(info.Name, directoryInfo.Name));
        foreach (DirectoryInfo info in directories)
        {
            if ((info.Attributes & FileAttributes.Hidden) != 0 && !ShowHidden)
                continue;
            fileEntries.Add(new FileSystemEntry(info.FullName, true));
        }

        FileInfo[] files = di.GetFiles();
        Array.Sort(files, (info, fileInfo) => string.Compare(info.Name, fileInfo.Name));
        foreach (FileInfo info in files)
        {
            if ((info.Attributes & FileAttributes.Hidden) != 0 && !ShowHidden)
                continue;
            fileEntries.Add(new FileSystemEntry(info.FullName, true));
        }

        _files = fileEntries.ToArray();
        
        return true;
    }

    private struct FileSystemEntry
    {
        public readonly string Name;
        
        public readonly string FullPath;

        public readonly bool IsDirectory;

        public FileSystemEntry(string fullPath, bool isDirectory)
        {
            Name = Path.GetFileName(fullPath);
            FullPath = fullPath;
            IsDirectory = isDirectory;
        }
    }
}