using System;
using System.Collections.Generic;

namespace Commerce_Connector_Data.DataModel;

public partial class PropertyMapping
{
    public int MappingId { get; set; }

    public string ModelType { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string LocationType { get; set; } = null!;

    public string MappingsJson { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
