#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Testdemo.Models;
using demoasm2.Data;
using demoasm2.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace demoasm2.Controllers
{
    public class StoresController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<AppUser> _userManager;

        public StoresController(UserContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Check()
        {
            AppUser user = await _userManager.GetUserAsync(HttpContext.User);
            Store store = await _context.Store.FirstOrDefaultAsync(s => s.UId == user.Id);


            if (store != null)
            {

                return RedirectToAction("Index", "Books");

            }
            else
            {
                return RedirectToAction("Create", "Stores");
            }
        }

        // GET: Stores
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Index()
        {
            AppUser user = await _userManager.GetUserAsync(HttpContext.User);
            var userContext = _context.Store.Where(s => s.UId == user.Id).FirstOrDefault();
            return View(userContext);
        }

        // GET: Stores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // GET: Stores/Create
        [Authorize(Roles = "Seller")]
        public IActionResult Create()
        {
            
            return View();
        }

        // POST: Stores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Slogan")] Store store)
        {
            AppUser user = await _userManager.GetUserAsync(HttpContext.User);

            _context.Add(store);
            store.UId = user.Id;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Stores/Edit/5
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            ViewData["UId"] = new SelectList(_context.Users, "Id", "Id", store.UId);
            return View(store);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Slogan,UId")] Store store)
        {
            if (id != store.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(store);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UId"] = new SelectList(_context.Users, "Id", "Id", store.UId);
            return View(store);
        }

        // GET: Stores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var store = await _context.Store.FindAsync(id);
            _context.Store.Remove(store);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreExists(int id)
        {
            return _context.Store.Any(e => e.Id == id);
        }
    }
}
