using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using aert_csharp_extensions;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Interop.Word;
using ApplicationClass = Microsoft.Office.Interop.Word.ApplicationClass;
using System.Diagnostics;

namespace aert_csharp_helpers
{
    public class OfficeHelper
    {
        #region IsOffice

        public static bool IsOffice(string fileIn)
        {
            return IsWord(fileIn) || IsPowerpoint(fileIn) || IsExcel(fileIn);
        }

        private static bool IsWord(string fileIn)
        {
            string ext = (Path.GetExtension(fileIn) ?? "").ToUpper();
            return ext.Equals(".DOC") || ext.Equals(".DOCX");
        }

        private static bool IsPowerpoint(string fileIn)
        {
            string ext = (Path.GetExtension(fileIn) ?? "").ToUpper();
            return ext.Equals(".PPT") || ext.Equals(".PPTX");
        }

        private static bool IsExcel(string fileIn)
        {
            string ext = (Path.GetExtension(fileIn) ?? "").ToUpper();
            return ext.Equals(".XLS") || ext.Equals(".XLSX");
        }

        #endregion

        public static bool ConvertToPdf(string fileIn, string fileOut)
        {
            if (IsWord(fileIn))
                return ConvertWordToPdf(fileIn, fileOut);
            if (IsPowerpoint(fileIn))
                return ConvertPowerpointToPdf(fileIn, fileOut);
            if (IsExcel(fileIn))
                return ConvertExcelToPdf(fileIn, fileOut);
            return false;
        }

        private static bool ConvertExcelToPdf(string fileIn, string fileOut)
        {
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Visible = false;
            excelApp.DisplayAlerts = false;
            Workbook workbook = null;
            Workbooks workbooks = null;
            try
            {
                workbooks = excelApp.Workbooks;
                workbook = workbooks.Open(fileIn);

                foreach (Worksheet ws in workbook.Worksheets.OfType<Worksheet>())
                {
                    ws.PageSetup.Orientation = XlPageOrientation.xlLandscape;
                    ws.PageSetup.Zoom = false;
                    ws.PageSetup.FitToPagesWide = 1;
                    ws.PageSetup.FitToPagesTall = false;
                }

                workbook.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF,
                    fileOut,
                    XlFixedFormatQuality.xlQualityStandard,
                    true,
                    true,
                    Type.Missing,
                    Type.Missing,
                    false,
                    Type.Missing);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close(XlSaveAction.xlDoNotSaveChanges);
                    while (Marshal.FinalReleaseComObject(workbook) != 0)
                    {
                    }
                    workbook = null;
                }
                if (workbooks != null)
                {
                    workbooks.Close();
                    while (Marshal.FinalReleaseComObject(workbooks) != 0)
                    {
                    }
                    workbooks = null;
                }
                if (excelApp != null)
                {
                    excelApp.Quit();
                    excelApp.Application.Quit();
                    while (Marshal.FinalReleaseComObject(excelApp) != 0)
                    {
                    }
                    excelApp = null;
                }
            }

            return true;
        }

        private static bool ConvertWordToPdf(string fileIn, string fileOut)
        {
            ApplicationClass wordApplication = new ApplicationClass();

            Document wordDocument = null;

            object paramSourceDocPath = fileIn;
            object paramMissing = Type.Missing;

            string paramExportFilePath = fileOut;

            const WdExportFormat paramExportFormat = WdExportFormat.wdExportFormatPDF;
            const bool paramOpenAfterExport = false;
            const WdExportOptimizeFor paramExportOptimizeFor = WdExportOptimizeFor.wdExportOptimizeForPrint;
            const WdExportRange paramExportRange = WdExportRange.wdExportAllDocument;
            const int paramStartPage = 0;
            const int paramEndPage = 0;
            const WdExportItem paramExportItem = WdExportItem.wdExportDocumentContent;
            const bool paramIncludeDocProps = true;
            const bool paramKeepIrm = true;

            const WdExportCreateBookmarks paramCreateBookmarks = WdExportCreateBookmarks.wdExportCreateWordBookmarks;

            const bool paramDocStructureTags = true;
            const bool paramBitmapMissingFonts = true;
            // ReSharper disable once InconsistentNaming
            const bool paramUseISO19005_1 = false;

            try
            {
                // Open the source document.
                wordDocument = wordApplication.Documents.Open(
                    ref paramSourceDocPath, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing);
                //string tempFile = Path.Combine(Path.GetDirectoryName(fileOut), Path.GetFileName(fileIn));
                //wordDocument.SaveAs(tempFile);
                //wordDocument.Close();

                // Export it in the specified format.
                if (wordDocument != null)
                    wordDocument.ExportAsFixedFormat(paramExportFilePath,
                        paramExportFormat, paramOpenAfterExport,
                        paramExportOptimizeFor, paramExportRange, paramStartPage,
                        paramEndPage, paramExportItem, paramIncludeDocProps,
                        paramKeepIrm, paramCreateBookmarks, paramDocStructureTags,
                        paramBitmapMissingFonts, paramUseISO19005_1,
                        ref paramMissing);
            }
            catch (Exception ex)
            {
                // Respond to the error
                Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                // Close and release the Document object.
                if (wordDocument != null)
                {
                    wordDocument.Close(WdSaveOptions.wdDoNotSaveChanges, ref paramMissing,
                        ref paramMissing);
                    wordDocument = null;
                }

                // Quit Word and release the ApplicationClass object.
                if (wordApplication != null)
                {
                    wordApplication.Quit(ref paramMissing, ref paramMissing,
                        ref paramMissing);
                    wordApplication = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return true;
        }

        private static bool ConvertPowerpointToPdf(string fileIn, string fileOut)
        {
            Microsoft.Office.Interop.PowerPoint.Application ppApp =
                new Microsoft.Office.Interop.PowerPoint.Application();
            Microsoft.Office.Interop.PowerPoint.Presentation presentation = ppApp.Presentations.Open(fileIn,
                Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoFalse,
                Microsoft.Office.Core.MsoTriState.msoFalse);

            presentation.ExportAsFixedFormat(fileOut,
                PpFixedFormatType.ppFixedFormatTypePDF,
                PpFixedFormatIntent.ppFixedFormatIntentPrint,
                Microsoft.Office.Core.MsoTriState.msoFalse,
                PpPrintHandoutOrder.ppPrintHandoutHorizontalFirst,
                PpPrintOutputType.ppPrintOutputSlides,
                Microsoft.Office.Core.MsoTriState.msoFalse,
                null,
                PpPrintRangeType.ppPrintAll,
                "",
                false,
                false,
                false,
                true,
                false, // PDF-A
                System.Reflection.Missing.Value);

            presentation.Close();
            presentation = null;
            ppApp = null;
            GC.Collect();
            return true;
        }
    }
}