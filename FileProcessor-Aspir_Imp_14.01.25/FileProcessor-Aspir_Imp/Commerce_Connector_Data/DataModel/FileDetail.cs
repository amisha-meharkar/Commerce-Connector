using System;
using System.Collections.Generic;

namespace Commerce_Connector_Data.DataModel;

public partial class FileDetail
{
    public int JobId { get; set; }

    public string? FilePath { get; set; }

    public string? FileType { get; set; }

    public string? LocationType { get; set; }

    public string? ModelType { get; set; }

    public string? MappingsJson { get; set; }

    public int? LocationConfigId { get; set; }

    public virtual LocationConfiguration? LocationConfig { get; set; }
}
