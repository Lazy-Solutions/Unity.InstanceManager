﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace InstanceManager.Utility
{

    /// <summary>Provides methods for enumerating and applying window layouts.</summary>
    public static class WindowLayoutUtility
    {

        static readonly MethodInfo loadWindowLayout;
        static readonly MethodInfo getCurrentLayoutPath;
        static readonly PropertyInfo layoutsModePreferencesPath;

        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        static WindowLayoutUtility()
        {

            var windowLayout = Type.GetType("UnityEditor.WindowLayout,UnityEditor");

            loadWindowLayout = windowLayout?.GetMethod("LoadWindowLayout", flags, null, new[] { typeof(string), typeof(bool) }, null);
            getCurrentLayoutPath = windowLayout?.GetMethod("GetCurrentLayoutPath", flags);
            layoutsModePreferencesPath = windowLayout.GetProperty("layoutsModePreferencesPath", flags);

            layoutsPath = (string)layoutsModePreferencesPath?.GetValue(null);

        }

        /// <summary>Gets whatever the utility was able to find the internal unity methods or not.</summary>
        public static bool isAvailable => !(layoutsPath is null);

        /// <summary>The path to the layouts folder.</summary>
        public static string layoutsPath { get; private set; }

        /// <summary>Finds all available layouts.</summary>
        public static Layout[] availableLayouts =>
            Directory.GetFiles(layoutsPath, "*.wlt").
            Select(path => new Layout(path)).
            ToArray();

        /// <summary>Finds the specified layout by name.</summary>
        public static Layout Find(string name) =>
            availableLayouts.FirstOrDefault(l => l.name == name);

        public struct Layout
        {

            /// <summary>Path on disk to this layout.</summary>
            public string path { get; }

            /// <summary>The name of this layout.</summary>
            public string name { get; }

            public Layout(string path)
            {
                this.path = path;
                name = Path.GetFileNameWithoutExtension(path);
            }

            /// <summary>Applies this layout, if available.</summary>
            public void Apply()
            {
                if (isAvailable && File.Exists(path) && path.EndsWith(".wlt"))
                    loadWindowLayout?.Invoke(null, new object[] { path, true });
            }

        }

    }

}
