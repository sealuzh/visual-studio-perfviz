namespace InSituVisualization.TelemetryCollector.Model
{
    /// <summary>
    /// The abstract telemetry collected on a specific method
    /// </summary>
    public abstract class Method
    {
        protected Method(string documentationCommentId)
        {
            DocumentationCommentId = documentationCommentId;
        }

        /// <summary>
        /// DocumentationCommentId represents the mean of identification of a specific method symbol in a project
        /// <see href="https://github.com/dotnet/roslyn/blob/master/src/Compilers/Core/Portable/DocumentationCommentId.cs"/>
        /// </summary>
        public string DocumentationCommentId { get; }
    }
}
