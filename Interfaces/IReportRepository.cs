using alposim.DTO;

namespace alposim.Interfaces
{
    
    public interface IReportRepository
    {
        Task<ReportDto> GetDailyReportsAsync(DateTime date);
        Task<IEnumerable<ReportDto>> GetWeeklyReportsAsync(int month, int year);
        Task<IEnumerable<ReportDto>> GetMonthlyReportsAsync(int year);
        Task<IEnumerable<ReportDto>> GetYearlyReportsAsync();
    }
}