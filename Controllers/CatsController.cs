using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wg_backend.Models;


namespace wg_backend.Controllers
{
    [Route("")]
    [ApiController]

    public class CatsController : ControllerBase
    {
        private readonly WgContext _context;
   



        public CatsController(WgContext context)
        {
            _context = context;
            if (_context.Cats.Count() == 0)
            {
                Console.WriteLine("Котиков нет");
            }
          
        }

        // GET /ping
        [HttpGet]
        [Route("")]
        [Route("ping")]
        public String[] GetPing()
        {
            IPHostEntry heserver = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = heserver.AddressList.ToList().Where(p => p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).FirstOrDefault().ToString();

            string msg = "Cats Service. Version 0.1";
            return new string[] { msg, "Ваш IP=", ipAddress }; ;
        }

        // GET /cats /cats?attribute=name&order=asc
        [HttpGet]
        [Route("cats")]
        public async Task<ActionResult<IEnumerable<Cat>>> GetCatsAtribut(string attribute = "no", string order = "no", int limit = -1, int offset = 0)
        {
            List<Cat> catics = new List<Cat>();
            catics = await _context.Cats.ToListAsync();


            switch (order)
            {
                case "asc":

                    switch (attribute)
                    {
                        case "name": catics = catics.OrderBy(n => n.Name).ToList(); break;
                        case "color": catics = catics.OrderBy(n => n.Color).ToList(); break;
                        case "tail_length": catics = catics.OrderBy(n => n.Tail_length).ToList(); break;
                        case "Whiskers_length": catics = catics.OrderBy(n => n.Whiskers_length).ToList(); break;

                    }; break;

                case "desc":
                    switch (attribute)
                    {
                        case "name": catics = catics.OrderByDescending(n => n.Name).ToList(); break;
                        case "color": catics = catics.OrderByDescending(n => n.Color).ToList(); break;
                        case "tail_length": catics = catics.OrderByDescending(n => n.Tail_length).ToList(); break;
                        case "Whiskers_length": catics = catics.OrderByDescending(n => n.Whiskers_length).ToList(); break;

                    }; break;
            }
            if ((limit > 0) & (limit < catics.Count())) catics = catics.Skip(offset).Take(limit).ToList();
            return catics;
        }



        // GET /cats
        [HttpGet]
        [Route("color")]
        public async Task<ActionResult<IEnumerable<Cat_colors_info>>> Get()
        {
            List<Cat> cats = await _context.Cats.ToListAsync();
            List<Cat_colors_info> list_cat_Colors_Info = new List<Cat_colors_info>();
            var colorgroup = from cat in cats
                             group cat by cat.Color into g
                             select new Cat_colors_info() { Color = g.Key, Count = g.Count() };

            if (_context.Cat_colors_infs.Count() != 0)
            {

                foreach (var item in _context.Cat_colors_infs)
                {
                    _context.Cat_colors_infs.Remove(item);

                }
            }

            foreach (var item in colorgroup)
            {
                _context.Cat_colors_infs.Add(new Cat_colors_info() { Color = item.Color, Count = item.Count });
            }
            await _context.SaveChangesAsync();

            return colorgroup.ToList();
        }

        [HttpGet]
        [Route("stat")]
        public async Task<ActionResult<IEnumerable<Cats_stat>>> GetStat()
        {
            if (_context.Cats_stats.Count() != 0)
            {
                foreach (var item in _context.Cats_stats)
                {
                    _context.Cats_stats.Remove(item);
                }
                await _context.SaveChangesAsync();
            }
            List<Cat> cats = await _context.Cats.ToListAsync();

            Cats_stat cats_Stat = new Cats_stat();
            int count = _context.Cats.Count();
            int[] tail = _context.Cats.Select(t => t.Tail_length).ToArray();
            int[] whiskers = _context.Cats.Select(t => t.Whiskers_length).ToArray();
            //средняя длина хвоста
            cats_Stat.tail_length_mean = tail.Average();
            //средняя длина усов,
            cats_Stat.whiskers_length_mean = tail.Average();
            //медиана длин хвостов,
            int numberCount = count;
            int halfIndex = count / 2;
            var sortedNumbers = tail.OrderBy(n => n);
            if ((numberCount % 2) == 0)
            {
                cats_Stat.tail_length_median = ((sortedNumbers.ElementAt(halfIndex) +
                    sortedNumbers.ElementAt((halfIndex - 1))) / 2);
            }
            else
            {
                cats_Stat.tail_length_median = sortedNumbers.ElementAt(halfIndex);
            }
            //медиана длин усов,
            numberCount = count;
            halfIndex = count / 2;
            sortedNumbers = whiskers.OrderBy(n => n);
            if ((numberCount % 2) == 0)
            {
                cats_Stat.whiskers_length_median = ((sortedNumbers.ElementAt(halfIndex) +
                    sortedNumbers.ElementAt((halfIndex - 1))) / 2);
            }
            else
            {
                cats_Stat.whiskers_length_median = sortedNumbers.ElementAt(halfIndex);
            }
            //мода длин хвостов,
            cats_Stat.tail_length_mode = ModaCalc(tail);
            //мода длин усов.
            cats_Stat.whiskers_length_mode = ModaCalc(whiskers);
            _context.Cats_stats.Add(cats_Stat);
            await _context.SaveChangesAsync();
            return _context.Cats_stats.ToList();
        }

        // GET api/name
        [HttpGet("{name}")]
        public async Task<ActionResult<Cat>> GetCatName(string name)
        {
            var cat = await _context.Cats.FindAsync(name);

            if (cat == null)
            {
                return NotFound();
            }

            return cat;
        }

        // POST 
        [Route("cat")]
        [HttpPost]
        public async Task<ActionResult<Cat>> AddCat([FromBody] Cat cat)
        {
            var catNew = await _context.Cats.FindAsync(cat.Name);

            if (catNew != null)
            {
                return BadRequest("Такой кот уж есть., назови его по другому");
            }

            if (cat.Tail_length < 0 || cat.Whiskers_length < 0)
            {
                return BadRequest("Измерь еще раз хвоосты, усы");
            }

            _context.Cats.Add(cat);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCatName), new { name = cat.Name }, cat);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        //Вычисление моды
        static int[] ModaCalc(int[] inArr)
        {
            int[] arr = inArr;
            var arrMod = arr.GroupBy(p => p)
            .Select(g => new
            {
                Name = g.Key,
                Count = g.Count()
            });
            arrMod.GroupBy(p => p.Count);
            List<int> res = new List<int>();
            int max = 0;
            foreach (var item in arrMod)
            {
                if (item.Count >= max)
                {
                    res.Add(item.Name);
                    max = item.Count;
                }
            }
            return res.ToArray();
        }


    }
}
