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
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                //создание новой категории
                return View(category);
            }
            //edit категории
            category = _unitOfWork.Category.Get(id.GetValueOrDefault()); //id.GetValueOrDefault() если int nullable, то метод берет значение по дефолту (0) 
            if(category == null) { return NotFound(); }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //проверяет токен защиты от подделки
        public IActionResult Upsert(Category category)
        {
            if(ModelState.IsValid)
            {
                if(category.Id ==0)
                {
                    //create
                    _unitOfWork.Category.Add(category);
                    TempData["success"] = "Категория добавлена успешно!";
                }
                else
                {
                    //edit
                    _unitOfWork.Category.Update(category);
                    TempData["success"] = "Категория обновлена успешно!";
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Category.GetAll();
            return Json(new { data=allObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Category.Get(id);
            if(objFromDb ==null)
            {
                return Json(new {success =false, message="Error deleting" });
            }
            _unitOfWork.Category.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Удаление прошло успешно" });
        }
        #endregion
    }
}
