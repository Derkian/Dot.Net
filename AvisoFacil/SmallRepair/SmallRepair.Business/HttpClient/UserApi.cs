using SmallRepair.Business.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;


namespace SmallRepair.Business.HttpClient
{
    public class UserApi
    {
        public readonly System.Net.Http.HttpClient _httpClient;

        public UserApi(System.Net.Http.HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseMessage<UserApiResponse>> AddUserApiAsync(UserApiModel model)
        {
            ResponseMessage<UserApiResponse> responseMessage;

            try
            {
                var response = await _httpClient.PostAsJsonAsync("user/register", model);

                response.EnsureSuccessStatusCode();

                var userApiResponse = await response.Content.ReadAsAsync<UserApiResponse>();

                responseMessage = ResponseMessage<UserApiResponse>.Ok(userApiResponse);
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<UserApiResponse>.Fault(ex.Message);
            }

            return responseMessage;
        }
    }
}
