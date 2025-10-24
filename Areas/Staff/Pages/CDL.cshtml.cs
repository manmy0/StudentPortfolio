using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;

namespace StudentPortfolio.Areas.Staff.Pages
{
    [Authorize(Roles = "Staff")]
    public class CDLModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;

        public CDLModel(StudentPortfolio.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Cdl> Cdl { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Cdl = await _context.Cdls.ToListAsync();
        }
    }
}

