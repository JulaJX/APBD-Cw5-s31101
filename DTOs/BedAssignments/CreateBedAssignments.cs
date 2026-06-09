namespace APBD_DatabaseFirst.DTOs.BedAssignments;

public class CreateBedAssignmentDto
{
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
    public string BedType { get; set; } = null!;
    public string Ward { get; set; } = null!;
}