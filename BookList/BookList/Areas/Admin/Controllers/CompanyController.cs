using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookList.DataAccess.Repository.IRepository;
using BookList.Models;
using BookList.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookList.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CompanyController : Controller
    {

        private readonly IUnitOfWork _uniofWork;
        public CompanyController(IUnitOfWork uniofWork)
        {
            _uniofWork = uniofWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if(id == null)
            {
                // using for create
                return View(company);
            }

            // using for edit
            company = _uniofWork.Company.Get(id.GetValueOrDefault());
            if(company == null)
            {
                return NotFound();
            }
            return View(company);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if(company.Id == 0)
                {
                    _uniofWork.Company.Add(company);
                }
                else
                {
                    _uniofWork.Company.Update(company);
                }
                _uniofWork.Save();
                // return RedirectToAction("Index");
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }


        // API calls will be here
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _uniofWork.Company.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _uniofWork.Company.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _uniofWork.Company.Remove(objFromDb);
            _uniofWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}