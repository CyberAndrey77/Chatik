using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Repository
{
    public class LogRepository : IRepository<Log>
    {
        private readonly ChatDb _context;

        public LogRepository(ChatDb context)
        {
            _context = context;
        }

        public void Create(Log item)
        {
            _context.Logs.Add(item);
            Save();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Log GetElement(int id)
        {
            throw new NotImplementedException();
        }

        public List<Log> GetElementList(int id)
        {
            return _context.Logs.OrderByDescending(x => x.Time).ToList();
        }

        public List<Log> GetElementList(RecordType type, DateTime start, DateTime end)
        {
            return _context.Logs.Where(x => x.Type == type).Where(x => x.Time > start).Where(x => x.Time < end).OrderByDescending(x => x.Time)
                .ToList();
        }

        public List<Log> GetElementList(DateTime start, DateTime end)
        {
            return _context.Logs.Where(x => x.Time > start).Where(x => x.Time < end).OrderByDescending(x => x.Time).ToList();
        }

        public List<Log> GetElementList(RecordType type)
        {
            return _context.Logs.Where(x => x.Type == type).OrderByDescending(x => x.Time).ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Log item)
        {
            throw new NotImplementedException();
        }
    }
}
