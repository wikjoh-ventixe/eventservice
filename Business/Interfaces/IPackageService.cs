using Business.Dtos;
using Business.Models;

namespace Business.Interfaces;

public interface IPackageService
{
    Task<PackageResult<Package?>> CreatePackageAsync(CreatePackageRequestDto request);
    Task<PackageResult<bool>> DeletePackageAsync(int id);
    Task<PackageResult<IEnumerable<Package>>> GetAllPackagesByEventId(string eventId);
    Task<PackageResult<Package>> UpdatePackageAsync(UpdatePackageRequestDto request);
}
