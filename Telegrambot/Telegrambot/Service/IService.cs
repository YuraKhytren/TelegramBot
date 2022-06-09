using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telegrambot.Service
{
    public  interface IService
    {
         Task<ExchangeRateList> GetCurencyList(DateTime date);
    }
}
