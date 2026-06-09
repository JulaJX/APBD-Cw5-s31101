using APBD_DatabaseFirst.DTOs.BedAssignments;
using APBD_DatabaseFirst.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_DatabaseFirst.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPatients([FromQuery] string? search)
    {
        var result = await _patientService.GetPatientsAsync(search);
        return Ok(result);
    }

    [HttpPost("{pesel}/bedassignments")]
    public async Task<IActionResult> AssignBed(string pesel, [FromBody] CreateBedAssignmentDto request)
    {
        try
        {
            var result = await _patientService.AssignBedAsync(pesel, request);
            return Created($"/api/patients/{pesel}/bedassignments", result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}