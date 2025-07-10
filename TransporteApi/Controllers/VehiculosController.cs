using System.Linq;
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

        /// <summary>
        /// 1. Hacemnos el cotejamiento dentro del DbContext
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        [HttpGet("GetVehicleByBrand")]
        public async Task<ActionResult> GetVehicleByBrand (string brand)
        {
            try
            {
                var Results = await _context.Vehiculos.Where(x => x.Marca == brand).ToListAsync();
                var ResultsDOS = await _context.Vehiculos.Where(x => x.Marca == brand).ToArrayAsync();

                return Ok(ResultsDOS);
            }
            catch (Exception ex)
            {

                throw;
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// 2. Buscar por palabra contenida (Vehiculos.Modelo)
        /// </summary>
        /// <param name="palabra"></param>
        /// <returns></returns>
        [HttpGet("BuscarPorPalabraContenida_SobreModelo")]
        public async Task<ActionResult> BuscarPorPalabraContenida_SobreModelo(string palabra)
        {
            try
            {
                var Results = await _context.Vehiculos.Where(x => x.Modelo.Contains(palabra)).ToListAsync();
                return Ok(Results);
            }
            catch (Exception ex)
            {

                throw;
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// 3. Ordenar por año descendente
        /// </summary>
        /// <returns></returns>
        [HttpGet("OrdenarPorAnioDescendente")]
        public async Task<ActionResult> OrdenarPorAnioDescendente()
        {
            try
            {
                var Results = await _context.Vehiculos.OrderByDescending(x => x.Anio).ToListAsync();
                return Ok(Results);
            }
            catch (Exception ex)
            {

                throw;
                return BadRequest(ex);
            }
        }


        /// <summary>
        /// 4. Seleccionar solo algunas propiedades (proyección)
        /// </summary>
        /// <returns> Marca y Modelo </returns>
        [HttpGet("SeleccionarSoloAlgunasPropiedades")]
        public async Task<ActionResult> SeleccionarSoloAlgunasPropiedades()
        {
            try
            {
                var Results = await _context.Vehiculos.Select(x => new { x.Modelo, x.Marca}).ToListAsync();
                return Ok(Results);
            }
            catch (Exception ex)
            {

                throw;
                return BadRequest(ex);
            }
        }



        /// <summary>
        /// 5. Obtener un único elemento por ID
        /// </summary>
        /// <returns> Marca y Modelo </returns>
        [HttpGet("ObtenerUnicoElementoPorID")]
        public async Task<ActionResult> ObtenerUnicoElementoPorID(int id)
        {
            try
            {
                var Results = await _context.Vehiculos.FirstOrDefaultAsync(x => x.Id == id);
                return Ok(Results);
            }
            catch (Exception ex)
            {

                throw;
                return BadRequest(ex);
            }
        }


        /// <summary>
        /// 6. Contar cuántos vehículos hay por marca
        /// </summary>
        /// <returns> Marca y Modelo </returns>
        [HttpGet("ContarCuántosVehículosHayPorMarca")]
        public async Task<ActionResult> ContarCuántosVehículosHayPorMarca(int id)
        {
            try
            {
                var cantidadPorMarca = await _context.Vehiculos.GroupBy(v => v.Marca).Select(g => new { Marca = g.Key, Cantidad = g.Count() }).ToListAsync();                

                return Ok(cantidadPorMarca);
            }
            catch (Exception ex)
            {

                throw;
                return BadRequest(ex);
            }
        }
    }
}
