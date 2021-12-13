using BulkyBook.DataAcces.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin +","+SD.Role_Employee)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if(id ==null)
            {
                return View(company);
            }
            company = _unitOfWork.Companies.Get(id.GetValueOrDefault());
            if (company == null) return NotFound();
            return View(company);
            
        }
        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if(ModelState.IsValid)
            {
                if(company.Id !=0)
                {
                    //edit
                    _unitOfWork.Companies.Update(company);
                    TempData["success"] = "Данные компании были успешно обновлены!";
                }
                else
                {
                    _unitOfWork.Companies.Add(company);
                    TempData["success"] = "Новая компания успешно добавлена!";
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        #region API CALL

        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Companies.GetAll();
            return Json(new { data=allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Companies.Get(id);
            if(objFromDb ==null)
            {
                return Json(new { success = false, message = "Error deleting, object not found" });
            }
            _unitOfWork.Companies.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Удаление прошло успешно!" });
        }
        #endregion

    }
}
