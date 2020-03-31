using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookList.DataAccess.Repository.IRepository;
using BookList.Models;
using BookList.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookList.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _uniofWork;
        private readonly IWebHostEnvironment _hostEnvironment; // for upload images on server

        public ProductController(IUnitOfWork uniofWork, IWebHostEnvironment hostEnvironment)
        {
            _uniofWork = uniofWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _uniofWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = _uniofWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if(id == null)
            {
                // using for create
                return View(productVM);
            }

            // using for edit
            productVM.Product = _uniofWork.Product.Get(id.GetValueOrDefault());
            if(productVM.Product == null)
            {
                return NotFound();
            }
            return View(productVM);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if(files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images/products");
                    var extension = Path.GetExtension(files[0].FileName);

                    if(productVM.Product.ImageUrl != null)
                    {
                        // this is and edit and we need remove old image
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                else
                {
                    // update when they do not change the image
                    if(productVM.Product.Id != 0)
                    {
                        Product objFromDb = _uniofWork.Product.Get(productVM.Product.Id);
                        productVM.Product.ImageUrl = objFromDb.ImageUrl;
                    }
                }

                if(productVM.Product.Id == 0)
                {
                    _uniofWork.Product.Add(productVM.Product);
                }
                else
                {
                    _uniofWork.Product.Update(productVM.Product);
                }
                _uniofWork.Save();
                // return RedirectToAction("Index");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM.CategoryList = _uniofWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                productVM.CoverTypeList = _uniofWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                if(productVM.Product.Id != 0)
                {
                    productVM.Product = _uniofWork.Product.Get(productVM.Product.Id);
                }
            }
            return View(productVM);
        }
        

        // API calls will be here
        [HttpGet]
        public IActionResult GetAll()
        {
            // use includeProperties  for display category Name is product.js
            var allObj = _uniofWork.Product.GetAll(includeProperties:"Category,CoverType");
            return Json(new { data = allObj });
        }
         
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _uniofWork.Product.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // delete image
            string webRootPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, objFromDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            // delete product
            _uniofWork.Product.Remove(objFromDb);
            _uniofWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}