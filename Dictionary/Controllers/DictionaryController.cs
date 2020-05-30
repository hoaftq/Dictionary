using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Dictionary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        private readonly DictionaryContext context;

        public DictionaryController(DictionaryContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult<Word> Get([FromQuery]string word)
        {
            var foundWord = context.Words.Include(w => w.Definitions)
                                    .ThenInclude(d => d.Dictionary)
                                .Include(w => w.Definitions)
                                    .ThenInclude(d => d.WordClass)
                                .Include(w => w.Definitions)
                                    .ThenInclude(d => d.Usages)
                                .Include(w => w.Phases)
                                .Include(w => w.WordForms)
                                .Include(w => w.RelativeWords)
                                .FirstOrDefault(w => w.Content == word);
            if (foundWord == null)
            {
                return NotFound();
            }

            return foundWord;
        }

        [HttpPost]
        public IEnumerable<Word> Search([FromBody]string word)
        {
            return context.Words.Where(w => w.Content.StartsWith(word));
        }
    }
}
