using Newtonsoft.Json;
using rare_crew_c__test.DTO;
using rare_crew_c__test.Models;

namespace rare_crew_c__test.ApiService
{
    public class Service
    {
        private readonly HttpClient _http;
        private readonly string _url;
        private readonly string _endpoint;
        private readonly string _key;

        public Service(HttpClient http, IConfiguration configuration)
        {
            _http = http;
            _url = configuration["ApiSettings:Url"];
            _endpoint = configuration["ApiSettings:Endpoint"];
            _key = configuration["ApiSettings:Key"];
        }
        public async Task<List<Employee>> GetEmployees()
        {
            List<Employee> employees = new List<Employee>();
            HttpResponseMessage response = await _http.GetAsync(this._url + "/" + this._endpoint + this._key);
            var errorMsg = "";
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    employees = JsonConvert.DeserializeObject<List<Employee>>(jsonData);
                }
                else
                {
                    errorMsg = "Request failed: " + response.StatusCode;
                    await Console.Out.WriteLineAsync(errorMsg);
                }
            }
            catch (Exception ex)
            {
                errorMsg = "Unexpected error: " + ex.Message;
                await Console.Out.WriteLineAsync(errorMsg);

            }
            return employees;
        }
        public async Task<List<EmployeeDTO>> GetEmployeesWithWorkingHours(List<Employee> employees)
        {
            var employeesWorkingHours = employees.Where(x => !string.IsNullOrEmpty(x.EmployeeName) && x.DeletedOn == null && x.StarTimeUtc < x.EndTimeUtc)
                                              .GroupBy(x => x.EmployeeName)
                                              .Select(e =>
                                              {
                                                  double totalWorkingHours = e.Sum(t =>
                                                  {
                                                      var span = t.EndTimeUtc - t.StarTimeUtc;
                                                      var miliseconds = span.TotalMilliseconds;
                                                      return Math.Floor(miliseconds / 3600000);
                                                  });
                                                  return new EmployeeDTO
                                                  {
                                                      Name = e.Key,
                                                      Total_Hours = (int)totalWorkingHours
                                                  };
                                                  //Total_Hours = (int)e.Sum(w=>(w.EndTimeUtc - w.StarTimeUtc).TotalHours)
                                                  //*Zbog razlike izmedju kalkulisanja i zaokruzivanja brojeva u Angularu i C# uradio sam na ovaj nacin da bi se vrednosti poklopile
                                              })
                                              .OrderByDescending(e => e.Total_Hours)
                                              .ToList();
            return employeesWorkingHours;
        }
    }
}
