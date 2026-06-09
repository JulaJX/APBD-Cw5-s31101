using APBD_DatabaseFirst.Data;
using APBD_DatabaseFirst.DTOs.BedAssignments;
using APBD_DatabaseFirst.DTOs.Patients;
using APBD_DatabaseFirst.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_DatabaseFirst.Services;

public class PatientService : IPatientService
{
    private readonly HospitalDbContext _context;

    public PatientService(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<GetPatientDto>> GetPatientsAsync(string? search)
    {
        var query = _context.Patients
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            query = query.Where(p =>
                EF.Functions.Like(p.FirstName, pattern) ||
                EF.Functions.Like(p.LastName, pattern));
        }

        var result = await query
            .Select(p => new GetPatientDto
            {
                Pesel = p.Pesel,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Age = p.Age,
                Sex = p.Sex ? "Male" : "Female",
                Admissions = p.Admissions.Select(a => new AdmissionDto
                {
                    Id = a.Id,
                    AdmissionDate = a.AdmissionDate,
                    DischargeDate = a.DischargeDate,
                    Ward = new WardDto
                    {
                        Id = a.Ward.Id,
                        Name = a.Ward.Name,
                        Description = a.Ward.Description
                    }
                }).ToList(),
                BedAssignments = p.BedAssignments.Select(ba => new BedAssignmentDto
                {
                    Id = ba.Id,
                    From = ba.From,
                    To = ba.To,
                    Bed = new BedDto
                    {
                        Id = ba.Bed.Id,
                        BedType = new BedTypeDto
                        {
                            Id = ba.Bed.BedType.Id,
                            Name = ba.Bed.BedType.Name,
                            Description = ba.Bed.BedType.Description
                        },
                        Room = new RoomDto
                        {
                            Id = ba.Bed.Room.Id,
                            HasTv = ba.Bed.Room.HasTv,
                            Ward = new WardDto
                            {
                                Id = ba.Bed.Room.Ward.Id,
                                Name = ba.Bed.Room.Ward.Name,
                                Description = ba.Bed.Room.Ward.Description
                            }
                        }
                    }
                }).ToList()
            })
            .ToListAsync();

        return result;
    }

public async Task<CreateBedAssignmentResponseDto> AssignBedAsync(string pesel, CreateBedAssignmentDto request)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Pesel == pesel);
        if (patient is null)
            throw new KeyNotFoundException("Patient not found.");

        if (request.To.HasValue && request.To <= request.From)
            throw new ArgumentException("Field 'to' must be later than 'from'.");

        var candidateBeds = await _context.Beds
            .Include(b => b.BedType)
            .Include(b => b.Room)
                .ThenInclude(r => r.Ward)
            .Where(b => b.BedType.Name == request.BedType && b.Room.Ward.Name == request.Ward)
            .ToListAsync();

        if (!candidateBeds.Any())
            throw new KeyNotFoundException("No beds found for the given bed type and ward.");

        Bed? freeBed = null;

        foreach (var bed in candidateBeds)
        {
            var hasConflict = await _context.BedAssignments.AnyAsync(ba =>
                ba.BedId == bed.Id &&
                (
                    request.To == null
                        ? (ba.To == null || ba.To > request.From)
                        : ba.From < request.To && (ba.To == null || ba.To > request.From)
                ));

            if (!hasConflict)
            {
                freeBed = bed;
                break;
            }
        }

        if (freeBed is null)
            throw new KeyNotFoundException("No free bed available in the given period.");

        var assignment = new BedAssignment
        {
            PatientPesel = pesel,
            BedId = freeBed.Id,
            From = request.From,
            To = request.To
        };

        _context.BedAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        return new CreateBedAssignmentResponseDto
        {
            Id = assignment.Id,
            PatientPesel = assignment.PatientPesel,
            BedId = assignment.BedId,
            From = assignment.From,
            To = assignment.To
        };
    }
}