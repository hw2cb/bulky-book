using BulkyBook.DataAcces.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if(id == null)
            {
                return View(coverType);
            }
            var parameter = new DynamicParameters();
            parameter.Add(@"Id", id);
            coverType = _unitOfWork.SP_CALL.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);
            if (coverType == null) return NotFound();
            return View(coverType);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if(ModelState.IsValid)
            {
                //использование хранимых процедур
                var parameter = new DynamicParameters();
                parameter.Add("@Name", coverType.Name);
                if(coverType.Id == 0)
                {
                    //create
                    _unitOfWork.SP_CALL.Execute(SD.Proc_CoverType_Create, parameter);
                    TempData["success"] = "Тип обложки добавлен успешно!";
                }
                else
                {
                    parameter.Add("@Id", coverType.Id);
                    _unitOfWork.SP_CALL.Execute(SD.Proc_CoverType_Update, parameter);
                    TempData["success"] = "Тип обложки обновлен успешно!";
                    //edit
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            //использовани хранимых процедур
            var objFromDb = _unitOfWork.SP_CALL.List<CoverType>(SD.Proc_CoverType_GetAll, null);
            return Json(new { data = objFromDb });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            //использование хранимых процедур
            var parameter = new DynamicParameters();
            parameter.Add(@"Id", id);
            var objFromDb = _unitOfWork.SP_CALL.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);
            if(objFromDb ==null)
            {
                return Json(new { succes = false, message = "Error deleting" });
            }
            //удаление с помощью хранимых процедур
            _unitOfWork.SP_CALL.Execute(SD.Proc_CoverType_Delete, parameter);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Удаление прошло успешно" });
        }
        #endregion
    }
}
//2 3 4