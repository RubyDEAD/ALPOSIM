namespace alposim.DTO
{

    public class ReportDto
    {
        public string Period { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal COGS { get; set; } //Cost of Goods
        
        public decimal GrossProfit => Revenue - COGS;
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
    }
}