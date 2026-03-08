using System.Collections.Generic;
using System.IO;
using Unity.ProjectAuditor.Editor.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.ProjectAuditor.Editor.Modules
{
    public class RedundantGraphicRaycasterIssue : ICanvasIssue
    {
        private const string ID = "CAN0003";
        private const string Title = "Canvas: Redundant GraphicRaycaster";
        private const Areas ImpactedAreas = Areas.All;
        private const string Description =
            "<b>GraphicRaycaster</b> is redundant on the Canvas, there are no components " +
            "with input raycast enabled.";
        private const string Recommendation =
            "Remove <b>GraphicsRaycaster</b>";
        
        public Descriptor Descriptor => new(
            ID,
            Title,
            ImpactedAreas,
            Description,
            Recommendation
        )
        {
            MessageFormat = "GraphicsRaycaster is redundant on '{0}' canvas",
            Fixer = (issue, _) =>
            {
                GameObject loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(issue.Location.Path);
                Component component = loadedPrefab.GetComponent(typeof(GraphicRaycaster));
                if (!component)
                {
                    return;
                }
                
                Object.DestroyImmediate(component, true);
            }
        };

        public ReportItem Analyze(Canvas targetAsset, AssetAnalysisContext context)
        {
            if (!HasGraphicRaycaster(targetAsset))
            {
                return null;
            }
            
            if (HasRaycastEnabledGraphic(targetAsset) || HasInteractableSelectable(targetAsset))
            {
                return null;
            }

            string fileName = Path.GetFileNameWithoutExtension(context.AssetPath);
            return context.CreateIssue(IssueCategory.AssetIssue, Descriptor.Id, fileName).WithLocation(context.AssetPath);
        }

        private bool HasGraphicRaycaster(Canvas targetAsset)
        {
            return targetAsset.GetComponent(typeof(GraphicRaycaster));
        }
        
        private bool HasRaycastEnabledGraphic(Canvas targetAsset)
        {
            List<Graphic> graphics = new();
            targetAsset.GetComponentsInChildren(graphics);
            
            foreach (Graphic graphic in graphics)
            {
                if (graphic.raycastTarget)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool HasInteractableSelectable(Canvas targetAsset)
        {
            List<Selectable> selectables = new();
            targetAsset.GetComponentsInChildren(selectables);
            
            foreach (Selectable selectable in selectables)
            {
                if (selectable.IsInteractable())
                {
                    return true;
                }
            }

            return false;
        }
    }
}