using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Data;
using StudentPortfolio.Models;

namespace StudentPortfolio.Pages.CDL
{
    [Authorize]
    public class CDLModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;

        public CDLModel(StudentPortfolio.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Cdl> Cdl { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Cdl = await _context.Cdls.ToListAsync();
        }
    }
}
