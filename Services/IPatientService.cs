using APBD_DatabaseFirst.DTOs;
using APBD_DatabaseFirst.DTOs.BedAssignments;
using APBD_DatabaseFirst.DTOs.Patients;

namespace APBD_DatabaseFirst.Services;

public interface IPatientService
{
    Task<IEnumerable<GetPatientDto>> GetPatientsAsync(string? search);
    Task<CreateBedAssignmentResponseDto> AssignBedAsync(string pesel, CreateBedAssignmentDto request);
}