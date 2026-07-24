using Newtonsoft.Json;
using SkyTicket.API;
using SkyTicket.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SkyTicket.Client.Services
{
    public class ApiService
    {

        public async Task<Response> GetAirplanes(string urlBase, string controller)
        {
            try
            {
                var client = new HttpClient();

                client.BaseAddress = new Uri(urlBase);

                var response = await client.GetAsync(controller);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSucess = false,
                        Message = result
                    };
                }

                var airplanes = JsonConvert.DeserializeObject<List<Airplane>>(result);

                return new Response
                {
                    IsSucess = true,
                    Result = airplanes
                };

            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> PostAirplane(string urlBase, string controller, Airplane airplane)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var json = JsonConvert.SerializeObject(airplane);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(controller, content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSucess = false,
                        Message = result
                    };
                }

                return new Response
                {
                    IsSucess = true
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> PutAirplane(string urlBase, string controller, int id, Airplane airplane)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var json = JsonConvert.SerializeObject(airplane);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"{controller}/{id}", content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSucess = false,
                        Message = result
                    };
                }

                return new Response
                {
                    IsSucess = true
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> DeleteAirplane(string urlBase, string controller, int id)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var response = await client.DeleteAsync($"{controller}/{id}");
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSucess = false,
                        Message = result
                    };
                }

                return new Response
                {
                    IsSucess = true
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
    