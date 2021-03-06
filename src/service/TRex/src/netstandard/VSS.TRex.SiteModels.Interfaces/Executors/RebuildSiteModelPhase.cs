﻿namespace VSS.TRex.SiteModels.Interfaces.Executors
{
  public enum RebuildSiteModelPhase : byte
  {
    Unknown,

    /// <summary>
    /// The initial deletion phase is in progress
    /// </summary>
    Deleting,

    /// <summary>
    /// Source location containing TAG files is being scanned for appropriate TAG files and other data
    /// required to support the rebuilding process
    /// </summary>
    Scanning,

    /// <summary>
    /// TAG files are currently being submitted into the project
    /// </summary>
    Submitting,

    /// <summary>
    /// All TAG files have been submitted, and monitoring of progress is underway
    /// </summary>
    Monitoring,

    /// <summary>
    /// Performing cleanup operations prior to the process being completed
    /// </summary>
    Completion,

    /// <summary>
    /// All site model rebuild operations are complete
    /// </summary>
    Complete
  }
}
