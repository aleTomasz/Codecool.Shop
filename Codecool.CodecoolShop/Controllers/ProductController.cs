using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Codecool.CodecoolShop.Daos;
using Codecool.CodecoolShop.Daos.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Codecool.CodecoolShop.Models;
using Codecool.CodecoolShop.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Codecool.CodecoolShop.Utilities;

namespace Codecool.CodecoolShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        public ProductService ProductService { get; set; }

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
            ProductService = new ProductService(
                ProductDaoMemory.GetInstance(),
                ProductCategoryDaoMemory.GetInstance(),
                SupplierDaoMemory.GetInstance()
                );
        }

        public IActionResult Index(int? categoryId)
        {
            var categories = ProductService.GetAllCategories();
            ViewBag.Categories = categories;


            /*            var products = ProductService.GetProductsForCategory(1);*/
            // Sprawdzanie, czy categoryId jest przekazane
            var products = categoryId.HasValue
                ? ProductService.GetProductsForCategory(categoryId.Value)
                : ProductService.GetAllProducts(); // Wyœwietlanie wszystkich produktów, jeœli categoryId nie jest przekazane




            // Obliczanie liczby produktów w koszyku
            var order = GetOrderFromSession();
            int cartItemCount = order.Items.Sum(item => item.Quantity);  // Suma iloœci wszystkich elementów w koszyku

            // Przekazywanie liczby produktów w koszyku do widoku
            ViewBag.CartItemCount = cartItemCount;



            return View(products.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult ProductsBySupplier(int? supplierId)
        {
            var suppliers = ProductService.GetAllSuppliers();
            ViewBag.Suppliers = suppliers;

            var products = supplierId.HasValue
                ? ProductService.GetProductsForSupplier(supplierId.Value)
                : ProductService.GetAllProducts();



            // Obliczanie liczby produktów w koszyku
            var order = GetOrderFromSession();
            int cartItemCount = order.Items.Sum(item => item.Quantity);

            // Przekazywanie liczby produktów w koszyku do widoku
            ViewBag.CartItemCount = cartItemCount;




            return View(products.ToList());

        }
        public IActionResult AddToCart(int productId)
        {
            var product = ProductService.GetAllProducts().FirstOrDefault(p => p.Id == productId);

            var order = GetOrderFromSession();

            var listItem = order.Items.FirstOrDefault(item => item.Product.Id == productId);
            if (listItem != null)
            {
                listItem.Quantity++;
            }
            else
            {
                order.Items.Add(new ListItems { Product = product, Quantity = 1 });
            }
            SaveOrderToSession(order);
            TempData["SuccessMessage"] = "Product added to cart!";

            return RedirectToAction("Index");
        }
        private Order GetOrderFromSession()
        {
            // Odczyt zamówienia z sesji
            var order = HttpContext.Session.Get<Order>("Order");
            if (order == null)
            {
                order = new Order();
                HttpContext.Session.Set("Order", order);  // Zapisz nowy koszyk w sesji, jeœli nie istnieje
            }
            return order;
        }

        private void SaveOrderToSession(Order order)
        {
            HttpContext.Session.Set("Order", order);
        }

        public IActionResult ViewCart()
        {
            var order = GetOrderFromSession();



            int cartItemCount = order.Items.Sum(item => item.Quantity);  // Obliczenie liczby produktów w koszyku

            // Przekazanie liczby produktów do widoku za pomoc¹ ViewBag
            ViewBag.CartItemCount = cartItemCount;

            decimal totalAmount = order.Items.Sum(item => item.TotalPrice);
            ViewBag.TotalOrderAmount = totalAmount;



            return View(order);
        }

        public IActionResult IncreaseQuantity(int productId)
        {
            var order = GetOrderFromSession();
            var item = order.Items.FirstOrDefault(i => i.Product.Id == productId);

            if (item != null)
            {
                item.Quantity++;
                SaveOrderToSession(order);
            }

            return RedirectToAction("ViewCart");
        }

        public IActionResult DecreaseQuantity(int productId)
        {
            var order = GetOrderFromSession();
            var item = order.Items.FirstOrDefault(i => i.Product.Id == productId);

            if (item != null)
            {
                item.Quantity--;

                if (item.Quantity <= 0)
                {
                    order.Items.Remove(item);  // Usuñ produkt, jeœli iloœæ wynosi 0
                }

                SaveOrderToSession(order);
            }

            return RedirectToAction("ViewCart");
        }

        public IActionResult RemoveFromCart(int productId)
        {
            var order = GetOrderFromSession();
            var item = order.Items.FirstOrDefault(i => i.Product.Id == productId);

            if (item != null)
            {
                order.Items.Remove(item);  // Usuñ produkt
                SaveOrderToSession(order);
            }

            return RedirectToAction("ViewCart");
        }

        public IActionResult Checkout()
        {
            var model = new CheckoutFormModel();  // Utworzenie modelu dla formularza
            return View(model);  // Przekazanie modelu do widoku
        }


        public IActionResult ProcessCheckout(CheckoutFormModel model)
        {
            if (ModelState.IsValid)
            {
                // Pobranie zamówienia z sesji
                var order = GetOrderFromSession();

                if (order == null)  // Zabezpieczenie
                {
                    order = new Order();  // Jeœli zamówienie nie istnieje, utwórz nowe
                }

                // Przechowywanie danych u¿ytkownika w zamówieniu
                order.CustomerName = model.Name;
                order.CustomerEmail = model.EmailAddress;
                order.CustomerPhone = model.Phone;
/*                order.BillingAddress = model.BillingAddres;
                order.ShippingAddress = model.ShippingAddress;*/

                // Zapisz zaktualizowane zamówienie w sesji
                SaveOrderToSession(order);

                // Przekierowanie do strony p³atnoœci
                return RedirectToAction("Payment");
            }

            // Jeœli walidacja nie powiod³a siê, wróæ do widoku checkout
            return View("Checkout", model);
        }

        public IActionResult Payment()
        {
            // Wyœwietlenie strony p³atnoœci
            var order = GetOrderFromSession();
            if (order == null)
            {
                // Obs³u¿ sytuacjê, w której zamówienie nie istnieje (mo¿e przekierowanie na stronê koszyka?)
                return RedirectToAction("ViewCart");
            }
            return View(order);
        }


        public IActionResult ProcessPayment (string CardNumber, string CardHolderName, string ExpiryDate, string CVV)
        {
            if (ModelState.IsValid)
            {
                var order = GetOrderFromSession();

                return RedirectToAction("PaymentConfirmation");
            }
            return View("Payment", GetOrderFromSession());
        }

        public IActionResult PaymentConfirmation()
        {
            var order = GetOrderFromSession();
            return View(order);
        }
    }
}
