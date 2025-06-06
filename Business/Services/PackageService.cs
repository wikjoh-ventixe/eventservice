using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using System.Diagnostics;
using System.Reflection;

namespace Business.Services;

public class PackageService(IPackageRepository packageRepository) : IPackageService
{
    private readonly IPackageRepository _packageRepository = packageRepository;


    // CREATE
    public async Task<PackageResult<Package?>> CreatePackageAsync(CreatePackageRequestDto request)
    {
        if (request == null)
            return PackageResult<Package?>.BadRequest("Request cannot be null.");

        try
        {
            var packageEntity = new PackageEntity
            {
                Title = request.Title,
                EventId = request.EventId,
                SeatingArrangement = request.SeatingArrangement,
                Placement = request.Placement,
                Price = request.Price,
                Currency = request.Currency,
            };

            await _packageRepository.AddAsync(packageEntity);
            var result = await _packageRepository.SaveAsync();

            if (!result.Succeeded)
                return PackageResult<Package?>.InternalServerError($"Failed creating package. {result.ErrorMessage}");

            var createdPackageEntity = (await _packageRepository.GetOneAsync(x => x.Id == packageEntity.Id)).Data;
            if (createdPackageEntity == null)
                return PackageResult<Package?>.InternalServerError($"Failed retrieving package entity after creation.");

            var packageModel = new Package
            {
                Id = createdPackageEntity.Id,
                Title = createdPackageEntity.Title,
                SeatingArrangement = createdPackageEntity.SeatingArrangement,
                Placement = createdPackageEntity.Placement,
                Price = createdPackageEntity.Price,
                Currency = createdPackageEntity.Currency,
            };

            return PackageResult<Package?>.Created(packageModel);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return PackageResult<Package?>.InternalServerError($"Exception occurred in {MethodBase.GetCurrentMethod()!.Name}.");
        }
    }


    // READ
    public async Task<PackageResult<IEnumerable<Package>>> GetAllPackagesByEventId(string eventId)
    {
        var result = await _packageRepository.GetAllAsync(
            orderByDescending: false,
            sortBy: x => x.Price!,
            where: x => x.EventId == eventId);

        if (!result.Succeeded)
            return PackageResult<IEnumerable<Package>>.InternalServerError($"Failed retrieving package entities. {result.ErrorMessage}");

        var entities = result.Data;
        var packages = entities?.Select(x => new Package
        {
            Id = x.Id,
            EventId = x.EventId,
            Title = x.Title,
            SeatingArrangement = x.SeatingArrangement,
            Placement = x.Placement,
            Price = x.Price,
            Currency = x.Currency,
        });

        return PackageResult<IEnumerable<Package>>.Ok(packages!);
    }


    // UPDATE
    public async Task<PackageResult<Package>> UpdatePackageAsync(UpdatePackageRequestDto request)
    {
        if (request == null)
            return PackageResult<Package>.BadRequest("Request cannot be null.");

        try
        {
            var existingResult = await _packageRepository.GetOneAsync(x => x.Id == request.Id);
            if (!existingResult.Succeeded || existingResult.Data == null)
                return PackageResult<Package>.NotFound($"Package with id {request.Id} not found.");

            var entity = existingResult.Data;
            entity.Title = request.Title;
            entity.SeatingArrangement = request.SeatingArrangement;
            entity.Placement = request.Placement;
            entity.Price = request.Price;
            entity.Currency = request.Currency;

            _packageRepository.Update(entity);
            var result = await _packageRepository.SaveAsync();
            if (!result.Succeeded)
                return PackageResult<Package>.InternalServerError($"Failed updating package. {result.ErrorMessage}");

            var updatedResult = await _packageRepository.GetOneAsync(x => x.Id == request.Id);
            if (!updatedResult.Succeeded || updatedResult.Data == null)
                return PackageResult<Package>.InternalServerError("Failed retrieving entity after update.");

            var updatedEntity = updatedResult.Data;
            var updatedPackage = new Package
            {
                Id = updatedEntity.Id,
                EventId = updatedEntity.EventId,
                Title = updatedEntity.Title,
                SeatingArrangement = updatedEntity.SeatingArrangement,
                Placement = updatedEntity.Placement,
                Price = updatedEntity.Price,
                Currency = updatedEntity.Currency,
            };

            return PackageResult<Package>.Ok(updatedPackage);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return PackageResult<Package>.InternalServerError($"Exception occurred in {MethodBase.GetCurrentMethod()!.Name}.");
        }
    }


    // DELETE
    public async Task<PackageResult<bool>> DeletePackageAsync(int id)
    {
        try
        {
            var getResult = await _packageRepository.GetOneAsync(x => x.Id == id);
            if (!getResult.Succeeded || getResult.Data == null)
                return PackageResult<bool>.NotFound($"Package with id {id} not found.");

            var entity = getResult.Data;
            _packageRepository.Delete(entity);
            var result = await _packageRepository.SaveAsync();
            if (!result.Succeeded)
                return PackageResult<bool>.InternalServerError($"Failed deleting package with id {id}.");

            return PackageResult<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return PackageResult<bool>.InternalServerError($"Exception occurred in {MethodBase.GetCurrentMethod()!.Name}.");
        }
    }
}
