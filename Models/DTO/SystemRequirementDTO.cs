using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class SystemRequirementDTO
{
    public int SystemRequirementID { get; set; }
    public int GameID { get; set; }
    public string DeviceName { get; set; } = null!;
    public string RequirementType { get; set; } = null!;
    public string ? OperatingSystem { get; set; }
    public string? AdditionalNotes { get; set; }
    public string? Network { get; set; }
    public string? SoundCard { get; set; }
    public string? Storage { get; set; }
    public string? DirectX { get; set; }
    public string? Graphics { get; set; }
    public string? Memory { get; set; }
    public string? Processor { get; set; }


}
