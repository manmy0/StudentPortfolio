using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Data;
using StudentPortfolio.Models;

namespace StudentPortfolio.Areas.Staff.Pages
{
    public class FeedbackDashboard2Model : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;

        public FeedbackDashboard2Model(StudentPortfolio.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Feedback> Feedback { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Feedback = await _context.Feedbacks
                .Include(f => f.CompetencyTracker)
                .Include(f => f.Goal)
                .Include(f => f.User).ToListAsync();
        }
    }
}
