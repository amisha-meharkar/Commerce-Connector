using System;
using System.Collections.Generic;

namespace Commerce_Connector_Data.DataModel;

public partial class TemplateDetail
{
    public int Id { get; set; }

    public int ImportTemplateId { get; set; }

    public string TemplateName { get; set; } = null!;

    public int ImportHeadId { get; set; }

    public int FamilyId { get; set; }

    public int PromotionTypeId { get; set; }
}
