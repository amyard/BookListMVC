using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookList.DataAccess.Repository.IRepository;
using BookList.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookList.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private readonly IUnitOfWork _uniofWork;
        public CategoryController(IUnitOfWork uniofWork)
        {
            _uniofWork = uniofWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if(id == null)
            {
                // using for create
                return View(category);
            }

            // using for edit
            category = _uniofWork.Category.Get(id.GetValueOrDefault());
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if(category.Id == 0)
                {
                    _uniofWork.Category.Add(category);
                }
                else
                {
                    _uniofWork.Category.Update(category);
                }
                _uniofWork.Save();
                // return RedirectToAction("Index");
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        // API calls will be here
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _uniofWork.Category.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _uniofWork.Category.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _uniofWork.Category.Remove(objFromDb);
            _uniofWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}