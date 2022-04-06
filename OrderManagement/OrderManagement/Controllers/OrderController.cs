using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OrderManagement.DAL;
using OrderManagement.Models;
using X.PagedList;

namespace OrderManagement.Controllers
{
    public class OrderController : Controller
    {
        private IOrderRepository orderRepository;
        private IConfiguration _configuration;
        public OrderController(IOrderRepository orderRepository, IConfiguration configuration)
        {
            this.orderRepository = orderRepository;
            this._configuration = configuration;
        }
        // GET: OrderController
        public ActionResult ExportJson(OrderFilterViewModel model)
        {
            var list = orderRepository.Filter(
                model.StoreName,
                model.ManagerOrderId,
                model.CustomerOrderId,
                model.SellerOrderId,
                model.ItemOrderId,
                out int totalCount,
                null,
                false,
                1,
                1000_0000);


            var memory = new MemoryStream();
            var writer = new StreamWriter(memory);
            var serializer = new JsonSerializer();
            serializer.Serialize(writer, list);
            writer.Flush();

            memory.Position = 0;
            if (memory != Stream.Null)
                return File(memory, "application/json", $"Export_{DateTime.Now}.json");

            return NotFound();
        }

        public ActionResult ExportXml(OrderFilterViewModel model)
        {
            var list = orderRepository.Filter(
                model.StoreName,
                model.ManagerOrderId,
                model.CustomerOrderId,
                model.SellerOrderId,
                model.ItemOrderId,
                out int totalCount,
                null,
                false,
                1,
                1000_0000);


            var memory = new MemoryStream();
            var writer = new StreamWriter(memory);
            var serializer = new XmlSerializer(typeof(List<Order>));
            serializer.Serialize(writer, list);
            writer.Flush();

            memory.Position = 0;
            if (memory != Stream.Null)
                return File(memory, "application/xml", $"Export_{DateTime.Now}.xml");

            return NotFound();
        }

        public ActionResult ExportCsv(OrderFilterViewModel model)
        {
            var list = orderRepository.Filter(
                model.StoreName,
                model.ManagerOrderId,
                model.CustomerOrderId,
                model.SellerOrderId,
                model.ItemOrderId,
                out int totalCount,
                null,
                false,
                1,
                1000_0000);


            var memory = new MemoryStream();
            var writer = new StreamWriter(memory);
            var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csvWriter.WriteRecords(list);
            writer.Flush();

            memory.Position = 0;
            if (memory != Stream.Null)
                return File(memory, "text/csv", $"Export_{DateTime.Now}.csv");

            return NotFound();
        }

        public ActionResult ImportJson()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportJson(IFormFile importFile)
        {
            
            IList<Order> orders = null;
            if (importFile != null)
            {
                using (var stream = importFile.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    var serializer = new JsonSerializer();
                    orders =
                        (List<Order>)serializer.Deserialize(reader, typeof(List<Order>));
                }

                foreach (var order in orders)
                    orderRepository.Insert(order);
            }

            return RedirectToAction(nameof(Index));
        }
        public ActionResult ImportXml()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportXml(IFormFile importFile)
        {
            var orders = new List<Order>();
            if (importFile != null)
            {
                using (var stream = importFile.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    var serializer = new XmlSerializer(typeof(List<Order>));
                    orders = (List<Order>)serializer.Deserialize(reader);
                }

                foreach (var order in orders)
                    orderRepository.Insert(order);
            }
            else
            {
                ModelState.AddModelError("", "Empty file");
            }

            return RedirectToAction(nameof(Index));
        }
        public ActionResult ImportCsv()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportCsv(IFormFile importFile)
        {
            var orders = new List<Order>();
            if (importFile != null)
            {
                using (var stream = importFile.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    var serializer = new CsvReader(reader, CultureInfo.InvariantCulture);
                    orders = serializer.GetRecords<Order>().ToList<Order>();
                }

                foreach (var order in orders)
                    orderRepository.Insert(order);
            }
            else
            {
                ModelState.AddModelError("", "Empty file");
            }

            return RedirectToAction(nameof(Index));
        }


        public ActionResult Index()
        {
            var list = orderRepository.GetOrders();

            return View(list);
        }

        public ActionResult Filter(
            int page,
            string sortColumn,
            string sortDirection,
            OrderFilterViewModel model)
        {
            if (page <= 0)
                page = 1;

            int pageSize = _configuration.GetValue<int>("ViewSettings:PageSize");

            var orderListPage = orderRepository.Filter(
                model.StoreName,
                model.ManagerOrderId,
                model.CustomerOrderId,
                model.SellerOrderId,
                model.ItemOrderId,
                out int totalCount,
                sortColumn,
                "Desc".Equals(sortDirection, StringComparison.OrdinalIgnoreCase) ? true : false,
                page,
                pageSize);

            model.Orders = new StaticPagedList<Order>(orderListPage, page, pageSize, totalCount);
            model.CurrentPage = page;
            model.SortDirection = "DESC".Equals(sortDirection, StringComparison.OrdinalIgnoreCase)
                ? "ASC"
                : "DESC";

            return View(model);
        }

        // GET: OrderController/Details/5
        public ActionResult Details(int id)
        {
            var order = orderRepository.GetByIdSP(id);
            return View(order);
        }

        // GET: OrderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Order order)
        {
            try
            {
                orderRepository.Insert(order);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderController/Edit/5
        public ActionResult Edit(int id)
        {
            var order = orderRepository.GetByIdSP(id);
            return View(order);
        }

        // POST: OrderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Order order)
        {
            try
            {
                orderRepository.UpdateSP(order);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: OrderController/Delete/5
        public ActionResult Delete(int id)
        {
            var order = orderRepository.GetByIdSP(id);
            return View(order);
        }

        // POST: OrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                orderRepository.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
