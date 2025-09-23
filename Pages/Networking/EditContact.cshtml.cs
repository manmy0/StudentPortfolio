using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Data;
using StudentPortfolio.Models;

namespace StudentPortfolio.Pages.Networking
{
    public class EditContactModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;

        public EditContactModel(StudentPortfolio.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public IndustryContactLog IndustryContactLog { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var industrycontactlog =  await _context.IndustryContactLogs.FirstOrDefaultAsync(m => m.ContactId == id);
            if (industrycontactlog == null)
            {
                return NotFound();
            }
            IndustryContactLog = industrycontactlog;
           ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(IndustryContactLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IndustryContactLogExists(IndustryContactLog.ContactId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool IndustryContactLogExists(long id)
        {
            return _context.IndustryContactLogs.Any(e => e.ContactId == id);
        }
    }
}
