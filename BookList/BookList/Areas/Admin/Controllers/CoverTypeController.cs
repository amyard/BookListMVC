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
    public class CoverTypeController : Controller
    {

        private readonly IUnitOfWork _uniofWork;
        public CoverTypeController(IUnitOfWork uniofWork)
        {
            _uniofWork = uniofWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            CoverType CoverType = new CoverType();
            if(id == null)
            {
                // using for create
                return View(CoverType);
            }

            // using for edit
            CoverType = _uniofWork.CoverType.Get(id.GetValueOrDefault());
            if(CoverType == null)
            {
                return NotFound();
            }
            return View(CoverType);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType CoverType)
        {
            if (ModelState.IsValid)
            {
                if(CoverType.Id == 0)
                {
                    _uniofWork.CoverType.Add(CoverType);
                }
                else
                {
                    _uniofWork.CoverType.Update(CoverType);
                }
                _uniofWork.Save();
                // return RedirectToAction("Index");
                return RedirectToAction(nameof(Index));
            }
            return View(CoverType);
        }


        // API calls will be here
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _uniofWork.CoverType.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _uniofWork.CoverType.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _uniofWork.CoverType.Remove(objFromDb);
            _uniofWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}