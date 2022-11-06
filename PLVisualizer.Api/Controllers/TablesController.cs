﻿using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using PlVisualizer.Api.Dto.Tables;
using PlVisualizer.Api.Dto.Exceptions;
using PLVisualizer.BusinessLogic.Services;

namespace PlVisualizer.Controllers;

[ApiController]
[Route("tables")]
public class TablesController : Controller
{
    private ITablesService tablesService;
    public TablesController(ITablesService tablesService)
    {
        this.tablesService = tablesService;
    }

    [HttpGet]
    [Route("import/{spreadsheetId}")]
    public async Task<ActionResult<Lecturer[]>> GetLecturersViaLecturersTableAsync([FromRoute]string spreadsheetId)
    {
        try
        {
            return await tablesService.GetLecturersViaLecturersTableAsync(spreadsheetId);
        }
        catch (SpreadsheetNotFoundException)
        {
            return NotFound();
        }
        
    }
    
    [HttpPost]
    [Route("import/file")]
    public async Task<ActionResult> UploadFile([FromBody] IFormFile file)
    {
        try
        {
            await tablesService.UploadFile(file);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("import/config/{spreadsheetId}")]
    public async Task<ActionResult<Lecturer[]>> GetLecturerViaConfigAsync([FromRoute] string spreadsheetId)
    {
        try
        {
            return await tablesService.GetLecturersViaConfigAsync(spreadsheetId);
        }
        catch (SpreadsheetNotFoundException e)
        {
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("export/{spreadsheetId}")]
    public async Task<ActionResult> ExportLecturersAsync(string spreadsheetId, [FromBody]Lecturer[] lecturers)
    {
        try
        {
            await tablesService.ExportLecturersAsync(spreadsheetId, lecturers);
            return Ok();
        }
        catch (SpreadsheetNotFoundException e)
        {
            return NotFound();
        }
    }
}