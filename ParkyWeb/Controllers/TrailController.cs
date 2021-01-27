using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModels;
using ParkyWeb.Repository.IRepository;
using ParkyWeb.Utility;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class TrailController : Controller
    {
        private readonly INationalParkRepository _npRepository;
        private readonly ITrailRepository _tRepository;

        public TrailController(INationalParkRepository npRepository, ITrailRepository tRepository)
        {
            _npRepository = npRepository;
            _tRepository = tRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            var token = HttpContext.Session.GetString("JwToken");
            IEnumerable<NationalPark> nationalParkList = await _npRepository.GetAllAsync(SD.NationalParkAPIPath, token);

            TrailVM trailVM = new TrailVM()
            {
                NationalParkList = nationalParkList.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                Trail = new Trail()
            };


            if (id == null)
            {
                return View(trailVM);
            }
            else
            {
                trailVM.Trail = await _tRepository.GetAsync(SD.TrailAPIPath, id.GetValueOrDefault(),token);
                if(trailVM.Trail == null)
                {
                    return NotFound();
                }

                return View(trailVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(TrailVM trailVM)
        {
            var token = HttpContext.Session.GetString("JwToken");

            if (!ModelState.IsValid)
            {
                IEnumerable<NationalPark> nationalParkList = await _npRepository.GetAllAsync(SD.NationalParkAPIPath, token);
                trailVM.NationalParkList = nationalParkList.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });

                return View(trailVM);
            }

            if(trailVM.Trail.Id == 0)
            {
                trailVM.Trail.DateCreated = DateTime.Now;
                var succeed = await _tRepository.CreateAsync(SD.TrailAPIPath, trailVM.Trail,token);

                if (succeed)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                var succeed = await _tRepository.UpdateAsync(SD.TrailAPIPath, trailVM.Trail.Id, trailVM.Trail,token);
                if (succeed)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound();
                }
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllTrail()
        {
            var token = HttpContext.Session.GetString("JwToken");
            return Json(new { data = await _tRepository.GetAllAsync(SD.TrailAPIPath,token) });
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JwToken");
            var succeed = await _tRepository.DeleteAsync(SD.TrailAPIPath, id,token);

            if (succeed)
            {
                return Json(new { success = true, message = "Trail deleted successfully!" });
            }
            else
            {
                return Json(new { success = false, message = "Something goes wrong!" });
            }
        }
    }
}