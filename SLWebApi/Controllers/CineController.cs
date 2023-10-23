using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SLWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CineController : ControllerBase
    {
        [Route("")]
        [HttpGet]
        public IActionResult GetAll()
        {
            ML.Result result = BL.Cine.GetAll();

            if (result.Correct)
            {
                return Ok(result);
                // o return StatusCode(200, result);  ---> este es mas maleable ya que se puede manipular el codigo
            }
            else
            {
                return StatusCode(400, result);
                //return BadRequest(result); -- > funcionan igual ambos return
            }
        }

        [Route("{idCine}")]
        [HttpGet]
        public IActionResult GetById(int idCine)
        {
            ML.Result result = BL.Cine.GetById(idCine);

            if (result.Correct)
            {
                return StatusCode(200, result);
            }
            else
            {
                return BadRequest(result);
            }
        }


        [Route("")]
        [HttpPost]
        public IActionResult Add(ML.Cine cine)
        {
            ML.Result result = BL.Cine.Add(cine);

            if (result.Correct)
            {
                return StatusCode(200,result);
            }
            else
            {
                return StatusCode(400,result);
            }
        }


        [Route("{idCine}")]
        [HttpPut]
        public IActionResult Update(int idCine, [FromBody]ML.Cine cine)
        {
            cine.IdCine = idCine; //igualamos para jalar el id del headear haciea el body e ingrese al BL, completo.
            ML.Result result = BL.Cine.Update(cine);

            if(result.Correct)
            {
                return StatusCode(200,result);
            }
            else
            {
                return StatusCode(400,result);
            }
        }

        [Route("{idCine}")]
        [HttpDelete]
        public IActionResult Delete(int idCine) 
        { 
            ML.Result result = BL.Cine.Delete(idCine);

            if( result.Correct) 
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
