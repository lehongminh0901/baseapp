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
using Microsoft.AspNetCore.Identity;
using demoasm2.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace demoasm2.Controllers
{
    public class BooksController : Controller
    {
        private readonly UserContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<AppUser> _userManager;

        public BooksController(UserContext context,IWebHostEnvironment hostEnvironment, UserManager<AppUser> userManager)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        // GET: Books
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Index()
        {
            AppUser user = await _userManager.GetUserAsync(HttpContext.User);
            Store store = await _context.Store.FirstOrDefaultAsync(s => s.UId == user.Id);
            var userContext = _context.Book.Where(b => b.StoreId == store.Id).ToList();
            return View(userContext);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Store)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        [Authorize (Roles="Seller")]
        public IActionResult Create()
        {
           
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Isbn,Title,Pages,Author,Category,Price,Desc,Img")] Book book)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;

            string fileName = Path.GetFileNameWithoutExtension(book.Img.FileName);

            string extension = Path.GetExtension(book.Img.FileName);
            book.ImgUrl = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            string path = Path.Combine(wwwRootPath + "/Image/", fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await book.Img.CopyToAsync(fileStream);
            }


            _context.Add(book);
            AppUser user = await _userManager.GetUserAsync(HttpContext.User);
            Store store = await _context.Store.FirstOrDefaultAsync(s => s.UId == user.Id);
            book.StoreId = store.Id;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Id", book.StoreId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Isbn,Title,Pages,Author,Category,Price,Desc,ImgUrl,StoreId")] Book book)
        {
            if (id != book.Isbn)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Isbn))
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
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Id", book.StoreId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Store)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var book = await _context.Book.FindAsync(id);
            _context.Book.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(string id)
        {
            return _context.Book.Any(e => e.Isbn == id);
        }
    }
}
