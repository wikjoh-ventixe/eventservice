using Data.Context;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class PackageRepository(EventDbContext context) : BaseRepository<PackageEntity>(context), IPackgeRepository
{
}