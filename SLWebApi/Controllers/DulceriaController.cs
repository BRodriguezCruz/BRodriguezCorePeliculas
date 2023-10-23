using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SLWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DulceriaController : ControllerBase
    {
        [Route("")]
        [HttpGet]
        public IActionResult GetAll()
        {
            ML.Result result = BL.Dulceria.GetAll();

            if (result.Correct)
            {
                return StatusCode(200, result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [Route("{idProducto}")]
        [HttpGet]
        public IActionResult GetById(int idProducto)
        {
            ML.Result result = BL.Dulceria.GetById(idProducto);

            if (result.Correct)
            {
                return StatusCode(200, result);
            }
            else
            {
                return BadRequest(result);
            }
        }

    }
}
