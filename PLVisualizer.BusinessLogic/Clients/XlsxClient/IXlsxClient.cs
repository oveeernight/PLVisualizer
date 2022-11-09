﻿using Microsoft.AspNetCore.Http;
using PlVisualizer.Api.Dto.Tables;

namespace PLVisualizer.BusinessLogic.Clients.XlsxClient;

public interface IXlsxClient
{
    public void SetFile(IFormFile file);
    public void SetFile(string path);
    XlsxTableRow[] GetTableRows();
}