using CurriculumParser;
using NUnit.Framework;
using PlVisualizer.Api.Dto.Tables;
using PLVisualizer.BusinessLogic.Clients.XlsxClient;
using PLVisualizer.BusinessLogic.Providers.SpreadsheetProvider;

namespace PLVisualizerTest.TestXlsxClient;

public class TestXlsxClient
{
    private ISpreadsheetProvider spreadsheetProvider = new SpreadsheetProvider();
    private IXlsxClient xlsxClient = new XlsxClient();
    
    private static XlsxTableRow[] testTableRows =
    {
        new()
        {
            Term = 1, Subdivision = "Математико-механический факультет",
            PedagogicalTask = "058505 Учебная практика (научно-исследовательская работа)",
            DisciplineName = "Учебная практика (научно-исследовательская работа)",
            WorkType = "В присутствии преподавателя", Lecturer = "Литвинов Ю. В., доцент",
            SAPSubdivision2 = "Кафедра системного программирования",
            SAPSubdivision1 = "Факультет математико-механический",
            EducationalProgram = "№5665,2022: Математическое обеспечение и администрирование информационных систем"
        },
        new()
        {
            Term = 1, Subdivision = "Математико-механический факультет",
            PedagogicalTask = "002212 Программирование",
            DisciplineName = "Программирование",
            WorkType = "Промежуточная аттестация (зач)", Lecturer = "Литвинов Ю. В., доцент",
            SAPSubdivision2 = "Кафедра системного программирования",
            SAPSubdivision1 = "Факультет математико-механический",
            EducationalProgram = "№5162,2022: Технологии программирования"
        }
    };

    [Test]
    public void Test_XlsxClient_ReturnsCorrectModels()
    {
        var spreadsheetDocument =
            spreadsheetProvider.GetSpreadsheetDocument("../../../TestXlsxClient/SmallFileTest.xlsx");
        var tableRows = xlsxClient.GetTableRows(spreadsheetDocument);
        Assert.AreEqual(2, tableRows.Length);
        for (var i = 0; i < tableRows.Length; i++)
        {
            Assert.That(tableRows[i].Equals(testTableRows[i]));
        }
    }
}