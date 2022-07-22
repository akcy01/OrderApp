using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiparisApps.Data.Repository.IRepository;
using SiparisApps.Models;

namespace SiparisApps.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categoryList = _unitOfWork.Category.GetAll();
            return View(categoryList); //Sayfaya göndermek için view'in içine yazdık ! 
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                return RedirectToAction("Index"); 
            }

            return View(category);
        }

        public IActionResult Edit(int id) // bize bir ıd gelsin ki bu id'ye göre değiştirilebilsin.
        {
            if(id == null || id <= 0)
            {
                return NotFound();
            }

            var category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id); // Gelen Id id'ye eşit ise category'nin içine at demek bu.

            if(category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Delete(int id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }

            var category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id); // Gelen Id id'ye eşit ise category'nin içine at demek bu.

            if (category == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            return RedirectToAction("Index"); // Bizi geri Index sayfasına atıyor sildikten sonra bu.

        }
    }
}
