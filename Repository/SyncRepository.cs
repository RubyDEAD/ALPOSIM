using alposim.Data;
using alposim.DTO;
using alposim.Interfaces;
using alposim.Models;
using Microsoft.EntityFrameworkCore;

namespace alposim.Repository;

public class SyncRepository : ISyncRepository
{
    private readonly DbContextFactory _factory;
    private CancellationTokenSource? _syncCancellationToken;
    
    public SyncRepository(DbContextFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Sync>> GetAll()
    {
        using var local = _factory.CreateLocal();
        return await local.Syncs.ToListAsync();
    }

    public async Task<Sync?> GetSyncById(Guid id)
    {
        using var local = _factory.CreateLocal();
        return await local.Syncs.FindAsync(id);
    }

    public async Task<SyncStatusDto?> GetSyncStatus(Guid id)
    {
        using var local = _factory.CreateLocal();
        var sync = await local.Syncs.FindAsync(id);
        if (sync == null) return null;

        return new SyncStatusDto
        {
            SyncId = id,
            SyncDate = sync.SyncDate,
            Status = sync.Status
        };
    }

    public async Task<IEnumerable<Sync>> GetSyncByDate(DateTime startDate, DateTime endDate)
    {
        using var local = _factory.CreateLocal();
        return await local.Syncs
            .Where(s => s.SyncDate >= startDate && s.SyncDate <= endDate)
            .ToListAsync();
    }

    private async Task SyncProducts(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var local = _factory.CreateLocal();
        using var cloud = _factory.CreateCloud();

        var localProducts = await local.Products.ToListAsync(cancellationToken);
        var cloudProductIds = await cloud.Products
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        var toInsert = localProducts
            .Where(p => !cloudProductIds.Contains(p.Id))
            .ToList();
        var toUpdate = localProducts
            .Where(p => cloudProductIds.Contains(p.Id))  // ✅ fixed
            .ToList();

        if (toInsert.Any()) cloud.Products.AddRange(toInsert);
        if (toUpdate.Any()) cloud.Products.UpdateRange(toUpdate);

        await cloud.SaveChangesAsync(cancellationToken);
    }

    private async Task SyncCategories(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var local = _factory.CreateLocal();
        using var cloud = _factory.CreateCloud();

        var localCategories = await local.Categories.ToListAsync(cancellationToken);
        var cloudCategoryIds = await cloud.Categories
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var toInsert = localCategories
            .Where(c => !cloudCategoryIds.Contains(c.Id))
            .ToList();
        var toUpdate = localCategories
            .Where(c => cloudCategoryIds.Contains(c.Id))  // ✅ fixed
            .ToList();

        if (toInsert.Any()) cloud.Categories.AddRange(toInsert);
        if (toUpdate.Any()) cloud.Categories.UpdateRange(toUpdate);

        await cloud.SaveChangesAsync(cancellationToken);
    }

    private async Task SyncSales(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var local = _factory.CreateLocal();
        using var cloud = _factory.CreateCloud();

        var localSales = await local.Sales.ToListAsync(cancellationToken);
        var cloudSaleIds = await cloud.Sales
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        var toInsert = localSales
            .Where(s => !cloudSaleIds.Contains(s.Id))
            .ToList();
        var toUpdate = localSales
            .Where(s => cloudSaleIds.Contains(s.Id))  // ✅ fixed
            .ToList();

        if (toInsert.Any()) cloud.Sales.AddRange(toInsert);
        if (toUpdate.Any()) cloud.Sales.UpdateRange(toUpdate);

        await cloud.SaveChangesAsync(cancellationToken);
    }

    private async Task SyncUsers(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var local = _factory.CreateLocal();
        using var cloud = _factory.CreateCloud();

        var localUsers = await local.Users.ToListAsync(cancellationToken);
        var cloudUserIds = await cloud.Users  // ✅ fixed - was querying local
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        var toInsert = localUsers
            .Where(u => !cloudUserIds.Contains(u.Id))
            .ToList();
        var toUpdate = localUsers
            .Where(u => cloudUserIds.Contains(u.Id))  // ✅ fixed
            .ToList();

        if (toInsert.Any()) cloud.Users.AddRange(toInsert);
        if (toUpdate.Any()) cloud.Users.UpdateRange(toUpdate);

        await cloud.SaveChangesAsync(cancellationToken);
    }

    public async Task<Sync> StartSync()
    {
        _syncCancellationToken = new CancellationTokenSource();

        using var local = _factory.CreateLocal();
        using var cloud = _factory.CreateCloud();
        var sync = new Sync
        {
            SyncId = Guid.NewGuid(),
            SyncDate = DateTime.UtcNow,
            Status = SyncStatus.SyncLoading,
            Method =  SyncMethod.Pushed
        };

        local.Syncs.Add(sync);
        cloud.Syncs.Add(sync);
        await local.SaveChangesAsync();
        await cloud.SaveChangesAsync();
        try
        {
            await SyncProducts(_syncCancellationToken.Token);
            await SyncCategories(_syncCancellationToken.Token);
            await SyncSales(_syncCancellationToken.Token);
            await SyncUsers(_syncCancellationToken.Token);

            sync.Status = SyncStatus.Synced;
        }
        catch (OperationCanceledException)
        {
            sync.Status = SyncStatus.NotSynced;
            Console.WriteLine("Sync was cancelled.");
        }
        catch (Exception ex)
        {
            sync.Status = SyncStatus.NotSynced;
            Console.WriteLine($"Sync failed: {ex.Message}");
        }

        await local.SaveChangesAsync();
        await cloud.SaveChangesAsync();
        return sync;
    }

    public async Task<Sync?> StopSync()
    {
        using var local = _factory.CreateLocal();

        var activeSync = await local.Syncs
            .Where(s => s.Status == SyncStatus.SyncLoading)
            .OrderByDescending(s => s.SyncDate)
            .FirstOrDefaultAsync();

        if (activeSync == null) return null;

        _syncCancellationToken?.Cancel();
        activeSync.Status = SyncStatus.NotSynced;
        await local.SaveChangesAsync();
        return activeSync;
    }
    
    public async Task<Sync> PullSync()
    {
        var sync = new Sync
        {
            SyncId = Guid.NewGuid(),
            SyncDate = DateTime.UtcNow,
            Status = SyncStatus.SyncLoading,
            Method = SyncMethod.Pulled,
        };

        using var local = _factory.CreateLocal();
        using var cloud = _factory.CreateCloud();

        local.Syncs.Add(sync);
        cloud.Syncs.Add(sync);
        await local.SaveChangesAsync();
        await cloud.SaveChangesAsync();
        try
        {
            // pull products from cloud to local
            var cloudProducts = await cloud.Products.ToListAsync();
            var localProductIds = await local.Products.Select(p => p.Id).ToListAsync();

            var toInsert = cloudProducts.Where(p => !localProductIds.Contains(p.Id)).ToList();
            var toUpdate = cloudProducts.Where(p => localProductIds.Contains(p.Id)).ToList();

            if (toInsert.Any()) local.Products.AddRange(toInsert);
            if (toUpdate.Any()) local.Products.UpdateRange(toUpdate);

            // pull categories
            var cloudCategories = await cloud.Categories.ToListAsync();
            var localCategoryIds = await local.Categories.Select(c => c.Id).ToListAsync();

            var categoriesToInsert = cloudCategories.Where(c => !localCategoryIds.Contains(c.Id)).ToList();
            var categoriesToUpdate = cloudCategories.Where(c => localCategoryIds.Contains(c.Id)).ToList();

            if (categoriesToInsert.Any()) local.Categories.AddRange(categoriesToInsert);
            if (categoriesToUpdate.Any()) local.Categories.UpdateRange(categoriesToUpdate);

            // pull sales
            var cloudSales = await cloud.Sales.ToListAsync();
            var localSaleIds = await local.Sales.Select(s => s.Id).ToListAsync();

            var salesToInsert = cloudSales.Where(s => !localSaleIds.Contains(s.Id)).ToList();
            var salesToUpdate = cloudSales.Where(s => localSaleIds.Contains(s.Id)).ToList();

            if (salesToInsert.Any()) local.Sales.AddRange(salesToInsert);
            if (salesToUpdate.Any()) local.Sales.UpdateRange(salesToUpdate);

            await local.SaveChangesAsync();
            sync.Status = SyncStatus.Synced;
        }
        catch (Exception ex)
        {
            sync.Status = SyncStatus.NotSynced;
            Console.WriteLine($"Pull sync failed: {ex.Message}");
        }

        await local.SaveChangesAsync();
        await cloud.SaveChangesAsync();
        return sync;
    }
}