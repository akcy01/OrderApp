using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiparisApps.Data.Repository.IRepository;
using SiparisApps.Models.ViewModels;

namespace SiparisApps.Areas.Admin.Controllers
{   
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 
        }

        public IActionResult Index()
        {
            var orderList = _unitOfWork.OrderProduct.GetAll(x=>x.OrderStatus != "Delivered");
            return View(orderList);
        }

        /* to view the details from the admin panel */
        public IActionResult Details(int Id)
        {
            OrderVM = new OrderVM()
            {
                OrderProduct = _unitOfWork.OrderProduct.GetFirstOrDefault(o => o.Id == Id, includeProperties: "AppUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(d => d.OrderProductId == Id, includeProperties: "Product")
            };
            return View(OrderVM);

        }

        [HttpPost]

        public IActionResult Delivered(OrderVM orderVM)
        {
            var orderProduct = _unitOfWork.OrderProduct.GetFirstOrDefault(o=> o.Id == orderVM.OrderProduct.Id);
            orderProduct.OrderStatus = "Delivered";
            _unitOfWork.OrderProduct.Update(orderProduct);
            _unitOfWork.Save();

            return RedirectToAction("Details","Order", new {Id = orderVM.OrderProduct.Id});
        }

        [HttpPost]

        public IActionResult CancelOrder(OrderVM orderVM)
        {
            var orderProduct = _unitOfWork.OrderProduct.GetFirstOrDefault(o => o.Id == orderVM.OrderProduct.Id);
            orderProduct.OrderStatus = "Cancel";

            _unitOfWork.OrderProduct.Update(orderProduct);
            _unitOfWork.Save();

            return RedirectToAction("Details", "Order", new { Id = orderVM.OrderProduct.Id });
        }

        [HttpPost]
        public IActionResult UpdateOrderDetails(OrderVM orderVM)
        {
            var orderProduct = _unitOfWork.OrderProduct.GetFirstOrDefault(o => o.Id == orderVM.OrderProduct.Id);

            orderProduct.Address = orderVM.OrderProduct.Address;
            orderProduct.CellPhone = orderVM.OrderProduct.CellPhone;
            orderProduct.PostalCode = orderVM.OrderProduct.PostalCode;
            orderProduct.Name = orderVM.OrderProduct.Name;

            return RedirectToAction("Details", "Order", new { Id = orderVM.OrderProduct.Id });
        }

    }
}
