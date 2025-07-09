using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransporteAPI.Data;
using TransporteAPI.Models;

namespace TransporteApi.Controllers
{
    [Route("api/[controller]")]
    [Controller]
    public class VehiculosController : Controller
    {

        private readonly AppDbContext _context;

        public VehiculosController(AppDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehiculo>>> GetAll() =>
       await _context.Vehiculos.ToListAsync();

        [HttpGet("GetById")]
        public async Task<ActionResult<Vehiculo>> GetById(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            return vehiculo == null ? NotFound() : Ok(vehiculo);
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Create(Vehiculo vehiculo)
        {
            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = vehiculo.Id }, vehiculo);
        }

        [HttpPut("Update")]
        public async Task<ActionResult> Update(int id, Vehiculo vehiculo)
        {
            if (id != vehiculo.Id)
            {
                return BadRequest();
            }

            _context.Entry(vehiculo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult> Delete(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null) return NotFound();
            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();
            return NoContent();
        }









    }
}
