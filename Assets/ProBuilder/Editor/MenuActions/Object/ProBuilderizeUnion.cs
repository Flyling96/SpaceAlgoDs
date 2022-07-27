using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder.Csg;

namespace UnityEditor.ProBuilder.Actions
{
    sealed class ProBuilderizeUnion : MenuAction
    {
        bool m_Enabled;
        Pref<bool> m_Quads = new Pref<bool>("meshImporter.quads", true);
        Pref<bool> m_Smoothing = new Pref<bool>("meshImporter.smoothing", true);
        Pref<float> m_SmoothingAngle = new Pref<float>("meshImporter.smoothingAngle", 1f);

        public ProBuilderizeUnion()
        {
            MeshSelection.objectSelectionChanged += OnObjectSelectionChanged;

            OnObjectSelectionChanged(); // invoke once as we might already have a selection in Hierarchy
        }

        private void OnObjectSelectionChanged()
        {
            // can't just check if any MeshFilter is present because we need to know whether or not it's already a
            // probuilder mesh
            int meshCount = Selection.transforms.SelectMany(x => x.GetComponentsInChildren<MeshFilter>()).Count();
            m_Enabled = meshCount > 0 && meshCount != MeshSelection.selectedObjectCount;
        }

        public override ToolbarGroup group
        {
            get { return ToolbarGroup.Object; }
        }

        public override Texture2D icon
        {
            get { return IconUtility.GetIcon("Toolbar/Object_ProBuilderize", IconSkin.Pro); }
        }

        public override TooltipContent tooltip
        {
            get { return s_Tooltip; }
        }

        GUIContent m_QuadsTooltip = new GUIContent("Import Quads", "Create ProBuilder mesh using quads where " +
                "possible instead of triangles.");
        GUIContent m_SmoothingTooltip = new GUIContent("Import Smoothing", "Import smoothing groups by " +
                "testing adjacent faces against an angle threshold.");
        GUIContent m_SmoothingThresholdTooltip = new GUIContent("Smoothing Threshold", "When importing " +
                "smoothing groups any adjacent faces with an adjoining angle difference of less than this value will be " +
                "grouped together in a smoothing group.");

        private static readonly TooltipContent s_Tooltip = new TooltipContent
            (
                "ProBuilderize",
                @"Creates ProBuilder-modifiable objects from meshes."
            );

        public override bool enabled
        {
            get { return base.enabled && m_Enabled; }
        }

        protected override MenuActionState optionsMenuState
        {
            get { return MenuActionState.VisibleAndEnabled; }
        }

        protected override void OnSettingsGUI()
        {
            GUILayout.Label("ProBuilderize Options", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox("When Preserve Faces is enabled ProBuilder will try to group adjacent triangles into faces.", MessageType.Info);

            EditorGUI.BeginChangeCheck();

            m_Quads.value = EditorGUILayout.Toggle(m_QuadsTooltip, m_Quads);
            m_Smoothing.value = EditorGUILayout.Toggle(m_SmoothingTooltip, m_Smoothing);
            GUI.enabled = m_Smoothing;
            EditorGUILayout.PrefixLabel(m_SmoothingThresholdTooltip);
            m_SmoothingAngle.value = EditorGUILayout.Slider(m_SmoothingAngle, 0.0001f, 45f);

            GUI.enabled = true;

            if (EditorGUI.EndChangeCheck())
                ProBuilderSettings.Save();

            GUILayout.FlexibleSpace();

            GUI.enabled = enabled;

            if (GUILayout.Button("ProBuilderize"))
                EditorUtility.ShowNotification(PerformAction().notification);

            GUI.enabled = true;
        }

        protected override ActionResult PerformActionImplementation()
        {
            if(Selection.transforms.Count() < 1)
            {
                return ActionResult.NoSelection;
            }

            MeshImportSettings settings = new MeshImportSettings()
            {
                quads = m_Quads,
                smoothing = m_Smoothing,
                smoothingAngle = m_SmoothingAngle
            };

            return DoProBuilderize(Selection.transforms[0], settings);
        }

        [System.Obsolete("Please use DoProBuilderize(IEnumerable<MeshFilter>, pb_MeshImporter.Settings")]
        public static ActionResult DoProBuilderize(
            Transform root,
            bool preserveFaces)
        {
            return DoProBuilderize(root, new MeshImportSettings()
            {
                quads = preserveFaces,
                smoothing = false,
                smoothingAngle = 1f
            });
        }

        /// <summary>
        /// Adds pb_Object component without duplicating the objcet. Is undo-able.
        /// </summary>
        /// <param name="selected"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static ActionResult DoProBuilderize(
            Transform root,
            MeshImportSettings settings)
        {

            int i = 0;

            // Return immediately from the action so that the GUI can resolve. Displaying a progress bar interrupts the
            // event loop causing a layoutting error.
            EditorApplication.delayCall += () =>
            {
                var copy = GameObject.Instantiate(root, root.position,root.rotation);
                var union = new GameObject("Union");
                copy.transform.parent = union.transform;
                IEnumerable<MeshFilter> selected = new Transform[1] { copy }.SelectMany(x => x.GetComponentsInChildren<MeshFilter>()).Where(x => x != null);
                float count = selected.Count();
                foreach (var mf in selected)
                {
                    if (mf.sharedMesh == null)
                        continue;

                    Mesh sourceMesh = mf.sharedMesh;
                    Material[] sourceMaterials = mf.gameObject.GetComponent<MeshRenderer>()?.sharedMaterials;

                    try
                    {
                        var destination = Undo.AddComponent<ProBuilderMesh>(mf.gameObject);
                        var meshImporter = new MeshImporter(sourceMesh, sourceMaterials, destination);
                        meshImporter.Import(settings);

                        destination.Rebuild();
                        destination.Optimize();

                        i++;
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning("Failed ProBuilderizing: " + mf.gameObject.name + "\n" + e.ToString());
                    }

                    UnityEditor.EditorUtility.DisplayProgressBar("ProBuilderizing", mf.gameObject.name, i / count);
                }

                UnityEngine.ProBuilder.Csg.Model model = null;
                MeshFilter firstFilter = null;
                try
                {
                    foreach (var mf in selected)
                    {
                        if (model == null)
                        {
                            if (firstFilter == null)
                            {
                                firstFilter = mf;
                            }
                            else
                            {
                                model = Boolean.Union(firstFilter.gameObject, mf.gameObject);
                            }
                        }
                        else
                        {
                            model = Boolean.Union(model, mf.gameObject);
                        }

                    }

                    if (model != null)
                    {
                        var materials = model.materials.ToArray();
                        ProBuilderMesh pb = ProBuilderMesh.Create();
                        pb.GetComponent<MeshFilter>().sharedMesh = (Mesh)model;
                        pb.GetComponent<MeshRenderer>().sharedMaterials = materials;
                        MeshImporter importer = new MeshImporter(pb.gameObject);
                        importer.Import(new MeshImportSettings() { quads = true, smoothing = true, smoothingAngle = 1f });
                        pb.Rebuild();
                        pb.CenterPivot(null);
                        Selection.objects = new Object[] { pb.gameObject };
                        pb.name = "Union";
                    }

                    GameObject.DestroyImmediate(union);
                    UnityEditor.EditorUtility.ClearProgressBar();
                    MeshSelection.OnObjectSelectionChanged();
                    ProBuilderEditor.Refresh();
                }
                catch(System.Exception e)
                {
                    Debug.LogError(e);
                    UnityEditor.EditorUtility.ClearProgressBar();
                }
             };

            if (i < 1)
                return new ActionResult(ActionResult.Status.Canceled, "Nothing Selected");
            return new ActionResult(ActionResult.Status.Success, "ProBuilderize " + i + (i > 1 ? " Objects" : " Object").ToString());
        }


    }
}
