﻿using CurriculumParser;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Spreadsheet;
using PlVisualizer.Api.Dto.Tables;
using Discipline = PlVisualizer.Api.Dto.Tables.Discipline;

namespace PLVisualizer.BusinessLogic.Clients;

public class DocxClient
{
    public List<Lecturer> GetLecturersWithDisciplines(XlsxTableRow[] tableRows)
    {
        var lecturers = new Dictionary<string, Lecturer>();
        
        //group by program in order to parse the working plan at once
        var groupedByProgramRows = tableRows.GroupBy(row => row.EducationalProgram);
        foreach (var groupedByProgramRow in groupedByProgramRows)
        {
            var eduProgramCode = groupedByProgramRow.Key
                .Split(':')[0][1..] // slicing №
                .Replace(',', '-');

            var curriculumName = GetCurriculumName(eduProgramCode);
            var parser = new DocxCurriculum(curriculumName);
            var parserDisciplines = parser.Disciplines;
            var groupedByNameRows = groupedByProgramRow.GroupBy(row => row.DisciplineName);
            
            foreach (var groupedByNameRow in groupedByNameRows)
            {
                var disciplineCode = groupedByNameRow.Key.Split()[0];
                var disciplineFromParser =
                    parserDisciplines.FirstOrDefault(discipline => discipline.Code == disciplineCode);

                var currentLecturer = groupedByNameRow.First().Lecturer;
                if (lecturers.ContainsKey(currentLecturer))
                {
                    lecturers[currentLecturer].Disciplines.Add(CreateDiscipline(
                        implementations: disciplineFromParser!.Implementations,
                        curriculumName: curriculumName, groupedByNameRow: groupedByNameRow));
                    lecturers[currentLecturer].DistributedLoad +=
                        lecturers[currentLecturer].Disciplines[^1].ContactLoad;
                }
                else
                {
                    var discipline = CreateDiscipline(
                        implementations: disciplineFromParser!.Implementations,
                        curriculumName: curriculumName, groupedByNameRow: groupedByNameRow);
                    lecturers.Add(groupedByNameRow.First().Lecturer, new Lecturer
                    {
                        Name = groupedByNameRow.First().Lecturer,
                        DistributedLoad = discipline.ContactLoad,
                        Disciplines = new List<Discipline> {discipline}
                    });
                }
            }
        }

        return lecturers.Values.ToList();
    }

    private string GetCurriculumName(string curriculumCode)
    {
        var workingPlans = Directory.GetFiles("WorkingPlans");
        return workingPlans.FirstOrDefault(plan => plan.Contains(curriculumCode)) ?? string.Empty;
    }

    private int GetTermNumber(string term)
    {
        return int.Parse(term.Replace("Семестр", string.Empty));
    }

    private int GetContactLoad(string workHours)
    {
        var castedHours = workHours.Split().Select(int.Parse).ToArray();
        return castedHours.Take(9).Sum() + castedHours[10];
    }

    private Discipline CreateDiscipline(List<DisciplineImplementation> implementations, 
        string curriculumName,  IGrouping<string,XlsxTableRow> groupedByNameRow)
    {
        return  new Discipline
        {
            EducationalProgram = curriculumName,
            Terms = groupedByNameRow.Select(row => GetTermNumber(row.Term)).ToArray(),
            Content = $"{groupedByNameRow.First().DisciplineName}",
            ContactLoad = GetContactLoad(implementations.First().WorkHours ?? string.Empty)
        };
    }
}