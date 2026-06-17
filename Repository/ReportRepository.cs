using alposim.Data;
using alposim.DTO;
using alposim.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace alposim.Repository;

public class ReportRepository : IReportRepository
{
    private readonly LocalDbContext _context;

    public ReportRepository(LocalDbContext context)
    {
        _context = context;
    }

    public async Task<ReportDto> GetDailyReportsAsync(DateTime date)
    {
        var startDate = date.Date;                    // 2024-01-01 00:00:00
        var endDate = date.Date.AddDays(1).AddTicks(-1); // 2024-01-01 23:59:59

        var sales = await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .ToListAsync();

        return new ReportDto
        {
            Period = date.ToString("MMM dd, yyyy"),
            PeriodStartDate = startDate,
            PeriodEndDate = endDate,
            Revenue = sales.Sum(s => s.TotalPrice),
            COGS = sales.SelectMany(s => s.Items)
                        .Sum(i => i.UnitPrice * i.Quantity)
        };
    }

    public async Task<IEnumerable<ReportDto>> GetWeeklyReportsAsync(int month, int year)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        var sales = await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.CreatedAt >= startOfMonth && s.CreatedAt <= endOfMonth)
            .ToListAsync();

        return sales
            .GroupBy(s => GetWeekOfMonth(s.CreatedAt))
            .OrderBy(g => g.Key)
            .Select(g => new ReportDto
            {
                Period = $"Week {g.Key}",
                PeriodStartDate = g.Min(s => s.CreatedAt),
                PeriodEndDate = g.Max(s => s.CreatedAt),
                Revenue = g.Sum(s => s.TotalPrice),
                COGS = g.SelectMany(s => s.Items)
                        .Sum(i => i.UnitPrice * i.Quantity)
            });
    }

    public async Task<IEnumerable<ReportDto>> GetMonthlyReportsAsync(int year)
    {
        var startOfYear = new DateTime(year, 1, 1);
        var endOfYear = new DateTime(year, 12, 31);

        var sales = await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.CreatedAt >= startOfYear && s.CreatedAt <= endOfYear)
            .ToListAsync();

        return sales
            .GroupBy(s => s.CreatedAt.Month)
            .OrderBy(g => g.Key)
            .Select(g => new ReportDto
            {
                Period = new DateTime(year, g.Key, 1).ToString("MMM yyyy"), // "Jan 2024"
                PeriodStartDate = new DateTime(year, g.Key, 1),
                PeriodEndDate = new DateTime(year, g.Key, 1).AddMonths(1).AddDays(-1),
                Revenue = g.Sum(s => s.TotalPrice),
                COGS = g.SelectMany(s => s.Items)
                        .Sum(i => i.UnitPrice * i.Quantity)
            });
    }

    public async Task<IEnumerable<ReportDto>> GetYearlyReportsAsync()
    {
        var sales = await _context.Sales
            .Include(s => s.Items)
            .ToListAsync();

        return sales
            .GroupBy(s => s.CreatedAt.Year)
            .OrderBy(g => g.Key)
            .Select(g => new ReportDto
            {
                Period = g.Key.ToString(),           // "2024", "2025"
                PeriodStartDate = new DateTime(g.Key, 1, 1),
                PeriodEndDate = new DateTime(g.Key, 12, 31),
                Revenue = g.Sum(s => s.TotalPrice),
                COGS = g.SelectMany(s => s.Items)
                        .Sum(i => i.UnitPrice * i.Quantity)
            });
    }
    private int GetWeekOfMonth(DateTime date)
    {
        var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
        return (int)Math.Ceiling((date.Day + (int)firstDayOfMonth.DayOfWeek) / 7.0);
    }
}