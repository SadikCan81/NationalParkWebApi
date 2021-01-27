using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using ParkyWeb.Utility;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class NationalParkController : Controller
    {
        private readonly INationalParkRepository _npRepository;

        public NationalParkController(INationalParkRepository npRepository)
        {
            _npRepository = npRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            NationalPark nationalPark = new NationalPark();

            if (id == null)
            {
                return View(nationalPark);
            }
            else
            {
                var token = HttpContext.Session.GetString("JwToken");
                nationalPark = await _npRepository.GetAsync(SD.NationalParkAPIPath, id.GetValueOrDefault(), token);
                if (nationalPark == null)
                {
                    return NotFound();
                }
                return View(nationalPark);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(NationalPark nationalPark)
        {
            var succeed = false;
            var files = HttpContext.Request.Form.Files;
            var token = HttpContext.Session.GetString("JwToken");

            if (!ModelState.IsValid)
            {
                return View(nationalPark);
            }

            if (files.Count > 0)
            {
                byte[] img1 = null;
                using (var fs1 = files[0].OpenReadStream())
                {
                    using (var ms1 = new MemoryStream())
                    {
                        fs1.CopyTo(ms1);
                        img1 = ms1.ToArray();
                    }
                }
                nationalPark.Image = img1;
            }
            else
            {
                var objFromDb = await _npRepository.GetAsync(SD.NationalParkAPIPath, nationalPark.Id, token);
                if (objFromDb != null && objFromDb.Image != null)
                {
                    nationalPark.Image = objFromDb.Image;
                }
            }

            if (nationalPark.Id == 0)
            {
                succeed = await _npRepository.CreateAsync(SD.NationalParkAPIPath, nationalPark, token);
                if (succeed)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(nationalPark);
                }
            }
            else
            {
                //var objFromDb2 = await _npRepository.GetAsync(SD.NationalParkAPIPath, nationalPark.Id);
                //nationalPark.CreatedDate = objFromDb2.CreatedDate;

                succeed = await _npRepository.UpdateAsync(SD.NationalParkAPIPath, nationalPark.Id, nationalPark, token);
                if (succeed)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(nationalPark);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNationalPark()
        {
            var token = HttpContext.Session.GetString("JwToken");
            return Json(new { data = await _npRepository.GetAllAsync(SD.NationalParkAPIPath, token) });
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JwToken");
            var succeed = await _npRepository.DeleteAsync(SD.NationalParkAPIPath, id, token);

            if (succeed)
            {
                return Json(new { success = true, message = "National Park deleted successfully!" });
            }
            else
            {
                return Json(new { success = false, message = "Something goes wrong!" });
            }
        }
    }
}