using ManagerData.Constants;
using ManagerData.Contexts;
using ManagerData.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManagerData.Management.Implementation;

public class PartRepository(MainDbContext database, ILogger<PartRepository> logger) : IPartRepository
{
    private static readonly int[] SourceTypeArray = [1,2,3];

    public async Task<bool> CreateEntity(PartDataModel model)
    {
        try
        {
            var partType = await database.PartTypes.FindAsync(model.PartTypeId);
            if (partType is null)
            {
                if (SourceTypeArray.Contains(model.PartTypeId))
                    await SeedPartTypes();
                else
                    return false;
            }
            await database.Parts.AddAsync(model);
            var statuses = GetDefaultStatuses(model.Id);
            foreach (var status in statuses)
            {
                await database.PartTaskStatuses.AddAsync(status);
                await database.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    private List<PartTaskStatus> GetDefaultStatuses(Guid partId)
    {
        return
        [
            new PartTaskStatus
            {
                Name = "Новые",
                Order = 0,
                GlobalStatus = (int)GlobalTaskStatus.New,
                PartRoleId = null,
                IsFixed = true,
                PartId = partId,
                PartRole = null
            },
            new PartTaskStatus
            {
                Name = "В работе",
                Order = 1,
                GlobalStatus = (int)GlobalTaskStatus.InProgress,
                PartRoleId = null,
                IsFixed = false,
                PartId = partId,
                PartRole = null
            },
            new PartTaskStatus
            {
                Name = "Завершенные",
                Order = 110,
                GlobalStatus = (int)GlobalTaskStatus.Completed,
                PartRoleId = null,
                IsFixed = true,
                PartId = partId,
                PartRole = null
            },
            new PartTaskStatus
            {
                Name = "Отмененные",
                Order = 111,
                GlobalStatus = (int)GlobalTaskStatus.Cancelled,
                PartRoleId = null,
                IsFixed = true,
                PartId = partId,
                PartRole = null
            },
        ];
    }
    
    public async Task<bool> CreateEntity(Guid masterPartId, PartDataModel model)
    {
        try
        {
            var existingPart = await database.Parts
                .Where(m => m.Name == model.Name).FirstOrDefaultAsync();
            if (existingPart != null) 
                return false;

            await CreateEntity(model);

            if (masterPartId != Guid.Empty)
            {
                await LinkEntities(masterPartId, model.Id);
            }
            await database.SaveChangesAsync();

            return true;
        }
        catch(Exception ex) 
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<bool> AddToEntity(Guid destinationId, Guid sourceId)
    {
        try
        {
            await database.PartMembers.AddAsync(
                new PartMemberDataModel()
                {
                    PartId = destinationId,
                    MemberId = sourceId,
                    Privileges = 1 //TODO: Определить уровни привилегий
                }
            );

            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<bool> RemoveFromEntity(Guid destinationId, Guid sourceId)
    {
        try
        {
            var link = await database.PartMembers
                .Where(de => de.PartId == destinationId && de.MemberId == sourceId)
                .FirstOrDefaultAsync();

            if (link == null) return false;

            database.PartMembers.Remove(link);
            
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<bool> LinkEntities(Guid masterId, Guid slaveId)
    {
        try
        {
            var master = await database.Parts
                .Where(m => m.Id == masterId).FirstOrDefaultAsync();
            var slave = await database.Parts
                .Where(m => m.Id == slaveId).FirstOrDefaultAsync();
            if (master is null || slave is null)
                return false;

            slave.MainPartId = master.Id;
            slave.Level = master.Level + 1;
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<bool> UnlinkEntities(Guid masterId, Guid slaveId)
    {
        try
        {
            var link = await database.Parts.FindAsync(slaveId);

            if (link == null && link!.MainPartId == masterId) return false;

            link.MainPartId = null;
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<PartDataModel> GetEntityById(Guid id)
    {
        try
        {
            return await database.Parts
                .Where(m => m.Id == id).FirstOrDefaultAsync() ?? new PartDataModel();
        }
        catch
        {
            return new PartDataModel();
        }
    }

    public async Task<IEnumerable<PartDataModel>?> GetEntities()
    {
        try
        {
            return await database.Parts.ToListAsync();
        }
        catch
        {
            return [];
        }
    }

    public async Task<IEnumerable<PartDataModel>?> GetEntitiesById(Guid id)
    {
        try
        {
            var parts = await database.Parts
                .Where(c => c.MainPartId == id)
                .ToListAsync();

            return parts;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }

    public async Task<bool> UpdateEntity(PartDataModel model)
    {
        try
        {
            var department = await database.Parts.Where(c => c.Id == model.Id).FirstOrDefaultAsync();

            if (department == null) return false;
            
            // TODO: Нужно написать метод, принимающий множество параметров для их валидации
            if (!string.IsNullOrEmpty(model.Name))
                department.Name = model.Name;
            if (!string.IsNullOrEmpty(model.Description))
                department.Description = model.Description;
            if (model.Level >= 0)
                department.Level = model.Level;
            if (model.MainPartId.HasValue)
                department.MainPartId = model.MainPartId == Guid.Empty ? null : model.MainPartId;
            
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }
    
    public async Task<bool> DeleteEntity(Guid id)
    {
        try
        {
            var existingPart = await database.Parts.FindAsync(id);
            if (existingPart == null) 
                return false;

            database.Parts.Remove(existingPart);
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<List<PartDataModel>> GetLinks(Guid partId)
    {
        try
        {
            return await database.Parts
                .Where(pl => pl.MainPartId == partId ||
                    pl.Parts.Any(p => p.Id == partId)).ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }

    public async Task<IEnumerable<PartMemberDataModel>> GetPartMembers(Guid partId)
    {
        try
        {
            return await database.PartMembers.Where(pm => pm.PartId == partId).ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }

    public async Task<bool> SetPrivileges(Guid userId, Guid partId, int privilege)
    {
        try
        {
            var partMember = await database.PartMembers
                .Where(pm => pm.PartId == partId && pm.MemberId == userId)
                .FirstOrDefaultAsync();
            if (partMember == null)
            {
                var member = await database.Members
                    .Where(m => m.Id == userId).FirstOrDefaultAsync();
                if (member == null)
                    return false;
                await AddToEntity(partId, member.Id);
            }
            partMember = await database.PartMembers
                .Where(pm => pm.PartId == partId && pm.MemberId == userId)
                .FirstOrDefaultAsync();
            if (partMember == null) return false;
            
            partMember.Privileges = privilege;
            await database.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    private async Task SeedPartTypes()
    {
        await AddPartType(PartTypeConstants.Root);
        await AddPartType(PartTypeConstants.Group);
        await AddPartType(PartTypeConstants.Project);
    }

    private async Task<bool> AddPartType(string name)
    {
        try
        {
            var partType = await database.PartTypes.FirstOrDefaultAsync(r => r.Name == name);

            if (partType != null) return false;

            var part = new PartType
            {
                Name = name
            };

            await database.PartTypes.AddAsync(part);
            await database.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<ICollection<PartType>> GetPartTypes()
    {
        try
        {
            var parts = await database.PartTypes.ToListAsync();
        
            if (!parts.Any())
                await SeedPartTypes();
            parts = await database.PartTypes.ToListAsync();
            return parts;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }

    public async Task<bool> AddPartTaskStatus(PartTaskStatus status)
    {
        try
        {
            var part = await database.Parts.FindAsync(status.PartId);
            if (part == null)
                return false;
            await database.PartTaskStatuses.AddAsync(status);
            await database.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<bool> ChangePartTaskStatus(PartTaskStatus status)
    {
        try
        {
            var existingStatus = await database.PartTaskStatuses
                .FirstOrDefaultAsync(s => s.PartId == status.PartId && s.Id == status.Id);
            if (existingStatus == null)
                return false;
            if (!string.IsNullOrEmpty(status.Name))
                existingStatus.Name = status.Name;
            if (!existingStatus.IsFixed && status.Order is > 0 and < 110)
                existingStatus.Order = status.Order;
            if (!existingStatus.IsFixed && status.GlobalStatus is >= 0 and <= 5)
                existingStatus.GlobalStatus = status.GlobalStatus;
            if (status.PartRoleId.HasValue && status.PartRoleId != Guid.Empty)
                existingStatus.PartRoleId = status.PartRoleId;
            if (status.PartRoleId == Guid.Empty)
                existingStatus.PartRoleId = null;
            await database.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<bool> RemovePartTaskStatus(Guid partId, Guid partTaskStatusId)
    {
        try
        {
            var existingStatus = await database.PartTaskStatuses
                .FirstOrDefaultAsync(x => x.Id == partTaskStatusId && x.PartId == partId);
            if (existingStatus == null) 
                return false;

            database.PartTaskStatuses.Remove(existingStatus);
            await database.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return false;
        }
    }

    public async Task<ICollection<PartTaskStatus>> GetPartTaskStatuses(Guid partId)
    {
        try
        {
            return await database.PartTaskStatuses
                .Where(status => status.PartId == partId).ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[{DateTime.Now}]");
            return [];
        }
    }
};