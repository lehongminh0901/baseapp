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

namespace demoasm2.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderDetailsController(UserContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            AppUser user = await _userManager.GetUserAsync(HttpContext.User);
            var userContext = _context.OrderDetail.Include(o => o.Book).Include(o => o.Order)
                .Where(o=>o.OrderId==o.Order.Id&&o.Order.UId==user.Id);
            return View(await userContext.ToListAsync());
        }

        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetail
                .Include(o => o.Book)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // GET: OrderDetails/Create
        //public async Task <IActionResult> Create(string id)
        //{
            //String user = _userManager.GetUserId(HttpContext.User);
            ////List<Order> oder = await _context.Order.Where(o=> o.UId == user)
            //string user = _userManager.GetUserId(HttpContext.User);
            //Order order = new Order() {UId = user, OrderDate = DateTime.Now };
            ////List<OrderDetail> oderDetail = new List<OrderDetail> { };
            //OrderDetail orderDetail = new OrderDetail() { BookIsbn = id, OrderId = order.Id, Quantity = 1 };
            ////order.Total = oderDetail.Book.Price * oderDetail.Quantity;
            //var myorder = _context.OrderDetail.Where(o=>o.BookIsbn==id).FirstOrDefault();
            //foreach (var item in orderDetail)
            //{

            //    if (myorder != null)
            //    {
            //        myorder.Quantity++;
            //        _context.Update(myorder);
            //        return RedirectToAction("Index", "OrderDetails");
            //    }
            //    else
            //    {
            //        _context.Add(orderDetail);
            //    }
            //    await _context.SaveChangesAsync();
            //}
            //return RedirectToAction("Index" , " Home");

        //}

        // POST: OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetail.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["BookIsbn"] = new SelectList(_context.Book, "Isbn", "Isbn", orderDetail.BookIsbn);
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Id", orderDetail.OrderId);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,BookIsbn,Quantity")] OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.OrderId))
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
            ViewData["BookIsbn"] = new SelectList(_context.Book, "Isbn", "Isbn", orderDetail.BookIsbn);
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Id", orderDetail.OrderId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetail
                .Include(o => o.Book)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetail.FindAsync(id);
            _context.OrderDetail.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetail.Any(e => e.OrderId == id);
        }
    }
}
