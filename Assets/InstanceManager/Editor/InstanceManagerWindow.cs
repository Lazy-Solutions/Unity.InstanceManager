﻿using InstanceManager.Models;
using UnityEditor;
using UnityEngine;

namespace InstanceManager.Editor
{

    public partial class InstanceManagerWindow : EditorWindow
    {

        [MenuItem("Tools/Lazy/Instance Manager")]
        public static void Open()
        {
            var w = GetWindow<InstanceManagerWindow>();
            w.titleContent = new GUIContent("Instance Manager");
        }

        static class Style
        {

            public static GUIStyle margin { get; private set; }
            public static GUIStyle createButton { get; private set; }
            public static GUIStyle row { get; private set; }
            public static GUIStyle noItemsText { get; private set; }
            public static GUIStyle removeButton { get; private set; }

            public static GUIStyle secondaryInstanceMargin { get; private set; }
            public static GUIStyle elementMargin { get; private set; }

            public static GUIStyle searchBox { get; private set; }
            public static GUIStyle scenesList { get; private set; }
            public static GUIStyle scenesSeparator { get; private set; }

            public static GUIStyle moveSceneButton { get; private set; }

            public static GUIStyle menu { get; private set; }

            public static void Initialize()
            {

                margin ??= new GUIStyle() { margin = new RectOffset(12, 12, 12, 12) };
                createButton ??= new GUIStyle(GUI.skin.button) { padding = new RectOffset(12, 12, 6, 6) };
                row ??= new GUIStyle(EditorStyles.toolbar) { padding = new RectOffset(12, 12, 12, 6), fixedHeight = 42 };
                noItemsText ??= new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
                removeButton ??= new GUIStyle(GUI.skin.button) { margin = new RectOffset(12, 0, 0, 0) };

                secondaryInstanceMargin ??= new GUIStyle() { margin = new RectOffset(6, 6, 6, 6) };
                elementMargin ??= new GUIStyle() { margin = new RectOffset(0, 0, 12, 0) };

                searchBox ??= new GUIStyle(EditorStyles.textField) { margin = new RectOffset(0, 6, 0, 0) };
                scenesList ??= new GUIStyle(GUI.skin.box) { margin = new RectOffset(6, 6, 8, 6) };
                scenesSeparator ??= new GUIStyle(GUI.skin.box);
                scenesSeparator.normal.background = EditorGUIUtility.whiteTexture;

                moveSceneButton ??= new GUIStyle(GUI.skin.button) { fontSize = 18, fixedWidth = 21, fixedHeight = 21, padding = new RectOffset(2, 0, 0, 0) };
                menu ??= new GUIStyle(GUI.skin.button) { fontSize = 20, fixedWidth = 16, fixedHeight = 19 };

            }

        }

        static class Content
        {

            public static GUIContent noInstances { get; private set; }
            public static GUIContent status { get; private set; }
            public static GUIContent running { get; private set; }
            public static GUIContent notRunning { get; private set; }

            public static GUIContent emptyString { get; private set; }
            public static GUIContent close { get; private set; }
            public static GUIContent open { get; private set; }

            public static GUIContent showInExplorer { get; private set; }
            public static GUIContent options { get; private set; }
            public static GUIContent delete { get; private set; }

            public static GUIContent symLinkerUpdate { get; private set; }
            public static GUIContent symLinkerNotInstalled { get; private set; }
            public static GUIContent update { get; private set; }
            public static GUIContent install { get; private set; }
            public static GUIContent github { get; private set; }
            public static GUIContent createNewInstance { get; private set; }

            public static GUIContent back { get; private set; }
            public static GUIContent reload { get; private set; }
            public static GUIContent autoSync { get; private set; }
            public static GUIContent apply { get; private set; }
            public static GUIContent autoPlayMode { get; private set; }

            public static GUIContent scenesToOpen { get; private set; }
            public static GUIContent search { get; private set; }

            public static GUIContent up { get; private set; }
            public static GUIContent down { get; private set; }

            public static GUIContent movingInstances { get; private set; }
            public static GUIContent menu { get; private set; }
            public static GUIContent settings { get; private set; }

            public static void Initialize()
            {

                status ??= new GUIContent("Status:                     ");
                noInstances ??= new GUIContent("No instances found.");
                running ??= new GUIContent("Running");
                notRunning ??= new GUIContent("Not running");

                emptyString ??= new GUIContent(string.Empty);
                open ??= new GUIContent("Open");
                close ??= new GUIContent("Close");

                showInExplorer ??= new GUIContent("Show in explorer...");
                options ??= new GUIContent("Options...");
                delete ??= new GUIContent("Delete");

                symLinkerUpdate ??= new GUIContent("SymLinker.exe has an update available.");
                symLinkerNotInstalled ??= new GUIContent("SymLinker.exe is not installed.");
                update ??= new GUIContent("Update");
                install ??= new GUIContent("Install");
                github ??= new GUIContent("Github");
                createNewInstance ??= new GUIContent("Create new instance");

                back ??= new GUIContent("←");
                reload ??= new GUIContent("↻", "Sync with primary instance");
                autoSync ??= new GUIContent("Auto sync:");
                apply ??= new GUIContent("apply");
                autoPlayMode ??= new GUIContent("Automatically enter / exit play mode: ");

                scenesToOpen ??= new GUIContent("Scenes to open:");
                search ??= new GUIContent(" Search:");

                up ??= new GUIContent("▴");
                down ??= new GUIContent("▾");

                movingInstances ??= new GUIContent("Moving instances...");
                menu ??= new GUIContent("⋮");
                settings ??= EditorGUIUtility.IconContent("d_Preset.Context", "Settings");

            }

        }

        internal static InstanceManagerWindow window;

        void OnFocus()
        {
            InstanceManager.instances.Reload();
            view.OnFocus();
        }

        void OnEnable()
        {

            window = this;

            InstanceManager.instances.Reload();

            if (InstanceManager.isSecondInstance)
                SetInstance(InstanceManager.id);

            view.OnEnable();

        }

        void OnDisable()
        {
            view.OnDisable();
            window = null;
        }

        static readonly Color overlay = new Color(0, 0, 0, 0.5f);

        void OnGUI()
        {

            if (InstanceManager.instances.isMovingInstances)
            {
                SetView(mainView);
                GUI.enabled = false;
            }
            SetView(settingsView);

            if (GUIExt.UnfocusOnClick())
                Repaint();

            Style.Initialize();
            Content.Initialize();

            if (view.minSize.HasValue)
                minSize = view.minSize.Value;

            BeginCheckResize();
            view.OnGUI();
            EndCheckResize();

            if (InstanceManager.instances.isMovingInstances)
            {
                EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), overlay);
                var size = GUI.skin.label.CalcSize(Content.movingInstances);
                EditorGUI.LabelField(new Rect((position.width / 2) - (size.x / 2), (position.height / 2) - (size.y / 2), size.x, size.y), Content.movingInstances);
            }

        }

        #region View

        static UnityInstance instance;

        static View m_view;
        static View view => m_view ??= mainView;

        static View mainView { get; } = new MainView();
        static View instanceView { get; } = new InstanceView();
        static View settingsView { get; } = new SettingsView();

        public class View
        {
            public virtual void OnGUI() { }
            public virtual void OnFocus() { }
            public virtual void OnEnable() { }
            public virtual void OnDisable() { }
            public virtual Vector2? minSize { get; }
            public Rect position => window.position;
        }

        static void SetView(View view)
        {

            view ??= mainView;

            if (view == m_view)
                return;

            m_view?.OnDisable();
            m_view = view;
            m_view.OnEnable();

        }

        static void ClearInstance() =>
            SetInstance(null);

        static void CloseSettings() =>
            SetInstance(null);

        static void OpenSettings() =>
            SetView(settingsView);

        static void SetInstance(string id)
        {

            instance = !string.IsNullOrEmpty(id)
            ? InstanceManager.instances.Find(id)
            : null;

            SetView(instance is null ? mainView : instanceView);

        }

        #endregion
        #region Check resize

        public bool hasResized { get; private set; }

        Rect prevPos;
        protected void BeginCheckResize()
        {
            hasResized =
                prevPos.width != position.width ||
                prevPos.height != position.height;
        }

        protected void EndCheckResize() =>
            prevPos = position;

        #endregion

    }

}
