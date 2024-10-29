using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Admin.Reports
{
    public class EmployeeDataReportModel : PageModel
    {
        private readonly Index1Model _indexModel;

        public EmployeeDataReportModel(Index1Model indexModel)
        {
            _indexModel = indexModel; // Assigning the injected model
        }

        public ReportData Report => _indexModel.Report; // Property to access report data

        public async Task OnGetAsync()
        {
            await _indexModel.OnGetAsync(); // Call the data fetching method
        }
    }
}