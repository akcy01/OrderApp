using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SiparisApps.Data.Repository.IRepository;
using SiparisApps.Models;
using SiparisApps.Models.ViewModels;

namespace SiparisApps.Areas.Customer.Controllers
{
    [Area("Customer")] 
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartVM CartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        /* SİSTEME GİRİŞ YAPMIŞ KULLNICININ KENDİ SEPETİNDEKİ ÜRÜNLERİN LİSTESİNİ GÖREBİLMESİNİ İSTİYORUZ Index de bunu yapıyoruz */
        public IActionResult Index()
        { 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            CartVM = new CartVM()
            {   // Sisteme giriş yapan kullcı ID'si.
                ListCart =  _unitOfWork.Cart.GetAll(p => p.AppUserId == claim.Value, includeProperties: "Product"),
                OrderProduct = new()
            };

            foreach (var cart in CartVM.ListCart) //Sepetteki para işlemleri
            {
                cart.Price = cart.Product.Price * cart.Count;
                CartVM.OrderProduct.OrderPrice += (cart.Price);
            }

            return View(CartVM);
        }

        public IActionResult Order()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            CartVM = new CartVM()
            {   // Sisteme giriş yapan kullcı ID'si.
                ListCart = _unitOfWork.Cart.GetAll(p => p.AppUserId == claim.Value, includeProperties: "Product"),
                OrderProduct = new()
            };

            //AppUser  bilgilerini aldık ve listeledik
            CartVM.OrderProduct.AppUser = _unitOfWork.AppUser.GetFirstOrDefault(u=>u.Id == claim.Value);

            CartVM.OrderProduct.Name = CartVM.OrderProduct.AppUser.FullName;
            CartVM.OrderProduct.CellPhone = CartVM.OrderProduct.AppUser.CellPhone;
            CartVM.OrderProduct.Address = CartVM.OrderProduct.AppUser.Address;
            CartVM.OrderProduct.PostalCode = CartVM.OrderProduct.AppUser.PostalCode;

            foreach (var cart in CartVM.ListCart)
            {
                cart.Price = cart.Product.Price * cart.Count;
                CartVM.OrderProduct.OrderPrice += (cart.Price);
            }
            return View(CartVM);
        }

        [HttpPost]
        [ActionName("Order")]
        [ValidateAntiForgeryTokenAttribute]

        public IActionResult OrderPost(CartVM cartVM)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            CartVM = new CartVM()
            {   // Sisteme giriş yapan kullcı ID'si.
                ListCart = _unitOfWork.Cart.GetAll(p => p.AppUserId == claim.Value, includeProperties: "Product"),
                OrderProduct = new()
            };

            //AppUser  bilgilerini aldık ve listeledik
            //appUser'ı unitofwork'ten alma sebebimiz veritabanından çekiyor olmamız.
            //veritabanından çekmek yanlış bir işlem old için sayfadan çekiyoruz.Bu yüzden cartVm'i kullandık!
            AppUser appUser = _unitOfWork.AppUser.GetFirstOrDefault(u => u.Id == claim.Value);
            CartVM.OrderProduct.AppUser = appUser;
            CartVM.OrderProduct.OrderDate = System.DateTime.Now;
            CartVM.OrderProduct.AppUserId = claim.Value;
            CartVM.OrderProduct.Name = cartVM.OrderProduct.Name;
            CartVM.OrderProduct.CellPhone = cartVM.OrderProduct.CellPhone;
            CartVM.OrderProduct.Address = cartVM.OrderProduct.Address;
            CartVM.OrderProduct.PostalCode = cartVM.OrderProduct.PostalCode;
            CartVM.OrderProduct.OrderStatus = "Ordered";

            foreach (var cart in CartVM.ListCart)
            {
                cart.Price = cart.Product.Price * cart.Count;
                CartVM.OrderProduct.OrderPrice += (cart.Price);
            }

            _unitOfWork.OrderProduct.Add(CartVM.OrderProduct);
            _unitOfWork.Save();

            foreach (var cart in CartVM.ListCart)
            {
                OrderDetails OrderDetails = new()
                {
                    ProductId = cart.ProductId,
                    OrderProductId = CartVM.OrderProduct.Id, 
                    Price = cart.Price,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDetails.Add(OrderDetails);
                _unitOfWork.Save();
            }

            //Kullanıcıya ait olan sepetteki ürünleri bu listeye çektik!
            List<Cart> Carts = _unitOfWork.Cart.GetAll(u=>u.AppUserId == CartVM.OrderProduct.AppUserId).ToList();

            _unitOfWork.Cart.RemoveRange(Carts);
            _unitOfWork.Save();

            var cartCount = _unitOfWork.Cart.GetAll(u => u.AppUserId == claim.Value).ToList().Count;
            HttpContext.Session.SetInt32("SessionCartCount", cartCount);

            //İşlem bittikten sonraki gelecek olan sayfayı yazdık
            return RedirectToAction(nameof(Index), "Home", new {area="Customer"}); 
        }

        /* Count'u arttırıp azalttığım yeri yazdım burada. => Increase ve Decrease */
        public IActionResult Increase(int cartId)
        {
            var cart = _unitOfWork.Cart.GetFirstOrDefault(c => c.Id == cartId);

            cart.Count += 1;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Decrease(int cartId)
        {
            var cart = _unitOfWork.Cart.GetFirstOrDefault(c => c.Id == cartId);

            if(cart.Count > 1)
            {
                cart.Count -= 1;
            }
            /* Else ile ürünü sepetten siliyoruz */
            else
            {
                _unitOfWork.Cart.Remove(cart);
                //var cartCount = _unitOfWork.Cart.GetAll(u=>u.AppUserId == cart.AppUserId).ToList().Count - 1;
                //HttpContext.Session.SetInt32("SessionCartCount", cartCount);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

    }
}
