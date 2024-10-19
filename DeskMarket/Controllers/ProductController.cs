using DeskMarket.Data;
using DeskMarket.Data.Models;
using DeskMarket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using static DeskMarket.Common.Constants;

namespace DeskMarket.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext context;

        public ProductController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new ProductViewModel();
            model.Categories = await GetCategories();


            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            DateTime addedOn;

            if (!DateTime.TryParseExact(model.AddedOn, ProductAddedOnDateFormat,
                CultureInfo.CurrentCulture, DateTimeStyles.None, out addedOn))
            {
                throw new InvalidOperationException("Invalid date or time format");
            }
            var product = new Product
            {
                ProductName = model.ProductName,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                SellerId = GetCurrentUserId() ?? string.Empty,
                AddedOn = addedOn,
                CategoryId = model.CategoryId
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            return RedirectToAction("Index", "Product");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await context.Products
                .Where(p => !p.IsDeleted)
                .Select(p => new ProductIndexViewModel
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl ?? string.Empty,
                    IsSeller = p.SellerId == GetCurrentUserId(),
                    HasBought = p.ProductsClients.Any(pc => pc.ClientId != GetCurrentUserId())
                })
                .ToListAsync();


            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Cart(int id)
        {
            string CurrentUserId = GetCurrentUserId() ?? string.Empty;
            var model = await context.Products
                .Where(p => p.ProductsClients.Any(pc => pc.ClientId == CurrentUserId))
                .Select(p => new ProductCartViewModel
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await context.Products
                .Where(g => g.Id == id)
                .Where(p => p.IsDeleted == false)
                //.Include(p => p.ProductsClients)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            string CurrentUserId = GetCurrentUserId() ?? string.Empty;

            if (product.ProductsClients.Any(pc => pc.ClientId == CurrentUserId))
            {
                return BadRequest();
            }

            product.ProductsClients.Add(new ProductClient
            {
                ProductId = id,
                ClientId = CurrentUserId
            });

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            Product? product = await context.Products
                .Where(p => p.Id == id)
                .Where(p => p.IsDeleted == false)
                .Include(p => p.ProductsClients)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            string CurrentUserId = GetCurrentUserId() ?? string.Empty;

            ProductClient? productClient = product.ProductsClients
                .FirstOrDefault(pc => pc.ClientId == CurrentUserId);

            if (productClient != null)
            {
                product.ProductsClients.Remove(productClient);

                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Cart));
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await context.Products
                .Where(p => p.IsDeleted == false)
                .Where(p => p.Id == id)
                .Select(p => new ProductDetailsViewModel
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryName = p.Category.Name,
                    Seller = p.Seller.UserName,
                    HasBought = p.ProductsClients.Any(pc => pc.ClientId != GetCurrentUserId())
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await context.Products
                .Where(p => p.IsDeleted == false)
                .Where(p => p.Id == id)
                .AsNoTracking()
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    AddedOn = p.AddedOn.ToString(ProductAddedOnDateFormat),
                    CategoryId = p.CategoryId,
                    Categories = GetCategories().Result,
                    SellerId = p.SellerId
                })
                .FirstOrDefaultAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            DateTime addedOn;

            if (!DateTime.TryParseExact(model.AddedOn, ProductAddedOnDateFormat,
               CultureInfo.CurrentCulture, DateTimeStyles.None, out addedOn))
            {
                ModelState.AddModelError(nameof(ProductViewModel.AddedOn), "Invalid date or time format");

                return View(model);
            }

            Product? product = await context.Products
                   .Where(s => s.IsDeleted == false)
                   .FirstOrDefaultAsync(s => s.Id == id);

            product.ProductName = model.ProductName;
            product.Description = model.Description;
            product.Price = model.Price;
            product.ImageUrl = model.ImageUrl;
            product.AddedOn = addedOn;
            product.CategoryId = model.CategoryId;
            product.SellerId = GetCurrentUserId() ?? string.Empty;

            await context.SaveChangesAsync();

            return RedirectToAction("Index", "Product");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await context.Products
                .Where(p => p.IsDeleted == false)
                .Where(p => p.Id == id)
                .Select(p => new DeleteProductViewModel
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    SellerId = p.SellerId,
                    Seller = p.Seller.UserName
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteProductViewModel model)
        {
            Product? product = await context.Products
                .Where(p => p.IsDeleted == false)
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            if (product == null)
            {
                return NotFound();
            }

            product.IsDeleted = true;

            await context.SaveChangesAsync();

            return RedirectToAction("Index", "Product");
        }
        private async Task<List<Category>> GetCategories()
        {
            return await context.Categories.ToListAsync();
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
