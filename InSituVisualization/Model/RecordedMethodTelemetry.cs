﻿using System;

namespace InSituVisualization.Model
{
    /// <summary>
    /// Telemetry collected on a specific method
    /// </summary>
    public class RecordedMethodTelemetry
    {
        public RecordedMethodTelemetry(string documentationCommentId, string id, DateTime timestamp, IClientData clientData)
        {
            DocumentationCommentId = documentationCommentId ?? throw new ArgumentNullException(nameof(documentationCommentId));
            Id = id;
            Timestamp = timestamp;
            ClientData = clientData;
        }

        /// <summary>
        /// DocumentationCommentId represents the mean of identification of a specific method symbol in a project
        /// <see href="https://github.com/dotnet/roslyn/blob/master/src/Compilers/Core/Portable/DocumentationCommentId.cs"/>
        /// </summary>
        public string DocumentationCommentId { get; }

        public string Id { get; }
        public DateTime Timestamp { get; }
        public IClientData ClientData { get; }


        public override bool Equals(object obj)
        {
            return obj is RecordedMethodTelemetry recordedMethodTelemetry && Id.Equals(recordedMethodTelemetry.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

    }
}
