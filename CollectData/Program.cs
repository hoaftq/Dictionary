using DataAccess.Data;
using System;

namespace CollectData
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new DictionaryContext())
            {
                new TratuParser(context).Parse();
            }
        }
    }
}
