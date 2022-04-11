using Microsoft.AspNetCore.Mvc;
using ToDoApp.Models;
using ToDoApp.Repositories;
using ToDoApp.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ToDoApp.Controllers;

[ApiController]
[Route("api/Users")]
 [Authorize]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUsersRepository _user;
  //  private readonly IProductRepository _Product;
   // private readonly IOrderRepository _order;

    public UsersController(ILogger<UsersController> logger,
    IUsersRepository user)
    {
        _logger = logger;
        _user = user;
       //// _Product = Product;
       //this._order = _order;
    }

    [HttpGet]
    public async Task<ActionResult<List<UsersDTO>>> GetList()
    {
        var res = await _user.GetList();

        return Ok(res.Select(x => x.asDto));
    }

    [HttpGet("{user_id}")]
    public async Task<ActionResult> GetById([FromRoute] int user_id)
    {
        var res = await _user.GetById(user_id);

        if (res == null)
            return NotFound();

        var dto = res.asDto;
       // dto.Products = (await _Product.GetListByCustomerId(id))
                        //.Select(x => x.asDto).ToList();
      //  dto.Orders = (await _order.GetListByCustomerId(id)).Select(x => x.asDto).ToList();

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] UsersDTO Data)
    {
        var toCreateUsers = new Users
        {
            UserName = Data.UserName?.Trim(),
          //  Gender = Data.Gender?.Trim(),
            Password = Data.Password,
           // Email = Data.Email
            
          
          
        };

        var res = await _user.Create(toCreateUsers);

        return StatusCode(StatusCodes.Status201Created, res.asDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UsersCreateDTO Data)
    {
        var existingUsers = await _user.GetById(id);

        if (existingUsers == null)
            return NotFound();

        var toUpdateUsers = existingUsers with
        {
           
            UserName = Data.UserName?.Trim(),
            Password = Data.Password?.Trim(),
           // Email = Data.Email
        };

        var didUpdate = await _user.Update(toUpdateUsers);

        if (!didUpdate)
            return StatusCode(StatusCodes.Status500InternalServerError);

        return NoContent();
    }
    [HttpDelete("{user_id}")]
    public async Task<ActionResult> DeleteUsers([FromRoute] int user_id)
    {
        var existing = await _user.GetById(user_id);
        if (existing is null)
            return NotFound("No users found with given id");

        await _user.Delete(user_id);

        return NoContent();
    }

}
