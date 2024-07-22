using Microsoft.AspNetCore.Mvc;
using MyShop_DataAccess.Data;
using MyShop_Entities.Helper;
using MyShop_Entities.Models;
using MyShop_Entities.Repositories;
using MyShop_Entities.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyShop.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork , IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var product = new ProductsViewModel()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Categories.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
			     products = _unitOfWork.Products.GetAll(includeWord: "Category").ToList()

			};
           

            return View(product);
        }

        // Action to return data to data table as json
        [HttpGet]
        public IActionResult GetData()
        {
            var products = _unitOfWork.Products.GetAll(includeWord:"Category");
            return Json(new { data = products });
        }

        [HttpGet]
        public IActionResult Create()
        {
            var products = new ProductsViewModel()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Categories.GetAll().Select(x=> new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductsViewModel model , IFormFile file)
        {
            if (ModelState.IsValid)
            { 
                string rootPath = _webHostEnvironment.WebRootPath; // mean wwwroot
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var upload = Path.Combine(rootPath, @"Images\Product");
                    var extension = Path.GetExtension(file.FileName);

                    using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(filestream);

                    }
                    model.Product.Image = @"Images\Product\" + fileName + extension;
                }

           
            
                _unitOfWork.Products.Add(model.Product);
                _unitOfWork.Complet();
                TempData["Create"] = "Product has been Created";
                return RedirectToAction("Index");
            }
           
            return View(model.Product);

        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
          
            var products = new ProductsViewModel()
            {
                Product = _unitOfWork.Products.GetFirstOrDefualt(x=>x.Id ==id),
                CategoryList = _unitOfWork.Categories.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductsViewModel model , IFormFile? file)
        {
          
            if (ModelState.IsValid)
            {
                if (model.Product.Id == null)
                {
                    return NotFound();
                }

                string rootPath = _webHostEnvironment.WebRootPath; // mean wwwroot
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var upload = Path.Combine(rootPath, @"Images\Product");
                    var extension = Path.GetExtension(file.FileName);

                    if(model.Product.Image != null)
                    {
                        var oldImage = Path.Combine(rootPath, model.Product.Image.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);    
                        }

                    }

                    using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(filestream);

                    }
                    model.Product.Image = @"Images\Product\" + fileName + extension;
                }

                _unitOfWork.Products.Update(model.Product);
                _unitOfWork.Complet();
                TempData["Edit"] = "Product has been updated";
                return RedirectToAction("Index");
            }

            return View(model.Product);
        }



       
        public IActionResult DeleteProduct(int id)
        {
            var Product = _unitOfWork.Products.GetFirstOrDefualt(x => x.Id == id);
            if (Product == null)
            {
                return NotFound();
            }
            if (Product.Image != null && Product.Image != Guid.Empty.ToString())
            {
                var pathimage = Path.Combine(@"wwwroot/", "Images/Product/" , Product.Image.TrimStart('\\'));
                if (System.IO.File.Exists(pathimage))
                {
                    System.IO.File.Delete(pathimage);
                }
            }
            _unitOfWork.Products.Delete(Product);
            _unitOfWork.Complet();
             return RedirectToAction("Index");

        }

    
    }
}
