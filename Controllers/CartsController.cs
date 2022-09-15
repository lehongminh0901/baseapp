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
    public class CartsController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CartsController(UserContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            string user = _userManager.GetUserId(HttpContext.User);
            var userContext = _context.Cart.Include(c => c.Book)
                .Include(c => c.User)
                .Where(c => c.BookIsbn == c.Book.Isbn && c.UId == user);
            return View(await userContext.ToListAsync());
        }

        /*[HttpPost]
        [ValidateAntiForgeryToken]*/
        public async Task<IActionResult> Checkout()
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            List<Cart> myDetailsInCart = await _context.Cart
                .Where(c => c.UId == thisUserId)
                .Include(c => c.Book)
                .ToListAsync();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //Step 1: create an order
                    Order myOrder = new Order();
                    myOrder.UId = thisUserId;
                    myOrder.OrderDate = DateTime.Now;
                    myOrder.Total = myDetailsInCart.Select(c => c.Total)
                        .Aggregate((c1, c2) => c1 + c2);
                    _context.Add(myOrder);
                    await _context.SaveChangesAsync();

                    //Step 2: insert all order details by var "myDetailsInCart"
                    foreach (var item in myDetailsInCart)
                    {
                        OrderDetail detail = new OrderDetail()
                        {
                            OrderId = myOrder.Id,
                            BookIsbn = item.BookIsbn,
                            Quantity = item.Quantity
                        };
                        _context.Add(detail);
                    }
                    await _context.SaveChangesAsync();

                    //Step 3: empty/delete the cart we just done for thisUser
                    _context.Cart.RemoveRange(myDetailsInCart);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error occurred in Checkout" + ex);
                }
            }
            return RedirectToAction("Index", "Carts");
        
        }
        
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Book)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.UId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        [Authorize]
        public async Task<IActionResult> Create(string id,Cart cart)
        {
            string user = _userManager.GetUserId(HttpContext.User);
            
            var mycart = _context.Cart.Where(c => c.UId == user && c.BookIsbn == id).FirstOrDefault();
            var book = _context.Book.Where(b=>b.Isbn == id).FirstOrDefault();
            if (mycart == null)
            {
                mycart.UId = user;
                mycart.BookIsbn = id;
                mycart.Quantity = 1;
                mycart.Total = (double)(mycart.Quantity * book.Price);
                _context.Add(cart);

            }
            else
            {
                mycart.Quantity++;
                mycart.Total = (double)(mycart.Quantity * book.Price);

                _context.Update(mycart);
                
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Details" + id, "Home");
            //return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("UId,BookIsbn")] Cart cart)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(cart);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["BookIsbn"] = new SelectList(_context.Book, "Isbn", "Isbn", cart.BookIsbn);
        //    ViewData["UId"] = new SelectList(_context.Users, "Id", "Id", cart.UId);
        //    return View(cart);
        //}

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["BookIsbn"] = new SelectList(_context.Book, "Isbn", "Isbn", cart.BookIsbn);
            ViewData["UId"] = new SelectList(_context.Users, "Id", "Id", cart.UId);
            return View(cart);
        }
        public async Task<IActionResult> UpLoad(string id, int q, Cart cart)
        {
            string user = _userManager.GetUserId(HttpContext.User);
            cart = _context.Cart.Where(c => c.BookIsbn == id && c.UId == user).FirstOrDefault();
            var book = _context.Book.Where(b => b.Isbn == id).FirstOrDefault();
            cart.Quantity = q;
            _context.Update(cart);
            cart.Total= (double)(cart.Quantity * book.Price);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        // GET: Carts/Details/5
       /* public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Book)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.UId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }*/

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UId,BookIsbn")] Cart cart)
        {
            if (id != cart.UId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.UId))
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
            ViewData["BookIsbn"] = new SelectList(_context.Book, "Isbn", "Isbn", cart.BookIsbn);
            ViewData["UId"] = new SelectList(_context.Users, "Id", "Id", cart.UId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Book)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.UId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cart = await _context.Cart.FindAsync(id);
            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(string id)
        {
            return _context.Cart.Any(e => e.UId == id);
        }
    }
}
