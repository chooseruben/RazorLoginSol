using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RazorLogin.Models;
using RazorLogin.Data;
using System.Data;

namespace RazorLogin.Pages.Admin.Reports
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }


        public bool ButtonSub { get; set; } = false;
        public string startDay { get; set; } = string.Empty;
        public string endDay { get; set; } = string.Empty;


        public IList<ReportItem> PurchRep { get; set; } = new List<ReportItem>(new ReportItem[] { });
        public IList<ReportItem> TickRep { get; set; } = new List<ReportItem>(new ReportItem[] { });


        public IList<int> TotPurchaseInstances { get; set; } = new List<int>(new int[] { });
        public IList<int> TotTicketInstances { get; set; } = new List<int>(new int[] { });


        public IList<Purchase> PurchList { get; set; }
        public IList<Ticket> TickList { get; set; }


        public string ErrMsg { get; set; } = string.Empty;



        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync(string? from, string? to)
        {

            ButtonSub = true;

            if (from == null || to == null)
            {
                // no dates given
                return Page();
            }

            if (DateTime.Parse(to).Date < DateTime.Parse(from).Date)
            {
                ErrMsg = "Make sure your dates are correct. Swap them around, maybe?";
                return Page();
            }

            startDay = from;
            endDay = to;

            // Get all purchases between from and to dates
            var CustPurchases = from s in _context.Purchases.FromSqlRaw($"SELECT * FROM [dbo].[Purchase] WHERE Purchase_date >= '{from}' AND Purchase_date <= '{to}'")
                        select s;
            PurchList = await CustPurchases.ToListAsync();


            // Get all ticket purchases between from and to dates
            var CustTickets = from s in _context.Tickets.FromSqlRaw($"SELECT * FROM [dbo].[Ticket] WHERE Ticket_Purchase_date >= '{from}' AND Ticket_Purchase_date <= '{to}'")
                                select s;
            TickList = await CustTickets.ToListAsync();


            string maxDate = String.Empty;
            int maxCount = 0;

           

            for (var day = DateTime.Parse(from).Date; day.Date <= DateTime.Parse(to).Date; day = day.AddDays(1))
            {
                DateOnly daydate = DateOnly.FromDateTime(day);

                int datePurchCount = PurchList.Count(s => s.PurchaseDate == daydate);
                string date = day.ToShortDateString();
                if (datePurchCount > maxCount)
                {
                    maxCount = datePurchCount;
                    maxDate = date;
                }
                TotPurchaseInstances.Add(datePurchCount);
            }

            for (var day = DateTime.Parse(from).Date; day.Date <= DateTime.Parse(to).Date; day = day.AddDays(1))
            {
                DateOnly daydate = DateOnly.FromDateTime(day);

                int dateTickCount = TickList.Count(s => s.TicketPurchaseDate == daydate);
                string date = day.ToShortDateString();
                if (dateTickCount > maxCount)
                {
                    maxCount = dateTickCount;
                    maxDate = date;
                }
                TotTicketInstances.Add(dateTickCount);
            }




            ReportItem totalSUpload = new ReportItem();
            totalSUpload.Label = "Total number of purchases";
            totalSUpload.Value = TotPurchaseInstances.Sum().ToString();
            PurchRep.Add(totalSUpload);

            ReportItem minSUpload = new ReportItem();
            minSUpload.Label = "Least Purchases in a day";
            minSUpload.Value = TotPurchaseInstances.Min().ToString();
            PurchRep.Add(minSUpload);

            ReportItem maxSUpload = new ReportItem();
            maxSUpload.Label = "Most Purchases in a day";
            maxSUpload.Value = TotPurchaseInstances.Max().ToString();
            PurchRep.Add(maxSUpload);

            ReportItem maxSUploadDate = new ReportItem();
            maxSUploadDate.Label = "Day with most Frequent purchases";
            maxSUploadDate.Value = maxDate;
            PurchRep.Add(maxSUploadDate);

            ReportItem avgSUpload = new ReportItem();
            avgSUpload.Label = "Average daily purchses (Total number of Purchases/Calender Range)";
            avgSUpload.Value = ((float)TotPurchaseInstances.Sum() / TotPurchaseInstances.Count()).ToString();
            PurchRep.Add(avgSUpload);

            /////

            ReportItem totalSUpload2 = new ReportItem();
            totalSUpload2.Label = "Total number of Tickets sold";
            totalSUpload2.Value = TotTicketInstances.Sum().ToString();
            TickRep.Add(totalSUpload2);

            ReportItem minSUpload2 = new ReportItem();
            minSUpload2.Label = "Least Tickets sold in a day";
            minSUpload2.Value = TotTicketInstances.Min().ToString();
            TickRep.Add(minSUpload2);

            ReportItem maxSUpload2 = new ReportItem();
            maxSUpload2.Label = "Most tickets sold in a day";
            maxSUpload2.Value = TotTicketInstances.Max().ToString();
            TickRep.Add(maxSUpload2);

            ReportItem maxSUploadDate2 = new ReportItem();
            maxSUploadDate2.Label = "Day with most Frequent Ticket purchases";
            maxSUploadDate2.Value = maxDate;
            TickRep.Add(maxSUploadDate2);

            ReportItem avgSUpload2 = new ReportItem();
            avgSUpload2.Label = "Average daily Tickets (Total number of Tickets sold/Calender Range)";
            avgSUpload2.Value = ((float)TotTicketInstances.Sum() / TotTicketInstances.Count()).ToString();
            TickRep.Add(avgSUpload2);

            return Page();
        }
    }
}
