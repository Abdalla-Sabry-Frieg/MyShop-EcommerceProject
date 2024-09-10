using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MyShop_DataAccess.Data;
using MyShop_Entities.Helper;
using MyShop_Entities.Models;
using MyShop_Entities.Repositories;

namespace MyShop.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryController(IUnitOfWork unitOfWork , IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var category = _unitOfWork.Categories.GetAll();
            return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category , IFormFile file)
        {
            
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath; // mean wwwroot
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var upload = Path.Combine(rootPath, @"Images\Category");
                    var extension = Path.GetExtension(file.FileName);

                    using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(filestream);

                    }
                    category.Image= @"Images\Category\" + fileName + extension;
                }

                _unitOfWork.Categories.Add(category);
                _unitOfWork.Complet();
                TempData["Create"] = "Category has been deleted";
                return RedirectToAction("Index");
            }
            return View(category);

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _unitOfWork.Categories.GetFirstOrDefualt(x => x.Id == id);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var categoryFromDb = _unitOfWork.Categories.GetFirstOrDefualt(x => x.Id == category.Id);
                if (categoryFromDb == null)
                {
                    return NotFound();
                }

                string rootPath = _webHostEnvironment.WebRootPath; // mean wwwroot
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var upload = Path.Combine(rootPath, @"Images\Category");
                    var extension = Path.GetExtension(file.FileName);

                    if (category.Image != null)
                    {
                        var oldImage = Path.Combine(rootPath, category.Image.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);
                        }

                    }

                    using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(filestream);

                    }
                    category.Image = @"Images\Category\" + fileName + extension;
                }

                categoryFromDb.Name = category.Name;
                categoryFromDb.Description = category.Description;

                _unitOfWork.Categories.Update(categoryFromDb);
                _unitOfWork.Complet();
                TempData["Edit"] = "Category has been updated";
                return RedirectToAction("Index");
            }

            return View(category);
        }



        [HttpGet]
        public IActionResult Delete(int? id)
        {
            var category = _unitOfWork.Categories.GetFirstOrDefualt(x => x.Id == id);
            return View(category);
        }

        [HttpPost]
        public IActionResult DeleteCategory(int? id)
        {
            var category = _unitOfWork.Categories.GetFirstOrDefualt(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            if (category.Image != null && category.Image != Guid.Empty.ToString())
            {
                var pathimage = Path.Combine(@"wwwroot/", Helpers.imagesSaveCategory, category.Image);
                if (System.IO.File.Exists(pathimage))
                {
                    System.IO.File.Delete(pathimage);
                }
            }
            _unitOfWork.Categories.Delete(category);
            _unitOfWork.Complet();
            TempData["Delete"] = "Category has been deleted";
            return RedirectToAction("Index");
        }

        private void Image(Category model)
        {
            var file = HttpContext.Request.Form.Files;
            //Create
            if (file.Count() > 0)
            {
                var ImageName = Guid.NewGuid().ToString() + Path.GetExtension(file[0].FileName);
                var fileStream = new FileStream(Path.Combine(@"wwwroot/", Helpers.imagesSaveCategory, ImageName), FileMode.Create);
                file[0].CopyTo(fileStream);
                model.Image = ImageName;
            }
           
            else // Update
            {
                model.Image = model.Image;
            }
        }
    }
}
