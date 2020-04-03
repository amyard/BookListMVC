using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookList.DataAccess.Repository.IRepository;
using BookList.Models;
using BookList.Utility;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookList.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
            CoverType сoverType = new CoverType();
            if(id == null)
            {
                // using for create
                return View(сoverType);
            }

            // using for edit
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);                         // use "@Id"   - because we use such parameter in migrations
            сoverType = _uniofWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);
            if (сoverType == null)
            {
                return NotFound();
            }
            return View(сoverType);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType сoverType)
        {
            if (ModelState.IsValid)
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Name", сoverType.Name);

                if (сoverType.Id == 0)
                {
                    _uniofWork.SP_Call.Execute(SD.Proc_CoverType_Create, parameter);
                }
                else
                {
                    parameter.Add("@Id", сoverType.Id);
                    _uniofWork.SP_Call.Execute(SD.Proc_CoverType_Update, parameter);
                }
                _uniofWork.Save();

                // return RedirectToAction("Index");
                return RedirectToAction(nameof(Index));
            }
            return View(сoverType);
        }


        // API calls will be here
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _uniofWork.SP_Call.List<CoverType>(SD.Proc_CoverType_GetAll, null);
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        { 
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);     // use "@Id"   - because we use such parameter in migrations
            var objFromDb = _uniofWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);

            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _uniofWork.SP_Call.Execute(SD.Proc_CoverType_Delete, parameter);
            _uniofWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}