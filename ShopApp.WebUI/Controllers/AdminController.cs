using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.Entities;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
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
            return View();
        }

        [HttpPost]
        public IActionResult ProductCreate(ProductModel model)
        {
            var entity = new Product()
            {
                Name = model.Name,
                ImageUrl = model.ImageUrl,
                Description = model.Description,
                Price = model.Price
            };

            _productService.Create(entity);

            return RedirectToAction("ProductList");
        }

        public IActionResult ProductEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var entity = _productService.GetById((int)id);
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
                ImageUrl = entity.ImageUrl
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult ProductEdit(ProductModel model)
        {
            var entity = _productService.GetById(model.Id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = model.Name;
            entity.Price = model.Price;
            entity.Description = model.Description;
            entity.ImageUrl = model.ImageUrl;

            _productService.Update(entity);

            return RedirectToAction("ProductList");
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