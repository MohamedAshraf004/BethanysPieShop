using BethanysPieShop.Contexts;
using BethanysPieShop.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanysPieShop.Repositories
{
    public class PieRepository : IPieRepository
    {
        private readonly AppDbContext _dbContext;

        public PieRepository(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public IEnumerable<Pie> AllPies => _dbContext.Pies.Include(c=>c.Category);

        public IEnumerable<Pie> PiesOfTheWeek => _dbContext.Pies.Include(c => c.Category).Where(p => p.IsPieOfTheWeek);

        public Pie GetPieById(int pieId)
        {
           return _dbContext.Pies.FirstOrDefault(p => p.PieId == pieId);
        }
    }
}
