using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.Entities;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;

        public AdminController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public IActionResult ProductList()
        {
            return View(new ProductListModel()
            {
                Products = _productService.GetAll()
            });
        }

        [HttpGet]
        public IActionResult ProductCreate()
        {
            return View(new ProductModel());
        }

        [HttpPost]
        public IActionResult ProductCreate(ProductModel model)
        {
            if (ModelState.IsValid == true)
            {
                var entity = new Product()
                {
                    Name = model.Name,
                    ImageUrl = model.ImageUrl,
                    Description = model.Description,
                    Price = model.Price
                };

                if (_productService.Create(entity))
                {
                    return RedirectToAction("ProductList");
                }

                ViewBag.ErrorMessage = _productService.ErrorMessage;
                return View(model);
            }
            else
            {
                return View(model);
            }            
        }

        public IActionResult ProductEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var entity = _productService.GetByIdWithCategories((int)id);
            if (entity == null)
            {
                return NotFound();
            }
            var model = new ProductModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Price = entity.Price,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl,
                SelectedCategories = entity.ProductCategories.Select(i => i.Category).ToList()
            };

            ViewBag.Categories = _categoryService.GetAll();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductModel model, int[] categoryIds, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var entity = _productService.GetById(model.Id);
                if (entity == null)
                {
                    return NotFound();
                }
                
                entity.Name = model.Name;
                entity.Price = model.Price;
                entity.Description = model.Description;
                // entity.ImageUrl = model.ImageUrl;

                if (file != null)
                {
                    // https://stackoverflow.com/a/47129623 kaynagindan yararlanildi.
                    var newFileName = string.Empty;
                    var fileName = string.Empty;
                    string pathFile = string.Empty;                  

                    //Dosya ismini alir
                    fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');                    

                    //Benzersiz dosya ismi olusturur
                    var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                    //Dosya uzantisini alir
                    var FileExtension = Path.GetExtension(fileName);

                    // dosya ismi + dosya uzantisi birlestir
                    newFileName = myUniqueFileName + FileExtension;

                    // Combines two strings into a path.
                    // fileName = Path.Combine(_environment.WebRootPath, "wwwroot\\img") + $@"\{newFileName}";

                    // Dosyanin kopyalanacagi yer ve yeni dosya adi
                    pathFile = "wwwroot/img/" + newFileName;

                    entity.ImageUrl = newFileName;

                    // Formdan gonderilen resmi (file), pathFile bilgileriyle kopyala
                    using (var stream = new FileStream(pathFile, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                _productService.Update(entity, categoryIds);
                return RedirectToAction("ProductList");
            }

            ViewBag.Categories = _categoryService.GetAll();
            return View(model);
        }

        [HttpPost]
        public ActionResult ProductDelete(int productId)
        {
            var entity = _productService.GetById(productId);
            if (entity != null)
            {
                _productService.Delete(entity);
            }
            return RedirectToAction("ProductList");
        }

        public IActionResult CategoryList()
        {
            return View(new CategoryListModel()
            {
                Categories = _categoryService.GetAll()
            });
        }

        [HttpGet]
        public IActionResult CategoryCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CategoryCreate(CategoryModel model)
        {
            var entity = new Category() { 
                Name = model.Name
            };
            _categoryService.Create(entity);
            return RedirectToAction("CategoryList");
        }


        [HttpGet]
        public IActionResult CategoryEdit(int id)
        {
            var entity = _categoryService.GetByIdWithProducts(id);

            return View(new CategoryModel() { 
                Id = entity.Id,
                Name = entity.Name,
                Products = entity.ProductCategories.Select(p=>p.Product).ToList()
            });
        }

        [HttpPost]
        public IActionResult CategoryEdit(CategoryModel model)
        {
            var entity = _categoryService.GetById(model.Id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = model.Name;
            _categoryService.Update(entity);
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public ActionResult CategoryDelete(int categoryId)
        {
            var entity = _categoryService.GetById(categoryId);
            if (entity != null)
            {
                _categoryService.Delete(entity);
            }
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public ActionResult DeleteFromCategory(int categoryId, int productId)
        {
            _categoryService.DeleteFromCategory(categoryId, productId);

            return Redirect("/admin/categoryedit/" + categoryId);
        }
    }
}