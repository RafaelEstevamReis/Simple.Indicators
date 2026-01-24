using Microsoft.AspNetCore.Mvc;
using Simple.Indicators.API.Models;

namespace Simple.Indicators.API.Controllers
{
    [Route("api/indicators")]
    [ApiController]
    public class IndicatorsController : ControllerBase
    {
        [HttpGet("tables")]
        public IActionResult GetTables()
        {
            var tables = TableHelper.GetAllTables();
            return Ok(tables.Select(o => o.GetType().FullName));
        }

        [HttpGet("table/info")]
        public IActionResult GetTableInfo([FromQuery] string tableName)
        {
            var table = TableHelper.GetTableByName(tableName);
            if (table == null) return NotFound("Table not found");
            return Ok(new TableInfo
            {
                FirstYear = table.Data_StartYear,
                AvailableDataPoints = table.Data.Sum(o => o.Length),
                Kind = table.Kind.ToString(),
            });
        }

        [HttpGet("table/range")]
        public IActionResult GetTableRange([FromQuery] string tableName, [FromQuery] DateTime firstMonth, [FromQuery] DateTime lastMonth)
        {
            var table = TableHelper.GetTableByName(tableName);
            if (table == null) return NotFound("Table not found");

            var data = DataHelpers.GetValueSpan(table, firstMonth, lastMonth);

            return Ok(new TableData
            {
                TableName = table.GetType().FullName!,
                FirstYear = table.Data_StartYear,
                Kind = table.Kind.ToString(),
                Data = data,
            });
        }

    }

}
