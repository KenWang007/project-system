﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;

using Microsoft.VisualStudio.ProjectSystem.VS.Tree.Dependencies.Snapshot;
using Microsoft.VisualStudio.ProjectSystem.VS.Tree.Dependencies.Subscriptions;

namespace Microsoft.VisualStudio.ProjectSystem.VS.Tree.Dependencies.Models
{
    internal enum DiagnosticMessageSeverity
    {
        Info,
        Warning,
        Error,
    }

    internal class DiagnosticDependencyModel : DependencyModel
    {
        private static readonly DependencyIconSet s_errorIconSet = new DependencyIconSet(
            icon: ManagedImageMonikers.ErrorSmall,
            expandedIcon: ManagedImageMonikers.ErrorSmall,
            unresolvedIcon: ManagedImageMonikers.ErrorSmall,
            unresolvedExpandedIcon: ManagedImageMonikers.ErrorSmall);

        private static readonly DependencyIconSet s_warningIconSet = new DependencyIconSet(
            icon: ManagedImageMonikers.WarningSmall,
            expandedIcon: ManagedImageMonikers.WarningSmall,
            unresolvedIcon: ManagedImageMonikers.WarningSmall,
            unresolvedExpandedIcon: ManagedImageMonikers.WarningSmall);

        private readonly DiagnosticMessageSeverity _severity;

        public override DependencyIconSet IconSet => _severity == DiagnosticMessageSeverity.Error
            ? s_errorIconSet
            : s_warningIconSet;

        public override string Name { get; }

        public override int Priority => _severity == DiagnosticMessageSeverity.Error
            ? Dependency.DiagnosticsErrorNodePriority
            : Dependency.DiagnosticsWarningNodePriority;

        public override string ProviderType => PackageRuleHandler.ProviderTypeString;

        public DiagnosticDependencyModel(
            string originalItemSpec,
            DiagnosticMessageSeverity severity,
            string code,
            string message,
            ProjectTreeFlags flags,
            bool isVisible,
            IImmutableDictionary<string, string> properties)
            : base(originalItemSpec, originalItemSpec, flags, isResolved: false, isImplicit: false, properties: properties, isTopLevel: false, isVisible: isVisible)
        {
            Requires.NotNullOrEmpty(originalItemSpec, nameof(originalItemSpec));
            Requires.NotNullOrEmpty(message, nameof(message));

            _severity = severity;

            Name = message;
            Caption = string.IsNullOrWhiteSpace(code) ? message : string.Concat(code.ToUpperInvariant(), " ", message);

            Flags = Flags.Union(DependencyTreeFlags.DiagnosticNodeFlags);

            if (severity == DiagnosticMessageSeverity.Error)
            {
                Flags = Flags.Union(DependencyTreeFlags.DiagnosticErrorNodeFlags);
            }
            else
            {
                Flags = Flags.Union(DependencyTreeFlags.DiagnosticWarningNodeFlags);
            }
        }
    }
}
