using Newtonsoft.Json;
using SkyTicket.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Sockets;
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

        public async Task<Response> GetAirports(string urlBase, string controller)
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

                var airports = JsonConvert.DeserializeObject<List<Airport>>(result);

                return new Response
                {
                    IsSucess = true,
                    Result = airports
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

        public async Task<Response> PostAirport(string urlBase, string controller, Airport airport)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var json = JsonConvert.SerializeObject(airport);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(controller, content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                return new Response { IsSucess = true };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<Response> PutAirport(string urlBase, string controller, int id, Airport airport)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var json = JsonConvert.SerializeObject(airport);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"{controller}/{id}", content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                return new Response { IsSucess = true };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<Response> DeleteAirport(string urlBase, string controller, int id)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var response = await client.DeleteAsync($"{controller}/{id}");
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                return new Response { IsSucess = true };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<Response> GetPassengers(string urlBase, string controller)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var response = await client.GetAsync(controller);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                var passengers = JsonConvert.DeserializeObject<List<Passenger>>(result);

                return new Response { IsSucess = true, Result = passengers };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<Response> PostPassenger(string urlBase, string controller, Passenger passenger)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var json = JsonConvert.SerializeObject(passenger);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(controller, content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                return new Response { IsSucess = true };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<Response> PutPassenger(string urlBase, string controller, int id, Passenger passenger)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var json = JsonConvert.SerializeObject(passenger);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"{controller}/{id}", content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                return new Response { IsSucess = true };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<Response> DeletePassenger(string urlBase, string controller, int id)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var response = await client.DeleteAsync($"{controller}/{id}");
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                return new Response { IsSucess = true };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<Response> GetFlights(string urlBase, string controller)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var response = await client.GetAsync(controller);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                var flights = JsonConvert.DeserializeObject<List<Flight>>(result);

                return new Response { IsSucess = true, Result = flights };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<Response> PostFlight(string urlBase, string controller, Flight flight)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var json = JsonConvert.SerializeObject(flight);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(controller, content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                return new Response { IsSucess = true };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<Response> PutFlight(string urlBase, string controller, int id, Flight flight)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var json = JsonConvert.SerializeObject(flight);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"{controller}/{id}", content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                return new Response { IsSucess = true };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<Response> DeleteFlight(string urlBase, string controller, int id)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var response = await client.DeleteAsync($"{controller}/{id}");
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                return new Response { IsSucess = true };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<Response> GetTickets(string urlBase, string controller)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var response = await client.GetAsync(controller);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                var tickets = JsonConvert.DeserializeObject<List<Ticket>>(result);

                return new Response { IsSucess = true, Result = tickets };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<Response> GetSeatsByFlight(string urlBase, string controller, int flightId)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var response = await client.GetAsync($"{controller}/{flightId}/seats");
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                var seats = JsonConvert.DeserializeObject<List<Seat>>(result);

                return new Response { IsSucess = true, Result = seats };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<Response> PostTicket(string urlBase, string controller, Ticket ticket)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var json = JsonConvert.SerializeObject(ticket);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(controller, content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                return new Response { IsSucess = true };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<Response> DeleteTicket(string urlBase, string controller, int id)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var response = await client.DeleteAsync($"{controller}/{id}");
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                return new Response { IsSucess = true };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<Response> GetTicketsByFlight(string urlBase, string controller, int flightId)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var response = await client.GetAsync($"{controller}/flight/{flightId}");
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response { IsSucess = false, Message = result };
                }

                var tickets = JsonConvert.DeserializeObject<List<Ticket>>(result);

                return new Response { IsSucess = true, Result = tickets };
            }
            catch (Exception ex)
            {
                return new Response { IsSucess = false, Message = ex.Message };
            }
        }
    }
}
    