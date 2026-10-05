using System;
using System.Collections.Generic;

namespace Commerce_Connector_Data.DataModel;

public partial class LocationConfiguration
{
    public int Id { get; set; }

    public string? ApiEndpoint { get; set; }

    public string? ApiKey { get; set; }

    public string? ConnectionString { get; set; }

    public string? ContainerName { get; set; }

    public string? LocationType { get; set; }

    public string? Password { get; set; }

    public string? ServerAddress { get; set; }

    public string? Username { get; set; }

    public virtual ICollection<FileDetail> FileDetails { get; set; } = new List<FileDetail>();
}
