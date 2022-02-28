// See https://aka.ms/new-console-template for more information

using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

Uri endpoint = new("http://www.contoso.com/");
string apiKey = "<your-api-key>";

AzureKeyCredential keyCredential = new(apiKey);
DocumentAnalysisClient documentAnalysisClient = new DocumentAnalysisClient(endpoint, keyCredential);

Uri filePathUri =
    new Uri(
        "https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/sample-layout.pdf");

AnalyzeDocumentOperation analyzeDocumentOperation =
    await documentAnalysisClient.StartAnalyzeDocumentFromUriAsync(documentUri: filePathUri,
        modelId: "prebuilt-document");

await analyzeDocumentOperation.WaitForCompletionAsync();

AnalyzeResult analyzedDocumentResult = analyzeDocumentOperation.Value;
Console.WriteLine("Detected key-value pairs");

foreach (DocumentKeyValuePair keyValuePair in analyzedDocumentResult.KeyValuePairs)
{
    if (keyValuePair.Value == null)
    {
        Console.WriteLine($"Found key with no value: {keyValuePair.Confidence}");

    }
    else
    {
        Console.WriteLine($"Found key-value pair: {keyValuePair.Confidence}");
    }
}

foreach (DocumentEntity documentEntity in analyzedDocumentResult.Entities)
{
    Console.WriteLine(documentEntity.SubCategory == null
        ? $"Found entity {documentEntity.Content} with Confidence: {documentEntity.Confidence} and Category: {documentEntity.Category}"
        : $"Found Entity {documentEntity.Content} with Confidence {documentEntity.Confidence} and Category {documentEntity.Category}");
}

foreach (DocumentPage documentPage in analyzedDocumentResult.Pages)
{
    Console.WriteLine($"Document Page {documentPage.PageNumber} has {documentPage.Lines.Count} line(s), {documentPage.Words.Count} word(s),");
    Console.WriteLine($"and {documentPage.SelectionMarks.Count} selection mark(s)");

    for (int i = 0; i < documentPage.Lines.Count; i++)
    {
        DocumentLine line = documentPage.Lines[i];
        Console.WriteLine($"  Line {i} has content: '{line.Content}'.");

        Console.WriteLine($"    Its bounding box is:");
        Console.WriteLine($"      Upper left => X: {line.BoundingBox[0].X}, Y= {line.BoundingBox[0].Y}");
        Console.WriteLine($"      Upper right => X: {line.BoundingBox[1].X}, Y= {line.BoundingBox[1].Y}");
        Console.WriteLine($"      Lower right => X: {line.BoundingBox[2].X}, Y= {line.BoundingBox[2].Y}");
        Console.WriteLine($"      Lower left => X: {line.BoundingBox[3].X}, Y= {line.BoundingBox[3].Y}");

    }

    for (int i = 0; i < documentPage.SelectionMarks.Count; i++)
    {
        DocumentSelectionMark selectionMark = documentPage.SelectionMarks[i];

        Console.WriteLine($"  Selection Mark {i} is {selectionMark.State}.");
        Console.WriteLine($"    Its bounding box is:");
        Console.WriteLine($"      Upper left => X: {selectionMark.BoundingBox[0].X}, Y= {selectionMark.BoundingBox[0].Y}");
        Console.WriteLine($"      Upper right => X: {selectionMark.BoundingBox[1].X}, Y= {selectionMark.BoundingBox[1].Y}");
        Console.WriteLine($"      Lower right => X: {selectionMark.BoundingBox[2].X}, Y= {selectionMark.BoundingBox[2].Y}");
        Console.WriteLine($"      Lower left => X: {selectionMark.BoundingBox[3].X}, Y= {selectionMark.BoundingBox[3].Y}");
    }


}

Console.WriteLine("The following tables were extracted");

for (int i = 0; i < analyzedDocumentResult.Tables.Count; i++)
{
    DocumentTable table = analyzedDocumentResult.Tables[i];
    Console.WriteLine($"Table {i} has {table.RowCount} rows and {table.ColumnCount} columns");

    foreach (DocumentTableCell documentTableCell in table.Cells)
    {
        Console.WriteLine($"Cell {documentTableCell.RowIndex}, {documentTableCell.ColumnIndex} has kind '{documentTableCell.Kind} and content: {documentTableCell.Content}");
    }

}


