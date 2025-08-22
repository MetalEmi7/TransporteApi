using System.Linq;
using System.Text;
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
            ////Obtengo utilizando metodo del CORE
            //var vehiculo = await _context.Vehiculos.FindAsync(id);

            //Ejecucion de SP
            var vehiculo = await _context.Vehiculos.FromSqlRaw("EXEC [dbo].[sp_ObtenerVehiculoPorId] @Id = " + id ).ToListAsync();

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

            StringBuilder SB = new StringBuilder();

            SB.AppendLine("EXEC UpdateVehiculo");
            SB.AppendLine(vehiculo.Modelo != null ?"@Modelo" + vehiculo.Modelo : "");
            SB.AppendLine(vehiculo.Marca != null ? "@Marca" + vehiculo.Marca : "");
            SB.AppendLine(vehiculo.Anio != null ? "@Anio" + vehiculo.Anio : "");

            _context.Vehiculos.FromSqlRaw(SB.ToString());

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
        public async Task<ActionResult> GetVehicleByBrand(string brand)
        {
            try
            {
                /* * Buscamos vehículos por marca. 
                 * 
                 * Ejemplo: 
                 * 
                 * GET /api/Vehiculos/GetVehicleByBrand?brand=Toyota
                 * 
                 * Retorna todos los vehículos de la marca Toyota.
                 */
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
                /* * Buscamos vehículos cuyo modelo contenga la palabra proporcionada. */
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
                /* * Ordenamos los vehículos por año de forma descendente. */
                var Results = await _context.Vehiculos.OrderByDescending(x => x.Anio).ToListAsync();
                return Ok(Results);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
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
                /* * Seleccionamos solo algunas propiedades de los vehículos, en este caso Marca y Modelo. */
                var Results = await _context.Vehiculos.Select(x => new { x.Modelo, x.Marca }).ToListAsync();
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
                /* * Obtenemos un único elemento por ID. */

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
                /* * Agrupamos los vehículos por marca y contamos cuántos hay de cada una.
                 * 
                 * Resultado de ejemplo: 
                 * 
                 * Marca: Toyota
                 * Cantidad: 5
                 * 
                 * Marca: Ford
                 * Cantidad: 3
                 */

                var cantidadPorMarca = await _context.Vehiculos.GroupBy(v => v.Marca)
                    .Select(g => new { Marca = g.Key, Cantidad = g.Count() }).ToListAsync();

                /*
                 * Key: Marca
                 * Cantidad: Cantidad de vehículos por marca
                 */

                return Ok(cantidadPorMarca);
            }
            catch (Exception ex)
            {
                throw;
                return BadRequest(ex);
            }
        }

        /// <summary>
        ///  8.Obtener los 5 vehículos más nuevos
        /// </summary>
        /// <returns> Marca y Modelo </returns>
        [HttpGet("Obtener5VehículosMásNuevos")]
        public async Task<ActionResult> Obtener5VehículosMásNuevos()
        {
            try
            {
                var resultado = await _context.Vehiculos.OrderByDescending(x => x.Anio).Take(5).ToArrayAsync();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Borrar vehículos con ID en un intervalo específico.
        /// </summary>
        /// <param name="NumInicio">ID inicial del intervalo.</param>
        /// <param name="NumFinal">ID final del intervalo.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpDelete("BorrarVehiculosConIntervaloId")]
        public async Task<ActionResult> BorrarVehiculosConIntervaloId(int NumInicio, int NumFinal)
        {
            try
            {
                // Obtenemos los vehículos dentro del intervalo especificado.
                var vehiculosABorrar = await _context.Vehiculos
                    .Where(v => v.Id >= NumInicio && v.Id <= NumFinal)
                    .ToListAsync();

                if (!(vehiculosABorrar.Count() > 0))
                {
                    return NotFound("No se encontraron vehículos en el intervalo especificado.");
                }

                // Eliminamos los vehículos encontrados.
                _context.Vehiculos.RemoveRange(vehiculosABorrar);
                await _context.SaveChangesAsync();

                return Ok($"Se eliminaron {vehiculosABorrar.Count} vehículos en el intervalo de ID {NumInicio} a {NumFinal}.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error: {ex.Message}");
            }
        }
    }
}
