using System.Collections.Generic;
using System.IO;
using Unity.ProjectAuditor.Editor.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.ProjectAuditor.Editor.Modules
{
    public class MissingGraphicRaycasterIssue : ICanvasIssue
    {
        private const string ID = "CAN0001";
        private const string Title = "Canvas: Missing GraphicRaycaster";
        private const Areas ImpactedAreas = Areas.All;
        private const string Description =
            "<b>GraphicRaycaster</b> is missing on the Canvas, however there are components " +
            "with input raycast enabled. They will not catch input without <b>GraphicRaycaster</b>";
        private const string Recommendation =
            "Add <b>GraphicsRaycaster</b> or disable raycast on the components.";
        
        public Descriptor Descriptor => new(
            ID,
            Title,
            ImpactedAreas,
            Description,
            Recommendation
        )
        {
            MessageFormat = "Canvas '{0}' does not have GraphicsRaycaster",
            Fixer = (issue, _) =>
            {
                GameObject loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(issue.Location.Path);
                if (loadedPrefab.GetComponent(typeof(GraphicRaycaster)))
                {
                    return;
                }
                
                loadedPrefab.AddComponent(typeof(GraphicRaycaster));
            }
        };

        public ReportItem Analyze(Canvas targetAsset, AssetAnalysisContext context)
        {
            if (HasGraphicRaycaster(targetAsset))
            {
                return null;
            }
            
            if (!HasRaycastEnabledGraphic(targetAsset) && !HasInteractableSelectable(targetAsset))
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