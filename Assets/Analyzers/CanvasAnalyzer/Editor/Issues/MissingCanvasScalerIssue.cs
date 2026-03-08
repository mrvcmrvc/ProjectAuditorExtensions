using System.IO;
using Unity.ProjectAuditor.Editor.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.ProjectAuditor.Editor.Modules
{
    public class MissingCanvasScalerIssue : ICanvasIssue
    {
        private const string ID = "CAN0002";
        private const string Title = "Canvas: Missing CanvasScaler";
        private const Areas ImpactedAreas = Areas.All;
        private const string Description =
            "There is no <b>CanvasScaler</b> found on the Canvas.";
        private const string Recommendation =
            "Add <b>CanvasScaler</b>";
        
        public Descriptor Descriptor => new(
            ID,
            Title,
            ImpactedAreas,
            Description,
            Recommendation
        )
        {
            MessageFormat = "Canvas '{0}' does not have CanvasScaler",
            Fixer = (issue, _) =>
            {
                GameObject loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(issue.Location.Path);
                if (loadedPrefab.GetComponent(typeof(CanvasScaler)))
                {
                    return;
                }
                
                loadedPrefab.AddComponent(typeof(CanvasScaler));
            }
        };

        public ReportItem Analyze(Canvas targetAsset, AssetAnalysisContext context)
        {
            if (targetAsset.GetComponent(typeof(CanvasScaler)))
            {
                return null;
            }

            string fileName = Path.GetFileNameWithoutExtension(context.AssetPath);
            return context.CreateIssue(IssueCategory.AssetIssue, Descriptor.Id, fileName).WithLocation(context.AssetPath);
        }
    }
}