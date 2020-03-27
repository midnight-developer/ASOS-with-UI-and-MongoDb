using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASOS.Api.Data;
using ASOS.Api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASOS.Api.Controllers
{
  [Route("api/[controller]")]
  public class UserController : ControllerBase
  {
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserController(IUserRepository userRepository, IMapper mapper)
    {
      _userRepository = userRepository;
      _mapper = mapper;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
      var users = await _userRepository.GetAll();
      var userModel = _mapper.Map<IEnumerable<UserDto>>(users);
      return Ok(userModel);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
      var user = await _userRepository.Get(id);
      var userModel = _mapper.Map<UserDto>(user);
      return Ok(userModel);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody]CreateUserDto model)
    {
      
      var user = _mapper.Map<User>(model);
      await _userRepository.Create(user, model.Password);
      return Ok();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
      return Ok(await _userRepository.Delete(id));
    }
  }
}
