public class CreateBedAssignmentResponseDto
{
    public int Id { get; set; }
    public string PatientPesel { get; set; } = null!;
    public int BedId { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}