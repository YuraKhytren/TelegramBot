using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Telegrambot.Service
{
    public  class Service : IService
    {
        public  async Task<ExchangeRateList> GetCurencyList(DateTime date)
        {
            string dateTime="";
            try
            {
                dateTime = date.ToString("MM.dd.yyyy");
            }
            catch (Exception)
            {

            }

            HttpClient client = new HttpClient();
            HttpResponseMessage responseMessage = await client.GetAsync("https://api.privatbank.ua/p24api/exchange_rates?json&date=" + dateTime);
            HttpContent content = responseMessage.Content;
            var list = await content.ReadFromJsonAsync<ExchangeRateList>();
            
            return list;
        }
    }
}
