using Microsoft.AspNetCore.Mvc;
using ToDoApp.Models;
using ToDoApp.Repositories;
using ToDoApp.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ToDoApp.Controllers;

[ApiController]
[Route("api/to_do_list")]

[Authorize]


public class ToDoListController : ControllerBase
{
    private readonly ILogger<ToDoListController> _logger;
    private readonly IToDoListRepository _ToDoList;
    //  private readonly IProductRepository _Product;
    //private readonly IToDoListRepository _order;

    public ToDoListController(ILogger<ToDoListController> logger,
    IToDoListRepository ToDoList)
    {
        _logger = logger;
        _ToDoList = ToDoList;
        //// _Product = Product;
        // this._order = _order;
    }

    [HttpGet]
    public async Task<ActionResult<List<ToDoListDTO>>> GetList()
    {
        var res = await _ToDoList.GetList();

        return Ok(res.Select(x => x.asDto));
    }

    [HttpGet("{to_do_id}")]
    public async Task<ActionResult> GetById([FromRoute] int to_do_id)
    {
        //        var idClaim = User.Claims.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.InvariantCultureIgnoreCase));
        //      if(idClaim != null){
        //          return Ok($"This is your Id: {idClaim.value}");
        //      }
        //      return BadRequest("No Claim");
        var res = await _ToDoList.GetById(to_do_id);

        if (res == null)
            return NotFound();

        var dto = res.asDto;
        // dto.Products = (await _Product.GetListByCustomerId(id))
        //.Select(x => x.asDto).ToList();
        //  dto.Orders = (await _order.GetListByCustomerId(id)).Select(x => x.asDto).ToList();

        return Ok(dto);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Create([FromBody] ToDoListDTO Data)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity != null)
        {
            IEnumerable<Claim> claims = identity.Claims;
            var token = claims.First(claim => claim.Type == ClaimTypes.SerialNumber).Value;
            //Console.WriteLine("user id" + jijij);


            var toCreateToDoList = new ToDoList
            {

                Title = Data.Title,
                Description = Data.Description,
                CreatedAt = Data.CreatedAt,
                UpdatedAt = Data.UpdatedAt,
                IsCompleted = Data.IsCompleted,
                IsDeleted = Data.IsDeleted,
                UserId = int.Parse(token)

            };

            Console.WriteLine("todo " + toCreateToDoList);


            var res = await _ToDoList.Create(toCreateToDoList);

            return StatusCode(StatusCodes.Status201Created, res.asDto);

        }
        else
        {
            return StatusCode(StatusCodes.Status401Unauthorized, "Un");

        }

        //         var headers = Request.Headers.Authorization.ToString();
        // var jwt = headers.Replace("Bearer ", string.Empty);

        // Console.WriteLine(headers);

        // var handler = new JwtSecurityTokenHandler();
        // Console.WriteLine("can validate token: " + handler.CanValidateToken.ToString());
        // Console.WriteLine("can read token: " + handler.CanReadToken(jwt).ToString());

        // var jsonToken = handler.ReadJwtToken(jwt);
        // var tokenS = jsonToken as JwtSecurityToken;




        // var jti = tokenS.Claims.First(claim => claim.Type == ClaimTypes.SerialNumber).Value;


    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] ToDoListCreateDTO Data)
    {
        var existingToDoList = await _ToDoList.GetById(id);

        if (existingToDoList == null)
            return NotFound();

        var toUpdateToDoList = existingToDoList with
        {
            Title = Data.Title,
            Description = Data.Description,
        };

        var didUpdate = await _ToDoList.Update(toUpdateToDoList);

        if (!didUpdate)
            return StatusCode(StatusCodes.Status500InternalServerError);

        return NoContent();
    }
    [HttpDelete("{to_do_id}")]
    [Authorize]
    public async Task<ActionResult> DeleteUsers([FromRoute] int to_do_id)
    {
        var existing = await _ToDoList.GetById(to_do_id);
        if (existing is null)
            return NotFound("No list found with given id");

        await _ToDoList.Delete(to_do_id);

        return NoContent();
    }

}
