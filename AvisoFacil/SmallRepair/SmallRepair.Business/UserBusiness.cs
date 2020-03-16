using SmallRepair.Business.HttpClient;
using SmallRepair.Business.Model;
using SmallRepair.Management.Model;
using SmallRepair.Management.Repository;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmallRepair.Business
{
    public class UserBusiness
    {
        private readonly RepositoryEntity _repository;
        private readonly UserApi _userApi;
        private readonly string _systemId;

        public UserBusiness(RepositoryEntity repository,
            System.Net.Http.HttpClient httpClient,
            string systemId)
        {
            _repository = repository;
            _userApi = new UserApi(httpClient);
            _systemId = systemId;
        }

        public async Task<ResponseMessage<User>> AddAsync(User user)
        {
            ResponseMessage<User> responseMessage;

            try
            {
                User userDb = _repository.All<User>(a => a.Email == user.Email).FirstOrDefault();

                if (userDb == null)
                {
                    Company customer = _repository
                                        .All<Company>(a => a.IdCompany == user.IdCompany)
                                        .FirstOrDefault();


                    var userReponse = await _userApi.AddUserApiAsync(new UserApiModel()
                    {
                        CompanyId = customer.IdCompany,
                        Login = user.Email,
                        Name = user.Name,
                        SystemId = _systemId,
                        ConfirmPassword = user.Email,
                        Password = user.Email
                    });

                    user.ClaimId = userReponse.Object.Id;

                    _repository.Add(user);

                    _repository.SaveChanges();

                    responseMessage = ResponseMessage<User>.Ok(user);
                }
                else
                {
                    responseMessage = ResponseMessage<User>.Fault("Já existe um usuário com esse e-mail.");
                }
            }
            catch (Exception ex)
            {
                responseMessage = ResponseMessage<User>.Fault(ex.Message);
            }

            return responseMessage;
        }
    }
}
