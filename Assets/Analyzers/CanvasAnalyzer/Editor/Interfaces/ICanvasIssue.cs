using Unity.ProjectAuditor.Editor.Core;
using UnityEngine;

namespace Unity.ProjectAuditor.Editor.Modules
{
    public interface ICanvasIssue
    {
        public Descriptor Descriptor { get; }
        public ReportItem Analyze(Canvas targetAsset, AssetAnalysisContext context);
    }
}