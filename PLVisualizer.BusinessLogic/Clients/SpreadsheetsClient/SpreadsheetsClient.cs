﻿using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Vml.Office;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using PlVisualizer.Api.Dto.Exceptions;
using PlVisualizer.Api.Dto.Tables;

namespace PLVisualizer.BusinessLogic.Clients.SpreadsheetsClient;
using Google.Apis.Sheets.v4;

/// <summary>
/// Represents google spreadsheets client
/// </summary>
public class SpreadsheetsClient : ISpreadsheetsClient
{
    private readonly string applicationName = "PLVisualizer";
    private readonly string[] scopes = { SheetsService.Scope.Spreadsheets };
    private readonly GoogleCredential credential;
    private readonly SheetsService service;

    public SpreadsheetsClient()
    {
        // using var stream = new FileStream("../../../../../PLVisualizer/PLVisualizer.BusinessLogic/Clients/SpreadsheetsClient/credentials.json", 
        //     FileMode.Open, FileAccess.Read);
        credential = GoogleCredential.GetApplicationDefault().CreateScoped(scopes);
        service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            }
        );
    }

    public async Task ExportLecturersAsync(string spreadsheetId, Lecturer[] lecturers, string sheetTitle)
    {
        var range = $"{sheetTitle}!A:F";
        var valueRange = new ValueRange();
        var values = ToValues(lecturers);
        valueRange.Values = values;
        var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
        appendRequest.ValueInputOption =
            SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        await appendRequest.ExecuteAsync();

        var spreadsheet = await service.Spreadsheets.Get(spreadsheetId).ExecuteAsync();
        var sheet = spreadsheet.Sheets.FirstOrDefault(sheet => sheet.Properties.Title == sheetTitle);
        if (sheet == null)
        {
            throw new SheetNotFoundException();
        }
        var sheetId = sheet.Properties.SheetId;
        
        var formatTableRequests = GetFormatTableRequests(lecturers, sheetId);
        var formatRequest = service.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest
        {
            Requests = formatTableRequests
        }, spreadsheetId);
        await formatRequest.ExecuteAsync();
    }

    public async Task<Lecturer[]> GetLecturersAsync(string spreadsheetId, string sheetTitle)
    {
        var range = $"{sheetTitle}!A:F";
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = await request.ExecuteAsync();
        var values = response.Values.Skip(1).ToArray();
        return ToLecturersModel(values);
    }

    public async Task<ConfigTableRow[]> GetConfigTableRowsAsync(string spreadsheetId, string sheetTitle)
    {
        var range = $"{sheetTitle}!A:C";
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = await request.ExecuteAsync();
        var values = response.Values.Skip(1).ToArray();
        return ToConfigModel(values);
    }

    private static  IList<IList<object>> ToValues(Lecturer[] lecturers)
    {
        var values = new List<IList<object>> 
            { new List<object>() {"ФИО", "Должность", "Процент ставки", "Дисциплины", "Распределенная нагрузка", "Норматив"} };
        foreach (var lecturer in lecturers)
        {
            values.AddRange(lecturer.Disciplines.Select(discipline => new List<object>
            {
                lecturer.Name,
                lecturer.Post,
                lecturer.InterestRate,
                discipline.Content,
                lecturer.DistributedLoad,
                lecturer.Standard
            }));
        }

        return values;
    }

    private static ConfigTableRow[] ToConfigModel(IEnumerable<IList<object>> configTableRows)
    {
        return configTableRows.Select(configTableRow => new ConfigTableRow { 
                LecturerName = configTableRow[0].ToString() ?? string.Empty,
                Post = configTableRow[1].ToString() ?? string.Empty,
                InterestRate = int.Parse(configTableRow[2].ToString() ?? string.Empty) })
            .ToArray();
    }
    
    private  static Lecturer[] ToLecturersModel(IList<IList<object>> lecturers)
    {
        // otherwise response from google will contain 4 elements in a row
        const int lecturerHeaderCount = 6;
        var models = new List<Lecturer>();
        var previousLecturer = lecturers[0];
        var disciplines = new List<Discipline>();
        var lecturer = new Lecturer();
        for(var i = 0; i < lecturers.Count; i++)
        {
            //iterating through the same lecturer disciplines
            if (lecturers[i].Count != lecturerHeaderCount)
            { 
                var disciplineContent = lecturers[i][3].ToString() ?? string.Empty;
                var discipline = CreateDiscipline(disciplineContent);
                disciplines.Add(discipline);

                if (i == lecturers.Count - 1 || lecturers[i + 1].Count == lecturerHeaderCount)
                {
                    lecturer.Disciplines = disciplines;
                    models.Add(lecturer);
                    disciplines = new List<Discipline>();
                    lecturer = new Lecturer();
                }
            }
            else
            {
                lecturer.Name = lecturers[i][0].ToString() ?? string.Empty;
                lecturer.Post = lecturers[i][1].ToString() ?? string.Empty;
                lecturer.InterestRate = int.Parse(lecturers[i][2].ToString() ?? string.Empty);
                disciplines.Add(CreateDiscipline(lecturers[i][3].ToString() ?? string.Empty));
                lecturer.DistributedLoad = int.Parse(lecturers[i][4].ToString() ?? string.Empty);
                lecturer.Standard = int.Parse(lecturers[i][5].ToString() ?? string.Empty);
            }
        }

        return models.ToArray();
    }

    private static Discipline CreateDiscipline(string content)
    {
        // take content in [ ]
        var pattern = @"(?<=\[).+?(?=\])";
        var matches = Regex.Matches(content, pattern);
        var curriculum = matches[^1].Value;
        var contactLoad = int.Parse(matches[^2].Value);
        return new Discipline
        {
            //fill remaining properties using docx client
            Content = content,
            Code = content[..content.IndexOf(' ')],
            ContactLoad = contactLoad,
            EducationalProgram = curriculum[..curriculum.IndexOf(',')]
        };
    }

    private static List<Request> GetFormatTableRequests(Lecturer[] lecturers, int? sheetId)
    {
        var requests = new List<Request>();
        const int disciplinesColumnIndex = 3;
        const int columnsCount = 6;
        var currentDisciplinesCount = 0;
        foreach (var lecturer in lecturers)
        {
            var mergeLeftColumns = new Request
            {
                MergeCells = GetMergeCellsRequest(sheetId, startRowIndex: currentDisciplinesCount + 1,
                    endRowIndex: currentDisciplinesCount + lecturer.Disciplines.Count + 1,
                    startColumnIndex: 0, endColumnIndex: disciplinesColumnIndex)
            };
            var mergeRightColumns = new Request
            {
                MergeCells = GetMergeCellsRequest(sheetId, startRowIndex: currentDisciplinesCount + 1,
                    endRowIndex: currentDisciplinesCount + lecturer.Disciplines.Count + 1,
                    startColumnIndex: disciplinesColumnIndex + 1, endColumnIndex: columnsCount + 1)
            };
            requests.Add(mergeLeftColumns);
            requests.Add(mergeRightColumns);

            currentDisciplinesCount += lecturer.Disciplines.Count;
        }
        
        return requests;
    }

    private static MergeCellsRequest GetMergeCellsRequest(int? spreadsheetId, int startRowIndex, int endRowIndex,
        int startColumnIndex, int endColumnIndex)
    {
        return new MergeCellsRequest
        {
            Range = new GridRange
            {
                SheetId = spreadsheetId,
                StartRowIndex = startRowIndex,
                EndRowIndex = endRowIndex,
                StartColumnIndex = startColumnIndex,
                EndColumnIndex = endColumnIndex
            },
            MergeType = "MERGE_COLUMNS"
        };
    }
    
    
}