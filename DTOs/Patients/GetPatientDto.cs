namespace APBD_DatabaseFirst.DTOs.Patients;

public class GetPatientDto
{
    public string Pesel { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public string Sex { get; set; } = null!;
    public List<AdmissionDto> Admissions { get; set; } = new();
    public List<BedAssignmentDto> BedAssignments { get; set; } = new();
}