using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace XperienceCommunity.SqlBrowser.Admin.Models;

[CommandConfirmationModel]
public class ExportConfirmationDialogModel
{
    [DropDownComponent(
        Label = "Type",
        Options = "CSV\r\nExcel\r\nJSON")]
    [RequiredValidationRule]
    public string? ExportType { get; set; } = "CSV";


    [TextInputComponent(
        Label = "File name (without extension)",
        WatermarkText = "Leave empty to autogenerate")]
    public string? FileName { get; set; }
}
