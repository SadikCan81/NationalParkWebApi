using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class NationalParkRepository : INationalParkRepository
    {
        private readonly ApplicationDbContext _db;

        public NationalParkRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CreateNationalPark(NationalPark nationalPark)
        {
            if(nationalPark != null)
            {
                nationalPark.CreatedDate = DateTime.Now;
                _db.NationalParks.Add(nationalPark);
                return Save();
            }
            else
            {
                return false;
            }
        }

        public bool DeleteNationalPark(int id)
        {
            var removingNationalPark = _db.NationalParks.FirstOrDefault(x => x.Id == id);

            if(removingNationalPark != null)
            {
                _db.Remove(removingNationalPark);
                return Save();
            }
            else
            {
                return false;
            }
        }

        public NationalPark GetNationalPark(int id)
        {
            return _db.NationalParks.FirstOrDefault(x => x.Id == id);            
        }

        public ICollection<NationalPark> GetNationalParks()
        {
            return _db.NationalParks.OrderBy(a => a.Name).ToList();
        }

        public bool NationalParkExists(string name)
        {
            bool value = _db.NationalParks.Any(a => a.Name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public bool NationalParkExists(int id)
        {
            return _db.NationalParks.Any(a => a.Id == id);
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateNationalPark(NationalPark nationalPark)
        {
            if(nationalPark != null)
            {
                var createdDate = _db.NationalParks.AsNoTracking().FirstOrDefault(x => x.Id == nationalPark.Id).CreatedDate;
                nationalPark.CreatedDate = createdDate;

                _db.NationalParks.Update(nationalPark);
                return Save();
            }
            else
            {
                return false;
            }            
        }
    }
}
