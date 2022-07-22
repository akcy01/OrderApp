using Microsoft.AspNetCore.Mvc;
using SiparisApps.Data.Repository.IRepository;
using SiparisApps.Models;
using SiparisApps.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace SiparisApps.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var productList = _unitOfWork.Product.GetAll();
            return View(productList);
        }

        

        public IActionResult Crup(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(l => new SelectListItem
                {
                    Text = l.Name,
                    Value = l.Id.ToString()
                })
            };

            if (id == null || id<=0)
            {
                return View(productVM);
            }

            productVM.Product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);

            if(productVM.Product == null)
            {
                return View(productVM);
            }

            return View(productVM);
        }

        [HttpPost]
        public IActionResult Crup(ProductVM productVM,IFormFile file)//file sayesinde resim vs dosyaları yakalıycaz  /* Güncelleme Kısmı */
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;

            if(file != null)
            {
                string fileName = Guid.NewGuid().ToString(); //Dosya isimleri aynı olabilir bunları benzersiz hale getirmemiz lazım diye Guid kullandık!
                var uploadRoot = Path.Combine(wwwRootPath,@"img\products"); //Burda img dizinini yazcaz
                var extension = Path.GetExtension(file.FileName);

                /* Resim değiştirme işlemi A resmini B resmi yapma işlemi diyebiliriz */

                if(productVM.Product.Picture != null)
                {
                    var oldPicPath = Path.Combine(wwwRootPath, productVM.Product.Picture);
                    if(System.IO.File.Exists(oldPicPath)/* Dosya var mı varsa sil diyoruz burda */)
                    {
                        System.IO.File.Delete(oldPicPath);
                    }
                }

                using(var fileStreams = new FileStream(Path.Combine(uploadRoot,fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }

                productVM.Product.Picture = @"\img\products\" + fileName + extension;

            }

            if(productVM.Product.Id <= 0)
            {
                _unitOfWork.Product.Add(productVM.Product);
            }
            else
            {
                _unitOfWork.Product.Update(productVM.Product);
            }
                _unitOfWork.Save();
                return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if(id == null || id<=0)
            {
                return NotFound();
            }

            var product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}

