using BulkyBook.DataAcces.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        //
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i=>new SelectListItem { 
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverTypes.GetAll().Select(i=>new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })

            };
            if(id == null || id ==0)
            {
                return View(productVM);
                //create
            }
            productVM.Product = _unitOfWork.Products.Get(id.GetValueOrDefault());
            if (productVM.Product == null) return NotFound();
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            //баг, старая картинка не удаляется исправить
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath; //получаем путь к файлам приложения
                var files = HttpContext.Request.Form.Files; //получаем файлы отправленные запросом
                if(files.Count>0)
                {
                    //ГУИД использум что бы дать название файлу. Шанс повторения названия файла минимальный
                    string fileName = Guid.NewGuid().ToString(); //ГУИД глобальный уникальный идентификатор
                    
                    var uploads = Path.Combine(webRootPath, @"images\products"); //Combine объединяет две строки в один путь, путь загрузки
                    var extension = Path.GetExtension(files[0].FileName);//возвращает расширение файла
                    if(productVM.Product.ImageUrl !=null)
                    {
                        //значит картинка уже есть и нужно удалить старую

                        //объединяем путь к файлам приложения и путь к картинке, причем удаляем обратный слэш из начала
                        
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(imagePath)) //существует ли данный файл
                        {
                            System.IO.File.Delete(imagePath);//удаляем
                        }
                    }
                    using(var filesStreams = new FileStream(Path.Combine(uploads,fileName+extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                else
                {
                    //если картину не меняют
                    if(productVM.Product.Id !=0)
                    {
                        Product objFromDb = _unitOfWork.Products.Get(productVM.Product.Id);
                        productVM.Product.ImageUrl = objFromDb.ImageUrl;
                    }
                }

                if (productVM.Product.Id == 0)
                {
                    //create
                    _unitOfWork.Products.Add(productVM.Product);
                    TempData["success"] = "Товар добавлен успешно!";
                }
                else
                {
                    _unitOfWork.Products.Update(productVM.Product);
                    TempData["success"] = "Товар обновлен успешно!";
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                productVM.CoverTypeList = _unitOfWork.CoverTypes.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                if(productVM.Product.Id != 0)
                {
                    productVM.Product = _unitOfWork.Products.Get(productVM.Product.Id);
                }
                return View(productVM);
            }
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Products.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = allObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Products.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error deleting" });
            }
            string webRootPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, objFromDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath)) //существует ли данный файл
            {
                System.IO.File.Delete(imagePath);//удаляем
            }
            _unitOfWork.Products.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Удаление прошло успешно" });
        }
        #endregion


    }
}
